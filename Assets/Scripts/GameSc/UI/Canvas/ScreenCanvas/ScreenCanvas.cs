using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ScreenCanvas : MonoBehaviour
{
    public GameObject upperPanel;
    public GameObject bottomLeft;
    public GameObject winnerAnnounce;
    public GameObject chooseShowDown;

    // Waiting for other players text
    public GameObject waitingText;

    // BottomLeft components
    public List<Image> playerCards;
    public List<Image> communityCards;
    public Image foldImage;
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

    // SidePot Component
    public SidePotPanel sidePotPanel;

    // ChooseShowDown Component
    public ShowDownPanel showDownPanel;

    // GameOver Text & return to lobby btn
    public Animator gameOverTextAnim;
    public Animator lobbyBtnAnim;

    // Constants
    private const float WAIT_SEC = 0.001f;

    /*
    *   State and stage property
    *   Update GameTable first and then update these prorperty.
    */
    private GameTable.Stage m_stage;
    private Player.State m_state;

    public GameTable.Stage stage{ // Cards UI update here
        get => m_stage;
        set{
            // Find corresponding player
            Player player = GameManager.gameTable.GetPlayerByName(GameManager.thisPlayer.name);

            if(player == null)
            {
                return;
            }
            switch(value)
            {
                case GameTable.Stage.PREFLOP:
                    // Init playerCards
                    UpdatePlayerCards(player.cards);

                    // Show playerCards
                    //StartCoroutine(PlayerCardsAnimRoutine());
                    break;
                case GameTable.Stage.FLOP:
                case GameTable.Stage.TURN:
                case GameTable.Stage.RIVER:
                    UpdateCommunityCards(GameManager.gameTable.communityCards);
                    //StartCoroutine(CommunityCardsAnimRoutine(value));
                    break;

                case GameTable.Stage.ROUND_FIN:
                    break;
                case GameTable.Stage.UNCONTESTED:
                case GameTable.Stage.POT_FIN:

                    /* Init winner panel and show */
                    winnerPanel.InitWinnerPanel();
                    winnerPanel.ShowPanel();

                    /* Hide upper buttons and buttom left */
                    upperPanel.SetActive(false);
                    bottomLeft.transform.localScale = Vector3.zero; // For animator problem
                    // bottomLeft.SetActive(false);

                    /* Give winning chips to winners */
                    GameManager.gameTable.PayEachPotWinners();
                    
                    /* Update totalChips and pot chips */
                    UpdateTotalChips(player.totalChips);
                    UpdatePotChips(0);

                    /* Update showDown Panel cards */
                    UpdateShowDownPanel(player.cards);

                    break;
                case GameTable.Stage.GAME_FIN:
                    StartCoroutine(GameFinRoutine());
                    break;
            }
            m_stage = value;
        }
    }

    private IEnumerator GameFinRoutine()
    {
        // Show GameOver Text and show Exit to Lobby button
        gameOverTextAnim.SetTrigger("in");
        yield return new WaitForSeconds(5.0f);
    }

    public Player.State state{ // Chips etc update here.
        get => m_state;
        set{
            // Find corresponding player
            Player thisPlayer = GameManager.gameTable.GetPlayerByName(GameManager.thisPlayer.name);
            print($"Screen scene Value = {value}");

            switch(value)
            {
                case Player.State.IDLE: // Init
                    UpdateTotalChips(thisPlayer.totalChips);
                    UpdatePotChips(GameManager.gameTable.pot);

                    // Turn off the unecessary GUI if it is on screen
                    if(pieButtonAnim.GetBool("isIn"))
                    {
                        pieButtonAnim.SetBool("isIn", false);
                    }
                    if(bettingPanelAnim.GetBool("isIn"))
                    {
                        bettingPanelAnim.SetBool("isIn", false);
                    }

                    break;
                case Player.State.CHECK:
                    break;
                case Player.State.FOLD:
                    if(playerCardAnims[0].GetBool("isIn"))
                    {
                        bool temp = thisPlayer.state == Player.State.FOLD ? true : false;
                        foldImage.gameObject.SetActive(temp);
                    }
                    break;

                default: // SMALL, BIG, BET, CALL, RAISE, ALLIN
                    UpdateTotalChips(thisPlayer.totalChips);
                    UpdatePotChips(GameManager.gameTable.pot);
                    break;
            }
            m_state = value;
        }
    }
    

    // Public method for current turn response
    public void EnableTurn(PieButton.ActionState state, int callChips = 0)
    {
        UpdateActionGUI(state, callChips);
        TogglePieButtonAnim(true);
    }

    public void InitCanvas()
    {
        // Turn off the unecessary GUI if it is on screen
        if(pieButtonAnim.GetBool("isIn"))
        {
            pieButtonAnim.SetBool("isIn", false);
        }
        if(bettingPanelAnim.GetBool("isIn"))
        {
            bettingPanelAnim.SetBool("isIn", false);
        }
        
        /* Init player cards */
        if(playerCardAnims[0].GetCurrentAnimatorStateInfo(0).IsName("In"))
        {
            /* Reset all the cards */
            TogglePlayerCardsAnim(0, false);
            TogglePlayerCardsAnim(1, false);
        }

        /* Init Community cards */
        for(int i = 0; i < 5; i++)
            {
                if(communityCardAnims[i].GetCurrentAnimatorStateInfo(0).IsName("In"))
                {
                    ToggleCommunityCardsAnim(i, false);
                }
                else
                {
                    break;
                }
            }

        if(winnerPanel.gameObject.activeSelf)
        {
            winnerPanel.gameObject.SetActive(false);
        }
        if(showDownPanel.gameObject.activeSelf)
        {
            showDownPanel.gameObject.SetActive(false);
        }
        foldImage.gameObject.SetActive(false);

        /* Init pot Chips and player chips */
        UpdatePotChips(0);
        Player p = GameManager.gameTable.GetPlayerByName(GameManager.thisPlayer.name);
        UpdateTotalChips(p.totalChips);
    }



    /****************************************************************************************************************
    *                                                Update contents
    ****************************************************************************************************************/
    private void UpdatePlayerCards(List<Card> cards)
    {
        if(cards.Count < 2)
        {
            return;
        }
        
        // Update BottomLeft cards
        playerCards[0].sprite = CardSprite.GetInstance().GetSprite(cards[0]);
        playerCards[1].sprite = CardSprite.GetInstance().GetSprite(cards[1]);

        // Update Winner Anounce cards
        winnerPanel.playerCards[0].sprite = CardSprite.GetInstance().GetSprite(cards[0]);
        winnerPanel.playerCards[1].sprite = CardSprite.GetInstance().GetSprite(cards[1]);

        // Update showdownPanel cards
        showDownPanel.InitShowDown(cards);
    }

    private void UpdateCommunityCards(List<Card> cards)
    {
        for(int i = 0; i < cards.Count; i++)
        {
            // Update BottomLeft cards
            communityCards[i].sprite = CardSprite.GetInstance().GetSprite(cards[i]);

            // Update Winner Anounce Cards
            winnerPanel.communityCards[i].sprite = CardSprite.GetInstance().GetSprite(cards[i]);
        }
    }

    private void UpdateTotalChips(int chips)
    {
        StartCoroutine(UpdateTotalChipsRoutine(chips));
    }

    private void UpdatePotChips(int chips)
    {
        StartCoroutine(UpdatePotChipsRoutine(chips));
    }

    private void UpdateActionGUI(PieButton.ActionState state, int callChips = 0)
    {
        // Update pieButtons
        foreach(PieButton p in pieButtons)
        {
            p.UpdateContents(state, callChips);
        }

        /* Doesn't need to Init bettingPanel if it is ALLIN_FOLD */
        if(state == PieButton.ActionState.ALLIN_FOLD)
        {
            return;
        }

        // Update BettingPanel
        bettingPanel.InitContents(callChips * 2); // Set floor and ceiling bet in bettingPanel
        //bettingPanel.SetMinBet(callChips * 2); // Set minimum bet to double up, Not Fully implemented yet.
    }

    private void UpdateWinnerAnounce()
    {
        winnerPanel.InitWinnerPanel();
    }

    private void UpdateSidePotPanel(List<Player> players)
    {
        sidePotPanel.InitSidePot(players);
    }

    private void UpdateShowDownPanel(List<Card> cards)
    {
        if(cards.Count < 2)
        {
            return;
        }
        showDownPanel.InitShowDown(cards);
    }
    
    /****************************************************************************************************************
    *                                                Animation toggling
    ****************************************************************************************************************/
    public void TogglePlayerCardsAnim(int idx, bool isIn)
    {
        playerCardAnims[idx].SetBool("isIn", isIn);
    }

    public void ToggleCommunityCardsAnim(int idx, bool isIn)
    {
        communityCardAnims[idx].SetBool("isIn", isIn);
    }

    /* private void TogglePieButtonAnim()
    {
        pieButtonAnim.SetBool("isIn", !pieButtonAnim.GetBool("isIn"));
    } */

    private void TogglePieButtonAnim(bool isIn)
    {
        pieButtonAnim.SetBool("isIn", isIn);
    }

    private void ToggleBettingPanelAnim()
    {
        bettingPanelAnim.SetBool("isIn", !bettingPanelAnim.GetBool("isIn"));
    }

    private const float CARD_SEC = 1f;

    /****************************************************************************************************************
    *                                                Action Button methods
    ****************************************************************************************************************/
    // CHECK_BET_FOLD, CALL_RAISE_FOLD, CHECK_RAISE_FOLD(Big blind), ALLIN_FOLD

    /* Action routine : Update gameTable -> Send it to server ->
     Receive updated gameTable from server -> Update gameTable and UI */
    public void OnPieLeft() // CHECK, CALL, ALLIN
    {
        switch(pieButtons[0].State)
        {
            case PieButton.ActionState.CHECK_BET_FOLD:
            case PieButton.ActionState.CHECK_RAISE_FOLD:
                GameManager.gameTable.TakeAction(GameManager.thisPlayer.name, Player.State.CHECK);
                GameMsgHandler.TossTable();
                TogglePieButtonAnim(false);
                break;
            case PieButton.ActionState.CALL_RAISE_FOLD:
                GameManager.gameTable.TakeAction(GameManager.thisPlayer.name,
                 Player.State.CALL, GameManager.gameTable.roundBetMax);
                
                GameMsgHandler.TossTable();
                TogglePieButtonAnim(false);
                break;
            case PieButton.ActionState.ALLIN_FOLD:
                GameManager.gameTable.TakeAction(GameManager.thisPlayer.name, Player.State.ALLIN);

                GameMsgHandler.TossTable();
                TogglePieButtonAnim(false);
                break;
        }
    }

    public void OnPieRight()
    {
        if(pieButtons[0].State == PieButton.ActionState.ALLIN_FOLD) // When player should have to all in or fold
        {
            GameManager.gameTable.TakeAction(GameManager.thisPlayer.name, Player.State.ALLIN);

            GameMsgHandler.TossTable();
            TogglePieButtonAnim(false);
            return;
        }

        TogglePieButtonAnim(false);
        ToggleBettingPanelAnim();
    }

    public void OnPieButtom() // Fold btn
    {
        GameManager.gameTable.TakeAction(GameManager.thisPlayer.name, Player.State.FOLD);

        GameMsgHandler.TossTable();

        TogglePieButtonAnim(false);
    }

    public void OnTimerTimeOut(int idx)
    {
        // If it isn't player's turn
        int pIdx = GameManager.gameTable.GetIterPosByName(GameManager.thisPlayer.name);
        Player current = GameManager.gameTable.GetCurrentPlayer();
        if(pIdx != idx || !current.name.Equals(GameManager.thisPlayer.name))
        {
            return;
        }

        // Forces the action to be taken
        switch(pieButtons[0].State)
        {
            case PieButton.ActionState.CHECK_BET_FOLD:
            case PieButton.ActionState.CHECK_RAISE_FOLD:
                OnPieLeft();
                break;
            case PieButton.ActionState.CALL_RAISE_FOLD:
            case PieButton.ActionState.ALLIN_FOLD:
                OnPieButtom();
                break;
        }
    }

    
    public void OnBettingPanelAllIn()
    {
        GameManager.gameTable.TakeAction(GameManager.thisPlayer.name, Player.State.ALLIN);

        GameMsgHandler.TossTable();

        ToggleBettingPanelAnim();
    }

    public void OnBettingpanelCancel()
    {
        ToggleBettingPanelAnim();
        TogglePieButtonAnim(true);
    }

    public void OnBettingPanelBet()
    {
        switch(pieButtons[0].State)
        {
            case PieButton.ActionState.CHECK_BET_FOLD: // Bet
                Player.State pState = Player.State.BET;
                // All in case
                if(bettingPanel.Ceil == bettingPanel.betChips)
                {
                    pState = Player.State.ALLIN;
                }
                GameManager.gameTable.TakeAction(GameManager.thisPlayer.name, pState,
                bettingPanel.betChips);
                break;
            case PieButton.ActionState.CALL_RAISE_FOLD: // Raise
            case PieButton.ActionState.CHECK_RAISE_FOLD:
                Player.State s = Player.State.RAISE;
                // All in case
                if(bettingPanel.Ceil == bettingPanel.betChips)
                {
                    s = Player.State.ALLIN;
                }
                GameManager.gameTable.TakeAction(GameManager.thisPlayer.name, s,
                bettingPanel.betChips);
                break;
        }
        
        GameMsgHandler.TossTable();
        ToggleBettingPanelAnim();
    }

    /****************************************************************************************************************
    *                                                Extra Methods
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
            while(playerChips_Num < chips)
            {
                playerChips_Num += GameManager.gameTable.sbChip;
                playerChips.text = GetChipString(playerChips_Num);
                yield return new WaitForSeconds(WAIT_SEC);
            }
        }
        else if(playerChips_Num > chips)
        {
            while(playerChips_Num > chips)
            {
                playerChips_Num -= GameManager.gameTable.sbChip;
                playerChips.text = GetChipString(playerChips_Num);
                yield return new WaitForSeconds(WAIT_SEC);
            }
        }
        playerChips.text = GetChipString(chips);
    }

    private IEnumerator UpdatePotChipsRoutine(int chips)
    {
        if(pot_Num < chips)
        {
            while(pot_Num < chips)
            {
                pot_Num += GameManager.gameTable.sbChip;
                pot.text = GetChipString(pot_Num);
                yield return new WaitForSeconds(WAIT_SEC);
            }
        }
        else if(pot_Num > chips)
        {
            while(pot_Num > chips)
            {
                pot_Num -= GameManager.gameTable.sbChip;
                pot.text = GetChipString(pot_Num);
                yield return new WaitForSeconds(WAIT_SEC);
            }
        }
        pot.text = GetChipString(chips);
    }
}
