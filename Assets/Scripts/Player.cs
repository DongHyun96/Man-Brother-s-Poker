using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public string name;

    // Whether player is available to get invitation
    public bool invitable = true;
    
    /********************************************************************************************************
    In game ref
    *********************************************************************************************************/
    public enum State{
        IDLE
    }
    public State state;

    public int totalChips;
    public int betChips;

    public List<Card> cards;
    /********************************************************************************************************/

    public Player(string name)
    {
        this.name = name;
    }
    public Player(){}

}
