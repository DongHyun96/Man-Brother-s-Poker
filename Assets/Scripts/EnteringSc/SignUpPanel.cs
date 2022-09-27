using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SignUpPanel : MonoBehaviour
{

    [SerializeField] private Animator anim;
    public static GameObject warning;
    [SerializeField] private InputField inputField;

    void Awake()
    {   
        warning = transform.Find("DupText").gameObject;

        inputField.onValueChanged.AddListener(delegate { RemoveSpaces(); });
    }

    public void AcceptBtnPressed()
    {
        if(string.IsNullOrEmpty(inputField.text))
        {
            // SignUpPanel.warning.SetActive(true);
            warning.SetActive(true);
            return;
        }
        
        MainMessage msg = new MainMessage(MainMessage.MessageType.SIGNUP, inputField.text, GameManager.thisPlayer.character);
        MainMsgHandler.SendMessage(msg);
        //anim.SetTrigger("Hide");

    }

    private void RemoveSpaces()
    {
        inputField.text = inputField.text.Replace(" ", "");
    }

}
