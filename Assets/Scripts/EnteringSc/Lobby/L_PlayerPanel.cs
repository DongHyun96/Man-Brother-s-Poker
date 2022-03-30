using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L_PlayerPanel : MonoBehaviour
{
    public GameObject thisPlayerObj;
    public GameObject playerObj;

    private const int LIMIT = 20;

    public void InitPlayerPanel()
    {
        // Init children
        
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        
        
        // Init thisPlayer
        GameObject thisPlayer = Instantiate(thisPlayerObj, transform);

        // Init other players, Add invitable first, and then add non-invitable
        // Count limit - 20
        
        int count = 0;
        foreach(KeyValuePair<string, Player> pair in GameManager.allOthers) 
        {
            if(count < LIMIT)
            {
                if(pair.Value.invitable)
                {
                    GameObject player = Instantiate(playerObj, transform);
                    player.GetComponent<PlayerPrefab>().SetName_Dot(pair.Key, pair.Value.invitable);
                }
            }
            else
            {
                break;
            }
            count++;
        }

        foreach(KeyValuePair<string, Player> pair in GameManager.allOthers) 
        {
            if(count < LIMIT)
            {
                if(!pair.Value.invitable)
                {
                    GameObject player = Instantiate(playerObj, transform);
                    player.GetComponent<PlayerPrefab>().SetName_Dot(pair.Key, pair.Value.invitable);
                }
            }
            else{
                break;
            }
            count++;
        }

    }




}
