using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotWinnerManager
{

    /* (potMoney, pot involvedPlayers) --> BUTTOM / (MainPot, players), (sidepot1, players), ... /TOP*/
    public Stack<KeyValuePair<int, List<Player>>> pots = new Stack<KeyValuePair<int, List<Player>>>();

    /* Pot winners --> (LastSidePot, winners) ... (MainPot, winners)*/
    public Stack<KeyValuePair<int, List<Player>>> potWinnerStack = new Stack<KeyValuePair<int, List<Player>>>();


    public PotWinnerManager(List<Player> players)
    {
        CalculatePots(players);
        CalculateWinners(pots);
    }

    public PotWinnerManager() {}

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

            /* pots.Add(new KeyValuePair<int, List<Player>>(potMoney, involved)); */
            pots.Push(new KeyValuePair<int, List<Player>>(potMoney, involved));

            /* Take out the current pot bet from players' totalBet */
            for(int j = i; j < players.Count; j++)
            {
                players[j].totalBet -= currentPotBet;
            }
        }
    }

    /* Set up winners */
    private void CalculateWinners(Stack<KeyValuePair<int, List<Player>>> potStack)
    {
        while(potStack.Count != 0)
        {
            KeyValuePair<int, List<Player>> kvPair = potStack.Pop();

            BestHand currentBest = new BestHand();
            List<Player> winners = new List<Player>();

            /* Check current pot winners */
            foreach(Player p in kvPair.Value)
            {
                if(currentBest.bestHandCombi.Count == 0)
                {
                    currentBest = p.bestHand;
                    winners.Add(p);
                    continue;
                }

                if(p.bestHand.hand > currentBest.hand)
                {
                    /* New winner */
                    winners.Clear();
                    winners.Add(p);
                    continue;
                }
                if(p.bestHand.hand == currentBest.hand)
                {
                    bool isSplit = true;

                    /* Check through all the cards in bestHandCombi */
                    for(int i = 0; i < 5; i++)
                    {
                        int currentWinner_CardNum = winners[0].bestHand.bestHandCombi[i].num;
                        int challenger_CardNum = p.bestHand.bestHandCombi[i].num;

                        if(currentWinner_CardNum < challenger_CardNum)
                        {
                            /* New winner */
                            winners.Clear();
                            winners.Add(p);
                            isSplit = false;
                            break;
                        }
                        if(currentWinner_CardNum > challenger_CardNum)
                        {
                            isSplit = false;
                            break;
                        }
                    }

                    /* Split */
                    if(isSplit)
                    {
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


}
