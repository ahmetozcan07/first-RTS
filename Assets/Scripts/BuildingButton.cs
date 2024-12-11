using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingButton : MonoBehaviour
{
    public Building building;
    public PlayerController controller;

    void Start()
    {
        controller = GetComponentInParent<PlayerController>();
    }
    void Update()
    {
        if (building.ageToBuild <= controller.playerAge)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive (false);
        }
    }
}
