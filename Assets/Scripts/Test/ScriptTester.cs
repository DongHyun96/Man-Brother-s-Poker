using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.Linq;
using System.Diagnostics;
using UnityEngine.UI;


public class ScriptTester : MonoBehaviour
{

    private List<Card> deck = new List<Card>();
    private List<Card> comcards = new List<Card>();

    [SerializeField]
    private List<Image> comCardsUI = new List<Image>();

    //public List<Image[]> playerCardsUI = new List<Image[]>();

    [SerializeField]
    // private List<SubPlaceHolder> playerCardsUI = new List<SubPlaceHolder>();
    private SubPlaceHolder[] playerCardsUI = new SubPlaceHolder[6];
    
    public GameObject exampleGameObj;

    public GameObject chips;

    // Start is called before the first frame update
    void Start()
    {

        /* Stack<int> stack = new Stack<int>();

        stack.Push(0);
        stack.Push(1);
        stack.Push(2);
        stack.Push(3);
        stack.Push(4);

        int[] intArray = stack.ToArray();

        foreach(int i in intArray)
        {
            print(i);
        } */

        List<Card> cards = new List<Card>();
        cards.Add(new Card(Card.Suit.CLUB, 0));
        cards.Add(new Card(Card.Suit.CLUB, 1));
        cards.Add(new Card(Card.Suit.CLUB, 2));

        List<Card> newCards = cards.ConvertAll(c => new Card(c.suit, c.num));
        newCards[0].num = 100;
        print(cards[0].num);
    }

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
        CardListUtil.Shuffle<Card>(deck);
    }

    private Card DrawCard()
    {
        Card card = deck[0];
        deck.RemoveAt(0);
        return card;
    }

    private string GetChipString(int chips)
    {
        return (chips < 1000) ? chips.ToString() : (Math.Round(chips / 1000f, 2)).ToString() + "k";
    }

    private void GiveCardToRiver(List<Player> players)
    {
        InitDeck();

        /* Draw players' cards */
        DrawCard();
        for(int i = 0; i < 2; i++)
        {
            int idx = 0;
            foreach(Player p in players)
            {
                p.cards.Add(DrawCard());
                playerCardsUI[idx].map[i].sprite = CardSprite.GetInstance().GetSprite(p.cards[i]);
                idx++;
            }
        }

        /* ComCards */
        DrawCard();

        /* Flop */
        for(int i = 0; i < 3; i++)
        {
            comcards.Add(DrawCard());
        }

        /* Turn */
        DrawCard();
        comcards.Add(DrawCard());

        /* River */
        DrawCard();
        comcards.Add(DrawCard());

        for(int i = 0; i < comcards.Count; i++)
        {
            comCardsUI[i].sprite = CardSprite.GetInstance().GetSprite(comcards[i]);
        }


        }
    
    private IEnumerator ExampleCoroutine()
    {
        print("Hello1");
        yield return StartCoroutine(ExampleCoroutine2());
        print("Hello2");
    }

    private IEnumerator ExampleCoroutine2()
    {
        print("ExampleCoroutine2 - breakpoint 1");
        yield return StartCoroutine(ExampleCoroutine3());
        print("ExampleCoroutine2 - breakpoint 2");
    }

    private IEnumerator ExampleCoroutine3()
    {
        print("ExampleCoroutine 3 - breakpoint 1");
        yield return StartCoroutine(ExampleCoroutine4());
        print("ExampleCoroutine 3 - breakPoint 2");
    }

    private IEnumerator ExampleCoroutine4()
    {
        print("Entering rock bottom");
        yield return null;
    }
    
}
