using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

public class GameTable
{
    public Guid id; // Same id with Room id

    public List<Player> players = new List<Player>();
    public int registerCount;
    
    public int iterPos;
    public int UTG; // For the next round start

    /*
    * ROUND_FIN : when round is finished(proflop, flop...)
    * POT_FIN : when pot is finished
    * GAME_FIN : When game over
    * ------------------------------------------------------------------------------
    * PREFLOP -> ROUND_FIN -> FLOP -> ROUND_FIN -> ... -> POT_FIN -> ... -> GAME_FIN
    * ------------------------------------------------------------------------------
    */

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Stage{
        PREFLOP, FLOP, TURN, RIVER, ROUND_FIN, UNCONTESTED, POT_FIN, GAME_FIN
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TableStatus{
        //IDLE, CHECK, BET ,ALLIN, FINISHED, FINAL_WINNER
        IDLE, CHECK, BET, ALLIN
    }
    
    public Room.Mode mode;

    private Stage m_stage;
    private TableStatus m_tableStatus;

    /*
    * Update table contents through these property.
    */
    //[JsonConverter(typeof(StringEnumConverter))]
    public Stage stage
    {
        get => m_stage;

        set
        {
            Debug.Log(tableStatus);
            if(tableStatus != TableStatus.IDLE)
            {
                return;
            }

            Debug.Log("ENTERING STAGE SETTER");
            switch(value)
            {
                case Stage.PREFLOP:
                    Init_Preflop();
                    break;
                case Stage.FLOP:
                    break;
                case Stage.TURN:
                    break;
                case Stage.RIVER:
                    break;
                case Stage.ROUND_FIN:
                    break;
                case Stage.POT_FIN:
                    break;
                case Stage.GAME_FIN:
                    break;
            }
            m_stage = value;
        }
    }
    
    //[JsonConverter(typeof(StringEnumConverter))]
    public TableStatus tableStatus
    {
        get => m_tableStatus;

        set
        {
            //switch()
            m_tableStatus = value;
        }
    }


    public List<Card> deck = new List<Card>();
    public List<Card> communityCards = new List<Card>();

    public int pot;
    public int sbChip;
    public int roundBetMax;
    
    public GameTable() {}

    public GameTable(Guid id, List<Player> players, Room.Mode mode, Room.BuyIn buyIn)
    {
        this.id = id;
        this.players = players;
        this.mode = mode;
        InitBuyIn_And_Sb(buyIn);
    }

    /****************************************************************************************************************
    *                                 Initialization methods (table init / stage init)
    ****************************************************************************************************************/
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

    private void Init_Preflop()
    {
        // Init players
        foreach(Player p in players)
        {
            p.state = Player.State.IDLE;
            p.roundBet = 0;
            p.totalBet = 0;

            p.cards.Clear();
        }

        // Init table fields
        tableStatus = TableStatus.IDLE;
        communityCards.Clear();
        pot = 0;

        // Small, big betting
        TakeAction(players[GetPrev(GetPrev(UTG))].name, Player.State.BET, sbChip);
        TakeAction(players[GetPrev(UTG)].name, Player.State.RAISE, sbChip * 2);

        // Draw cards to player
        DrawCard(); // Remove first cards
        for(int i = 0; i < 2; i++)
        {
            foreach(Player p in players)
            {
                p.cards.Add(DrawCard());
            }
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

            if(players[tempIter].state != Player.State.FOLD && players[tempIter].state != Player.State.ALLIN)
            {
                return tempIter;
            }
        }
        // tempIter reaches 0
        tempIter = players.Count;

        while(tempIter != inputPos)
        {
            tempIter--;
            if(players[tempIter].state != Player.State.FOLD && players[tempIter].state != Player.State.ALLIN)
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

            if(players[tempIter].state != Player.State.FOLD && players[tempIter].state != Player.State.ALLIN)
            {
                return tempIter;
            }
        }
        // tempIter reaches top
        tempIter = -1;

        while(tempIter != inputPos)
        {
            tempIter++;
            if(players[tempIter].state != Player.State.FOLD && players[tempIter].state != Player.State.ALLIN)
            {
                return tempIter;
            }
        }

        return tempIter;
    }

    public Player GetCurrentPlayer()
    {
        return players[iterPos];
    }

    private void Next()
    {
        iterPos = GetNext(iterPos);
    }
    /****************************************************************************************************************
    *                                                Deck methods
    ****************************************************************************************************************/
    private void InitDeck()
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

    private Card DrawCard()
    {
        Card card = deck[0];
        deck.RemoveAt(0);
        return card;
    }
    /****************************************************************************************************************
    *                                                Action methods
    ****************************************************************************************************************/
    // Action by some player
    public void TakeAction(string actor, Player.State action, int chips = 0)
    {
        Player player = GetPlayerByName(actor);
        if(player == null)
        {
            return;
        }

        // Update player and gameTable status or stage
        switch(action)
        {
            case Player.State.CHECK:
                player.Check(); // Update player
                tableStatus = TableStatus.CHECK; // Update gameTable
                break;
            case Player.State.BET:

                // Update player
                player.Bet(chips);

                // Update gameTable
                tableStatus = TableStatus.BET;
                pot += player.roundBet;
                roundBetMax = player.roundBet;

                break;
            case Player.State.CALL:

                // Update gameTable's pot before updating player's roundBet
                pot += (player.totalChips <= chips) ? player.totalChips - player.roundBet : chips - player.roundBet;
                
                player.Call(chips); // Update player

                break;
            case Player.State.RAISE:
                // Update gameTable's pot before updating player's roundBet
                pot += (player.totalChips <= chips) ? player.totalChips - player.roundBet : chips - player.roundBet;

                player.Raise(chips);

                roundBetMax = player.roundBet;
            
                break;
            case Player.State.ALLIN:
                pot += player.totalChips - player.roundBet;
                player.AllIn();
                
                roundBetMax = (roundBetMax < player.roundBet) ? player.roundBet : roundBetMax;

                break;
            case Player.State.FOLD:
                player.Fold();

                // Check uncontested
                int cnt = 0;
                foreach(Player p in players)
                {
                    cnt += p.state == Player.State.FOLD ? 1 : 0;
                }
                if(cnt == players.Count - 1)
                {
                    stage = Stage.UNCONTESTED;
                    return;
                }
                break;
        }

        // check if the table round is over
        if(IsRoundOver())
        {
            // Check if the table status is ALLIN
            int foldCnt = 0;
            int allInCnt = 0;
            foreach(Player p in players)
            {
                foldCnt = p.state == Player.State.FOLD ? foldCnt + 1 : foldCnt;
                allInCnt = p.state == Player.State.ALLIN ? allInCnt + 1 : allInCnt;
            }

            if(players.Count - foldCnt - allInCnt <= 1)
            {
                tableStatus = TableStatus.ALLIN;
                return;
            }
            else 
            {
                // ROUND_FIN
                stage = Stage.ROUND_FIN;
                return;
            }
        }

        // Update iterpos
        Next();
        
    }
    /****************************************************************************************************************
    *                                                Boolean methods
    ****************************************************************************************************************/

    // Check whether PREFLOP, FLOP... is over
    private bool IsRoundOver()
    {
        switch(tableStatus)
        {
            case TableStatus.IDLE:
                return false;
            case TableStatus.CHECK:
                foreach(Player p in players)
                {
                    if(p.state != Player.State.FOLD && p.state != Player.State.CHECK)
                    {
                        return false;
                    }
                }
                return true; // When everyone checked except fold
            case TableStatus.BET:
                int bet = -1;
                
                foreach(Player p in players)
                {
                    if(p.state != Player.State.FOLD && p.state != Player.State.ALLIN)
                    {
                        if(bet == -1)
                        {
                            bet = p.roundBet;
                        }
                        else if(p.roundBet != bet)
                        {
                            return false;
                        }
                    }
                }
                return true; // When every player's bet is same except fold and allIn(side pot)
            default:
                return false; //Dummy
        }
    }

    /****************************************************************************************************************
    *                                                     Extra
    ****************************************************************************************************************/
    public Player GetPlayerByName(string name)
    {
        foreach(Player p in players)
        {
            if(p.name.Equals(name))
            {
                return p;
            }
        }
        return null;
    }

    
}
