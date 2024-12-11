using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviourPunCallbacks
{
    public int myTeam;
    public GameObject selectionIndicator;
    public GameObject[] selectionIndicators;
    public GameObject buildingUI;

    private PhotonView PV;
    public PlayerController playerController;
    public BuildingPlacement placement;

    public int ageToBuild;

    //public int goldToBuild;
    //public int foodToBuild;
    //public int woodToBuild;
    //public int stoneToBuild;
    //public int ironToBuild;

    public int[] materialsToBuild = new int[5]; // gold, food, wood, stone, iron to build

    void Start()
    {
        PV = GetComponent<PhotonView>();

        if (playerController == null)
        {
            try
            {
                playerController = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerController>();
                playerController.myBuildingsList.Add(this);
                myTeam = playerController.team;

                if (selectionIndicator == null)
                {
                    selectionIndicator = selectionIndicators[myTeam - 1];
                }
            }
            catch { }
        }
    }

    public void OpenUI()
    {
        if(PV.IsMine)
        {
            if (selectionIndicator.activeSelf && buildingUI != null)
            {
                buildingUI.SetActive(true);
            }
        }
    }

    public void CloseUI()
    {
        if (PV.IsMine)
        {
            buildingUI.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if(playerController != null)
        {
            playerController.myBuildingsList.Remove(this);
        }
    }
}
