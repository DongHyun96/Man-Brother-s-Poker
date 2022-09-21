using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayManager : MonoBehaviour
{
    private static DisplayManager m_instance;
    public static DisplayManager Instance
    {
        get => m_instance;
        private set{ m_instance = value; }
    }
    private void Awake() {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Init d_mode
            d_mode = (Screen.fullScreenMode == FullScreenMode.FullScreenWindow) ?
            DisplayMode.FULL : DisplayMode.WINDOWED;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public enum DisplayMode{
        FULL, WINDOWED 
    }

    private DisplayMode d_mode;
    public DisplayMode displayMode
    {
        get => d_mode;

        set
        {
            switch(value)
            {
                case DisplayMode.FULL:
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    
                    // Set default resolution
                    Screen.SetResolution(1920, 1080, true);
                    break;
                case DisplayMode.WINDOWED:
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    break;
            }
            d_mode = value;
        }
    }
}
