using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableChipHandler : MonoBehaviour
{
    [SerializeField] private List<GameObject> playerChips;

    [SerializeField] private List<GameObject> bettingChips;

    [SerializeField] private GameObject potChips;

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

        playerChips[idx] = ChipHolder.GetInstance().GetChipPrefab(chips, buyIn);

    }


    public void UpdateBettingChips(int idx, int chips)
    {
        if(bettingChips[idx] != null)
        {
            Destroy(bettingChips[idx]);
        }

        bettingChips[idx] = ChipHolder.GetInstance().GetChipPrefab(chips, buyIn);
    }

    public void UpdatePotChips(int chips)
    {
        if(potChips != null)
        {
            Destroy(potChips);
        }

        potChips = ChipHolder.GetInstance().GetPotPrefab(chips, buyIn);
    }

    /* Chips moving methods */
}
