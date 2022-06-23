using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SidePotPanel : MonoBehaviour
{
    public Text name;

    public void InitSidePot(List<Player> players)
    {
        if(!string.IsNullOrEmpty(name.text))
        {   
            name.text = "";
        }
        foreach(Player p in players)
        {
            name.text += p.name + " "; 
        }
    }

    public void ClearSidePot()
    {
        name.text = "";
    }
}
