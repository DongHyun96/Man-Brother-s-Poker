using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacter : MonoBehaviour
{
    public GameObject characterObject;

    public Animator animator;

    public void Init(Player.Character c)
    {
        /* Setting character and animator */
        GameObject container = CharacterHolder.GetInstance().GetCharacterObj(c);
        characterObject = Instantiate(container, transform.position, transform.rotation);
        animator = characterObject.GetComponent<Animator>();
    }

    

    
}
