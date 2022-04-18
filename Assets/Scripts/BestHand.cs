using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


public class BestHand
{
    public Hand bestHand;
    public List<Card> bestHandCombi = new List<Card>();

    public BestHand(List<Card> playerCards, List<Card> communityCards)
    {
        List<Card> cards = new List<Card>();

        cards.AddRange(playerCards);
        cards.AddRange(communityCards);

        // Init bestHand and bestHandCombi
        CalculateBestHand(cards);

    }

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
        if(bestHand != Hand.HIGHCARD)
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

                // Add four of a kind card first
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
        bool check_three = true;
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

            if(map.Count == 2) // map --> {(someNum, 3), (someNum, 2)}
            {
                /* Full house */
                int twoPairNum = map.FirstOrDefault(x => x.Value == 2).Key;
                int tripleNum = map.FirstOrDefault(x => x.Value == 3).Key;

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
                    if(c.num == twoPairNum)
                    {
                        temp.Add(c);
                    }
                }
                SetFieldValues(Hand.FULLHOUSE, temp);
                return;
            }
        } 
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
        this.bestHand = hand;
        this.bestHandCombi = new List<Card>();
        this.bestHandCombi.AddRange(cards);
    }
    private void SetFieldValues(Hand hand, List<Card> cards)
    {
        this.bestHand = hand;
        this.bestHandCombi = cards;
    }

}