using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class PieButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private const float THRESH_HOLD = 0.1f;
    private const float BUTTON_SCALE = 1.07f;


    [SerializeField] private Text actionIndicator;
    [SerializeField] private Animator indicatorAnim;
    [SerializeField] private List<Image> icons = new List<Image>();
    
    //private List<string> actions = new List<string>();
    private string[] actions = new string[3];

    public enum ActionState{
        CHECK_BET_FOLD, CALL_RAISE_FOLD, CHECK_RAISE_FOLD, ALLIN_FOLD
    }


    public ActionState state{ get; private set; }

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
            case "PieLeft":
                actionIndicator.text = actions[0];
                break;
            case "PieRight":
                actionIndicator.text = actions[1];
                break;
            case "PieButtom":
                actionIndicator.text = actions[2];
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

    public void UpdateContents(ActionState state, int callChips = 0)
    {
        this.state = state;
        switch(state)
        {
            case ActionState.CHECK_BET_FOLD:
                // set up actionTexts
                actions[0] = "CHECK";
                actions[1] = "BET";
                actions[2] = "FOLD";

                // set up icons sprite
                icons[0].sprite = IconSprite.GetInstance().GetSprite(Player.State.CHECK);
                icons[1].sprite = IconSprite.GetInstance().GetSprite(Player.State.BET);
                icons[2].sprite = IconSprite.GetInstance().GetSprite(Player.State.FOLD);

                break;
            case ActionState.CALL_RAISE_FOLD:
                // set up actionTexts
                actions[0] = "CALL " + GetChipString(callChips);
                actions[1] = "RAISE";
                actions[2] = "FOLD";

                // set up icons sprite
                icons[0].sprite = IconSprite.GetInstance().GetSprite(Player.State.CALL);
                icons[1].sprite = IconSprite.GetInstance().GetSprite(Player.State.RAISE);
                icons[2].sprite = IconSprite.GetInstance().GetSprite(Player.State.FOLD);
                break;
            case ActionState.CHECK_RAISE_FOLD:
                actions[0] = "CHECK";
                actions[1] = "RAISE";
                actions[2] = "FOLD";

                // set up icons sprite
                icons[0].sprite = IconSprite.GetInstance().GetSprite(Player.State.CHECK);
                icons[1].sprite = IconSprite.GetInstance().GetSprite(Player.State.RAISE);
                icons[2].sprite = IconSprite.GetInstance().GetSprite(Player.State.FOLD);
    
                break;
            case ActionState.ALLIN_FOLD:
                actions[0] = "CALL ALL IN";
                actions[1] = "CALL ALL IN";
                actions[2] = "FOLD";
                
                icons[0].sprite = IconSprite.GetInstance().GetSprite(Player.State.ALLIN);
                icons[1].sprite = IconSprite.GetInstance().GetSprite(Player.State.ALLIN);
                icons[2].sprite = IconSprite.GetInstance().GetSprite(Player.State.FOLD);
                break;
        }
    }

    private string GetChipString(int chips)
    {
        return (chips < 1000) ? chips.ToString() : (Math.Round(chips / 1000f, 2)).ToString() + "k";
    }

    
}
