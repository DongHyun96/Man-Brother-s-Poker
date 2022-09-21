using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public abstract class MenuFunctions : MonoBehaviour
{
    [SerializeField] protected Text title;
    [SerializeField] protected GameObject buttonsPanel;
    [SerializeField] protected GameObject optionsPanel;
    [SerializeField] protected Toggle[] screenMode;

    [SerializeField] protected Slider bgmSlider;
    [SerializeField] protected Slider sfxSlider;

    protected enum MenuState{
        IDLE, OPTIONS, CREDITS
    }

    protected MenuState mState = MenuState.IDLE;

    protected MenuState menuState{

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

                    // Init Display mode toggle
                    int isOnIdx = (int)DisplayManager.Instance.displayMode;
                    screenMode[isOnIdx].isOn = true;

                    // Init slider value
                    float bgmVol = SoundManager.GetInstance().GetBgmLvl();
                    float sfxVol = SoundManager.GetInstance().GetSfxLvl();

                    bgmSlider.value = GetSliderValueFromVolume(bgmVol);
                    sfxSlider.value = GetSliderValueFromVolume(sfxVol);

                    break;
                case MenuState.CREDITS:
                    break;
            }
            mState = value;
        }

        get => mState;

    }

    // When menu is on
    public void OnAnimIn()
    {
        menuState = MenuState.IDLE;
    }

    public void OnX()
    {
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("Hide");
    }

    public void OnOptions()
    {
        menuState = MenuState.OPTIONS;
    }

    public void OnExitGame()
    {
        Application.Quit();
    }

    public abstract void OnCredits();
    public abstract void OnExitToLobby();

    public void OnScreenModeChange()
    {
        for(int i = 0; i < screenMode.Length; i++)
        {
            if(screenMode[i].isOn)
            {
                DisplayManager.Instance.displayMode = (DisplayManager.DisplayMode)i;
                break;
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

    protected float GetVolumeFromSliderValue(float value)
    {
        return (float)ExtensionMethods.Remap(value, bgmSlider.minValue, bgmSlider.maxValue, SoundManager.MIN, SoundManager.MAX);
    }
    protected float GetSliderValueFromVolume(float value)
    {
        return (float)ExtensionMethods.Remap(value, SoundManager.MIN, SoundManager.MAX, bgmSlider.minValue, bgmSlider.maxValue);
    }
}
