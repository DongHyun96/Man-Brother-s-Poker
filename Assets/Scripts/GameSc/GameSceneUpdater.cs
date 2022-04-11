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
        PlayerCanvas targetCanvas = GetCanvasFromName(p.name);
        targetCanvas.playerState = p.state; // Update sender's canvas
        UpdateTabs(); // Update rankings

        /* Update screenCanvas */
        screenCanvas.state = p.state;

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
                    int big = table.GetPrev(table.UTG);

                    // Big blind PREFLOP case
                    if(table.stage == GameTable.Stage.PREFLOP && table.players[big].Equals(table.GetCurrentPlayer()))
                    {
                        if(table.roundBetMax == table.GetCurrentPlayer().roundBet)
                        {
                            // All players didn't raised
                            screenCanvas.EnableTurn(PieButton.ActionState.CHECK_RAISE_FOLD, table.roundBetMax);
                        }
                    }
                    else
                    {
                        PieButton.ActionState a = table.GetCurrentPlayer().totalChips <= table.roundBetMax ?
                        PieButton.ActionState.ALLIN_FOLD : PieButton.ActionState.CALL_RAISE_FOLD;
                        screenCanvas.EnableTurn(a, table.roundBetMax);
                    }

                    break;
                //case GameTable.TableStatus.ALLIN:

            }
        }
        
        GetCanvasFromName(table.GetCurrentPlayer().name).EnableTurn(); // Enable timer from playerCanvas
        

    }

    public void StartRound() // GameTable and player all set! Init the Game scene
    {
        GameTable table = GameManager.gameTable;
        int current_UTG = table.UTG;
        int bigPos = table.GetPrev(current_UTG);
        int smallPos = table.GetPrev(bigPos);

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

    /****************************************************************************************************************
    *                                                Extra methods
    ****************************************************************************************************************/
    private PlayerCanvas GetCanvasFromName(string name)
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
            PlayerCanvas c = GetCanvasFromName(p.name);
            c.UpdateTab(p);
        }
    }

}
