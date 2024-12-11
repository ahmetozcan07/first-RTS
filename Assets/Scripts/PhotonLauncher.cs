using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using Photon.Realtime;
using TMPro;
using System.Linq;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    public static PhotonLauncher Instance { get; private set; }

    [SerializeField] TMP_InputField roomNameInput;
    [SerializeField] TMP_Text roomName;

    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject RoomListItemPrefab;

    public Transform PlayerListContent;
    [SerializeField] GameObject PlayerListItemPrefab;

    [SerializeField] GameObject startButton;

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

    void Start()
    {
        PhotonNetwork.GameVersion = "0.0.1";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        PhotonNetwork.NickName =
            "Player " + UnityEngine.Random.Range(0, 1000).ToString("0000");
    }

    public void CreateRoom()
    {
        if (!PhotonNetwork.IsConnected)
            return;

        if (string.IsNullOrEmpty(roomNameInput.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInput.text, new RoomOptions { MaxPlayers = 4 } );
    }

    public override void OnJoinedRoom()
    {
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        MenuManager.Instance.OpenMenu(MenuManager.Instance.menus[3]);

        Player[] players = PhotonNetwork.PlayerList;
        int playerCount = players.Length;

        if (playerCount > 4)
        {
            PhotonNetwork.LeaveRoom();
            MenuManager.Instance.OpenMenu(MenuManager.Instance.menus[0]);
        }

        foreach (Transform t in PlayerListContent)
        {
            Destroy(t.gameObject);
        }
        for (int i = 0; i < playerCount; i++)
        {
            Instantiate(PlayerListItemPrefab, PlayerListContent)
                .GetComponent<PlayerListItem>()
                .SetUp(players[i], i + 1);
        }

        startButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Create room failed");
    }

    public void LeaveRoom() { PhotonNetwork.LeaveRoom(); }

    public void JoinRoom(RoomInfo info) { PhotonNetwork.JoinRoom(info.Name); }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu(MenuManager.Instance.menus[1]);
        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform t in roomListContent)
        {
            Destroy(t.gameObject);
        }
        Debug.Log("oda yok " + roomList.Count);
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
            {
                Debug.Log("Room Name: " +roomList[i].Name + ", RemovedFromList: " + roomList[i].RemovedFromList);
                continue;
            }
            Debug.Log("oda olu�mas� laz�m: " + roomList[i].Name);
            Instantiate(RoomListItemPrefab, roomListContent)
              .GetComponent<RoomListItem>()
              .SetUp(roomList[i]);
        }
    }

    public void OnClickBack() { PhotonNetwork.LoadLevel(0); }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        int playerCount = PhotonNetwork.PlayerList.Length;
        if (playerCount > 4)
        {
            return;
        }
        Instantiate(PlayerListItemPrefab, PlayerListContent)
            .GetComponent<PlayerListItem>()
            .SetUp(newPlayer, playerCount);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Player[] players = PhotonNetwork.PlayerList;
        int playerCount = players.Length;

        foreach (Transform t in PlayerListContent)
        {
            Destroy(t.gameObject);
        }
        for (int i = 0; i < playerCount; i++)
        {
            Instantiate(PlayerListItemPrefab, PlayerListContent)
                .GetComponent<PlayerListItem>()
                .SetUp(players[i], i + 1);
        }
    }


    public void StartGame()
    {
        foreach (Transform t in roomListContent)
        {
            Destroy(t.gameObject);
        }
        PhotonNetwork.LoadLevel(1);
    }
}
