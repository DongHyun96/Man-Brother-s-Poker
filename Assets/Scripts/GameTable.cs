using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameTable
{
    public Guid id; // Same id with Room id

    public List<Player> players;
    
    public int iterPos;

    public enum Stage{
        PREFLOP, FLOP, TURN, RIVER, FIN
    }
    public enum TableStatus{
        IDLE, CHECK, BET ,ALLIN, FINISHED, FINAL_WINNER
    }
    
    public Stage stage;
    public TableStatus tableStatus;

    public List<Card> deck;
    public List<Card> communityCards;
    
    
}
