using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHolder : MonoBehaviour
{
    /* In order -->  MALE, MALE_CAP, FEMALE, POCICE, ZOMBIE */
    [SerializeField] private List<GameObject> characters;
    
    private static CharacterHolder m_instance;
    public static CharacterHolder Instance
    {
        get
        {
            if(m_instance == null) {
                m_instance = FindObjectOfType<CharacterHolder>();

            }
            return m_instance;
        }
    }


    public GameObject GetCharacterObj(Player.Character c)
    {
        return characters[(int)c];
    }
}
