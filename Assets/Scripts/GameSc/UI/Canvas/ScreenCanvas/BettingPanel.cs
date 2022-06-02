using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BettingPanel : MonoBehaviour
{
    // Used for slider's left side text
    [SerializeField]
    private Text minBet;
    private int min = 0; // 0

    // Used for slider's right side text
    [SerializeField]
    private Text maxBet;
    private int max; // 100

    [SerializeField]
    private Slider slider;

    // Used for betting chip text
    [SerializeField]
    private Text betting;
    public int betChips;
    
    // These for slider area restriction, these are actual chip values (e.g.1K room => 0, 1000)
    private int floor; // 0
    private int ceil; // 1000

    public int Ceil
    {
        get => ceil;
    }

    public void InitContents(int minChips)
    {
        Player thisP = GameManager.gameTable.GetPlayerByName(GameManager.thisPlayer.name);

        // Restrict minChips to sbChip
        minChips = minChips < GameManager.gameTable.sbChip ? GameManager.gameTable.sbChip : minChips;

        // Setting floor and ceil
        floor = minChips;
        ceil = thisP.totalChips + thisP.roundBet;

        // When floor exceeds ceiling
        floor = (floor > ceil) ? ceil : floor;

        int tempMin = 0;
        while(tempMin * 2 <= floor)
        {
            if(tempMin == 0)
            {
                if(GameManager.gameTable.sbChip * 20 > floor)
                {
                    break;
                }
                tempMin = GameManager.gameTable.sbChip * 20;
            }
            else
            {
                tempMin *= 2;
            }
        }

        minBet.text = "0";

        int tempMax = tempMin == 0 ? GameManager.gameTable.sbChip * 20 : tempMin * 2;
        SetMax(tempMax);
        
        SetBet(floor);
        slider.value = GetSliderValueFromChips(betChips);
    }

    /* Decrease Maximum Bet*/
    public void OnLeftButton()
    {
        if(max / 2 <= floor || max / 2 < GameManager.gameTable.sbChip * 20)
        {
            return;
        }

        SetMax(max / 2);

        // Update current betting and slider pos
        if(betChips > max)
        {
            // When previous betting amount exceeds current range -> Set to maximum amount of current range
            SetBet(max);
            slider.value = GetSliderValueFromChips(max);
        }
        else
        {
            slider.value = GetSliderValueFromChips(betChips);
        }
        
    }

    /* Increase Maximum Bet */
    public void OnRightButton()
    {
        if(max >= ceil)
        {
            return;
        }
        SetMax(max * 2);

        // Update current betting and slider pos
        int sliderValue = (int)Mathf.Ceil(GetSliderValueFromChips(betChips));
        slider.value = sliderValue;
        SetBet(GetChipsFromSliderValue(sliderValue));
    }

    /* Update betChips and bettingText here */
    public void OnSliderValueChange()
    {
        // Restriction
        float sliderMin = Mathf.Ceil(GetSliderValueFromChips(floor));
        float sliderMax = Mathf.Ceil(GetSliderValueFromChips(ceil));

        slider.value = slider.value < sliderMin ? sliderMin : slider.value;
        slider.value = slider.value > sliderMax ? sliderMax : slider.value;

        int bet = GetChipsFromSliderValue(slider.value);
        
        SetBet(bet);
    }

    private void SetMax(int chips)
    {
        max = chips;
        maxBet.text = GetChipString(max);

        // Update slider's range
        ReRangeSlider(max);
        
    }

    private void SetBet(int chips)
    {
        betChips = chips;

        // All in case
        if(betChips >= ceil)
        {
            betChips = ceil;
            betting.text = "All";
            return;
        }
        betting.text = GetChipString(betChips);
    }

    private float GetSliderValueFromChips(int chips)
    {
        return ExtensionMethods.Remap(chips, min, max, slider.minValue, slider.maxValue);
    }
    
    private int GetChipsFromSliderValue(float value)
    {
        return (int)ExtensionMethods.Remap(value, slider.minValue, slider.maxValue, min, max);
    }

    private void ReRangeSlider(int max)
    {
        // var sliderMaxDefault = max / GameManager.gameTable.sbChip; // Default slider max value
        
        int minMax = GameManager.gameTable.sbChip * 20; // Minimum slider max value 

        if(max < minMax * 2)
        {
            slider.maxValue = 20; 
        }
        else if(max <= minMax * 8)
        {
            slider.maxValue = 40;
        }
        else
        {
            slider.maxValue = 32;
        }
    }


    private string GetChipString(int chips)
    {
        return (chips < 1000) ? chips.ToString() : (Math.Round(chips / 1000f, 2)).ToString() + "k";
    }
}
