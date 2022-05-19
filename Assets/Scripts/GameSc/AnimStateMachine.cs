using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Use this for animation event assistant */
public class AnimStateMachine : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimTurningPointHandler pointHandler = animator.gameObject.GetComponent<AnimTurningPointHandler>();
        pointHandler.OnAnimStart();    
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimTurningPointHandler pointHandler = animator.gameObject.GetComponent<AnimTurningPointHandler>();
        pointHandler.OnAnimTurningPoint();
    }

}
