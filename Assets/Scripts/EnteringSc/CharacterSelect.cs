using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    // Character : MALE, MALE_CAP, FEMALE, POLICE, ZOMBIE

    private int current = 0;

    public List<GameObject> characters = new List<GameObject>();

    public GameObject characterContainers;

    public Animator SignUpAnim;

    public void OnLeftButton()
    {
        Prev();
    }

    public void OnRightButton()
    {
        Next();
    }

    public void OnAccept()
    {
        // Set GameManager.thisPlayer's character
        GameManager.thisPlayer = new Player((Player.Character)current);

        // Disable this panel
        this.gameObject.SetActive(false);

        // Disable Character gameObj
        characterContainers.SetActive(false);

        // Enable SignUpPanel
        SignUpAnim.SetTrigger("Show");
    }

    public GameObject Next()
    {
        characters[current].SetActive(false);
        if(current == characters.Count - 1)
        {
            current = 0;
            characters[current].SetActive(true);
            return characters[current];
        }

        // default
        current++;
        characters[current].SetActive(true);
        return characters[current];
    }

    public GameObject Prev()
    {
        characters[current].SetActive(false);
        if(current == 0)
        {
            current = characters.Count - 1;
            characters[current].SetActive(true);
            return characters[current];
        }

        // default
        current--;
        characters[current].SetActive(true);
        return characters[current];
    }
}
