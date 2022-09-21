using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/* Controls player game character object's animations */
public class GameCharacter : MonoBehaviour
{
    public GameObject characterObject;

    private Animator animator;

    public enum AnimType{
        GREET, BET, THROWCARDS, POTWIN, GAMEWIN
    }

    public void Init(Player.Character c)
    {
        /* Setting character and animator */
        GameObject container = CharacterHolder.Instance.GetCharacterObj(c);
        characterObject = Instantiate(container, transform.position, transform.rotation);
        animator = characterObject.GetComponent<Animator>();
    }

    public void AnimateCharacter(AnimType type)
    {
        if(characterObject == null)
        {
            return;
        }

        /* Init AnimTurningPointHandler */
        characterObject.GetComponent<AnimTurningPointHandler>().IsTurningPointPassed = false;

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
                int random = rnd.Next(1, 3); // Without 'BreakDance'

                // Set foot on the ground
                /* float x = characterObject.transform.position.x;
                float z = characterObject.transform.position.z;
                characterObject.transform.position = new Vector3(x, 0.05f, z); */

                animator.SetInteger("winIndex", random);
                animator.SetTrigger("winGame");
                break;
        }
    }
}
