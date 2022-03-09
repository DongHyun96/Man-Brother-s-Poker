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

    public static State state{

        get => m_state;

        set{
            switch(value)
            {
                case State.LOBBY:
                    if(state == State.PLAYING) // PLAYING -> LOBBY : Change invitable to true and notify to other players
                    {
                        thisPlayer.invitable = true;
                    }
                    break;
                case State.ROOM:
                    if(state == State.PLAYING) // PLAYING -> ROOM : Change invitable to true and notify to other players
                    {
                        thisPlayer.invitable = true;
                    }
                    break;
                case State.PLAYING:
                    break;
            }
            m_state = value;
        }
    }

    private static State m_state = State.LOGIN;

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
