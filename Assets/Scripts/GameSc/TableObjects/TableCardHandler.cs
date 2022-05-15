using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableCardHandler : MonoBehaviour
{
    /* Place the prefab clone object as child of these objects */
    [SerializeField] private List<GameObject> playerFirstCards;
    [SerializeField] private List<GameObject> playerSecondCards;
    [SerializeField] private List<GameObject> communityCards;
    [SerializeField] private List<GameObject> discards;

    public void DrawFirstCard(int idx, Card c)
    {
        /* Destroy child if it has one */
        for(int i = 0; i < playerFirstCards[idx].transform.childCount; i++)
        {
            Destroy(playerFirstCards[idx].transform.GetChild(i).gameObject);
        }

        /* Set card */
        GameObject prefab = CardHolder.GetInstance().GetCardPrefab(c);
        GameObject container = Instantiate(prefab);
        SetParent(container.transform, playerFirstCards[idx].transform);

        /* Play first card giving anim */

    }

    public void DrawSecondCard(int idx, Card c)
    {
        /* Set card */
        /* Play second card giving anim */
    }

    public void DrawCommunityCard(int idx, Card c)
    {
        /* Set card */
        /* Play */
    }

    public void Discard(int idx)
    {
        /* Play */
    }

    private void Start() {
        /* Card c = new Card(Card.Suit.CLUB, 5);
        GameObject prefab = CardHolder.GetInstance().GetCardPrefab(c);
        GameObject container = Instantiate(prefab);
        SetParent(container.transform, playerFirstCards[0].transform);

        Animator anim = playerFirstCards[0].GetComponent<Animator>();
        StartCoroutine(example(anim)); */
    }

    private void SetParent(Transform child, Transform parent)
    {
        child.parent = parent;
        child.localPosition = Vector3.zero;
        child.localRotation = Quaternion.identity;
        child.localScale = Vector3.one;
    }

    private IEnumerator example(Animator anim)
    {
        yield return new WaitForSeconds(7.0f);
        anim.SetInteger("index", 0);
        anim.SetTrigger("first");
        yield return new WaitForSeconds(3.0f);
        anim.SetTrigger("goToDeck");
        yield return new WaitForSeconds(3.0f);
        anim.SetInteger("index", 3);
        anim.SetTrigger("second");

        yield return new WaitForSeconds(3.0f);

        anim.SetTrigger("goToDeck");
        yield return new WaitForSeconds(3.0f);
        
        anim.SetInteger("index", 0);
        anim.SetTrigger("community");

    }
}
