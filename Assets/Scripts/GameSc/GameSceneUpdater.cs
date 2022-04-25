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
    public static GameSceneUpdater instance;

    public List<PlayerCanvas> playerCanvas;

    public ScreenCanvas screenCanvas;

    // Character animators

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
            playerCanvas[i].name = GameManager.gameTable.players[i].name;
        }

        // Init screenCanvas(maybe)

        // Init characters

    }


    // Facade methods to update the canvas and anim
    public void UpdateGameScene(Player p)
    {
        GameTable table = GameManager.gameTable;

        /* Update target playerCanvas */
        PlayerCanvas targetCanvas = GetPlayerCanvasFromName(p.name);
        targetCanvas.playerState = p.state; // Update sender's canvas
        UpdateTabs(); // Update rankings


        // screenCanvas.state = p.state;

        /* Check if the stage is finished*/
        switch(table.stage)
        {
            case GameTable.Stage.ROUND_FIN:
                // Round fin animation routine needed
                StartCoroutine(RoundFinRoutine());
                return;
            case GameTable.Stage.UNCONTESTED:
                return;
            case GameTable.Stage.POT_FIN:
                StartCoroutine(PotFinRoutine());
                return;
            case GameTable.Stage.GAME_FIN:
                return;
            
            default:
                /* Update screenCanvas */
                screenCanvas.state = p.state;
                break;
        }

        /* Check iterator turn */
        if(GameManager.thisPlayer.name.Equals(table.GetCurrentPlayer().name))
        {
            // CHECK_BET_FOLD, CALL_RAISE_FOLD, CHECK_RAISE_FOLD, ALLIN_FOLD
            switch(table.tableStatus)
            {
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
    /****************************************************************************************************************
    *                                                Starting methods
    ****************************************************************************************************************/
    public void StartGame() // GameTable and player all set! Init the Game scene
    {
        GameTable table = GameManager.gameTable;
        
        int smallPos = table.SB_Pos;
        int bigPos = table.GetNext(smallPos);
        int current_UTG = table.GetNext(bigPos);

        //print(table.players[smallPos].name);
        //print(playerCanvas[smallPos])
        
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


            if(canvas.name.Equals(table.players[current_UTG].name)) // UTG(first actor)
            {
                canvas.EnableTurn();
            } 
        }

        // Set up ScreenCanvas
        if(GameManager.thisPlayer.name.Equals(table.players[smallPos].name)) // Small Blind
        {
            screenCanvas.state = Player.State.SMALL;
        }
        else if(GameManager.thisPlayer.name.Equals(table.players[bigPos].name)) // Big blind
        {
            screenCanvas.state = Player.State.BIG;
        }
        else // Other players
        {
            screenCanvas.state = Player.State.IDLE;
        }

        if(GameManager.thisPlayer.name.Equals(table.players[current_UTG].name)) // UTG (First Actor)
        {
            screenCanvas.EnableTurn(PieButton.ActionState.CALL_RAISE_FOLD, table.sbChip * 2); // Consider All in here
        }

        screenCanvas.stage = GameTable.Stage.PREFLOP;    
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
    public IEnumerator RoundFinRoutine()
    {
        yield return new WaitForSeconds(2.0f);

        // Init playerCanvas ActionGUI
        foreach(PlayerCanvas canvas in playerCanvas)
        {
            canvas.playerState = Player.State.IDLE;
        }

        // Show Round fin table animation here
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

        /* Round fin table anim */
        yield return new WaitForSeconds(2.0f);

        /* ShowDown anim */
        foreach(Player p in GameManager.gameTable.potWinnerManager.showDown)
        {
            PlayerCanvas pCanvas = GetPlayerCanvasFromName(p.name);
            
            yield return new WaitForSeconds(1.5f);
        }

        /* Showing winner routine */
                
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

}
