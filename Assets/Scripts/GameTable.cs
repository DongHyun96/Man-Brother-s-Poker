using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameTable
{
    public Guid id; // Same id with Room id

    public List<Player> players = new List<Player>();
    public int registerCount;
    
    public int iterPos;
    public int UTG; // For the next round start

    public enum Stage{
        PREFLOP, FLOP, TURN, RIVER, FIN
    }
    public enum TableStatus{
        //IDLE, CHECK, BET ,ALLIN, FINISHED, FINAL_WINNER
        IDLE, CHECK, BET, ALLIN
    }
    
    public Room.Mode mode;
    public Stage stage;
    public TableStatus tableStatus;

    public List<Card> deck = new List<Card>();
    public List<Card> communityCards = new List<Card>();

    public int pot;
    public int sbChip;
    
    public GameTable() {}

    public GameTable(Guid id, List<Player> players, Room.Mode mode, Room.BuyIn buyIn)
    {
        this.id = id;
        this.players = players;
        this.mode = mode;
        InitBuyIn_And_Sb(buyIn);
    }

    public void Init_Preflop()
    {
        // Init players
        foreach(Player p in players)
        {
            p.state = Player.State.IDLE;
            p.stageBet = 0;
            p.totalBet = 0;

            p.cards.Clear();
        }

        // Draw cards to player
        DrawCard(); // Remove first cards
        for(int i = 0; i < 2; i++)
        {
            foreach(Player p in players)
            {
                p.cards.Add(DrawCard());
            }
        }

        // Init table fields
        stage = Stage.PREFLOP;
        tableStatus = TableStatus.IDLE;
        communityCards.Clear();
        pot = 0;
        
    }

    private void InitBuyIn_And_Sb(Room.BuyIn buyIn)
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
    /****************************************************************************************************************
    *                                                Iterator methods
    ****************************************************************************************************************/
    public int GetPrev(int inputPos) // Returns the first previous player with status != FOLD
    {
        int tempIter = inputPos;
        while(tempIter > 0)
        {
            tempIter--;

            if(players[tempIter].state != Player.State.FOLD)
            {
                return tempIter;
            }
        }
        // tempIter reaches 0
        tempIter = players.Count;

        while(tempIter != inputPos)
        {
            tempIter--;
            if(players[tempIter].state != Player.State.FOLD)
            {
                return tempIter;
            }
        }

        return tempIter;
    }

    public int GetNext(int inputPos)
    {
        int tempIter = inputPos;
        while(tempIter < players.Count - 1)
        {
            tempIter++;

            if(players[tempIter].state != Player.State.FOLD)
            {
                return tempIter;
            }
        }
        // tempIter reaches top
        tempIter = -1;

        while(tempIter != inputPos)
        {
            tempIter++;
            if(players[tempIter].state != Player.State.FOLD)
            {
                return tempIter;
            }
        }

        return tempIter;
    }

    public void Next()
    {
        iterPos = GetNext(iterPos);
    }




    /****************************************************************************************************************
    *                                                Deck methods
    ****************************************************************************************************************/
    public void InitDeck()
    {
        // Init deck
        deck.Clear();

        for(int i = 0; i < 13; i++) // Add each suit cards
        {
            deck.Add(new Card(Card.Suit.CLUB, i));
            deck.Add(new Card(Card.Suit.DIAMOND, i));
            deck.Add(new Card(Card.Suit.HEART, i));
            deck.Add(new Card(Card.Suit.SPADE, i));
        }

        // Shuffle deck
        ListShuffler.Shuffle<Card>(deck);
    }

    public Card DrawCard()
    {
        Card card = deck[0];
        deck.RemoveAt(0);
        return card;
    }

    
}
