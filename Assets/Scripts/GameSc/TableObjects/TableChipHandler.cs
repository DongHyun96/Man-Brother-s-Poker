using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/* 
Usage - Update Starting point chips and then move chips -> 
Automatically update by animation finish events
*/
public class TableChipHandler : MonoBehaviour
{
    /* Place Holders */
    [SerializeField] private List<Transform> playerChipsT;
    [SerializeField] private List<Transform> bettingChipsT;
    [SerializeField] private Transform potChipsT;

    /* GameObject containers */
    private GameObject[] playerChipsContainer = new GameObject[6];
    private GameObject[] betChipsContainer = new GameObject[6];
    private GameObject potChipsContainer;

    /* These are for transformation (Moving chips) */
    public List<GameObject> m_playerChips;
    public List<GameObject> m_potChips;

    private int buyIn;

    public enum ContentType{
        PLAYER, BET, POT
    }

    public enum AnimType{
        BET, BET_TO_POT, POT_TO_PLAYER
    }

    public void InitBuyIn(int buyIn)
    {
        this.buyIn = buyIn;
    }

    public void UpdateChips(ContentType type, int idx, int chips)
    {
        switch(type)
        {
            case ContentType.PLAYER:
                UpdatePlayerChips(idx, chips);
                break;
            case ContentType.BET:
                UpdateBettingChips(idx, chips);
                break;
            case ContentType.POT:
                UpdatePotChips(chips);
                break;
        }
    }

    public void MoveChips(AnimType type, int idx, int chips, int destinationChips)
    {
        /* Get corresponding anim trigger, destination content Type and chip list */
        string trigger = "";
        ContentType destContent;
        List<GameObject> chipList = GetCorrespondingChipList(type, out trigger, out destContent);

        /* Set contents */
        GameObject prefab = ChipHolder.GetInstance().GetChipPrefab(chips, buyIn);
        if(prefab != null)
        {
            GameObject container = Instantiate(prefab);
            SetParent(container.transform, chipList[idx].transform);
        }

        /* If animation finished, Destroy child object and update arrival point chips by animation event */
        /* Set chip amounts before event is activated */
        ChipObjectUpdater updater = chipList[idx].GetComponent<ChipObjectUpdater>();
        updater.contentType = destContent;
        updater.chips = destinationChips;
        updater.idx = idx;
        
        /* Animate moving chips */
        Animator anim = chipList[idx].GetComponent<Animator>();
        anim.SetInteger("index", idx);
        anim.SetTrigger(trigger);
        
    }

    /* idx -> position */
    private void UpdatePlayerChips(int idx, int chips)
    {
        if(playerChipsContainer[idx] != null)
        {
            /* Destroy previous prefab */
            Destroy(playerChipsContainer[idx]);
        }

        GameObject container = ChipHolder.GetInstance().GetChipPrefab(chips, buyIn);
        if(container != null)
        {
            playerChipsContainer[idx] = Instantiate(container, playerChipsT[idx].position, playerChipsT[idx].rotation);
        }

    }

    private void UpdateBettingChips(int idx, int chips)
    {
        if(betChipsContainer[idx] != null)
        {
            Destroy(betChipsContainer[idx]);
        }

        GameObject container = ChipHolder.GetInstance().GetChipPrefab(chips, buyIn);
        if(container != null)
        {
            betChipsContainer[idx] = Instantiate(container, bettingChipsT[idx].position, bettingChipsT[idx].rotation);
        }
    }

    private void UpdatePotChips(int chips)
    {
        if(potChipsContainer != null)
        {
            Destroy(potChipsContainer);
        }

        GameObject container = ChipHolder.GetInstance().GetPotPrefab(chips, buyIn);
        if(container != null)
        {
            potChipsContainer = Instantiate(container, potChipsT.position, potChipsT.rotation);
        }
    }

    private void SetParent(Transform child, Transform parent)
    {
        child.parent = parent;
        child.localPosition = Vector3.zero;
        child.localRotation = Quaternion.identity;
        child.localScale = Vector3.one;
    }

    private List<GameObject> GetCorrespondingChipList(AnimType t, out string trigger, out ContentType destContent)
    {
        switch(t)
        {
            case AnimType.BET:
                trigger = "bet";
                destContent = ContentType.BET;
                return m_playerChips;
            case AnimType.BET_TO_POT:
                trigger = "betToPot";
                destContent = ContentType.POT;
                return m_playerChips;
            case AnimType.POT_TO_PLAYER:
                trigger = "potToPlayer";
                destContent = ContentType.PLAYER;
                return m_potChips;
            default:
                trigger = "";
                destContent = ContentType.PLAYER;
                return null;
        }
    }
}
