using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotWinnerManager
{

    /* (potMoney, pot involvedPlayers) --> BUTTOM / (MainPot, players), (sidepot1, players), ... /TOP */
    public List<KeyValuePair<int, List<Player>>> pots = new List<KeyValuePair<int, List<Player>>>();

    /* Contains each Pot winners --> (LastSidePot, winners) ... (MainPot, winners) */
    public Stack<KeyValuePair<int, List<Player>>> potWinnerStack = new Stack<KeyValuePair<int, List<Player>>>();

    /* Show down order */
    public List<Player> showDown = new List<Player>();

    public PotWinnerManager() {}

    public PotWinnerManager(List<Player> players)
    {
        HandleBackStraightException(players); // Change back straight ace number to -1(Lowest)

        CalculatePots(players);

        CalculateWinners(pots);
    }

    /* Set up the potMaps */
    private void CalculatePots(List<Player> players)
    {
        /* Sort players by totalBet */
        players = GetSortedPlayersByTotalBet(players);

        /* Get PotMap */
        for(int i = 0; i < players.Count; i++)
        {
            if(players[i].totalBet == 0)
            {
                continue;
            }

            /* Get potMoney and involved players */
            int currentPotBet = players[i].totalBet;
            int potMoney = currentPotBet * (players.Count - i);

            List<Player> involved = new List<Player>();
            involved.AddRange(players.GetRange(i, players.Count - i));

            pots.Add(new KeyValuePair<int, List<Player>>(potMoney, involved));
            // pots.Push(new KeyValuePair<int, List<Player>>(potMoney, involved));

            /* Take out the current pot bet from players' totalBet */
            for(int j = i; j < players.Count; j++)
            {
                players[j].totalBet -= currentPotBet;
            }
        }
    }

    /* Set up winners and showDown players */
    private void CalculateWinners(List<KeyValuePair<int, List<Player>>> pots)
    {
        for(int i = pots.Count - 1; i >= 0; i--)
        {
            KeyValuePair<int, List<Player>> kvPair = pots[i];

            BestHand currentBest = new BestHand();
            List<Player> winners = new List<Player>();

            /* Delete players who folded or lost previous side pot */
            kvPair.Value.RemoveAll(x => x.state == Player.State.FOLD);

            /* Check current pot winners */
            foreach(Player p in kvPair.Value)
            {
                if(currentBest.bestHandCombi.Count == 0)
                {
                    /* Init */
                    currentBest = p.bestHand;
                    winners.Add(p);
                    TryAddToShowDown(p);
                    continue;
                }

                if(p.bestHand.hand > currentBest.hand)
                {
                    /* New winner */
                    currentBest = p.bestHand;
                    winners.Clear();
                    winners.Add(p);
                    TryAddToShowDown(p);
                    continue;
                }

                /* Same Hand ranking */
                if(p.bestHand.hand == currentBest.hand)
                {
                    bool isSplit = true;

                    /* Check through all the cards in bestHandCombi */
                    for(int j = 0; j < 5; j++)
                    {
                        int currentWinner_CardNum = winners[0].bestHand.bestHandCombi[j].num;
                        int challenger_CardNum = p.bestHand.bestHandCombi[j].num;

                        if(currentWinner_CardNum < challenger_CardNum)
                        {
                            /* New winner */
                            currentBest = p.bestHand;
                            winners.Clear();
                            winners.Add(p);
                            TryAddToShowDown(p);
                            isSplit = false;
                            break;
                        }
                        if(currentWinner_CardNum > challenger_CardNum)
                        {
                            /* Challenger lost */
                            isSplit = false;
                            break;
                        }
                        
                    }

                    /* Split */
                    if(isSplit)
                    {
                        TryAddToShowDown(p);
                        winners.Add(p);
                    }
                }
                
            }
            KeyValuePair<int, List<Player>> potWinnerPair = new KeyValuePair<int, List<Player>>(kvPair.Key, winners);
            potWinnerStack.Push(potWinnerPair);
        }
    }


    private List<Player> GetSortedPlayersByTotalBet(List<Player> players)
    {
        players.Sort(
            (x, y) =>
                x == null ? (y == null ? 0 : -1) : (y == null ? 1 : x.totalBet.CompareTo(y.totalBet))
        );

        return players;
    }


    /* Handling back straight exception.
    Change Ace number to -1  */
    private void HandleBackStraightException(List<Player> players)
    {
        foreach(Player p in players)
        {
            if(p.bestHand.hand != Hand.STRAIGHT_FLUSH && p.bestHand.hand != Hand.STRAIGHT)
            {
                continue;
            }

            /* Back straight case */
            // Ace 5 4 3 2 --> Ace(12) to 1(-1)
            if(p.bestHand.bestHandCombi[0].num == 12 && p.bestHand.bestHandCombi[1].num == 3)
            {
                int idx = 0;
                for(int newNum = 3; newNum >= -1; newNum--)
                {
                    p.bestHand.bestHandCombi[idx].num = newNum;
                    idx++;
                }           
            }
        }
    }

    private void TryAddToShowDown(Player p)
    {
        if(showDown.Contains(p))
        {
            return;
        }
        showDown.Add(p);
    }

    public void PayEachPotWinners()
    {
        while(potWinnerStack.Count != 0)
        {
            KeyValuePair<int, List<Player>> pot = potWinnerStack.Pop();

            foreach(Player p in pot.Value)
            {
                p.totalChips += pot.Key / pot.Value.Count;
            }
        }
    }




}
