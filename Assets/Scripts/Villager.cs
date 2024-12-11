using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Villager : Unit
{
    public Transform Workplace;

    public GameObject hammer;
    public GameObject pickaxe;
    public GameObject plow;
    public GameObject axe;

    //private float WorkingDistance = 6f;

    private int carryingCapacityLight = 20;
    private int carryingCapacityHeavy = 10;

    private int[] pack = new int[1];

    bool isCoroutineWorking = false;

    PlayerStats playerStats;

    void Start()
    {
        base.Start();
        playerStats = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerStats>();
    }

    private void Update()
    {
        base.Update();
        if (Workplace != null)
        {
            if (shouldStop && !agent.isStopped)
            {
                agent.isStopped = true;
            }
        }
        else
        {
            pickaxe.SetActive(false);
            plow.SetActive(false);
            hammer.SetActive(false);
            axe.SetActive(false);
        }
    }


    private void OnDestroy()
    {
        base.OnDestroy();
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if(Workplace != null && other.transform == Workplace)
        {
            shouldStop = true;
        }
        if (other.name.Contains("tree"))
        {
            animator.SetBool("isLumbering", true);
            axe.SetActive(true);
        }
        if (other.name.Contains("Stone") || other.name.Contains("Gold") || other.name.Contains("Iron"))
        {
            animator.SetBool("isMining", true);
            pickaxe.SetActive(true);
        }
        if (other.name.Contains("Farm"))
        {
            animator.SetBool("isFarming", true);
            plow.SetActive(true);
        }
        if (other.name.Contains("plant"))
        {
            animator.SetBool("isGathering", true);
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (Workplace != null && other.transform == Workplace)
        {
            shouldStop = true;
        }
        Gatherable gatherable = other.gameObject.GetComponent<Gatherable>();

        if (other.name.Contains("Farm"))
        {
            animator.SetBool("isFarming", true);
            plow.SetActive(true);
            if (!isCoroutineWorking)
            {
                isCoroutineWorking = true;
                StartCoroutine(GatherRepeatedly(gatherable, 0, carryingCapacityLight));
            }
        }
        if (other.name.Contains("plant"))
        {
            animator.SetBool("isGathering", true);
            if (!isCoroutineWorking)
            {
                isCoroutineWorking = true;
                StartCoroutine(GatherRepeatedly(gatherable, 0, carryingCapacityLight));
            }
        }
        if (other.name.Contains("tree"))
        {
            animator.SetBool("isLumbering", true);
            if(!isCoroutineWorking)
            {
                isCoroutineWorking = true;
                StartCoroutine(GatherRepeatedly(gatherable, 1, carryingCapacityLight));
            }
        }
        if (other.name.Contains("Gold"))
        {
            animator.SetBool("isMining", true);
            pickaxe.SetActive(true);
            if (!isCoroutineWorking)
            {
                isCoroutineWorking = true;
                StartCoroutine(GatherRepeatedly(gatherable, 2, carryingCapacityHeavy));
            }
        }
        if (other.name.Contains("Stone"))
        {
            animator.SetBool("isMining", true);
            pickaxe.SetActive(true);
            if (!isCoroutineWorking)
            {
                isCoroutineWorking = true;
                StartCoroutine(GatherRepeatedly(gatherable, 3, carryingCapacityHeavy));
            }
        }
        if (other.name.Contains("Iron"))
        {
            animator.SetBool("isMining", true);
            pickaxe.SetActive(true);
            if (!isCoroutineWorking)
            {
                isCoroutineWorking = true;
                StartCoroutine(GatherRepeatedly(gatherable, 4, carryingCapacityHeavy));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        shouldStop = false;
        if (other.name.Contains("tree"))
        {
            animator.SetBool("isLumbering", false);
        }
        if (other.name.Contains("Stone") || other.name.Contains("Gold") || other.name.Contains("Iron"))
        {
            animator.SetBool("isMining", false);
        }
        if (other.name.Contains("Farm"))
        {
            animator.SetBool("isFarming", false);
        }
        if (other.name.Contains("plant"))
        {
            animator.SetBool("isGathering", false);
        }
    }

    private IEnumerator GatherRepeatedly(Gatherable gatherable, int index, int carryingAmount)
    {
        yield return new WaitForSeconds(5);
        gatherable.Gather(carryingAmount);
        isCoroutineWorking = false;
        if(gatherable.amount <= 0)
        {
            Workplace = null;
            animator.SetBool("isLumbering", false);
            animator.SetBool("isMining", false);
            animator.SetBool("isGathering", false);
        }
        playerStats.inventory[index] += carryingAmount;
    }
}
