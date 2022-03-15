using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel_H : MonoBehaviour
{
    public Text name;

    private void Awake() {
        name.text = GameManager.thisPlayer.name;
    }
}
