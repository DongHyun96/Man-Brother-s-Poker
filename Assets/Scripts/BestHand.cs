using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


public class BestHand
{
    public Hand hand;
    public List<Card> bestHandCombi = new List<Card>();

    public BestHand(List<Card> playerCards, List<Card> communityCards)
    {
        List<Card> cards = new List<Card>();

        cards.AddRange(playerCards);
        cards.AddRange(communityCards);

        // Init bestHand and bestHandCombi
        CalculateBestHand(cards);

    }
    public BestHand() {}

    private void CalculateBestHand(List<Card> cards)
    {

        if(cards.Count != 7)
        {
            return;
        }


        /* Sort Cards */
        cards.sortCards();

        /* Get five combinations */
        List<Card[]> cardCombinations = CardListUtil.GenerateFiveCombi(cards);

        /* Check Royal / Straight flush / flush / straight */
        bool check_flush = true;
        bool check_straight = true;

        foreach(Card[] combi in cardCombinations)
        {
            bool isStraight = IsStraight(combi);
            bool isFlush = IsFlush(combi);

            if(isStraight && isFlush)
            {
                if(combi[0].num == 12) 
                {
                    /* ROYAL */
                    SetFieldValues(Hand.ROYAL_FLUSH, combi);
                    return;
                }
                /* STRAIGHT FLUSH */
                SetFieldValues(Hand.STRAIGHT_FLUSH, combi);
                return;
            }
            
            else if(!isStraight && isFlush && check_flush)
            {
                /* FLUSH */
                SetFieldValues(Hand.FLUSH, combi);

                /* Only check lower numbers of straight flush on upcoming loop */
                check_flush = false;
                check_straight = false;
            }

            else if(isStraight && !isFlush && check_straight)
            {
                /* STRAIGHT */
                SetFieldValues(Hand.STRAIGHT, combi);

                /* Check lower numbers of straight flush or flush on upcoming loop */
                check_straight = false;
            }
        }

        /* Breakout point - If a stright or flush is made, Four of a kind or a full house
        won't be made so break out at this point. */
        if(hand != Hand.HIGHCARD)
        {
            return;
        } 

        Dictionary<int, int> countMap = new Dictionary<int, int>(); // Key: card num, value: count
        foreach(Card c in cards)
        {
            if(!countMap.ContainsKey(c.num))
            {
                countMap.Add(c.num, 1);
            }
            else
            {
                countMap[c.num] += 1;
            }
        }

        /* Check four of a kind */
        foreach(int key in countMap.Keys)
        {
            if(countMap[key] == 4)
            {
                /* FOUR_OF_A_KIND */
                List<Card> temp = new List<Card>();

                // Add four of a kind cards first
                foreach(Card c in cards)
                {
                    if(c.num == key)
                    {
                        temp.Add(c);
                    }
                }
                cards.RemoveAll(card => card.num == key);

                // Add one card(Most highest card left)
                temp.Add(cards[0]);
                SetFieldValues(Hand.FOUR_OF_A_KIND, temp);
                return;
            }
        }

        /* Check Full house / three of a kind / twoPair / pair */
        bool check_triple = true;
        bool check_twoPair = true;
        bool check_pair = true;

        foreach(Card[] combi in cardCombinations)
        {
            Dictionary<int, int> map = new Dictionary<int, int>(); // Key: card num, value: count

            foreach(Card c in combi)
            {
                if(!map.ContainsKey(c.num))
                {
                    map.Add(c.num, 1);
                }
                else
                {
                    map[c.num] += 1;
                }
            }

            var tripleContainer = map.FirstOrDefault(x => x.Value == 3);
            var pairContainer = map.FirstOrDefault(x => x.Value == 2);
            
            if(map.Count == 2) // map --> {(someNum, 3), (someNum, 2)}
            {
                /* FULL_HOUSE */
                int tripleNum = tripleContainer.Key;
                int pairNum = pairContainer.Key;

                List<Card> temp = new List<Card>();
                // Set up three of a kind first and then two pair
                foreach(Card c in combi)
                {
                    if(c.num == tripleNum)
                    {
                        temp.Add(c);
                    }
                }
                foreach(Card c in combi)
                {
                    if(c.num == pairNum)
                    {
                        temp.Add(c);
                    }
                }
                SetFieldValues(Hand.FULLHOUSE, temp);
                return;
            }

            if(tripleContainer.Value != 0 && check_triple)
            {
                /* THREE_OF_A_KIND */

                int tripleNum = tripleContainer.Key;
                List<Card> temp = new List<Card>();
                List<Card> other = new List<Card>();
                // Insert three of a kind first
                foreach(Card c in combi)
                {
                    if(c.num == tripleNum)
                    {
                        temp.Add(c);
                    }
                    else
                    {
                        other.Add(c);
                    }
                }
                temp.AddRange(other);
                
                SetFieldValues(Hand.THREE_OF_A_KIND, temp);
                check_triple = false;
                check_twoPair = false;
                check_pair = false;
                continue;
            }

            if(map.Count == 3 && check_twoPair)
            {
                /* TWO_PAIR */

                List<Card> temp = new List<Card>();
                Card left = new Card();

                // Insert two pair first
                foreach(var kvPair in map)
                {
                    if(kvPair.Value == 2)
                    {
                        foreach(Card c in combi)
                        {
                            if(c.num == kvPair.Key)
                            {
                                temp.Add(c);
                            }
                        }
                    }
                    else // Other card left
                    {
                        foreach(Card c in combi)
                        {
                            if(c.num == kvPair.Key)
                            {
                                left = c;
                            }
                        }
                    }
                }
                temp.Add(left);
                SetFieldValues(Hand.TWOPAIR, temp);
                check_twoPair = false;
                check_pair = false;
                continue;
            }

            if(map.Count == 4 && check_pair)
            {
                /* PAIR */
                
                List<Card> temp = new List<Card>();
                List<Card> other = new List<Card>();

                // Insert pair first
                foreach(Card c in combi)
                {
                    if(c.num == pairContainer.Key)
                    {
                        temp.Add(c);
                    }
                    else
                    {
                        other.Add(c);
                    }
                }
                temp.AddRange(other);

                SetFieldValues(Hand.PAIR, temp);
                check_pair = false;
            }
        }

        /* Breakout point */
        if(hand != Hand.HIGHCARD)
        {
            return;
        }

        /* High Card */
        SetFieldValues(Hand.HIGHCARD, cardCombinations[0]);

    }

	private bool IsStraight(Card[] cards) 
    {
		int current = -1;

        for(int i = 0; i < 5; i++)
        {
            if(current == -1)
            {
                current = cards[i].num;
                continue;
            }

            if(current - 1 == cards[i].num)
            {
                current = cards[i].num;
                continue;
            }

            /* Back straight case exception (A 2 3 4 5)*/
            if(i == 1 && current == 12 && cards[i].num == 3)
            {
                current = cards[i].num;
                continue;
            }
            return false;
        }
        return true;
	}

    private bool IsFlush(Card[] cards)
    {
        Card.Suit current = cards[0].suit;

        for(int i = 1; i < 5; i++)
        {
            if(current == cards[i].suit)
            {
                continue;
            }
            return false;
        }
        return true;
        
    }



    private void SetFieldValues(Hand hand, Card[] cards)
    {
        this.hand = hand;
        this.bestHandCombi = new List<Card>();
        this.bestHandCombi.AddRange(cards);
    }
    private void SetFieldValues(Hand hand, List<Card> cards)
    {
        this.hand = hand;
        this.bestHandCombi = cards;
    }

    /* This will set proper orders of best hand combinations for  */
/*     private List<Card> GetOrderOfCombi(Hand hand, Dictionary<int, int> map, List<Card> cards)
    {
        List<Card> temp = new List<Card>();
        switch(hand)
        {
            case Hand.FOUR_OF_A_KIND:
                // Add four of a kind cards first
                foreach(Card c in cards)
                {
                    if(c.num == key)
                    {
                        temp.Add(c);
                    }
                }
                break;
            case Hand.FULLHOUSE:
                break;
            case Hand.THREE_OF_A_KIND:
                break;
            case Hand.TWOPAIR:
                break;
            case Hand.PAIR:
                break;
            default:
                return cards;
        }
        return null;
    } */

}