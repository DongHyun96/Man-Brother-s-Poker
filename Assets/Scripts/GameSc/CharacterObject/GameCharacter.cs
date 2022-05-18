using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/* Controls player game character object's animations */
public class GameCharacter : MonoBehaviour
{
    private GameObject characterObject;

    private Animator animator;

    public enum AnimType{
        GREET, BET, THROWCARDS, POTWIN, GAMEWIN
    }

    public void Init(Player.Character c)
    {
        /* Setting character and animator */
        GameObject container = CharacterHolder.GetInstance().GetCharacterObj(c);
        characterObject = Instantiate(container, transform.position, transform.rotation);
        animator = characterObject.GetComponent<Animator>();
    }

    public void animateCharacter(AnimType type)
    {
        System.Random rnd = new System.Random();
        switch(type)
        {
            case AnimType.GREET:
                animator.SetTrigger("greet");
                break;
            case AnimType.BET:
                animator.SetTrigger("bet");
                break;
            case AnimType.THROWCARDS:
                animator.SetTrigger("throwCards");
                break;
            case AnimType.POTWIN:
                int randomIdx = rnd.Next(0, 3);

                animator.SetInteger("winIndex", randomIdx);
                animator.SetTrigger("winPot");
                break;
            case AnimType.GAMEWIN:
                int random = rnd.Next(0, 3);

                animator.SetInteger("winIndex", random);
                animator.SetTrigger("winGame");
                break;
        }
    }
     
}
