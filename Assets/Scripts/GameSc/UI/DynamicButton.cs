using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DynamicButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private const float THRESH_HOLD = 0.1f;
    private const float BUTTON_SCALE = 1.07f;


    public Text actionIndicator;
    public Animator indicatorAnim;

    void Start()
    {
        this.GetComponent<Image>().alphaHitTestMinimumThreshold = THRESH_HOLD;
    }

    // Override
    public void OnPointerEnter(PointerEventData data)
    {
        transform.localScale = Vector3.one * BUTTON_SCALE;

        switch(this.tag)
        {
            // Unimplemented yet
            case "PieLeft":
                // change actionIndicator.text
                actionIndicator.text = "CHECK";
                print("1");
                break;
            case "PieRight":
                // change actionIndicator.text
                actionIndicator.text = "BET";

                print("2");
                
                break;
            case "PieButtom":
                // change actionIndicator.text
                actionIndicator.text = "FOLD";

                print("3");

                break;
        }
        indicatorAnim.SetBool("isIn", true);
    }


    // Override
    public void OnPointerExit(PointerEventData data)
    {
        transform.localScale = Vector3.one;
        indicatorAnim.SetBool("isIn", false);
        
    }

    
}
