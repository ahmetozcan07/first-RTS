using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
using static Photon.Pun.UtilityScripts.PunTeams;



public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance { get; private set; }
    public Spawnpoint[] SpawnPoints;
    public Transform VoiceChatIndicator;

    public List<GameObject> allUnitsList = new List<GameObject>();

    public int[] playerPopulations = new int[4];

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    private void Update()
    {
        UpdatePopulations();
    }

    void UpdatePopulations()
    {
        int[] temp = new int[4];
        foreach (GameObject go in allUnitsList)
        {
            Unit unit = go.GetComponent<Unit>();
            temp[unit.myTeam - 1]++;
        }
        for (int i = 0; i < playerPopulations.Length; i++)
        {
            playerPopulations[i] = temp[i];
        }
    }

    public Transform GetSpawnpoint(int playerTeam)
    {
        if (playerTeam == 1)
        {
            return SpawnPoints[0].transform;
        }
        else if (playerTeam == 2)
        {
            return SpawnPoints[1].transform;
        }
        else if (playerTeam == 3)
        {
            return SpawnPoints[2].transform;
        }
        else 
        {
            return SpawnPoints[3].transform;
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 1)
        {
            MenuManager.Instance.CloseMenu(MenuManager.Instance.menus[3]);
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), transform.position, transform.rotation);
        }
    }

}