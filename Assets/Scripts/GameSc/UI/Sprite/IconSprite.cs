using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class IconSprite : MonoBehaviour
{
    private static IconSprite instance;

    [SerializeField] private Sprite small;
    [SerializeField] private Sprite big;
    [SerializeField] private Sprite check;
    [SerializeField] private Sprite bet; // also used as call
    [SerializeField] private Sprite raise;
    [SerializeField] private Sprite allin;
    [SerializeField] private Sprite fold;

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
