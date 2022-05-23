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
            /*
            MainMessage message = new MainMessage();
            //message.guid = Guid.NewGuid();

           // print(message.guid);
           List<Player> players = new List<Player>();
           for(int i = 0; i < 5; i++)
           {
                Player p = new Player($"{i}");
                players.Add(p);
           }

           players[0].totalChips = 25;
           players[1].totalChips = 5;
           players[2].totalChips = 5;
           players[3].totalChips = 27;
           players[4].totalChips = 10;

           List<Player> sorted = players.OrderByDescending(x => x.totalChips).ToList();
           foreach(Player p in sorted)
           {
               print($"{p.name}: {p.totalChips}");
           }

        */

        //print(236.IntRound(-1));


        //List<Card> playerCards = new List<Card>();
        //System.Random rnd = new System.Random();

        // p1Cards.Add(new Card(Card.SUit))

        Player p1 = new Player("p1");
        Player p2 = new Player("p2");
        Player p3 = new Player("p3");
        Player p4 = new Player("p4");
        Player p5 = new Player("p5");
        Player p6 = new Player("p6");

        p1.totalBet = 5;
        p2.totalBet = 20;
        p3.totalBet = 20;
        p4.totalBet = 25;
        p5.totalBet = 40;
        p6.totalBet = 70;
/*         p1.totalBet = 5;
        p2.totalBet = 5;
        p3.totalBet = 5;
        p4.totalBet = 5;
        p5.totalBet = 5;
        p6.totalBet = 5; */

        List<Player> players = new List<Player>();
        players.Add(p1);
        players.Add(p2);
        players.Add(p3);
        players.Add(p4);
        players.Add(p5);
        players.Add(p6);

        GiveCardToRiver(players);

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        /* Calculate Best hand */
        foreach(Player p in players)
        {
            p.bestHand = new BestHand(p.cards, comcards);
        }

        foreach(Player p in players)
        {
            print($"{p.name} hand = {p.bestHand.hand}");
        }

        PotWinnerManager manager = new PotWinnerManager(players);

        stopwatch.Stop();
        print("Time Take: " + stopwatch.ElapsedMilliseconds + "ms");


/*         while(manager.pots.Count != 0)
        {
            KeyValuePair<int, List<Player>> currentPot = manager.pots.Pop();

            print("Current Total Pot: " + currentPot.Key);
            foreach(Player p in currentPot.Value)
            {
                print(p.name + ": " + p.totalBet);
            }
            print("\n");
        } */
/*         foreach(Player p in players)
        {
            print($"{p.name} hand = {p.bestHand.hand}");
            foreach(Card c in p.bestHand.bestHandCombi)
            {
                print(c);
            }
            print("");

        } */

        print("\n");
        print("\n");
        print("\n");
        print("\n");
        print("\n");


        foreach(KeyValuePair<int, List<Player>> kvPair in manager.potWinnerStack)
        {
            print($"Current pot: {kvPair.Key}");
            foreach(Player p in kvPair.Value)
            {
                print($"Winners: {p.name}");
            }
        }


        print("\n");
        print("\n");
        print("\n");
        print("\n");
        print("\n");
        print("ShowDown Order");
        foreach(Player p in manager.showDown)
        {
            print(p.name);
        }

        Player player1 = new Player("player1");
        Player player2 = new Player("player2");
        Player player3 = new Player("player3");
        Player player4 = new Player("player4");

        List<Player> newList = new List<Player>();
        newList.Add(player1);
        newList.Add(player2);
        newList.Add(player3);
        newList.Add(player4);

        List<Player> exampleList = new List<Player>();
        exampleList.AddRange(newList.GetRange(0, 2));

        exampleList.RemoveAll(x => x.Equals(player1));
        print(newList.Count);

        // SceneVisibilityManager.instance.Show()
        
        /* exampleGameObj.hideFlags = HideFlags.None; */


        LazyAction.GetWkr().Act(() => 
        {
            print("Adding");
        }
        , 2.0f);
        
        StartCoroutine(ExampleCoroutine());
    }

    private void Update() {
        chips.transform.Translate(Vector3.forward * Time.deltaTime);
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
