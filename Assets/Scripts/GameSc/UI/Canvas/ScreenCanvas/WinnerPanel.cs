using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class WinnerPanel : MonoBehaviour
{
    public Text hands;
    
    public List<Image> playerCards;
    public List<Image> communityCards;
    
    public Text winner;
    public Text pots;

    private string GetChipString(int chips)
    {
        return (chips < 1000) ? chips.ToString() : (Math.Round(chips / 1000f, 2)).ToString() + "k";
    }

    // Best hand needed
    public void InitWinnerPanel()
    {
        throw new NotImplementedException();
    }


}
