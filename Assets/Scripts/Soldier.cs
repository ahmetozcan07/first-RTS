using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Unit
{
    public Transform attackTarget;

    public float Damage;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();   
    }

    private void Update()
    {
        base.Update();
        if (attackTarget != null)
        {
            if (shouldStop && !agent.isStopped)
            {
                agent.isStopped = true;
            }
        }
    }

    [PunRPC]
    public void UpdateAttackTarget(Transform transform)
    {
        attackTarget = transform;
    }

    public void TriggerDamage()
    {
        attackTarget.GetComponent<Health>().TakeDamage(Damage);
        if (attackTarget.GetComponent<Health>().health <= 0) { 
            attackTarget = null;
            PV.RPC(nameof(UpdateAttackTarget), RpcTarget.Others, attackTarget);
        }
    }

    [PunRPC]
    public void ApplyDamage()
    {
        Debug.Log("caný azalt");
        attackTarget.GetComponent<Health>().TakeDamage(Damage);
        if(attackTarget.GetComponent<Health>().health <= 0) { attackTarget = null; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(attackTarget != null && attackTarget.gameObject == other.gameObject)
        {
            shouldStop = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (attackTarget != null && attackTarget.gameObject == other.gameObject)
        {
            shouldStop = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (attackTarget != null && attackTarget.gameObject == other.gameObject)
        {
            shouldStop = false;
        }
    }
}
