using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.Linq;
using System.Diagnostics;

public class ScriptTester : MonoBehaviour
{
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
    
    List<Card> comCards = new List<Card>();
    List<Card> playerCards = new List<Card>();
    System.Random rnd = new System.Random();

/*     for(int i = 0; i < 5; i++)
    {
        //comCards.Add(new Card((Card.Suit)rnd.Next(0, 4), rnd.Next(0, 13)));
        comCards.Add(new Card(Card.Suit.CLUB, 2));
        comCards.Add(new Card(Card.Suit.CLUB, 7));
        comCards.Add(new Card(Card.Suit.CLUB, 4));
        comCards.Add(new Card(Card.Suit.CLUB, 5));
        comCards.Add(new Card(Card.Suit.CLUB, 6));

    }
    for(int i = 0; i < 2; i++)
    {
        //playerCards.Add(new Card((Card.Suit)rnd.Next(0, 4), rnd.Next(0, 13)));
        playerCards.Add(new Card(Card.Suit.HEART, 5));
        playerCards.Add(new Card(Card.Suit.DIAMOND, 6));
    }
 */
    /* BestHand Calculator testing */
/*     comCards.Add(new Card(Card.Suit.SPADE, 5));
    comCards.Add(new Card(Card.Suit.DIAMOND, 3));
    comCards.Add(new Card(Card.Suit.HEART, 5));
    comCards.Add(new Card(Card.Suit.CLUB, 5));
    comCards.Add(new Card(Card.Suit.HEART, 11));

    playerCards.Add(new Card(Card.Suit.DIAMOND, 12));
    playerCards.Add(new Card(Card.Suit.SPADE, 3));

    Stopwatch stopwatch = new Stopwatch(); //객체 선언
    stopwatch.Start(); // 시간측정 시작

    BestHand hand = new BestHand(playerCards, comCards);


    print(hand.bestHand);
    foreach(Card c in hand.bestHandCombi)
    {
        print(c);
    }

    stopwatch.Stop();
    print("Time spent: " + stopwatch.ElapsedMilliseconds + "ms"); */

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

    List<Player> players = new List<Player>();
    players.Add(p1);
    players.Add(p2);
    players.Add(p3);
    players.Add(p4);
    players.Add(p5);
    players.Add(p6);

    PotWinnerManager manager = new PotWinnerManager(players);

    while(manager.pots.Count != 0)
    {
        KeyValuePair<int, List<Player>> currentPot = manager.pots.Pop();

        print("Current Total Pot: " + currentPot.Key);
        foreach(Player p in currentPot.Value)
        {
            print(p.name + ": " + p.totalBet);
        }
        print("\n");
    }

    print("\n");
    print("\n");
    print("\n");
    print("\n");
    print("\n");

    while(manager.potWinnerStack.Count != 0)
    {
        KeyValuePair<int, List<Player>> currentWinners = manager.potWinnerStack.Pop();

        print($"Current pot: {currentWinners.Key}");
        foreach(Player p in currentWinners.Value)
        {
            print($"Winners: {p.name}");
        }
    }



#if TEST
    print("testing");
#else
    print("Real symbol");
#endif
    }
}
