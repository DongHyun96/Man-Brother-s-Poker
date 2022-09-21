using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Use this object to sync each animations */  
/* start animation and then return the AnimTurningPointHandler of the end */
public class WorldAnimHandler : MonoBehaviour
{
    public List<GameCharacter> gameCharacters;
    public TableChipHandler chipHandler;
    public TableCardHandler cardHandler;
    
    public List<Animator> chairAnims;


    public void Init(GameTable table)
    {
        /* Init Characters */
        for(int i = 0; i < table.players.Count; i++)
        {
            gameCharacters[i].Init(table.players[i].character);
        }
        
        /* Init tableChipHandler's buyIn */
        chipHandler.InitBuyIn(table.buy);
    }

    public IEnumerator AnimateGreetings()
    {
        foreach(GameCharacter character in gameCharacters)
        {
            character.AnimateCharacter(GameCharacter.AnimType.GREET);
        }
        int playerCount = GameManager.gameTable.players.Count;
        AnimTurningPointHandler h = gameCharacters[playerCount - 1].characterObject.GetComponent<AnimTurningPointHandler>();
        yield return StartCoroutine(WaitForTurningPoint(h));
    }

    public IEnumerator AnimateFold(int idx)
    {
        gameCharacters[idx].AnimateCharacter(GameCharacter.AnimType.THROWCARDS);
        AnimTurningPointHandler h = gameCharacters[idx].characterObject.GetComponent<AnimTurningPointHandler>();
        yield return StartCoroutine(WaitForTurningPoint(h));

        cardHandler.FoldPlayerCards(idx);
        AnimTurningPointHandler handler = cardHandler.playerSecondCards[idx].GetComponent<AnimTurningPointHandler>();
        yield return StartCoroutine(WaitForTurningPoint(handler));
    }

    public IEnumerator AnimateShowDown(int pIdx, List<bool> showDownBool)
    {
        /* Animate character's fliping */
        gameCharacters[pIdx].AnimateCharacter(GameCharacter.AnimType.THROWCARDS);
        AnimTurningPointHandler h = gameCharacters[pIdx].characterObject.GetComponent<AnimTurningPointHandler>();
        yield return StartCoroutine(WaitForTurningPoint(h));

        /* Animate cards */
        for(int i = 0; i < showDownBool.Count; i++)
        {
            if(showDownBool[i])
            {
                cardHandler.FlipPlayerCard(pIdx, i);
            }
        }
        yield return new WaitForSeconds(1f);
    }

    /* idx = position */
    public IEnumerator AnimateBetting(int idx, Player p)
    {
        gameCharacters[idx].AnimateCharacter(GameCharacter.AnimType.BET);

        /* Wait for turningPoint */
        AnimTurningPointHandler h = gameCharacters[idx].characterObject.GetComponent<AnimTurningPointHandler>();
        yield return StartCoroutine(WaitForTurningPoint(h));

        // Update player chips and then animate betting chips
        int totalChips = p.totalChips;
        int betChips = p.roundBet;
        chipHandler.UpdateChips(TableChipHandler.ContentType.PLAYER, idx, totalChips);
        chipHandler.MoveChips(TableChipHandler.AnimType.BET, idx, betChips, betChips);
        
        AnimTurningPointHandler handler = chipHandler.m_playerChips[idx].GetComponent<AnimTurningPointHandler>();
        yield return StartCoroutine(WaitForTurningPoint(handler));
    }

    public IEnumerator AnimateWinningPot(int idx)
    {
        /* Animate Characters' winning pot */
        gameCharacters[idx].AnimateCharacter(GameCharacter.AnimType.POTWIN);
        yield return null;
    }

    public IEnumerator AnimateWinningGame(int idx)
    {
        yield return null;

        /* Pull the chair */
        chairAnims[idx].SetInteger("index", idx);
        chairAnims[idx].SetTrigger("out");

        /* Animate Character's Game win */
        gameCharacters[idx].AnimateCharacter(GameCharacter.AnimType.GAMEWIN);
    }

    public IEnumerator RoundFinRoutine(List<Player> players, int potChips)
    {
        /* Collecting players' chip into pot */
        
        // Play collecting chips sound if there is chips
        if(chipHandler.IsBetChipsIn)
        {
            SfxHolder.GetInstance().PlaySfx(GameSfxHolder.SoundType.CHIP, 1);
        }

        /* Update betting chips to zero and then move chips */
        for(int i = 0; i < players.Count; i++)
        {
            chipHandler.UpdateChips(TableChipHandler.ContentType.BET, i, 0);
            chipHandler.MoveChips(TableChipHandler.AnimType.BET_TO_POT, i, players[i].roundBet, potChips);   
        }
        AnimTurningPointHandler h = chipHandler.m_playerChips[players.Count - 1].GetComponent<AnimTurningPointHandler>();
        yield return StartCoroutine(WaitForTurningPoint(h));
    }

    public IEnumerator PreflopRoutine(GameTable table, int myIdx)
    {
        // Play card shuffling sound
        SfxHolder.GetInstance().PlaySfx(GameSfxHolder.SoundType.CARD, 0);
        yield return new WaitForSeconds(1f);

        List<Player> players = table.players;
        cardHandler.DrawCard(TableCardHandler.DrawType.DISCARD, 0, new Card(Card.Suit.CLUB, 0));

        yield return new WaitForSeconds(0.2f);
        
        /* Give first card to small blind first */
        int idx = table.SB_Pos;
        do
        {
            Card c = players[idx].cards[0];
            cardHandler.DrawCard(TableCardHandler.DrawType.FIRST, idx, c); 
            idx = table.GetNext(idx, true);
            yield return new WaitForSeconds(0.2f);
        }
        while(idx != table.SB_Pos);

        do
        {
            Card c = players[idx].cards[1];
            cardHandler.DrawCard(TableCardHandler.DrawType.SECOND, idx, c);
            idx = table.GetNext(idx, true);
            yield return new WaitForSeconds(0.2f);
        }
        while(idx != table.SB_Pos);

        // ThisPlayer bankrupt case
        if(table.players[myIdx].state == Player.State.FOLD)
        {
            yield break;
        }
        
        AnimTurningPointHandler h = cardHandler.playerSecondCards[myIdx].GetComponent<AnimTurningPointHandler>();
        yield return StartCoroutine(WaitForTurningPoint(h));
    }

    public IEnumerator FlopTurnRiverRoutine(GameTable.Stage stage, List<Card> comm)
    {
        int idx = (int)stage;
        cardHandler.DrawCard(TableCardHandler.DrawType.DISCARD, idx, new Card(Card.Suit.CLUB, 1));
        yield return new WaitForSeconds(0.2f);
        AnimTurningPointHandler h = cardHandler.communityCards[idx + 1].GetComponent<AnimTurningPointHandler>();
        switch(stage)
        {
            case GameTable.Stage.FLOP:
                for(int i = 0; i < 3; i++)
                {
                    cardHandler.DrawCard(TableCardHandler.DrawType.COMMUNITY, i, comm[i]);
                    yield return new WaitForSeconds(0.2f);
                }
                break;
            case GameTable.Stage.TURN:
                cardHandler.DrawCard(TableCardHandler.DrawType.COMMUNITY, 3, comm[3]);
                yield return new WaitForSeconds(0.2f);
                break;
            case GameTable.Stage.RIVER:
                cardHandler.DrawCard(TableCardHandler.DrawType.COMMUNITY, 4, comm[4]);
                yield return new WaitForSeconds(0.2f);
                break;
        }
        yield return StartCoroutine(WaitForTurningPoint(h));
    }

    public IEnumerator PrepareNextPotRoutine(Stack<KeyValuePair<int, List<Player>>> potWinnerStack)
    {
        /* Collecting cards and give winner pot chips */
        cardHandler.GatherCardsToDeck();

        chipHandler.UpdateChips(TableChipHandler.ContentType.POT, 0, 0);

        int potChipIdx = 0;

        while(potWinnerStack.Count != 0)
        {
            KeyValuePair<int, List<Player>> potPair = potWinnerStack.Pop();

            foreach(Player p in potPair.Value)
            {
                int idx = GameManager.gameTable.GetIterPosByName(p.name);
                potChipIdx = idx;
                int chips = potPair.Key / potPair.Value.Count;
                chipHandler.MoveChips(TableChipHandler.AnimType.POT_TO_PLAYER, idx, chips, p.totalChips);
            }
        }

        AnimTurningPointHandler h = chipHandler.m_potChips[potChipIdx].GetComponent<AnimTurningPointHandler>();
        yield return StartCoroutine(WaitForTurningPoint(h));
    }

    private IEnumerator WaitForTurningPoint(AnimTurningPointHandler handler)
    {
        /* while(!handler.IsTurningPointPassed)
        {
            yield return null;
        } */
        yield return new WaitUntil(() => handler.IsTurningPointPassed);

        // yield return new WaitForSecondsRealtime
    }
}
