using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.Linq;

public class ScriptTester : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
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


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
