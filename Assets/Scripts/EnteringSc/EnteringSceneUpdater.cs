using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EnteringSceneUpdater : MonoBehaviour
{
    public static EnteringSceneUpdater instance;

    public UnityEvent onLobbyPlayersUpdate;

    public delegate void LobbyChatUpdate(string name, string chat);
    public event LobbyChatUpdate onLobbyChatUpdate;


    public static EnteringSceneUpdater GetInstance()
    {
        if(instance == null)
        {
            instance = FindObjectOfType<EnteringSceneUpdater>();
        }
        return instance; // It may be null if it calls in invalid scene.
    }

    public void UpdateChat(string name, string chat)
    {
        EnteringSceneUpdater.GetInstance().onLobbyChatUpdate(name, chat);
    }

}
