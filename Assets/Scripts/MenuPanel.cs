using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MenuPanel : MonoBehaviour
{
    private enum MenuState{
        IDLE, OPTIONS, CREDITS
    }

    private MenuState mState = MenuState.IDLE;
    private MenuState menuState{

        set
        {
            switch(value)
            {
                case MenuState.IDLE:
                    title.text = "MENU";
                    buttonsPanel.SetActive(true);
                    optionsPanel.SetActive(false);
                    break;
                case MenuState.OPTIONS:
                    title.text = "OPTIONS";
                    optionsPanel.SetActive(true);
                    buttonsPanel.SetActive(false);

                    // Init slider value
                    float bgmVol = SoundManager.GetInstance().GetBgmLvl();
                    float sfxVol = SoundManager.GetInstance().GetSfxLvl();

                    bgmSlider.value = GetSliderValueFromVolume(bgmVol);
                    sfxSlider.value = GetSliderValueFromVolume(sfxVol);
                    break;
                case MenuState.CREDITS:
                    break;
            }
        }

        get => mState;

    }

    [SerializeField] private Text title;
    [SerializeField] private GameObject buttonsPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private Toggle[] screenMode;

    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    public void OnAnimIn()
    {
        menuState = MenuState.IDLE;
    }

    public void OnX()
    {
        EntPanelController.GetInstance().UpdatePanel(EntPanelController.Panel.MENU);
    }

    public void OnOptions()
    {
        menuState = MenuState.OPTIONS;
    }

    public void OnCredits()
    {
        
    }

    public void OnExitGame()
    {
        Application.Quit();
    }

    /* Option methods */

    public void OnScreenModeChange()
    {
        foreach(Toggle t in screenMode)
        {
            if(t.isOn)
            {
                // Do something
            }
        }
    }

    public void OnBgmSliderChange()
    {
        float volume = GetVolumeFromSliderValue(bgmSlider.value);
        SoundManager.GetInstance().SetBgmLvl(volume);
    }

    public void OnSfxSliderChange()
    {
        float volume = GetVolumeFromSliderValue(sfxSlider.value);
        SoundManager.GetInstance().SetSfxLvl(volume);
    }

    public void OnOptionsAccept()
    {
        menuState = MenuState.IDLE;
    }

    private float GetVolumeFromSliderValue(float value)
    {
        return (float)ExtensionMethods.Remap(value, bgmSlider.minValue, bgmSlider.maxValue, SoundManager.MIN, SoundManager.MAX);
    }
    private float GetSliderValueFromVolume(float value)
    {
        return (float)ExtensionMethods.Remap(value, SoundManager.MIN, SoundManager.MAX, bgmSlider.minValue, bgmSlider.maxValue);
    }
    
}
