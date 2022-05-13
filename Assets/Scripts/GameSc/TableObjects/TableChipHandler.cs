using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TableChipHandler : MonoBehaviour
{
    /* Place Holders */
    [SerializeField] private List<GameObject> playerChips;
    [SerializeField] private List<GameObject> bettingChips;
    [SerializeField] private GameObject potChips;

    /* These are for transformation (Moving chips) */
    [SerializeField] private List<GameObject> m_playerChips;
    [SerializeField] private List<GameObject> m_potChips;

    private int buyIn;

    public void InitBuyIn(int buyIn)
    {
        this.buyIn = buyIn;
    }

    /* idx -> position */
    public void UpdatePlayerChips(int idx, int chips)
    {
        if(playerChips[idx] != null)
        {
            /* Destroy previous prefab */
            Destroy(playerChips[idx]);
        }

        GameObject container = ChipHolder.GetInstance().GetChipPrefab(chips, buyIn);
        playerChips[idx] = Instantiate(container, playerChips[idx].transform.position, playerChips[idx].transform.rotation);

    }

    public void UpdateBettingChips(int idx, int chips)
    {
        if(bettingChips[idx] != null)
        {
            Destroy(bettingChips[idx]);
        }

        GameObject container = ChipHolder.GetInstance().GetChipPrefab(chips, buyIn);
        bettingChips[idx] = Instantiate(container, bettingChips[idx].transform.position, bettingChips[idx].transform.rotation);
    }

    public void UpdatePotChips(int chips)
    {
        if(potChips != null)
        {
            Destroy(potChips);
        }

        GameObject container = ChipHolder.GetInstance().GetPotPrefab(chips, buyIn);
        potChips = Instantiate(container, potChips.transform.position, potChips.transform.rotation);
        
    }

    /* Chips moving Coroutine */
    public IEnumerator MovePlayerChipsToBetting(int idx, int chips)
    {
        /* Set moving chips gameObject as child */
        GameObject prefab = ChipHolder.GetInstance().GetChipPrefab(chips, buyIn);
        GameObject container = Instantiate(prefab);
        SetParent(container.transform, m_playerChips[idx].transform);
        
        /* Animate moving chips */
        Animator anim = m_playerChips[idx].GetComponent<Animator>();
        anim.SetTrigger("bet");
        
        
        /* Wait for animation to be finished */
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        /* Update arrival point chips */
        UpdateBettingChips(idx, chips);

        /* Destroy child object*/
        Destroy(container);
    }

    public IEnumerator MoveBettingsToPotChips(List<int> roundChips, int potChips)
    {
        /* Set moving chips gameObject as child */
        List<GameObject> containers = new List<GameObject>();
        for(int i = 0; i < m_playerChips.Count; i++)
        {
            if(roundChips[i] == 0)
            {
                continue;
            }

            GameObject prefab = ChipHolder.GetInstance().GetChipPrefab(roundChips[i], buyIn);
            GameObject container = Instantiate(prefab);
            SetParent(container.transform, m_playerChips[i].transform);
            containers.Add(container);

            /* Animate moving chips */
            Animator anim = m_playerChips[i].GetComponent<Animator>();
            anim.SetTrigger("pot");
        }

        /* Wait for last animation to be finished */
        Animator lastAnim = m_playerChips.Last().GetComponent<Animator>();
        yield return new WaitForSeconds(lastAnim.GetCurrentAnimatorStateInfo(0).length);

        /* Update pot chips */
        UpdatePotChips(potChips);

        /* Destroy all child objects */
        foreach(GameObject container in containers)
        {
            Destroy(container);
        }
    }

    public IEnumerator MovePotToPlayer(int idx, int movingChips, int playerTotalChips)
    {
        GameObject prefab = ChipHolder.GetInstance().GetPotPrefab(movingChips, buyIn);
        GameObject container = Instantiate(prefab);
        SetParent(container.transform, m_potChips[idx].transform);

        /* Animate moving chips */
        Animator anim = m_potChips[idx].GetComponent<Animator>();
        anim.SetTrigger("player");

        /* Wait for animation to be finished */
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        /* Update arrival point chips */
        UpdatePlayerChips(idx, playerTotalChips);

        /* Destroy child object */
        Destroy(container);
    }

    private void Start() {
        InitBuyIn(20000);
        // MovePlayerChipsToBetting(3, 5000);
        //StartCoroutine(MovePlayerChipsToBetting(3, 5000));
        StartCoroutine(example());
    }

    private void SetParent(Transform child, Transform parent)
    {
        child.parent = parent;
        child.localPosition = Vector3.zero;
        child.localRotation = Quaternion.identity;
        child.localScale = Vector3.one;
    }

    private IEnumerator example()
    {
        yield return new WaitForSeconds(3.0f);
        /* List<int> roundChips = new List<int>();
        int potChips = 0;
        for(int i = 0; i < 6; i++)
        {
            roundChips.Add(500);
            potChips += 500;
        }

        StartCoroutine(MoveBettingsToPotChips(roundChips, potChips)); */
        /* StartCoroutine(MovePotToPlayer(3, 7000, 250000)); */
        StartCoroutine(MovePotToPlayer(0, 8000, 350000));
    }
}
