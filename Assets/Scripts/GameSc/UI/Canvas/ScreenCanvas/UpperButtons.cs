using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperButtons : MonoBehaviour
{
    [SerializeField] private Animator menu;
    [SerializeField] private Animator rankGuide;
    [SerializeField] private Animator keyGuide;

    public void OnMenu()
    {
        toggleAnimation(menu);
    }

    public void OnRankGuide()
    {
        rankGuide.SetBool("isIn", !rankGuide.GetBool("isIn"));
    }

    public void OnKeyGuide()
    {
        keyGuide.SetBool("isIn", !keyGuide.GetBool("isIn"));
    }

    private void toggleAnimation(Animator a)
    {
        if (a.GetCurrentAnimatorStateInfo(0).IsName("Out") || a.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            a.SetTrigger("Show");
        else
            a.SetTrigger("Hide");
    }


    private void Update() {
        if(Input.GetKeyDown(KeyCode.R))
        {
            rankGuide.SetBool("isIn", !rankGuide.GetBool("isIn"));
        }
    }
}
