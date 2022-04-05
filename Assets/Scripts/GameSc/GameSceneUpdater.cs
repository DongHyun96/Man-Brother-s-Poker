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
    public void UpdateGameScene()
    {

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
            if(canvas.name.Equals(table.players[current_UTG].name)) // UTG (First Actor)
            {
                canvas.playerState = Player.State.IDLE;
                canvas.EnableTurn();
            }

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

        // Set up ScreenCanvas
        if(GameManager.thisPlayer.name.Equals(table.players[current_UTG].name)) // UTG (First Actor)
        {
            screenCanvas.state = Player.State.IDLE;
            screenCanvas.EnableTurn(PieButton.ActionState.CALL_RAISE_FOLD, table.sbChip * 2); // Consider All in here
        }

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
        screenCanvas.stage = GameTable.Stage.PREFLOP;
        
    }

}
