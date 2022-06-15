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

    public WorldAnimHandler worldAnimHandler;

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
        for(int i = 0; i < GameManager.gameTable.players.Count; i++)
        {
            playerCanvas[i].Init(GameManager.gameTable.players[i].name);
        }

        // Init worldAnimHandler
        worldAnimHandler.Init(GameManager.gameTable);
    }


    // Facade methods to update the canvas and anim
    public void UpdateGameScene(Player p)
    {
        try{
            StartCoroutine(UpdateGameSceneRoutine(p));
        }
        catch(Exception e)
        {
            print("Try Catch");
        }
        // StartCoroutine(UpdateGameSceneRoutine(p));
    }

    public void ShowDownCardsByPlayer(string name, List<bool> showDownBool)
    {
        PlayerCanvas canvas = GetPlayerCanvasFromName(name);

        /* Show down cards (playerCanvas) */
        canvas.OpenCards(showDownBool[0], showDownBool[1]);

        /* Show down cards (Table Animation) */
        int pIdx = GameManager.gameTable.GetIterPosByName(name);
        StartCoroutine(worldAnimHandler.AnimateShowDown(pIdx, showDownBool));
        
        /* Check if the show down is over */
        if(IsShowDownOver())
        {
            Stack<KeyValuePair<int, List<Player>>> PWS = GameManager.gameTable.potWinnerManager.potWinnerStack;
            StartCoroutine(PrepareNextPotRoutine(PWS));
        }
    }

    /****************************************************************************************************************
    *                                                Starting methods
    ****************************************************************************************************************/
    public void StartGame() // GameTable and player all set! Init the Game scene
    {
        StartCoroutine(GameStartRoutine(GameManager.gameTable));
    }

    /****************************************************************************************************************
    *                                                Coroutine
    ****************************************************************************************************************/
    private IEnumerator GameStartRoutine(GameTable table)
    {
        /* Animate characters' greeting */
        if(!isFirstGameStart)
        {
            yield return StartCoroutine(worldAnimHandler.AnimateGreetings());
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
        
        print("Before initing sceenCanvas cards: ");
        Player p = GameManager.gameTable.GetPlayerByName(GameManager.thisPlayer.name);
        foreach(Card c in p.cards)
        {
            print(c.ToString());
        }

        screenCanvas.InitCanvas();
        screenCanvas.stage = GameTable.Stage.PREFLOP;

        /* Init players' chip */
        for(int i = 0; i < table.players.Count; i++)
        {
            int chips = table.players[i].totalChips;
            worldAnimHandler.chipHandler.UpdateChips(TableChipHandler.ContentType.PLAYER, i, chips);
        }

        /* Animate small big betting */
        StartCoroutine(worldAnimHandler.AnimateBetting(smallPos, table.players[smallPos]));
        yield return StartCoroutine(worldAnimHandler.AnimateBetting(bigPos, table.players[bigPos]));

        /* Animate cards - Table first and then screen cards */
        int myIdx = table.GetIterPosByName(GameManager.thisPlayer.name);
        yield return StartCoroutine(worldAnimHandler.PreflopRoutine(table, myIdx));

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

    private IEnumerator UpdateGameSceneRoutine(Player p)
    {
        GameTable table = GameManager.gameTable;

        /* Update target playerCanvas */
        PlayerCanvas targetCanvas = GetPlayerCanvasFromName(p.name);
        targetCanvas.playerState = p.state;
        UpdateTabs();

        /* Update totalChips and pot chips or enable fold image */
        screenCanvas.state = p.state;

        /* Animate target player's animations */
        int targetIdx = table.GetIterPosByName(p.name);
        switch(p.state)
        {
            case Player.State.CHECK:
                yield return new WaitForSeconds(0.5f);
                break;
            case Player.State.BET:
            case Player.State.CALL:
            case Player.State.RAISE:
            case Player.State.ALLIN:
                yield return StartCoroutine(worldAnimHandler.AnimateBetting(targetIdx, p));
                break;
            case Player.State.FOLD:
                yield return StartCoroutine(worldAnimHandler.AnimateFold(targetIdx));
                break;
        }

        /* Wait for couple of sec here */
        yield return new WaitForSeconds(1.5f);

        /* check if the stage is finished */
        switch(table.stage)
        {
            case GameTable.Stage.ROUND_FIN:
                // Round fin animation routine needed
                //screenCanvas.state = p.state;
                StartCoroutine(RoundFinRoutine());
                yield break;
            case GameTable.Stage.UNCONTESTED:
                StartCoroutine(UncontestedRoutine());
                yield break;
            case GameTable.Stage.POT_FIN:
                StartCoroutine(PotFinRoutine());
                yield break;
            case GameTable.Stage.GAME_FIN:
                yield break;
            
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
                    int currentPTotal = table.GetCurrentPlayer().totalChips + table.GetCurrentPlayer().roundBet;
                    PieButton.ActionState a = currentPTotal <= table.roundBetMax ?
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


    private IEnumerator RoundFinRoutine()
    {
        yield return new WaitForSeconds(2.0f);

        // Init playerCanvas ActionGUI
        foreach(PlayerCanvas canvas in playerCanvas)
        {
            canvas.playerState = Player.State.IDLE;
        }

        // Show Round fin table animation here (Collecting chips to pot)
        yield return StartCoroutine(worldAnimHandler.RoundFinRoutine(GameManager.gameTable.players, GameManager.gameTable.pot));
        yield return new WaitForSeconds(0.5f);

        // Update GameTable to next round
        GameManager.gameTable.UpdateToNextRound();

        // Update all canvas

        /* Set up ScreenCanvas */
        screenCanvas.state = Player.State.IDLE; 

        /* Update community cards in gameTable */
        screenCanvas.stage = GameManager.gameTable.stage;

        /* All in status player cards showdown */
        if(GameManager.gameTable.tableStatus == GameTable.TableStatus.ALLIN)
        {
            // Card showDown if it isn't shown
            List<Player> players = GameManager.gameTable.players;
            for(int i = 0; i < players.Count; i++)
            {  
                if(players[i].state != Player.State.FOLD)
                {
                    PlayerCanvas pCanvas = GetPlayerCanvasFromName(players[i].name);

                    // If it is already shown
                    if(pCanvas.card1.gameObject.activeSelf)
                    {
                        break;
                    }

                    List<bool> showDownBool = new List<bool>();
                    showDownBool.Add(true);
                    showDownBool.Add(true);
                    yield return StartCoroutine(worldAnimHandler.AnimateShowDown(i, showDownBool));
                    
                    pCanvas.OpenCards(true, true);
                }
            }
        }

        /* Animate table cards */
        yield return StartCoroutine(
            worldAnimHandler.FlopTurnRiverRoutine(
                GameManager.gameTable.stage, GameManager.gameTable.communityCards
                ));

        /* Animate screenCanvas cards */
        switch(GameManager.gameTable.stage)
        {
            case GameTable.Stage.FLOP:
                for(int i = 0; i < 3; i++)
                {
                    screenCanvas.ToggleCommunityCardsAnim(i, true);
                    yield return new WaitForSeconds(0.2f);
                }
                break;
            case GameTable.Stage.TURN:
                screenCanvas.ToggleCommunityCardsAnim(3, true);
                break;
            case GameTable.Stage.RIVER:
                screenCanvas.ToggleCommunityCardsAnim(4, true);
                break;
        }
        yield return new WaitForSeconds(1f);

        /* All in status recursion */
        if(GameManager.gameTable.tableStatus == GameTable.TableStatus.ALLIN)
        {
            // Reach base case
            if(GameManager.gameTable.stage == GameTable.Stage.POT_FIN)
            {
                yield return StartCoroutine(PotFinRoutine());
                yield break;
            }

            // Recursive RoundFinRoutine here
            yield return StartCoroutine(RoundFinRoutine());
        }
        
        /* Enable turn */
        GetPlayerCanvasFromName(GameManager.gameTable.GetCurrentPlayer().name).EnableTurn();
        if(GameManager.thisPlayer.name.Equals(GameManager.gameTable.GetCurrentPlayer().name))
        {
            screenCanvas.EnableTurn(PieButton.ActionState.CHECK_BET_FOLD, 0);
        }
    }


    private IEnumerator PotFinRoutine()
    {
        yield return new WaitForSeconds(2.0f);
        
        // Init playerCanvas ActionGUI 
        foreach(PlayerCanvas canvas in playerCanvas)
        {
            canvas.playerState = Player.State.IDLE;
        }

        // Init ScreenCanvas
        //screenCanvas.state = Player.State.IDLE;

        /* Round fin table anim (Collecting chips to pot) */
        /* If table Status is equal to ALLIN, then RoundFinRoutine already took place + ShowDown doesn't needed */
        if(GameManager.gameTable.tableStatus != GameTable.TableStatus.ALLIN)
        {
            yield return StartCoroutine(worldAnimHandler.RoundFinRoutine(GameManager.gameTable.players, GameManager.gameTable.pot));
        
            yield return new WaitForSeconds(0.5f);

            /* ShowDown / Animation needed */
            foreach(Player p in GameManager.gameTable.potWinnerManager.showDown)
            {
                int pIdx = GameManager.gameTable.GetIterPosByName(p.name);
                List<bool> showDownBool = new List<bool>();
                showDownBool.Add(true);
                showDownBool.Add(true);
                yield return StartCoroutine(worldAnimHandler.AnimateShowDown(pIdx, showDownBool));
                PlayerCanvas pCanvas = GetPlayerCanvasFromName(p.name);
                pCanvas.OpenCards(true, true);
            }
        }
        yield return new WaitForSeconds(1.5f);
    
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
            Stack<KeyValuePair<int, List<Player>>> PWS = GameManager.gameTable.potWinnerManager.potWinnerStack;
            yield return StartCoroutine(PrepareNextPotRoutine(PWS));
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

    private IEnumerator UncontestedRoutine()
    {
        yield return new WaitForSeconds(2.0f);

        // Init playerCanvas ActionGUI
        foreach(PlayerCanvas canvas in playerCanvas)
        {
            canvas.playerState = Player.State.IDLE;
        }

        /* (Collecting chips to pot) */
        yield return StartCoroutine(worldAnimHandler.RoundFinRoutine(GameManager.gameTable.players, GameManager.gameTable.pot));
        yield return new WaitForSeconds(2.0f);

        /* Cam works and showing winner routine */
        screenCanvas.stage = GameTable.Stage.UNCONTESTED; // PayEachPotWinners on GameTable in here
        yield return new WaitForSeconds(5.0f);

        /* Set cam to original position */
        /* Close winning panel and Showupper buttons, button left */
        screenCanvas.winnerPanel.HidePanel();
        screenCanvas.upperPanel.SetActive(true);
        // screenCanvas.bottomLeft.SetActive(true);
        screenCanvas.bottomLeft.transform.localScale = Vector3.one;


        /* Update player tab */
        UpdateTabs();

        /* Prepare Next pot */

        /* Making PotWinnerStack */
        Stack<KeyValuePair<int, List<Player>>> PWS = new Stack<KeyValuePair<int, List<Player>>>();
        List<Player> uncontestedPlayer = new List<Player>();
        foreach(Player p in GameManager.gameTable.players)
        {
            if(p.state != Player.State.FOLD)
            {
                uncontestedPlayer.Add(p);
                break;
            }
        }
        KeyValuePair<int, List<Player>> kvPair = new KeyValuePair<int, List<Player>>(GameManager.gameTable.pot, uncontestedPlayer);
        PWS.Push(kvPair);

        yield return StartCoroutine(PrepareNextPotRoutine(PWS));
    }

    private IEnumerator PrepareNextPotRoutine(Stack<KeyValuePair<int, List<Player>>> potWinnerStack)
    {
        yield return new WaitForSeconds(3.5f);

        /* Play table preparation animations */
        yield return StartCoroutine(worldAnimHandler.PrepareNextPotRoutine(potWinnerStack));
        
        /* Go to next pot */
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
