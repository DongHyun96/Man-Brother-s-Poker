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
    [SerializeField] private List<GameObject> playerChips;
    [SerializeField] private List<GameObject> bettingChips;
    [SerializeField] private GameObject potChips;

    /* These are for transformation (Moving chips) */
    [SerializeField] private List<GameObject> m_playerChips;
    [SerializeField] private List<GameObject> m_potChips;

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

    /* idx -> position */
    private void UpdatePlayerChips(int idx, int chips)
    {
        if(playerChips[idx] != null)
        {
            /* Destroy previous prefab */
            Destroy(playerChips[idx]);
        }

        GameObject container = ChipHolder.GetInstance().GetChipPrefab(chips, buyIn);
        playerChips[idx] = Instantiate(container, playerChips[idx].transform.position, playerChips[idx].transform.rotation);

    }

    private void UpdateBettingChips(int idx, int chips)
    {
        if(bettingChips[idx] != null)
        {
            Destroy(bettingChips[idx]);
        }

        GameObject container = ChipHolder.GetInstance().GetChipPrefab(chips, buyIn);
        bettingChips[idx] = Instantiate(container, bettingChips[idx].transform.position, bettingChips[idx].transform.rotation);
    }

    private void UpdatePotChips(int chips)
    {
        if(potChips != null)
        {
            Destroy(potChips);
        }

        GameObject container = ChipHolder.GetInstance().GetPotPrefab(chips, buyIn);
        potChips = Instantiate(container, potChips.transform.position, potChips.transform.rotation);
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
        GameObject container = Instantiate(prefab);
        SetParent(container.transform, chipList[idx].transform);

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
        MoveChips(AnimType.BET, 0, 20000, 20000);
        yield return new WaitForSeconds(3.0f);
        MoveChips(AnimType.BET_TO_POT, 3, 150000, 400000);
        yield return new WaitForSeconds(15f);
        UpdateChips(ContentType.POT, 0, 0);
        MoveChips(AnimType.POT_TO_PLAYER, 3, 400000, 600000);
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
