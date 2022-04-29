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
    public int SB_Pos; // For the next round start

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
        // IDLE, CHECK, BET ,ALLIN, FINISHED, FINAL_WINNER
        IDLE, CHECK, BET, ALLIN
    }
    
    public Room.Mode mode;

    [JsonProperty("stage")]
    private Stage m_stage;
    [JsonProperty("tableStatus")]
    private TableStatus m_tableStatus;

    /*
    * Update table contents through these property.
    */
    [JsonIgnore]
    public Stage stage
    {
        get => m_stage;
        
        set
        {
            /* if(tableStatus != TableStatus.IDLE)
            {
                return;
            } */
            switch(value)
            {
                case Stage.PREFLOP:
                    Init_Preflop();
                    break;
                case Stage.FLOP:
                    Init_Flop();
                    break;
                case Stage.TURN:
                    Init_Turn();
                    break;
                case Stage.RIVER:
                    Init_River();
                    break;
                case Stage.UNCONTESTED:
                    Init_Uncontested();
                    break;
                case Stage.ROUND_FIN:
                    break;
                case Stage.POT_FIN:
                    Init_PotFin();
                    break;
                case Stage.GAME_FIN:
                    Init_GameFin();
                    break;
            }
            m_stage = value;
        }
    }
    
    [JsonIgnore]
    public TableStatus tableStatus
    {
        get => m_tableStatus;
        set
        {
            if(value == TableStatus.ALLIN)
            {
                /* Get ShowDown order */

                /* Draw cards to community card till the end */

                /* 정산 */
            }
            m_tableStatus = value;
        }
    }

    [JsonIgnore]
    public List<Card> deck = new List<Card>();

    public List<Card> communityCards = new List<Card>();

    public int pot;
    public int sbChip;
    public int roundBetMax;

    public PotWinnerManager potWinnerManager;

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
                sbChip = 250;
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
        Debug.Log("Entering preflop");
        
        // Init players
        foreach(Player p in players)
        {
            p.state = Player.State.IDLE;
            p.roundBet = 0;
            p.totalBet = 0;

            p.cards.Clear();
        }

        // init new SB_Pos and iterPos(UTG) / Have to consider player's broken or not
        SB_Pos = GetNext(SB_Pos);
        iterPos = GetNext(GetNext(SB_Pos));

        // Init table fields
        tableStatus = TableStatus.IDLE;
        communityCards.Clear();
        pot = 0;

        // Small, big betting
        Player small = players[SB_Pos];
        Player big = players[GetNext(SB_Pos)];

        small.Bet(sbChip);
        tableStatus = TableStatus.BET;
        pot += small.roundBet;

        pot += (big.totalChips <= sbChip * 2) ? big.totalChips - big.roundBet : sbChip * 2 - big.roundBet;
        big.Raise(sbChip * 2);
        roundBetMax = big.roundBet;
        
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

    private void Init_Flop()
    {
        Debug.Log("Entering Flop");

        FlopTurnRiverRoutine();

        /* Draw Cards */
        DrawCard(); // Remove first card
        for(int i = 0; i < 3; i++)
        {
            communityCards.Add(DrawCard());
        }
        
    }

    private void Init_Turn()
    {
        Debug.Log("Entering Turn");
        FlopTurnRiverRoutine();

        /* Draw Card */
        DrawCard();
        communityCards.Add(DrawCard()); 
    }

    private void Init_River()
    {
        Debug.Log("Entering River");
        FlopTurnRiverRoutine();

        /* Draw Card */
        DrawCard();
        communityCards.Add(DrawCard());
    }

    private void FlopTurnRiverRoutine()
    {
        /* Init players */
        foreach(Player p in players)
        {
            if(p.state != Player.State.ALLIN && p.state != Player.State.FOLD)
            {
                p.state = Player.State.IDLE;
            }
            p.roundBet = 0;
        }

        /* Init table fields */
        tableStatus = TableStatus.IDLE;

        /* Set iterPos */
        iterPos = (players[SB_Pos].state != Player.State.FOLD && players[SB_Pos].state != Player.State.ALLIN)
         ? SB_Pos : GetNext(SB_Pos);
        roundBetMax = 0;
    }

    private void Init_Uncontested()
    {
        // One pot winner
        foreach(Player p in players)
        {
            if(p.state != Player.State.FOLD)
            {
                p.totalChips += pot;
                return;
            }
        }
    }

    private void Init_PotFin()
    {
        /* Get Players' best hand */
        foreach(Player p in players)
        {
            p.bestHand = new BestHand(p.cards, communityCards);
        }

        /* Calculate PotWinner */
        potWinnerManager = new PotWinnerManager(players);
        

        /* potWinnerManager.PayEachPotWinners(); */
    }
    private void Init_GameFin()
    {
        throw new NotImplementedException();
    }
    /****************************************************************************************************************
    *                                                Iterator methods
    ****************************************************************************************************************/
    public int GetPrev(int inputPos) // Returns the first previous player with status != FOLD, ALLIN
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
                // Check if it is uncontested
                if(IsUncontested())
                {
                    stage = Stage.UNCONTESTED;
                    return;
                }
                break;
        }

        // check if the table round is over
        if(IsRoundOver())
        {
            // Check if it is uncontested
            if(IsUncontested())
            {
                stage = Stage.UNCONTESTED;
                return;
            }

            // Check if the pot is over
            if(IsPotOver())
            {
                Debug.Log("POT FIN ENTERED!");
                stage = Stage.POT_FIN;
                return;
            }

            // Check if the table status is ALLIN
            int foldCnt = 0;
            int allInCnt = 0;
            foreach(Player p in players)
            {
                foldCnt += p.state == Player.State.FOLD ? 1 : 0;
                allInCnt += p.state == Player.State.ALLIN ? 1 : 0;
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
                
                // PREFLOP case exception(Big blind check)
                if(stage == Stage.PREFLOP && iterPos == GetNext(SB_Pos))
                {
                    return true;
                }
                
                foreach(Player p in players)
                {
                    if(p.state != Player.State.FOLD && p.state != Player.State.CHECK)
                    {
                        return false;
                    }
                }
                return true; // When everyone checked except fold
            case TableStatus.BET:

                // PREFLOP case exception (After small blind action)
                if(stage == Stage.PREFLOP && roundBetMax == sbChip * 2 && iterPos == SB_Pos)
                {
                    return false;
                }

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

    private bool IsUncontested()
    {
        int cnt = 0;

        foreach(Player p in players)
        {
            cnt += p.state == Player.State.FOLD ? 1 : 0;
        }

        if(cnt == players.Count - 1)
        {
            return true;
        }
        return false;
    }

    /* Premise - IsRoundOver checked already */
    private bool IsPotOver()
    {
        if(stage == Stage.RIVER)
        {
            return true;
        }
        return false;
    }
    /****************************************************************************************************************
    *                                                Pot related methods
    ****************************************************************************************************************/
    public bool IsInShowDown(string name)
    {
        if(potWinnerManager == null)
        {
            return false;
        }

        foreach(Player p in potWinnerManager.showDown)
        {
            if(name.Equals(p.name))
            {
                return true;
            }
        }
        return false;
    }

    public void PayEachPotWinners()
    {
        /* Uncontested */
        if(stage == Stage.UNCONTESTED)
        {
            foreach(Player p in players)
            {
                if(p.state != Player.State.FOLD)
                {
                    p.totalChips += pot;
                }
            }
        }
        /* All round fin */
        if(potWinnerManager == null)
        {
            return;
        }

        while(potWinnerManager.potWinnerStack.Count != 0)
        {
            KeyValuePair<int ,List<Player>> potPair = potWinnerManager.potWinnerStack.Pop();

            foreach(Player p in potPair.Value)
            {
                GetPlayerByName(p.name).totalChips += potPair.Key / potPair.Value.Count;
            }
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

    // 0 3 4 5
    public void UpdateToNextRound()
    {
        stage = communityCards.Count == 0 ? Stage.FLOP :
        (communityCards.Count == 3) ? Stage.TURN :
        (communityCards.Count == 4) ? Stage.RIVER : Stage.POT_FIN;
    }

}
