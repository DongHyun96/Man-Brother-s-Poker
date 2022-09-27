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
    public Timer timer;

    // TabPanel animators
    public Animator tabPanelAnim;

    // ActionPanel animators
    public Animator iconAnim;
    public Animator actionTextAnim;
    public Animator chipTextAnim;

    // GameFin fields
    private bool isGameOver = false;
    public Text winnerText;

    // States enum
    private Player.State m_playerState;

    /*
    * Update GameTable first and then update this prorperty.
    */
    public Player.State playerState{
        get => m_playerState;
        set{
            // Find corresponding player
            Player player = GameManager.gameTable.GetPlayerByName(name);
            
            if(player == null)
            {
                return;
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
                    if(timer.gameObject.GetComponent<Animator>().GetBool("isIn"))
                    {
                        ToggleTimer(false);
                    }

                    // If the table status is ALLIN, do not close the cards
                    if(GameManager.gameTable.tableStatus != GameTable.TableStatus.ALLIN)
                    {
                        CloseCards();
                    }
                    break;
                case Player.State.SMALL:
                case Player.State.BIG:
                    // Init contents
                    UpdateTab(player);
                    UpdateCard(player.cards);
                    
                    UpdateActionPanel(value, player.roundBet); // sbChip all in 고려
                    CloseCards();
                    
                    // Toggle panel animation
                    ToggleActionIconAndChip();
                    break;

                case Player.State.CHECK:
                case Player.State.FOLD:
                    // Init timer
                    ToggleTimer(false);

                    // Init contents
                    UpdateActionPanel(value);

                    // Play TakeActionSound
                    SfxHolder.GetInstance().PlaySfx(GameSfxHolder.SoundType.ACTION, 1);
                    
                    // Action routine
                    StartCoroutine(ActionAnimationRoutine());
                    break;
                case Player.State.BET:
                case Player.State.CALL:
                case Player.State.RAISE:
                case Player.State.ALLIN:
                    // Init timer
                    ToggleTimer(false);

                    UpdateTab(player);
                    UpdateActionPanel(value, player.roundBet);

                    // Play TakeActionSound
                    SfxHolder.GetInstance().PlaySfx(GameSfxHolder.SoundType.ACTION, 1);

                    StartCoroutine(ActionAnimationRoutine());
                    break;
                default:
                    break;
            }
            m_playerState = value;
        }
    }

    /* First initialization */
    public void Init(string name)
    {
        /* Activate self */
        gameObject.SetActive(true);

        /* Set name */
        this.name = name;
    }

    // Public method for current turn response here
    public void EnableTurn()
    {
        // Enable Timer
        print($"EnableTurn from playerCanvas: {name}");
        ToggleTimer(true);
    }

    public void GameOver()
    {
        if(!gameObject.activeSelf)
        {
            return;
        }

        // Update Tab and Fix the tab
        isGameOver = true;

        Player player = GameManager.gameTable.GetPlayerByName(name);
        int rank = GetRanking();

        UpdateTab(player);
        if(!tabPanelAnim.GetBool("isIn"))
        {
            tabPanelAnim.SetBool("isIn", true);
        }

        switch(GameManager.gameTable.mode)
        {
            case Room.Mode.LASTMAN:
                // Only show rank of lastMan
                if(rank != 1)
                {
                    tab_rank.text = "";
                }
                break;
        }
        
        // Show winnerText if the rank is 1st
        if(rank == 1)
        {
            StartCoroutine(WinnerTextRoutine());
        }
    }
    

    private IEnumerator WinnerTextRoutine()
    {
        winnerText.text = "Y";
        for(int i = 0; i < 7; i++)
        {
            yield return new WaitForSeconds(0.2f);
            winnerText.text += "E";
        }
        yield return new WaitForSeconds(0.2f);
        winnerText.text += "!";
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
                case -1:
                    // Left in game scene case
                    tab_rank.text = "Left";
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
    *                                               Cards toggling
    ****************************************************************************************************************/
    public void OpenCards(bool isFirstOpen, bool isSecondOpen)
    {
        card1.gameObject.SetActive(true);
        card2.gameObject.SetActive(true);

        Color defaultColor = new Color(41 / 255f, 41 / 255f, 41 / 255f);
        Sprite back = CardSprite.GetInstance().GetBack();

        card1.color = isFirstOpen ? Color.white : defaultColor;
        card2.color = isSecondOpen ? Color.white : defaultColor;
        
        card1.sprite = isFirstOpen ? card1.sprite : back;
        card2.sprite = isSecondOpen ? card2.sprite : back;
    }

    public void CloseCards()
    {
        card1.gameObject.SetActive(false);
        card2.gameObject.SetActive(false);
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

    private void ToggleTimer(bool isActive)
    {
        //timerAnim.SetBool("isIn", isActive);
        timer.IsTimerActive = isActive;
    }

    private IEnumerator ActionAnimationRoutine()
    {
        if(iconAnim.GetBool("isIn")) // If iconAnim is on already, turn off
        {
            ToggleActionIconAndChip();
        }

        ToggleActionText(); // ToggleActionText first

        yield return new WaitForSeconds(1f); // Wait 1 sec
        
        ToggleActionText();
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

        // Remove left players
        int idx = 0;
        while(idx < sorted.Count)
        {
            if(!sorted[idx].isInGame)
            {
                sorted.RemoveAt(idx);
            }
            else{
                ++idx;
            }
        }
        
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
        if(!isGameOver && Input.GetKeyDown(KeyCode.Tab)) // Toggle tab panel
        {
            ToggleTabPanel();
        }
    }
}
