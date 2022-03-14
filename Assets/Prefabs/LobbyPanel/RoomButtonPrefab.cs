using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomButtonPrefab : MonoBehaviour
{
    public Text title;
    public Image dot;
    public Text num;
    public Text buyIn;

    public Text chicken;
    public Text lastMan;
    public Text heads;

    public Text OX;

    private Color greenDot = new Color(24/255f, 255/255f, 8/255f);
    private Color redDot = new Color(245/255f, 78/255f ,81/255f);

    private Color modeRed = new Color(243/255f, 86/255f, 29/255f);
    private Color modeGrey = new Color(149/255f, 149/255f, 149/255f);


    public void SetContents(string title, bool isPlaying, int n, Room.BuyIn b, Room.Mode mode, bool isSecured)
    {
        this.title.text = title;

        if(!isPlaying && n < 6)
        {
            dot.color = greenDot;
        }
        else
        {
            dot.color = redDot;
        }

        num.text = n + "/" + "6";

        switch(b)
        {
            case Room.BuyIn.ONE:
                buyIn.text = "1k";
                break;
            case Room.BuyIn.FIVE:
                buyIn.text = "5k";
                break;
            case Room.BuyIn.TEN:
                buyIn.text = "10k";
                break;
            case Room.BuyIn.TWENTY:
                buyIn.text = "20k";
                break;
            case Room.BuyIn.FIFTY:
                buyIn.text = "50k";
                break;
            case Room.BuyIn.HUNDRED:
                buyIn.text = "100k";
                break;
            default:
                break;
        }

        switch(mode)
        {
            case Room.Mode.CHICKEN:
                chicken.color = modeRed;
                break;
            case Room.Mode.LASTMAN:
                lastMan.color = modeRed;
                break;
            case Room.Mode.HEADS:
                heads.color = modeRed;
                break;
            default:
             break;
        }

        if(isSecured)
        {
            OX.text = "O";
        }
        else
        {
            OX.text = "X";
        }


    }


}
