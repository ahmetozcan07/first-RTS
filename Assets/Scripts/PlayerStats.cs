using JetBrains.Annotations;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviourPunCallbacks
{
    //public int food = 1000;
    //public int wood = 500;
    //public int gold = 100;
    //public int stone = 100;
    //public int iron = 0;

    public int[] inventory = new int[5]; //food wood gold stone iron

    public int populationLimit;
    public int population = 0;

    public Canvas canvas;
    public GameObject[] stats;
    PlayerController playerController;
    PhotonView PV;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        PV = GetComponent<PhotonView>();
        UpdateStats();

        if (!PV.IsMine)
        {
            Destroy(canvas);
        }
    }

    void Update()
    {
        int houseCount = 0;
        foreach(Building building in playerController.myBuildingsList)
        {
            if (building.name.Contains("House"))
            {
                houseCount++;
            }
        }
        populationLimit = 10 + houseCount * 10;
        UpdateStats();
    }

    void UpdateStats()
    {
        for(int  i = 0; i < stats.Length - 1; i++)
        {
            stats[i].GetComponent<TextMeshProUGUI>().text = " " + inventory[i];
        }
        //stats[0].GetComponent<TextMeshProUGUI>().text = "Food: " + food;
        //stats[1].GetComponent<TextMeshProUGUI>().text = "Wood: " + wood;
        //stats[2].GetComponent<TextMeshProUGUI>().text = "Gold: " + gold;
        //stats[3].GetComponent<TextMeshProUGUI>().text = "Stone: " + stone;
        //stats[4].GetComponent<TextMeshProUGUI>().text = "Iron: " + iron;
        population = RoomManager.Instance.playerPopulations[playerController.team - 1];
        stats[5].GetComponent<TextMeshProUGUI>().text = "Population: " + population + "/" + populationLimit;
    }
}
