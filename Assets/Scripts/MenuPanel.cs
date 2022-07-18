using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Ent scene menuPanel */
public class MenuPanel : MenuFunctions
{
    public override void OnCredits()
    {
        menuState = MenuState.CREDITS;
    }

    public override void OnExitToLobby()
    {
        throw new System.NotImplementedException();
    }
}
