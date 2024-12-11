using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PaladinDeathState : StateMachineBehaviour
{
    Health health;

    private float animationLength;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        health = animator.gameObject.GetComponent<Health>();

        AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(layerIndex);
        if (clips.Length > 0)
        {
            animationLength = clips[0].clip.length;
        }
        health.Collapse(animationLength);
    }
}
