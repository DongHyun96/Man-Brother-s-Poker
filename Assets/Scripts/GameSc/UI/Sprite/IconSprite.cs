using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class IconSprite : MonoBehaviour
{
    private static IconSprite instance;

    public Sprite small;
    public Sprite big;
    public Sprite check;
    public Sprite bet; // also used as call
    public Sprite raise;
    public Sprite allin;
    public Sprite fold;

    public static IconSprite GetInstance()
    {
        if(instance == null)
        {
            instance = FindObjectOfType<IconSprite>();
        }
        return instance;
    }

    public Sprite GetSprite(Player.State s)
    {
        switch(s)
        {
            case Player.State.SMALL:
                return small;
            case Player.State.BIG:
                return big;
            case Player.State.CHECK:
                return check;
            case Player.State.BET:
                return bet;
            case Player.State.CALL:
                return bet;
            case Player.State.RAISE:
                return raise;
            case Player.State.ALLIN:
                return allin;
            case Player.State.FOLD:
                return fold;
            default:
                return null;

        }
    }
}
