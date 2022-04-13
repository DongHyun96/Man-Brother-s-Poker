using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;

public class Player
{
    public string name;

    // Whether player is available to get invitation
    public bool invitable = true;
    
    // Player's character
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Character
    {
        MALE, MALE_CAP, FEMALE, POLICE, ZOMBIE
    }
    public Character character;


    /********************************************************************************************************
    In game ref
    *********************************************************************************************************/
    [JsonConverter(typeof(StringEnumConverter))]
    public enum State{
        IDLE, SMALL, BIG, CHECK, BET, CALL, RAISE, ALLIN, FOLD
    }
    public State state = State.IDLE;

    public int totalChips;
    public int roundBet;
    public int totalBet;

    public List<Card> cards = new List<Card>();
    /********************************************************************************************************/

    public Player(string name)
    {
        this.name = name;
    }
    public Player(string name, Character character)
    {
        this.name = name;
        this.character = character;
    }
    public Player(Character character)
    {
        this.character = character;
    }
    public Player(){}


    /********************************************************************************************************
    *                                               Action methods
    *********************************************************************************************************/
    public void Check()
    {
        state = State.CHECK;
    }


    public void Bet(int chips)
    {
        if(totalChips <= chips)
        {
            AllIn();
            return;
        }
        state = State.BET;
        roundBet = chips;
        totalBet += chips;
        totalChips -= chips;
    }


    public void Call(int chips)
    {
        if(totalChips <= chips)
        {
            AllIn();
            return;
        }
        state = State.CALL;
        
        totalBet += chips - roundBet;
        totalChips -= chips - roundBet;
        roundBet = chips;

        
    }


    public void Raise(int chips)
    {
        if(totalChips <= chips)
        {
            AllIn();
            return;
        }
        state = State.RAISE;
 
        totalBet += chips - roundBet;
        totalChips -= chips - roundBet;
        roundBet = chips;

    }   
    public void AllIn()
    {
        state = State.ALLIN;
        roundBet = totalChips;
        totalBet += totalChips;
        totalChips = 0;
    }
    public void Fold()
    {
        state = State.FOLD;
    }

}
