using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class WinnerPanel : MonoBehaviour
{

    public Text hands;
    
    public List<Image> playerCards;
    public List<Image> communityCards;
    
    public Text winner;
    public Text pots;

    public SidePotPanel sidePotPanel;

    private Color defaultColor = new Color(41 / 255f, 41 / 255f, 41 / 255f);

    private string GetChipString(int chips)
    {
        return (chips < 1000) ? chips.ToString() : (Math.Round(chips / 1000f, 2)).ToString() + "k";
    }

    public void ShowPanel()
    {
        //winnerPanelGameObject.SetActive(true);
        gameObject.SetActive(true);

        if(!String.IsNullOrEmpty(sidePotPanel.name.text))
        {
            sidePotPanel.gameObject.SetActive(true);
            //sidePotGameObject.SetActive(true);
        }
        else
        {
            sidePotPanel.gameObject.SetActive(false);
            // sidePotGameObject.SetActive(false);
        }
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
        sidePotPanel.gameObject.SetActive(false);
    }

    // Best hand needed
    public void InitWinnerPanel()
    {
        /* UNCONTESTED */
        if(GameManager.gameTable.stage == GameTable.Stage.UNCONTESTED)
        {
            hands.text = "UNCONTESTED";

            // Clear sidePot text
            sidePotPanel.ClearSidePot();
            
            foreach(Image img in playerCards)
            {
                img.sprite = CardSprite.GetInstance().back;
                img.color = defaultColor;
            }
            foreach(Image img in communityCards)
            {
                img.sprite = CardSprite.GetInstance().back;
                img.color = defaultColor;
            }
            for(int i = 0; i < GameManager.gameTable.communityCards.Count; i++)
            {
                communityCards[i].color = Color.white;
                communityCards[i].sprite = CardSprite.GetInstance().GetSprite(GameManager.gameTable.communityCards[i]);
            }

            foreach(Player p in GameManager.gameTable.players)
            {
                if(p.state != Player.State.FOLD)
                {
                    winner.text = p.name;
                    break;
                }
            }
            pots.text = GetChipString(GameManager.gameTable.pot);
            return;
        }

        /* POT_FIN */
        PotWinnerManager potManager = GameManager.gameTable.potWinnerManager;
        
        KeyValuePair<int, List<Player>> mainPot = potManager.potWinnerStack.Peek();
        
        // Init playerCards to mainPot first player
        InitPlayerCards(mainPot.Value[0].cards);

        // Init Community cards
        InitCommunityCards(GameManager.gameTable.communityCards);
        
        // Init winners
        winner.text = "";
        foreach(Player p in mainPot.Value)
        {
            winner.text += p.name + " ";
        }

        // Init pots
        pots.text = GetChipString(mainPot.Key);

        // Init hands
        InitHand(mainPot.Value[0].bestHand.hand);

        // Init sidePot if exist
        InitSidePot();
        
    }

    private void InitPlayerCards(List<Card> cards)
    {
        if(cards.Count == 2)
        {   
            for(int i = 0; i < 2; i++)
            {
                playerCards[i].color = Color.white;
                playerCards[i].sprite = CardSprite.GetInstance().GetSprite(cards[i]);
            }
        }
    }

    private void InitCommunityCards(List<Card> cards)
    {
        if(cards.Count == 5)
        {
            for(int i = 0; i < 5; i++)
            {
                communityCards[i].color = Color.white;
                communityCards[i].sprite = CardSprite.GetInstance().GetSprite(cards[i]);
            }
        }
    }

    private void InitHand(Hand hand)
    {
        switch(hand)
        {
            case Hand.HIGHCARD:
                hands.text = "HIGH CARD";
                break;
            case Hand.PAIR:
                hands.text = "PAIR";
                break;
            case Hand.TWOPAIR:
                hands.text = "TWO PAIR";
                break;
            case Hand.THREE_OF_A_KIND:
                hands.text = "THREE OF A KIND";
                break;
            case Hand.STRAIGHT:
                hands.text = "STRAIGHT";
                break;
            case Hand.FLUSH:
                hands.text = "FLUSH";
                break;
            case Hand.FULLHOUSE:
                hands.text = "FULL HOUSE";
                break;
            case Hand.FOUR_OF_A_KIND:
                hands.text = "FOUR OF A KIND";
                break;
            case Hand.STRAIGHT_FLUSH:
                hands.text = "STRAIGHT FLUSH";
                break;
            case Hand.ROYAL_FLUSH:
                hands.text = "ROYAL FLUSH";
                break;
            default:
                break;
        }
    }

    private void InitSidePot()
    {
        if(GameManager.gameTable.potWinnerManager == null)
        {
            return;
        }
        Stack<KeyValuePair<int, List<Player>>> winnerStack = GameManager.gameTable.potWinnerManager.potWinnerStack;
        
        // Mainpot ~ LastSidePot
        KeyValuePair<int, List<Player>>[] kvPairArray = winnerStack.ToArray();

        // Clear
        sidePotPanel.name.text = "";

        // Fist side pot ~ Last side pot
        for(int i = 1; i < kvPairArray.Length; i++)
        {
            sidePotPanel.name.text += GetChipString(kvPairArray[i].Key) + " - ";

            foreach(Player p in kvPairArray[i].Value)
            {
                sidePotPanel.name.text += p.name + " ";
            }
            sidePotPanel.name.text += "\n";
        }
        
    }


}
