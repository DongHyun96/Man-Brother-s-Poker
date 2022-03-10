using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class L_PanelChat : MonoBehaviour
{
    public Text chatText;

    public InputField inputField;

     void Awake()
    {
        EnteringSceneUpdater.GetInstance().onLobbyChatUpdate += AddChat;  // Subscribe
    }

    public void AddChat(string name, string chat)
    {
        if(chatText.text.Length > 400) // Restricting text length
        {
            int index = chatText.text.IndexOf("\n");
            chatText.text = chatText.text.Remove(0, index + 1);
        }
        if(name.Equals(GameManager.thisPlayer.name))
        {
            chatText.text = chatText.text + "<color=#FF5600>" + name + " : " + chat + "</color>" + "\n";
        }
        else
        {
            chatText.text = chatText.text + "<color=#00EEFF>" + name + " : " + chat + "</color>" + "\n";
        }
    }

    public void onBtnPressed()
    {
        if(String.IsNullOrEmpty(inputField.text))
        {
            return;
        }
        
        // Send message to server side
        MainMessage message = new MainMessage(MainMessage.MessageType.CHAT, GameManager.thisPlayer.name, inputField.text);
        MainMsgHandler.SendMessage(message);
    }
}
