using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/*
* Controls GameScene UI & GUI animation update
* Controls characters animation
*/
public class GameSceneUpdater : MonoBehaviour
{   
    private static GameSceneUpdater instance;

    public List<PlayerCanvas> playerCanvas;

    public ScreenCanvas screenCanvas;

    // Characters
    public List<GameCharacter> characters;

    // Table animators, Maybe making tableObjectController
    public TableChipHandler tableChipHandler;
    public TableCardHandler tableCardHandler;

    public bool isFirstGameStart = false;

    public static GameSceneUpdater GetInstance()
    {
        if(instance == null)
        {
            instance = FindObjectOfType<GameSceneUpdater>();
        }
        return instance;
    }

    public void InitSettings()
    {
        // Init panel's name and enable the panel or not
        // Init Characters
        for(int i = 0; i < GameManager.gameTable.players.Count; i++)
        {
            playerCanvas[i].Init(GameManager.gameTable.players[i].name);
            characters[i].Init(GameManager.gameTable.players[i].character);
        }

        // Init tableChipHandler's buyIn
        tableChipHandler.InitBuyIn(GameManager.gameTable.buy);
    }


    // Facade methods to update the canvas and anim
    public void UpdateGameScene(Player p)
    {
        GameTable table = GameManager.gameTable;

        /* Update target playerCanvas */
        PlayerCanvas targetCanvas = GetPlayerCanvasFromName(p.name);
        targetCanvas.playerState = p.state; // Update sender's canvas
        UpdateTabs(); // Update rankings


        screenCanvas.state = p.state;

        /* Check if the stage is finished*/
        switch(table.stage)
        {
            case GameTable.Stage.ROUND_FIN:
                // Round fin animation routine needed
                //screenCanvas.state = p.state;
                StartCoroutine(RoundFinRoutine());
                return;
            case GameTable.Stage.UNCONTESTED:
                StartCoroutine(UncontestedRoutine());
                return;
            case GameTable.Stage.POT_FIN:
                StartCoroutine(PotFinRoutine());
                return;
            case GameTable.Stage.GAME_FIN:
                return;
            
            default:
                /* Update screenCanvas */
                //screenCanvas.state = p.state;
                break;
        }

        /* Check iterator turn */
        if(GameManager.thisPlayer.name.Equals(table.GetCurrentPlayer().name))
        {
            // CHECK_BET_FOLD, CALL_RAISE_FOLD, CHECK_RAISE_FOLD, ALLIN_FOLD
            switch(table.tableStatus)
            {
                case GameTable.TableStatus.IDLE:
                case GameTable.TableStatus.CHECK:
                    screenCanvas.EnableTurn(PieButton.ActionState.CHECK_BET_FOLD);
                    break;
                case GameTable.TableStatus.BET:
                    int big = table.GetNext(table.SB_Pos);

                    // Big blind PREFLOP case
                    if(table.stage == GameTable.Stage.PREFLOP && table.players[big].Equals(table.GetCurrentPlayer()))
                    {
                        if(table.roundBetMax == table.GetCurrentPlayer().roundBet)
                        {
                            // All players didn't raised
                            screenCanvas.EnableTurn(PieButton.ActionState.CHECK_RAISE_FOLD, table.roundBetMax);
                            break;
                        }
                    }
                    PieButton.ActionState a = table.GetCurrentPlayer().totalChips <= table.roundBetMax ?
                    PieButton.ActionState.ALLIN_FOLD : PieButton.ActionState.CALL_RAISE_FOLD;
                    screenCanvas.EnableTurn(a, table.roundBetMax);


                    break;
                case GameTable.TableStatus.ALLIN:
                    break;
                    

            }
        }
        
        /* Enable iterator turn on iterTurn player's canvas */
        GetPlayerCanvasFromName(table.GetCurrentPlayer().name).EnableTurn(); // Enable timer from playerCanvas
    }

    public void ShowDownCardsByPlayer(string name, List<bool> showDownBool)
    {
        PlayerCanvas canvas = GetPlayerCanvasFromName(name);

        /* Show down cards */
        canvas.OpenCards(showDownBool[0], showDownBool[1]);

        /* Some animation needed */
        
        /* Check if the show down is over */
        if(IsShowDownOver())
        {
            /* Wait for the last player shows the card */
            /* Prepare next pot */
            LazyAction.GetWkr().Act(
            ()=>{
                GameManager.gameTable.stage = GameTable.Stage.PREFLOP;
                StartGame();
            }, 3.0f);
        
        }
    }

    /****************************************************************************************************************
    *                                                Starting methods
    ****************************************************************************************************************/
    public void StartGame() // GameTable and player all set! Init the Game scene
    {
        StartCoroutine(GameStartRoutine(GameManager.gameTable));
    }

    public void StartRound()
    {
        GameTable table = GameManager.gameTable;

        /* Set up playerCanvas table turn */
        GetPlayerCanvasFromName(table.GetCurrentPlayer().name).EnableTurn();

        /* Set up ScreenCanvas */
        screenCanvas.state = Player.State.IDLE;

        if(GameManager.thisPlayer.name.Equals(table.GetCurrentPlayer().name))
        {
            screenCanvas.EnableTurn(PieButton.ActionState.CHECK_BET_FOLD, 0);
        }

        screenCanvas.stage = table.stage;
    }

    /****************************************************************************************************************
    *                                                Coroutine
    ****************************************************************************************************************/
    private IEnumerator GameStartRoutine(GameTable table)
    {
        /* Animate characters' greeting */
        if(!isFirstGameStart)
        {
            foreach(GameCharacter character in characters)
            {
                character.AnimateCharacter(GameCharacter.AnimType.GREET);
            }
            /* Wait till greetings finished */
            yield return StartCoroutine(WaitForTurningPoint(characters[1].characterObject.GetComponent<AnimTurningPointHandler>()));
        }

        int smallPos = table.SB_Pos;
        int bigPos = table.GetNext(smallPos);
        int current_UTG = table.iterPos;

        // Set up playerCanvas
        foreach(PlayerCanvas canvas in playerCanvas)
        {   

            if(canvas.name.Equals(table.players[smallPos].name)) // Small Blind
            {
                canvas.playerState = Player.State.SMALL;
            }
            else if(canvas.name.Equals(table.players[bigPos].name)) // Big blind
            {
                canvas.playerState = Player.State.BIG;
            }
            else // Other players
            {
                canvas.playerState = Player.State.IDLE;
            }
        }

        // Init screenCanvas
        screenCanvas.InitCanvas();
        screenCanvas.stage = GameTable.Stage.PREFLOP;

        /* Init players' chip */
        for(int i = 0; i < table.players.Count; i++)
        {
            int chips = table.players[i].totalChips;
            tableChipHandler.UpdateChips(TableChipHandler.ContentType.PLAYER, i, chips);
        }

        /* Animate small big betting */
        characters[smallPos].AnimateCharacter(GameCharacter.AnimType.BET);
        characters[bigPos].AnimateCharacter(GameCharacter.AnimType.BET);

        /* Animate chips */
        yield return StartCoroutine(WaitForTurningPoint(characters[smallPos].characterObject.GetComponent<AnimTurningPointHandler>()));

        // Update player chips and then animate betting chips
        int smallTotal = table.players[smallPos].totalChips;
        int bigTotal = table.players[bigPos].totalChips;
        int smallBet = table.players[smallPos].roundBet;
        int bigBet = table.players[bigPos].roundBet;
        tableChipHandler.UpdateChips(TableChipHandler.ContentType.PLAYER, smallPos, smallTotal);
        tableChipHandler.UpdateChips(TableChipHandler.ContentType.PLAYER, bigPos, bigTotal);
        tableChipHandler.MoveChips(TableChipHandler.AnimType.BET, smallPos, smallBet, smallBet);
        tableChipHandler.MoveChips(TableChipHandler.AnimType.BET, bigPos, bigBet, bigBet);

        /* Animate cards - Table first and then screen cards */
        tableCardHandler.DrawCard(TableCardHandler.DrawType.DISCARD, 0, new Card(Card.Suit.CLUB, 0));

        yield return new WaitForSeconds(0.2f);

        for(int i = 0; i < table.players.Count; i++)
        {
            Card c = table.players[i].cards[0];
            tableCardHandler.DrawCard(TableCardHandler.DrawType.FIRST, i, c);
            yield return new WaitForSeconds(0.2f);
        }
        for(int i = 0; i < table.players.Count; i++)
        {
            Card c = table.players[i].cards[1];
            tableCardHandler.DrawCard(TableCardHandler.DrawType.SECOND, i, c);
            yield return new WaitForSeconds(0.2f);
        }

        int myIdx = table.GetIterPosByName(GameManager.thisPlayer.name);

        yield return StartCoroutine(
            WaitForTurningPoint(tableCardHandler.playerFirstCards[myIdx].GetComponent<AnimTurningPointHandler>()));

        screenCanvas.TogglePlayerCardsAnim(0, true);
        yield return new WaitForSeconds(0.5f);
        screenCanvas.TogglePlayerCardsAnim(1, true);
        yield return new WaitForSeconds(1f);

        /* Enable turn */
        playerCanvas[current_UTG].EnableTurn();
        if(myIdx == current_UTG)
        {
            screenCanvas.EnableTurn(PieButton.ActionState.CALL_RAISE_FOLD, table.sbChip * 2);
        }
    }

    public IEnumerator RoundFinRoutine()
    {
        yield return new WaitForSeconds(2.0f);

        // Init playerCanvas ActionGUI
        foreach(PlayerCanvas canvas in playerCanvas)
        {
            canvas.playerState = Player.State.IDLE;
        }

        // Show Round fin table animation here (Collecting chips to pot etc.)
        yield return new WaitForSeconds(2.0f);

        // Update GameTable to next round
        GameManager.gameTable.UpdateToNextRound();

        // Update all canvas
        StartRound();
    }

    public IEnumerator PotFinRoutine()
    {
        yield return new WaitForSeconds(2.0f);
        
        // Init playerCanvas ActionGUI 
        foreach(PlayerCanvas canvas in playerCanvas)
        {
            canvas.playerState = Player.State.IDLE;
        }

        // Init ScreenCanvas
        //screenCanvas.state = Player.State.IDLE;

        /* Round fin table anim (Collecting chips to pot etc.) */
        yield return new WaitForSeconds(2.0f);

        /* ShowDown / Animation needed */
        foreach(Player p in GameManager.gameTable.potWinnerManager.showDown)
        {
            PlayerCanvas pCanvas = GetPlayerCanvasFromName(p.name);
            pCanvas.OpenCards(true, true);
            yield return new WaitForSeconds(1.5f);
        }
        
        /* Cam works and Showing winner routine / Update totalChips and potchips */
        screenCanvas.stage = GameTable.Stage.POT_FIN;
        
        yield return new WaitForSeconds(5.0f);
        
        /* Set cam to original position  */
        /* Close winningPanel and Show upper buttons, Buttom left */
        screenCanvas.winnerPanel.HidePanel();
        screenCanvas.upperPanel.SetActive(true);
        // screenCanvas.bottomLeft.SetActive(true);
        screenCanvas.bottomLeft.transform.localScale = Vector3.one;
        
        /* Update player tabs */
        UpdateTabs();
        
        /* If all cards are shown, Go to next pot game */
        if(IsShowDownOver())
        {
            LazyAction.GetWkr().Act(
            ()=>{
                GameManager.gameTable.stage = GameTable.Stage.PREFLOP;
                StartGame();
            }, 3.0f);
            yield break;
        }
        
        /* ShowDown choose panel */
        if(!GameManager.gameTable.IsInShowDown(GameManager.thisPlayer.name))
        {
            if(GameManager.gameTable.GetPlayerByName(GameManager.thisPlayer.name).state != Player.State.FOLD)
            {
                screenCanvas.chooseShowDown.SetActive(true);
            }
        }
        
    }

    public IEnumerator UncontestedRoutine()
    {
        yield return new WaitForSeconds(2.0f);

        // Init playerCanvas ActionGUI
        foreach(PlayerCanvas canvas in playerCanvas)
        {
            canvas.playerState = Player.State.IDLE;
        }

        /* (Collecting chips to pot etc.) */
        yield return new WaitForSeconds(2.0f);

        /* Cam works and showing winner routine */
        screenCanvas.stage = GameTable.Stage.UNCONTESTED;
        yield return new WaitForSeconds(5.0f);

        /* Set cam to original position */
        /* Close winning panel and Showupper buttons, button left */
        screenCanvas.winnerPanel.HidePanel();
        screenCanvas.upperPanel.SetActive(true);
        // screenCanvas.bottomLeft.SetActive(true);
        screenCanvas.bottomLeft.transform.localScale = Vector3.one;


        /* Update player tab */
        UpdateTabs();

        /* Prepare Next stage */
        print("Prepare next pot");
        
        GameManager.gameTable.stage = GameTable.Stage.PREFLOP;
        StartGame();
    }

    private IEnumerator WaitForTurningPoint(AnimTurningPointHandler handler)
    {
        while(!handler.IsTurningPointPassed)
        {
            yield return null;
        }
    }

    /****************************************************************************************************************
    *                                                Extra methods
    ****************************************************************************************************************/
    private PlayerCanvas GetPlayerCanvasFromName(string name)
    {
        foreach(PlayerCanvas c in playerCanvas)
        {
            if(name.Equals(c.name))
            {
                return c;
            }
        }
        return null;
    }

    private void UpdateTabs()
    {
        foreach(Player p in GameManager.gameTable.players)
        {
            PlayerCanvas c = GetPlayerCanvasFromName(p.name);
            c.UpdateTab(p);
        }
    }

    private bool IsShowDownOver()
    {
        int cnt = 0;

        foreach(PlayerCanvas canvas in playerCanvas)
        {
            cnt += canvas.card1.gameObject.activeSelf ? 1 : 0;
        }

        int showDownCnt = 0;
        foreach(Player p in GameManager.gameTable.players)
        {
            showDownCnt += p.state != Player.State.FOLD ? 1 : 0;
        }
        
        return showDownCnt == cnt ? true : false;
    }

}
