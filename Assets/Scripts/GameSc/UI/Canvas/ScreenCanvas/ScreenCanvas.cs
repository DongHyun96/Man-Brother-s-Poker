using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ScreenCanvas : MonoBehaviour
{
    public GameObject upperPanel;
    public GameObject bottomLeft;
    public GameObject winnerAnounce;
    public GameObject chooseShowDown;

    // BottomLeft components
    public List<Image> playerCards;
    public List<Image> communityCards;
    private int playerChips_Num;
    private int pot_Num;
    public Text playerChips;
    public Text pot;

    public List<Animator> playerCardAnims;
    public List<Animator> communityCardAnims;

    // ActionGUI components
    public PieButton[] pieButtons = new PieButton[3];
    public BettingPanel bettingPanel;
    public Animator pieButtonAnim;
    public Animator bettingPanelAnim;

    // WinnerAnounce Component
    public WinnerPanel winnerPanel;

    // ChooseShowDown Component
    public ShowDownPanel showDownPanel;

    // Constants
    private const float WAIT_SEC = 0.001f;

    private void Start() {
        //UpdateTotalChips(50000);
        //TogglePieButtonAnim();
    }

    /****************************************************************************************************************
    *                                                Update contents
    ****************************************************************************************************************/
    public void UpdatePlayerCards(List<Card> cards)
    {
        // Update BottomLeft cards
        playerCards[0].sprite = CardSprite.GetInstance().GetSprite(cards[0]);
        playerCards[1].sprite = CardSprite.GetInstance().GetSprite(cards[1]);

        // Update Winner Anounce cards
        winnerPanel.playerCards[0].sprite = CardSprite.GetInstance().GetSprite(cards[0]);
        winnerPanel.playerCards[1].sprite = CardSprite.GetInstance().GetSprite(cards[1]);

        // Update showdownPanel cards
        showDownPanel.InitShowDown(cards);
    }

    public void UpdateCommunityCards(List<Card> cards)
    {
        for(int i = 0; i < cards.Count; i++)
        {
            // Update BottomLeft cards
            communityCards[i].sprite = CardSprite.GetInstance().GetSprite(cards[i]);

            // Update Winner Anounce Cards
            winnerPanel.communityCards[i].sprite = CardSprite.GetInstance().GetSprite(cards[i]);
        }
    }

    public void UpdateTotalChips(int chips)
    {
        StartCoroutine(UpdateTotalChipsRoutine(chips));
    }

    public void UpdatePotChips(int chips)
    {
        StartCoroutine(UpdatePotChipsRoutine(chips));
    }

    public void UpdateActionGUI(PieButton.ActionState state, int callChips = 0)
    {
        // Update pieButtons
        foreach(PieButton p in pieButtons)
        {
            p.UpdateContents(state, callChips);
        }

        // Update BettingPanel
        bettingPanel.SetMinBet(callChips * 2); // Set minimum bet to double up, Not Fully implemented yet.
    }

    public void UpdateWinnerAnounce()
    {
        winnerPanel.InitWinnerPanel(); // Not Implemented yet.. Needs to implement bestHand & bestHandcalculator
    }

    public void UpdateShowDownPanel(List<Card> cards)
    {
        showDownPanel.InitShowDown(cards);
    }
    
    /****************************************************************************************************************
    *                                                Animation toggling
    ****************************************************************************************************************/
    public void TogglePlayerCardsAnim(int idx)
    {
        playerCardAnims[idx].SetBool("isIn", !playerCardAnims[idx].GetBool("isIn"));
    }

    public void ToggleCommunityCardsAnim(int idx)
    {
        communityCardAnims[idx].SetBool("isIn", !communityCardAnims[idx].GetBool("isIn"));
    }

    public void TogglePieButtonAnim()
    {
        pieButtonAnim.SetBool("isIn", !pieButtonAnim.GetBool("isIn"));
    }

    public void ToggleBettingPanelAnim()
    {
        bettingPanelAnim.SetBool("isIn", !bettingPanelAnim.GetBool("isIn"));
    }

    /****************************************************************************************************************
    *                                                Private Methods
    ****************************************************************************************************************/
    private string GetChipString(int chips)
    {
        return (chips < 1000) ? chips.ToString() : (Math.Round(chips / 1000f, 2)).ToString() + "k";
    }

    /*
    * Update chips UI slowly
    */
    private IEnumerator UpdateTotalChipsRoutine(int chips)
    {
        if(playerChips_Num < chips)
        {
            while(playerChips_Num != chips)
            {
                playerChips_Num += GameManager.gameTable.sbChip;
                playerChips.text = GetChipString(playerChips_Num);
                yield return new WaitForSeconds(WAIT_SEC);
            }
        }
        else if(playerChips_Num > chips)
        {
            while(playerChips_Num != chips)
            {
                playerChips_Num -= GameManager.gameTable.sbChip;
                playerChips.text = GetChipString(playerChips_Num);
                yield return new WaitForSeconds(WAIT_SEC);
            }
        }
    }

    private IEnumerator UpdatePotChipsRoutine(int chips)
    {
        if(pot_Num < chips)
        {
            while(pot_Num != chips)
            {
                pot_Num += GameManager.gameTable.sbChip;
                pot.text = GetChipString(pot_Num);
                yield return new WaitForSeconds(WAIT_SEC);
            }
        }
        else if(pot_Num > chips)
        {
            while(pot_Num != chips)
            {
                pot_Num -= GameManager.gameTable.sbChip;
                pot.text = GetChipString(pot_Num);
                yield return new WaitForSeconds(WAIT_SEC);
            }
        }
    } 
}
