using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L_PlayerPanel : MonoBehaviour
{
    public GameObject thisPlayerObj;
    public GameObject playerObj;

    public void InitPlayerPanel()
    {
        // Init children
        
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        
        
        // Init thisPlayer
        GameObject thisPlayer = Instantiate(thisPlayerObj, transform);
        thisPlayer.GetComponent<thisPlayerPrefab>().SetName();

        // Init other players, Add invitable first, and then add non-invitable
        int cnt = 0;
        foreach(KeyValuePair<string, Player> pair in GameManager.allOthers) 
        {
            if(cnt < 20 && pair.Value.invitable)
            {
                GameObject player = Instantiate(playerObj, transform);
                player.GetComponent<PlayerPrefab>().SetName_Dot(pair.Key, pair.Value.invitable);

            }
            else{
                break;
            }
            cnt++;
        }

        cnt = 0;
        foreach(KeyValuePair<string, Player> pair in GameManager.allOthers) 
        {
            if(cnt < 20 && !pair.Value.invitable)
            {
                GameObject player = Instantiate(playerObj, transform);
                player.GetComponent<PlayerPrefab>().SetName_Dot(pair.Key, pair.Value.invitable);

            }
            else{
                break;
            }
            cnt++;
        }

    }




}
