using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BettingPanel : MonoBehaviour
{
    [SerializeField]
    private Text minBet;
    private int min; // 0

    [SerializeField]
    private Text maxBet;
    private int max; // 100

    [SerializeField]
    private Slider slider;

    [SerializeField]
    private Text betting;
    public int betChips;
    

    // These for slider area restriction (e.g.1K room => 0, 1000)
    private int floor; // 0
    private int ceil; // 1000

    public void InitContents(int minChips)
    {
        Player thisP = GameManager.gameTable.GetPlayerByName(GameManager.thisPlayer.name);

        // Setting floor and ceil
        floor = minChips;
        ceil = thisP.totalChips + thisP.roundBet;

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
        SetMin(0);

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

        // Update current betting
        if(betChips > max)
        {
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

        // Update current slider pos
        slider.value = GetSliderValueFromChips(betChips);
        
    }

    /* Update betChips and bettingText here */
    public void OnSliderValueChange()
    {
        // Restriction
        float sliderMin = GetSliderValueFromChips(floor);
        float sliderMax = GetSliderValueFromChips(ceil);

        slider.value = slider.value < sliderMin ? sliderMin : slider.value;
        slider.value = slider.value > sliderMax ? sliderMax : slider.value;

        int bet = GetChipsFromSliderValue(slider.value);

        SetBet(bet);
    }

    private void SetMin(int chips)
    {
        min = chips;
        minBet.text = GetChipString(min);
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

        /* if(max <= minMax * 2)
        {
            slider.maxValue = sliderMaxDefault; 
        }
        else if(max == minMax * 4)
        {
            slider.maxValue = sliderMaxDefault / 2;
        }
        else if(max == minMax * 8)
        {
            slider.maxValue = sliderMaxDefault / 4;
        }
        else
        {
            slider.maxValue = 32;
        } */
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

    private void Start() {
        /* Just for testing */
      /*   GameManager.thisPlayer = new Player("Dongman");
        List<Player> players = new List<Player>();
        players.Add(GameManager.thisPlayer);
        GameManager.gameTable = new GameTable(Guid.NewGuid(), players, Room.Mode.CHICKEN, Room.BuyIn.HUNDRED);
        GameManager.thisPlayer.totalChips = 100000;
        InitContents(0); */
    }
    
}
