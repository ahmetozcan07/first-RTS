using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PaladinIdleState : StateMachineBehaviour
{
    NavMeshAgent agent;
    Soldier soldier;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.gameObject.GetComponent<NavMeshAgent>();
        soldier = animator.gameObject.GetComponent<Soldier>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (agent.velocity != Vector3.zero)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("Idle", false);
        }
        else if (soldier.attackTarget != null)
        {
            animator.SetBool("isAttacking", true);
            animator.SetBool("Idle", false);
        }
    }
}