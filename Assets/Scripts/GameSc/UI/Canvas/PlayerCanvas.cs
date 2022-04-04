using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class PlayerCanvas : MonoBehaviour
{

    public string name; // To do: Init name first

    public GameObject tabPanel;
    public GameObject cards;
    public GameObject actionPanel;

    // TabPanel components
    public Text tab_name;
    public Text tab_totalChips;
    public Text tab_rank;

    // Cards components
    public Image card1;
    public Image card2;

    // ActionPanel components
    public Image action_icon;
    public Text actionText;
    public Text chipText;
    public GameObject timer;
    public Image fill;

    // TabPanel animators
    public Animator tabPanelAnim;

    // ActionPanel animators
    public Animator iconAnim;
    public Animator actionTextAnim;
    public Animator chipTextAnim;
    public Animator timerAnim;

    /****************************************************************************************************************
    *                                                Update contents
    ****************************************************************************************************************/
    public void UpdateTab(Player p)
    {
        if(name.Equals(p.name))
        {
            // Init player name
            tab_name.text = name;

            // Update total chips
            tab_totalChips.text = GetChipString(p.totalChips);
            
            // Update rank    
            int rank = GetRanking();
            switch(rank)
            {
                case 1:
                    tab_rank.text = "1st";
                    break;
                case 2:
                    tab_rank.text = "2nd";
                    break;
                case 3:
                    tab_rank.text = "3rd";
                    break;
                default:
                    tab_rank.text = $"{rank}th";
                    break;
            }
            
        }
    }

    public void UpdateCard(List<Card> c)
    {
        card1.sprite = CardSprite.GetInstance().GetSprite(c[0]);
        card2.sprite = CardSprite.GetInstance().GetSprite(c[1]);
    }

    public void UpdateActionPanel(Player.State s, int chips)
    {
        // Update icon
        action_icon.sprite = IconSprite.GetInstance().GetSprite(s);

        // Update actionText
        switch(s)
        {
            case Player.State.CHECK:
                actionText.text = "Check";
                break;
            case Player.State.BET:
                actionText.text = "Bet";
                break;
            case Player.State.CALL:
                actionText.text = "Call";
                break;
            case Player.State.RAISE:
                actionText.text = "Raise";
                break;
            case Player.State.ALLIN:
                actionText.text = "All In";
                break;
            case Player.State.FOLD:
                actionText.text = "Fold";
                break;
            default:
                break;
        }

        // Update Chiptext
        chipText.text = GetChipString(chips);
    }

    /****************************************************************************************************************
    *                                         TabPanel animation toggling
    ****************************************************************************************************************/
    public void ToggleTabPanel()
    {
        tabPanelAnim.SetBool("isIn", !tabPanelAnim.GetBool("isIn"));
    }

    /****************************************************************************************************************
    *                                         ActionPanel animation toggling
    ****************************************************************************************************************/
    public void ToggleActionIcon()
    {
        iconAnim.SetBool("isIn", !iconAnim.GetBool("isIn"));
    }

    public void ToggleActionText()
    {
        actionTextAnim.SetBool("isIn", !actionTextAnim.GetBool("isIn"));
    }

    public void ToggleChipText()
    {
        chipTextAnim.SetBool("isIn", !chipTextAnim.GetBool("isIn"));
    }

    public void ToggleTimer()
    {
        timerAnim.SetBool("isIn", !timerAnim.GetBool("isIn"));
    }
 
    /****************************************************************************************************************
    *                                                    Private Methods
    ****************************************************************************************************************/
    private string GetChipString(int chips)
    {
        return (chips < 1000) ? chips.ToString() : (Math.Round(chips / 1000f, 2)).ToString() + "k";
    }

    private int GetRanking()
    {
        List<Player> sorted = GameManager.gameTable.players.OrderByDescending(x => x.totalChips).ToList();
        
        for(int cur = 0; cur < sorted.Count; cur++)
        {
            if(sorted[cur].name.Equals(name))
            {
                if(cur == 0)
                {
                    return cur + 1; // 1st rank
                }

                // Filtering the same rank
                int prev = cur - 1;
                while(cur > 0)
                {
                    if(sorted[prev].totalChips == sorted[cur].totalChips)
                    {
                        cur = prev;
                        prev--;
                    }
                    else
                    {
                        break;
                    }
                }
                return cur + 1;
            }
        }
        return -1; // Dummy return
    }
    /****************************************************************************************************************
    *                                                    Keys
    ****************************************************************************************************************/
    /*
    private void Start() {
        ToggleActionIcon();
    }
    */
    
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Tab)) // Toggle tab panel
        {
            ToggleTabPanel();

            // Toggle cards

            // Toggle ActionPanel animation
            // Needs to figure this out...
            // throw new NotImplementedException();
        }
    }
}
