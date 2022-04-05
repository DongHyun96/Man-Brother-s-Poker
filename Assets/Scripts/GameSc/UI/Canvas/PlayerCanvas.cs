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
    
    // States enum
    private Player.State m_playerState;

    /*
    * Update GameTable first and then update this prorperty.
    */
    public Player.State playerState{
        get => m_playerState;
        set{
            // Find corresponding player
            Player player = new Player();
            foreach(Player p in GameManager.gameTable.players)
            {
                if(p.name.Equals(name))
                {
                    player = p;
                    break;
                }
            }

            switch(value)
            {
                case Player.State.IDLE: // Init panel
                    // Init contents
                    UpdateTab(player);
                    UpdateCard(player.cards);

                    // Init panel anims
                    if(iconAnim.GetBool("isIn"))
                    {
                        ToggleActionIconAndChip();
                    }
                    if(timerAnim.GetBool("isIn"))
                    {
                        ToggleTimer();
                    }
                    break;

                case Player.State.SMALL:
                    // Init contents
                    UpdateTab(player);
                    UpdateActionPanel(Player.State.SMALL, player.stageBet); // sbChip all in 고려
                    
                    // Toggle panel animation
                    ToggleActionIconAndChip();
                    break;
                case Player.State.BIG:
                    UpdateTab(player);
                    UpdateActionPanel(Player.State.BIG, player.stageBet); // all in 고려

                    // Toggle panel animation
                    ToggleActionIconAndChip();
                    break;
                case Player.State.CHECK:
                    // Init contents
                    UpdateActionPanel(Player.State.CHECK);
                    
                    // Action routine
                    StartCoroutine(ActionAnimationRoutine());
                    break;
                case Player.State.BET:
                    // Init contents
                    UpdateTab(player);
                    UpdateActionPanel(Player.State.BET, player.stageBet);

                    // Action routine
                    StartCoroutine(ActionAnimationRoutine());
                    break;
                case Player.State.CALL:
                    UpdateTab(player);
                    UpdateActionPanel(Player.State.CALL, player.stageBet);

                    StartCoroutine(ActionAnimationRoutine());
                    break;
                case Player.State.RAISE:
                    UpdateTab(player);
                    UpdateActionPanel(Player.State.RAISE, player.stageBet);

                    StartCoroutine(ActionAnimationRoutine());
                    break;
                case Player.State.ALLIN:
                    UpdateTab(player);
                    UpdateActionPanel(Player.State.ALLIN, player.stageBet);

                    StartCoroutine(ActionAnimationRoutine());
                    break;
                case Player.State.FOLD:
                    UpdateActionPanel(Player.State.FOLD);

                    StartCoroutine(ActionAnimationRoutine());
                    break;
                default:
                    break;
            }
            m_playerState = value;
        }
    }

    // Public method for current turn response here
    public void EnableTurn()
    {
        // Enable Timer
        print($"EnableTurn from playerCanvas: {name}");
    }




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

    private void UpdateCard(List<Card> c)
    {
        if(c.Count < 2)
        {
            card1.sprite = null;
            card2.sprite = null;
            return;
        }

        card1.sprite = CardSprite.GetInstance().GetSprite(c[0]);
        card2.sprite = CardSprite.GetInstance().GetSprite(c[1]);
    }

    private void UpdateActionPanel(Player.State s, int chips = 0)
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
            default: //small, big (no needs to update actionText)
                break;
        }

        // Update Chiptext
        chipText.text = chips == 0 ? "" : GetChipString(chips);
    }

    /****************************************************************************************************************
    *                                         TabPanel animation toggling
    ****************************************************************************************************************/
    private void ToggleTabPanel()
    {
        tabPanelAnim.SetBool("isIn", !tabPanelAnim.GetBool("isIn"));
    }

    /****************************************************************************************************************
    *                                         ActionPanel animation toggling
    ****************************************************************************************************************/
    private void ToggleActionIconAndChip()
    {
        iconAnim.SetBool("isIn", !iconAnim.GetBool("isIn"));
        chipTextAnim.SetBool("isIn", !chipTextAnim.GetBool("isIn"));
    }

    private void ToggleActionText()
    {
        actionTextAnim.SetBool("isIn", !actionTextAnim.GetBool("isIn"));
    }

    private void ToggleTimer()
    {
        timerAnim.SetBool("isIn", !timerAnim.GetBool("isIn"));
    }

    private IEnumerator ActionAnimationRoutine()
    {
        if(iconAnim.GetBool("isIn")) // If iconAnim is on already, turn off
        {
            ToggleActionIconAndChip();
        }

        ToggleActionText(); // ToggleActionText first

        yield return new WaitForSeconds(1f); // Wait 1 sec

        ToggleActionIconAndChip(); // ToggleActionIcon

    }
 
    /****************************************************************************************************************
    *                                                    Extra
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
    
    private void Start() {
        //ToggleActionIcon();
    }
    
    
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