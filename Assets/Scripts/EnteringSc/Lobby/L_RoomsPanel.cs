using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class L_RoomsPanel : MonoBehaviour
{
    public GameObject prefab;

    public void InitRoomsPanel()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Init rooms
        // Count limit = 30
        int count = 0;
        foreach(KeyValuePair<Guid, Room> pair in GameManager.rooms)
        {
            if (count < 30)
            {
                GameObject container = Instantiate(prefab, transform);
                container.GetComponent<RoomButtonPrefab>().SetContents(pair.Key);
            }
            else
            {
                break;
            }
            count++;
        }
    }
}
