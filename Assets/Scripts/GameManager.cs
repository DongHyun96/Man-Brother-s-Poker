using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static Player thisPlayer;

    private static GameManager instance;

    // For faster search, Using dictionary instead of list
    public static Dictionary<string, Player> allOthers = new Dictionary<string, Player>();

    public enum State{
        LOGIN, LOBBY, ROOM, PLAYING
    }

    public State state{
        set{
            switch(value)
            {
                case State.LOBBY:
                    if(state == State.PLAYING)
                    {
                        thisPlayer.invitable = true;

                    }
                    break;
                case State.ROOM:
                    if(state == State.PLAYING)
                    {
                        thisPlayer.invitable = true;
                    }
                    break;
                case State.PLAYING:
                    break;
            }
            state = value;
        }
        get => state;
    }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public static GameManager GetInstance()
    {
        if(instance == null)
        {
            instance = FindObjectOfType<GameManager>();

            if (instance == null)
            {
                GameObject container = new GameObject("GameManager");
                instance = container.AddComponent<GameManager>();
            }
        }
        return instance;
    }



}
