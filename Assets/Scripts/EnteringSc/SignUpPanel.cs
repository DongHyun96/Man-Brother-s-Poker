using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignUpPanel : MonoBehaviour
{

    public Animator anim;
    public static GameObject warning;
    public InputField inputField;

    void Awake()
    {   
        warning = transform.Find("DupText").gameObject;
    }


    public void AcceptBtnPressed()
    {
        if(string.IsNullOrEmpty(inputField.text))
        {
            SignUpPanel.warning.SetActive(true);
            return;
        }
        
        MainMessage msg = new MainMessage(MainMessage.MessageType.SIGNUP, inputField.text, GameManager.thisPlayer.character);
        MainMsgHandler.SendMessage(msg);
        //anim.SetTrigger("Hide");
    }

}
