using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/* 
Holds the chip prefabs
 */
public class ChipHolder : MonoBehaviour
{
    /* 

    These prefabs represent approximated amount of chips
    Total count - 20
    [BuyIn * 1/10, BuyIn * 2/10 ... BuyIn / 2]
    
    ex) BuyIn 20k --> [0 ~ 2k, 2k ~ 4k, 4k ~ 6k, 6k ~ 8k, ... , 38k ~ 40k]
    
     */
    [SerializeField]
    private List<GameObject> chipPrefabs;

    [SerializeField] private List<GameObject> potPrefabs;

    private static ChipHolder instance;

    public static ChipHolder GetInstance()
    {
        if(instance == null)
        {
           instance = FindObjectOfType<ChipHolder>();
        }
        return instance;
    }

    /* Return empty gameObject if chip is 0 */
    public GameObject GetChipPrefab(int chips, int buyIn)
    {
        if(chips == 0)
        {
            return new GameObject("name");
        }

        int gap = buyIn / 10;

        for(int i = 0; i < chipPrefabs.Count; i++)
        {
            if(gap * i < chips && chips <= gap * (i + 1))
            {
                return chipPrefabs[i];
            }
        }

        // Reached maximum amount - return maximum amount of chip prefabs
        return chipPrefabs.Last();
    }

    /* Return empty if chip is 0 */
    public GameObject GetPotPrefab(int pot, int buyIn)
    {
        if(pot == 0)
        {
            return new GameObject("name");
        }

        int gap = buyIn / 5;

        for(int i = 0; i < potPrefabs.Count; i++)
        {
            if(gap * i < pot && pot <= gap * (i + 1))
            {
                return potPrefabs[i];
            }
        }

        // Reached maximum amount
        return potPrefabs.Last();
    }

    
}
