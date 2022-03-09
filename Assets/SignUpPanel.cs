using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SignUpPanel : MonoBehaviour
{

    public Animator anim;
    public static GameObject warning;

    void Awake()
    {   
        warning = transform.Find("DupText").gameObject;
    }


    public void AcceptBtnPressed()
    {
        InputField inputField = FindObjectOfType<InputField>();
        MainMsgHandler.SendMessage(new MainMessage(MainMessage.MessageType.SIGNUP, inputField.text));
        //anim.SetTrigger("Hide");
    }

}
