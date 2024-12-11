using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PaladinAttackState : StateMachineBehaviour
{
    NavMeshAgent agent;
    Soldier soldier;

    private float attackRate;
    private float attackTimer;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.gameObject.GetComponent<NavMeshAgent>();
        soldier = animator.gameObject.GetComponent<Soldier>();

        AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(layerIndex);
        if (clips.Length > 0)
        {
            float animationLength = clips[0].clip.length;
            attackRate = animationLength;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (agent.velocity != Vector3.zero)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isAttacking", false);
        }
        else if(soldier.attackTarget == null)
        {
            animator.SetBool("Idle", true);
            animator.SetBool("isAttacking", false);
        }
        else if(soldier.shouldStop)
        {
            if(attackTimer <= 0)
            {
                soldier.TriggerDamage();
                attackTimer = 1f / attackRate;
            }
            else
            {
                attackTimer -= Time.deltaTime;
            }
        }
        else
        {
            soldier.UpdateDestination(soldier.attackTarget.transform.position);
        }
    }
}
