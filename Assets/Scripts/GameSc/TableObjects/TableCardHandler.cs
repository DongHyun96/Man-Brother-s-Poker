using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableCardHandler : MonoBehaviour
{
    /* Place the prefab clone object as child of these objects */
    public List<GameObject> playerFirstCards;
    public List<GameObject> playerSecondCards;
    public List<GameObject> communityCards;
    public List<GameObject> discards;

    public enum DrawType{
        FIRST, SECOND, COMMUNITY, DISCARD
    }

    public void DrawCard(DrawType type, int idx, Card c)
    {
        /* Get corresponding anim trigger and card list */
        string trigger = "";
        List<GameObject> cardList = GetCorrespondingCardList(type, out trigger);

        /* Set Card */
        GameObject prefab = CardHolder.GetInstance().GetCardPrefab(c);
        GameObject container = Instantiate(prefab, prefab.transform.position, prefab.transform.rotation);
        SetParent(container.transform, cardList[idx].transform);

        /* Play anim */
        Animator anim = cardList[idx].GetComponent<Animator>();
        anim.SetInteger("index", idx);
        anim.SetTrigger(trigger);
    }

    public void FoldPlayerCards(int idx)
    {
        GameObject targetCard1 = playerFirstCards[idx];
        GameObject targetCard2 = playerSecondCards[idx];

        /* Play anim */
        Animator anim1 = targetCard1.GetComponent<Animator>();
        Animator anim2 = targetCard2.GetComponent<Animator>();
        anim1.SetTrigger("fold");
        anim2.SetTrigger("fold");
    }

    public void FlipPlayerCard(int pIdx, int cardIdx)
    {
        /* Get corresponding card object */
        GameObject targetCard = cardIdx == 0 ? playerFirstCards[pIdx] : playerSecondCards[pIdx];

        /* Play anim */
        Animator anim = targetCard.GetComponent<Animator>();
        anim.SetTrigger("flip");
    }

    /* To do - Delete gameObject after animation finished */
    /* Do this by animation events */
    public void GatherCardsToDeck()
    {
        foreach(GameObject obj in playerFirstCards)
        {
            Animator anim = obj.GetComponent<Animator>();
            anim.SetTrigger("goToDeck");
        }
        // foreach(GameObject obj in )
        for(int i = 0; i < playerFirstCards.Count; i++)
        {
            Animator firstCardAnim = playerFirstCards[i].GetComponent<Animator>();
            Animator secondCardAnim = playerSecondCards[i].GetComponent<Animator>();

            firstCardAnim.SetTrigger("goToDeck");
            secondCardAnim.SetTrigger("goToDeck");
        }
        foreach(GameObject obj in discards)
        {
            Animator anim = obj.GetComponent<Animator>();
            anim.SetTrigger("goToDeck");
        }
        foreach(GameObject obj in communityCards)
        {
            Animator anim = obj.GetComponent<Animator>();
            anim.SetTrigger("goToDeck");
        }

    }

    /* private void Start() {
        StartCoroutine(example());
    } */

    private void SetParent(Transform child, Transform parent)
    {
        child.parent = parent;
        child.localPosition = Vector3.zero;
        // child.localScale = 20 * Vector3.one;
    }

    private IEnumerator example()
    {
        yield return new WaitForSeconds(7.0f);
        Card c = new Card(Card.Suit.HEART, 12);
        /* Draw cards to player test */
        DrawCard(DrawType.DISCARD, 0, c);
        for(int i = 0; i < 6; i++)
        {
            yield return new WaitForSeconds(0.2f);
            DrawCard(DrawType.FIRST, i, c);
        }
        for(int i = 0; i < 6; i++)
        {
            yield return new WaitForSeconds(0.2f);
            DrawCard(DrawType.SECOND, i, c);
        }

        yield return new WaitForSeconds(2.0f);

        /* Draw cards to community test */
        DrawCard(DrawType.DISCARD, 1, c);
        for(int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.2f);
            DrawCard(DrawType.COMMUNITY, i, c);
        }
        yield return new WaitForSeconds(3.0f);

        /* Fold player cards */
        FoldPlayerCards(3);
        FoldPlayerCards(4);
        yield return new WaitForSeconds(3.0f);

        /* Flip players card */
        for(int i = 0; i < 6; i++)
        {
            FlipPlayerCard(0, i);
            FlipPlayerCard(1, i);
            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(5.0f);
        /* Gather cards to deck */
        GatherCardsToDeck();

    }

    private List<GameObject> GetCorrespondingCardList(DrawType t, out string trigger)
    {
        switch(t)
        {
            case DrawType.FIRST:
                trigger = "first";
                return playerFirstCards;
            case DrawType.SECOND:
                trigger = "second";
                return playerSecondCards;
            case DrawType.COMMUNITY:
                trigger = "community";
                return communityCards;
            case DrawType.DISCARD:
                trigger = "discard";
                return discards;
            default:
                trigger = "";
                return null;
        }
    }
}
