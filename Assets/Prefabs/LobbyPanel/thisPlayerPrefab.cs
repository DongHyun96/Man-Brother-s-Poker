using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class thisPlayerPrefab : MonoBehaviour
{
    public Text name;

    public void SetName()
    {
        name.text = GameManager.thisPlayer.name;
    }
}
