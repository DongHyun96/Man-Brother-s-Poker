using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHolder : MonoBehaviour
{
    /* In order -->  MALE, MALE_CAP, FEMALE, POCICE, ZOMBIE */
    public List<GameObject> characters;
    
    private static CharacterHolder instance;

    public static CharacterHolder GetInstance()
    {
        if(instance == null)
        {
            instance = FindObjectOfType<CharacterHolder>();
        }
        return instance;
    }

    public GameObject GetCharacterObj(Player.Character c)
    {
        return characters[(int)c];
    }
}
