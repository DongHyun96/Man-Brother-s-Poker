using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BettingPanel : MonoBehaviour
{
    [SerializeField]
    private Text minBet;

    [SerializeField]
    private Text maxBet;

    [SerializeField]
    private Slider slider;

    [SerializeField]
    private Text betting;
    
    public void OnLeftButton()
    {
        // Not implemented
    }

    public void OnRightButton()
    {
        // Not implemented
    }

    public void SetMinBet(int chips)
    {
        // Should think later on.
        slider.minValue = chips;
        minBet.text = GetChipString(chips);
    }



    private string GetChipString(int chips)
    {
        return (chips < 1000) ? chips.ToString() : (Math.Round(chips / 1000f, 2)).ToString() + "k";
    }
    
}
