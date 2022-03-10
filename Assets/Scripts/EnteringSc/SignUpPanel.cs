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
        
        MainMsgHandler.SendMessage(new MainMessage(MainMessage.MessageType.SIGNUP, inputField.text));
        //anim.SetTrigger("Hide");
    }

}
