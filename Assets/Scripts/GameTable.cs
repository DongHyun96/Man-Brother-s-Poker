using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameTable
{
    public Guid id; // Same id with Room id

    public List<Player> players;
    public int registerCount;
    
    public int iterPos;

    public enum Stage{
        PREFLOP, FLOP, TURN, RIVER, FIN
    }
    public enum TableStatus{
        IDLE, CHECK, BET ,ALLIN, FINISHED, FINAL_WINNER
    }
    
    public Room.Mode mode;
    public Stage stage;
    public TableStatus tableStatus;

    public List<Card> deck;
    public List<Card> communityCards;

    public int sbChip;
    
    public GameTable() {}

    public GameTable(Guid id, List<Player> players, Room.Mode mode, Room.BuyIn buyIn)
    {
        this.id = id;
        this.players = players;
        this.mode = mode;
        InitBuyIn_Sb(buyIn);
    }

    private void InitBuyIn_Sb(Room.BuyIn buyIn)
    {
        switch(buyIn)
        {
            case Room.BuyIn.ONE:
                foreach(Player p in players)
                {
                    p.totalChips = 1000;
                }
                sbChip = 5;
                break;
            case Room.BuyIn.FIVE:
                foreach(Player p in players)
                {
                    p.totalChips = 5000;
                }
                sbChip = 25;
                break;

            case Room.BuyIn.TEN:
                foreach(Player p in players)
                {
                    p.totalChips = 10000;
                }   
                sbChip = 50;
                break;

            case Room.BuyIn.TWENTY:
                foreach(Player p in players)
                {
                    p.totalChips = 20000;
                }
                sbChip  = 100;
                break;

            case Room.BuyIn.FIFTY:
                foreach(Player p in players)
                {
                    p.totalChips = 50000;
                }
                sbChip  = 250;
                break;

            case Room.BuyIn.HUNDRED:
                foreach(Player p in players)
                {
                    p.totalChips = 100000;
                }
                sbChip  = 500;
                break;

            default:
                break;
        }
    }
}
