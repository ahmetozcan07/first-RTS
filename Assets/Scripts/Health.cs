using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public int maxHealth = 100;
    public float health;
    public Animator animator;

    private PhotonView PV;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        if (health == 0)
        {
            health = maxHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        PV.RPC(nameof(RPC_TakeDamage), RpcTarget.All, damage);
    }

    [PunRPC]
    public void RPC_TakeDamage(float damage)
    {
        if (!PV.IsMine)
            return;

        health -= damage;
        if (animator != null)
        {
            animator.SetFloat("health", health);
        }

        //buildings have no animation
        if (health <= 0)
        {
            if (gameObject.TryGetComponent<Building>(out var building))
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    public void Collapse(float deathAnimationLength)
    {
        StartCoroutine(DestroyAfterAnimation(deathAnimationLength));
    }

    private IEnumerator DestroyAfterAnimation(float deathAnimationLength)
    {
        yield return new WaitForSeconds(deathAnimationLength);

        PhotonNetwork.Destroy(gameObject);
    }
}
