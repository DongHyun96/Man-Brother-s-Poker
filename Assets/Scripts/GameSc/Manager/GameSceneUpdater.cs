using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using System.Linq;
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

    public CamController camController;

    public List<GameObject> lights;

    public bool isFirstGameStart = false;

    private bool IsShowDownOver{
        get
        {
            int showDownOver = 0; // PCanvas enabled card count

            foreach(PlayerCanvas canvas in playerCanvas)
            {
                showDownOver += canvas.card1.gameObject.activeSelf ? 1 : 0;
            }

            int showDownCnt = 0; // Alive and isInGame player count
            foreach(Player p in GameManager.gameTable.players)
            {
                showDownCnt += (p.state != Player.State.FOLD && p.isInGame) ? 1 : 0;
            }

            return showDownOver >= showDownCnt ? true : false;
        }
    }

    private struct EnableTurnStruct
    {
        public int readyCount;
        public bool isReadyChecked;

        public int turnIdx;
        public PieButton.ActionState actionState;
        public int callChips;

        public EnableTurnStruct(int readyCount, bool isReadyChecked, int turnIdx, 
        PieButton.ActionState actionState, int callChips) {
            this.readyCount = readyCount;
            this.isReadyChecked = isReadyChecked;
            this.turnIdx = turnIdx;
            this.actionState = actionState;
            this.callChips =callChips;        
        }

        public void InitTurnValues(int turnIdx, PieButton.ActionState actionState, int callChips)
        {
            this.turnIdx = turnIdx;
            this.actionState = actionState;
            this.callChips = callChips;
        }

        // Enable turn for playerCanvas and screenCanvas
        public void EnableTurn(List<PlayerCanvas> pcanvas, ScreenCanvas sCanvas, List<GameObject> lights)
        {
            isReadyChecked = true;
            readyCount = 0;

            int myIdx = GameManager.gameTable.GetIterPosByName(GameManager.thisPlayer.name);
            if(myIdx == turnIdx)
            {
                sCanvas.EnableTurn(actionState, callChips);
                lights[myIdx].SetActive(true);
            }

            pcanvas[turnIdx].EnableTurn();
        }
    };

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
    public void UpdateGameScene(Player updater)
    {
        try{
            StartCoroutine(UpdateGameSceneRoutine(updater));
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
        if(IsShowDownOver)
        {
            Stack<KeyValuePair<int, List<Player>>> PWS = GameManager.gameTable.potWinnerManager.potWinnerStack;

            StartCoroutine(PrepareNextPotRoutine(PWS));
        } 
    }

    // When someone leaves the game
    public void OnPlayerLeft(string name)
    {
        // Make targetPlayer's isInGame false
        Player targetPlayer = GameManager.gameTable.GetPlayerByName(name);
        targetPlayer.isInGame = false;

        // If the game is over
        if(GameManager.gameTable.stage == GameTable.Stage.GAME_FIN)
        {
            return;
        }
        switch(GameManager.gameTable.stage)
        {
            case GameTable.Stage.ROUND_FIN:
            case GameTable.Stage.GAME_FIN:
                return;
            case GameTable.Stage.POT_FIN:
            case GameTable.Stage.UNCONTESTED:
                // Check if the showDown is over by leaving
                if(IsShowDownOver && !is_prepare_running)
                {
                    Stack<KeyValuePair<int, List<Player>>> PWS = GameManager.gameTable.potWinnerManager.potWinnerStack;
                    StartCoroutine(PrepareNextPotRoutine(PWS));
                }
                return;
        }

        if(GameManager.gameTable.tableStatus == GameTable.TableStatus.ALLIN)
        {
            // All in case
            return;
        }

        // If it is left player's turn and didn't fold yet
        if(GameManager.gameTable.GetCurrentPlayer().name.Equals(name))
        {
            if(GameManager.gameTable.GetCurrentPlayer().state != Player.State.FOLD)
            {
                // Wait for enableTurn and then takeAction
                StartCoroutine(OnPlayerLeft_TakeActionRoutine(targetPlayer));
            }
        }
    }

    private IEnumerator OnPlayerLeft_TakeActionRoutine(Player targetPlayer)
    {
        Timer timer = GetPlayerCanvasFromName(targetPlayer.name).timer;

        // Wait for EnableTurn
        yield return new WaitUntil(() => timer.IsTimerActive);

        // Take action
        GameManager.gameTable.TakeAction(targetPlayer.name, Player.State.FOLD);
        UpdateGameScene(targetPlayer);
    }
    

    // This refs for check if the players are all ready for enable turn
    private EnableTurnStruct enableTurnStruct = new EnableTurnStruct(0, false, 0, PieButton.ActionState.CHECK_BET_FOLD, 0);

    // Check everyone is available for EnableTurn
    public void CheckReady()
    {
        if(!enableTurnStruct.isReadyChecked)
        {
            enableTurnStruct.readyCount++;
            int leftPlayerCnt = GameManager.gameTable.players.Count(item => item.isInGame);

            if(enableTurnStruct.readyCount >= leftPlayerCnt)
            {
                enableTurnStruct.EnableTurn(playerCanvas, screenCanvas, lights);
            }
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
        enableTurnStruct.isReadyChecked = false;

        print("Entering GameStartRoutine");
        print("table.stage = " + table.stage);
        // print("table")
        /* Hide left players */
        for(int i = 0; i < table.players.Count; i++)
        {
            if(!table.players[i].isInGame)
            {
                playerCanvas[i].gameObject.SetActive(false);
                worldAnimHandler.gameCharacters[i].characterObject.SetActive(false);
            }
        }

        /* GameOver */
        if(table.stage == GameTable.Stage.GAME_FIN)
        {
            /* Game fin routine */
            StartCoroutine(GameFinRoutine());
            yield break;
        }

        /* Animate characters' greeting */
        if(!isFirstGameStart)
        {
            // First game start

            // Remove Waiting text
            screenCanvas.waitingText.SetActive(false);
            yield return StartCoroutine(worldAnimHandler.AnimateGreetings());
        }

        int smallPos = table.SB_Pos;
        int bigPos = table.GetNext(smallPos, true);
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
        Player p = GameManager.gameTable.GetPlayerByName(GameManager.thisPlayer.name);

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

        // If the player is bankrupt, do not Toggle cards
        if(table.players[myIdx].state == Player.State.FOLD)
        {
            yield return new WaitForSeconds(1.5f);
        }
        else
        {
            screenCanvas.TogglePlayerCardsAnim(0, true);
            yield return new WaitForSeconds(0.5f);
            screenCanvas.TogglePlayerCardsAnim(1, true);
            yield return new WaitForSeconds(1f);
        }

        /* check if the stage is finished */
        switch(table.stage)
        {
            case GameTable.Stage.ROUND_FIN:
                // Round fin animation routine needed
                //screenCanvas.state = p.state;
                StartCoroutine(RoundFinRoutine());
                yield break;
            case GameTable.Stage.UNCONTESTED:
                Player winner = new Player();
                foreach(Player player in table.players)
                {
                    if(player.state != Player.State.FOLD)
                    {
                        winner = player;
                        break;
                    }
                }
                StartCoroutine(UncontestedRoutine(winner));
                yield break;
            case GameTable.Stage.POT_FIN:
                StartCoroutine(PotFinRoutine());
                yield break;
            default:
                break;
        }

        /* Check if the turn is left player */
        bool isHandled = HandleLeftPlayerTurn(table.GetCurrentPlayer());
        if(isHandled)
        {
            print("From GameStartRoutine - isHandled");
            yield break;
        }

        print("Before Init Enable turn struct");

        /* Init Enable turn struct */
        enableTurnStruct.InitTurnValues(current_UTG, PieButton.ActionState.CALL_RAISE_FOLD, table.players[bigPos].totalBet);

        // Send ready msg
        GameMsgHandler.SendReady();
    }

    private IEnumerator UpdateGameSceneRoutine(Player updater)
    {
        enableTurnStruct.isReadyChecked = false;

        GameTable table = GameManager.gameTable;

        /* Turn off the lights */
        foreach(GameObject light in lights)
        {
            light.SetActive(false);
        }

        /* Update target playerCanvas */
        PlayerCanvas targetCanvas = GetPlayerCanvasFromName(updater.name);
        targetCanvas.playerState = updater.state;
        UpdateTabs();

        /* Update totalChips and pot chips or enable fold image */
        screenCanvas.state = updater.state;

        /* Animate target player's animations */
        int targetIdx = table.GetIterPosByName(updater.name);
        switch(updater.state)
        {
            case Player.State.CHECK:
                yield return new WaitForSeconds(0.5f);
                break;
            case Player.State.BET:
            case Player.State.CALL:
            case Player.State.RAISE:
            case Player.State.ALLIN:
                yield return StartCoroutine(worldAnimHandler.AnimateBetting(targetIdx, updater));
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
            
                Player winner = GameManager.gameTable.potWinnerManager.potWinnerStack.Peek().Value[0];
                StartCoroutine(UncontestedRoutine(winner));
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

        Player currentPlayer = table.GetCurrentPlayer();
        
        /* Check if the turn is left player */
        bool isHandled = HandleLeftPlayerTurn(currentPlayer);
        if(isHandled)
        {
            yield break;
        }
        
        /* Init enableTurn Struct fields */
        if(GameManager.thisPlayer.name.Equals(currentPlayer.name))
        {
            // CHECK_BET_FOLD, CALL_RAISE_FOLD, CHECK_RAISE_FOLD, ALLIN_FOLD
            switch(table.tableStatus)
            {
                case GameTable.TableStatus.IDLE:
                case GameTable.TableStatus.CHECK:
                    enableTurnStruct.InitTurnValues(table.iterPos, PieButton.ActionState.CHECK_BET_FOLD, 0);
                    break;
                case GameTable.TableStatus.BET:
                    int big = table.GetNext(table.SB_Pos);

                    // Big blind PREFLOP case
                    if(table.stage == GameTable.Stage.PREFLOP && table.players[big].Equals(currentPlayer))
                    {
                        if(table.roundBetMax == currentPlayer.roundBet)
                        {
                            // All players didn't raised
                            enableTurnStruct.InitTurnValues(table.iterPos, PieButton.ActionState.CHECK_RAISE_FOLD, table.roundBetMax);
                            break;
                        }
                    }
                    int currentPTotal = currentPlayer.totalChips + currentPlayer.roundBet;
                    PieButton.ActionState a = currentPTotal <= table.roundBetMax ?
                    PieButton.ActionState.ALLIN_FOLD : PieButton.ActionState.CALL_RAISE_FOLD;
                    enableTurnStruct.InitTurnValues(table.iterPos, a, table.roundBetMax);
                    break;
                case GameTable.TableStatus.ALLIN:
                    break;
            }
        }
        
        /* Init turnIdx */
        enableTurnStruct.turnIdx = table.iterPos;

        // Send ready msg
        GameMsgHandler.SendReady();
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

        // All in status recursion base case
        if(GameManager.gameTable.tableStatus == GameTable.TableStatus.ALLIN
        && GameManager.gameTable.stage == GameTable.Stage.POT_FIN)
        {
            yield return StartCoroutine(PotFinRoutine());
            yield break;
        }

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

        /* Animate table cards (New community cards) */
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
            // Recursive RoundFinRoutine here
            yield return StartCoroutine(RoundFinRoutine());
            yield break;
        }

        /* Check if the turn is left player */
        bool isHandled = HandleLeftPlayerTurn(GameManager.gameTable.GetCurrentPlayer());
        if(isHandled)
        {
            yield break;
        }
        
        /* Init Turn fields */
        enableTurnStruct.InitTurnValues(GameManager.gameTable.iterPos, PieButton.ActionState.CHECK_BET_FOLD, 0);

        // Send ready msg
        GameMsgHandler.SendReady();
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

            /* ShowDown */
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

        int winnerIdx = -1;
        foreach(Player p in GameManager.gameTable.potWinnerManager.GetMainPotWinners())
        {
            int idx = GameManager.gameTable.GetIterPosByName(p.name);
            winnerIdx = idx;
            StartCoroutine(worldAnimHandler.AnimateWinningPot(idx));
        }
        
        // Cam works if the main pot winner is only one man 
        if(GameManager.gameTable.potWinnerManager.GetMainPotWinners().Count == 1)
        {
            camController.SetCamToWinnerPos(winnerIdx);
        }

        // Play pot winning sfx
        SfxHolder.GetInstance().PlaySfx(GameSfxHolder.SoundType.WINNING, 0);

        yield return new WaitForSeconds(5.0f);
        
        /* Set cam to original position */
        if(!camController.isMovable)
        {
            camController.SetCamToPrev();
        }

        /* Close winningPanel and Show upper buttons, Buttom left */
        screenCanvas.winnerPanel.HidePanel();
        screenCanvas.upperPanel.SetActive(true);
        // screenCanvas.bottomLeft.SetActive(true);
        screenCanvas.bottomLeft.transform.localScale = Vector3.one;
        
        /* Update player tabs */
        UpdateTabs();
        
        /* If all cards are shown, Go to next pot game */
        if(IsShowDownOver)
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

    private IEnumerator UncontestedRoutine(Player winner)
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

        // Animate winners' character
        int idx = GameManager.gameTable.GetIterPosByName(winner.name);
        StartCoroutine(worldAnimHandler.AnimateWinningPot(idx));
        
        // Cam works
        camController.SetCamToWinnerPos(idx);

        // Play pot winning sfx
        SfxHolder.GetInstance().PlaySfx(GameSfxHolder.SoundType.WINNING, 0);

        yield return new WaitForSeconds(5.0f);

        /* Set cam to original position */
        camController.SetCamToPrev();

        /* Close winning panel and Showupper buttons, button left */
        screenCanvas.winnerPanel.HidePanel();
        screenCanvas.upperPanel.SetActive(true);
        // screenCanvas.bottomLeft.SetActive(true);
        screenCanvas.bottomLeft.transform.localScale = Vector3.one;


        /* Update player tab */
        UpdateTabs();

        // If this player is winner, show showDown choose panel
        if(GameManager.thisPlayer.name.Equals(winner.name))
        {
            screenCanvas.chooseShowDown.SetActive(true);
        }

        // yield return StartCoroutine(PrepareNextPotRoutine(PWS));
    }

    private IEnumerator GameFinRoutine()
    {

        // Init SCreenCanvas and show Game Over UI ( & Exit to Lobby btn )
        screenCanvas.InitCanvas();
        screenCanvas.stage = GameTable.Stage.GAME_FIN;

        // Play gameWin sfx
        SfxHolder.GetInstance().PlaySfx(GameSfxHolder.SoundType.WINNING, 1);

        // PlayerCanvas - Init playerCanvas and Show rankings and totalChips
        foreach(PlayerCanvas c in playerCanvas)
        {
            if(!c.gameObject.activeSelf)
            {
                // Left player's canvas
                continue;
            }
            c.playerState = Player.State.IDLE;
            c.GameOver();
        }

        // WorldAnimHandler - Put winner chairs' back and animate winner gameWinAnimation
        foreach(int idx in GameManager.gameTable.GetWinnersIndex())
        {
            StartCoroutine(worldAnimHandler.AnimateWinningGame(idx));
        }
        
        yield return new WaitForSeconds(5.0f);

        // Show Return to lobby btn
        screenCanvas.lobbyBtnAnim.SetTrigger("in");
    }

    private bool is_prepare_running = false;
    private IEnumerator PrepareNextPotRoutine(Stack<KeyValuePair<int, List<Player>>> potWinnerStack)
    {
        is_prepare_running = true;

        yield return new WaitForSeconds(3.5f);

        /* Play table preparation animations */
        yield return StartCoroutine(worldAnimHandler.PrepareNextPotRoutine(potWinnerStack));

        // If the table status was all in, init deck here
        if(GameManager.gameTable.tableStatus == GameTable.TableStatus.ALLIN)
        {
            print("Init Deck by next deck");
            GameManager.gameTable.deck = GameManager.gameTable.nextDeck;
        }
        
        /* Go to next pot */
        GameManager.gameTable.stage = GameTable.Stage.PREFLOP;
        StartGame();
        is_prepare_running = false;
    }

    /****************************************************************************************************************
    *                                                Extra methods
    ****************************************************************************************************************/
    /* Return true if the handling took place */
    private bool HandleLeftPlayerTurn(Player current)
    {
        // Check if the turn is left player
        if(!current.isInGame)
        {
            // Check if the player didn't fold yet and then takeAction
            if(current.state != Player.State.FOLD)
            {
                GameManager.gameTable.TakeAction(current.name, Player.State.FOLD);

                // UpdateGameSceneRoutine
                UpdateGameScene(current);
                return true;
            }
        }
        return false;
    }

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
}

