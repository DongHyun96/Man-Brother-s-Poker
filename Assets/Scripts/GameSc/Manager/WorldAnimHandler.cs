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

    public IEnumerator PreflopRoutine(List<Player> players, int myIdx)
    {
        cardHandler.DrawCard(TableCardHandler.DrawType.DISCARD, 0, new Card(Card.Suit.CLUB, 0));

        yield return new WaitForSeconds(0.2f);

        for(int i = 0; i < players.Count; i++)
        {
            Card c = players[i].cards[0];
            cardHandler.DrawCard(TableCardHandler.DrawType.FIRST, i, c);
            yield return new WaitForSeconds(0.2f);
        }
        for(int i = 0; i < players.Count; i++)
        {
            Card c = players[i].cards[1];
            cardHandler.DrawCard(TableCardHandler.DrawType.SECOND, i, c);
            yield return new WaitForSeconds(0.2f);
        }

        AnimTurningPointHandler h = cardHandler.playerSecondCards[myIdx].GetComponent<AnimTurningPointHandler>();
        yield return StartCoroutine(WaitForTurningPoint(h));
    }

    private IEnumerator WaitForTurningPoint(AnimTurningPointHandler handler)
    {
        while(!handler.IsTurningPointPassed)
        {
            yield return null;
        }
    }
}
