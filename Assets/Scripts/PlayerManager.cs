using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    GameObject player;
    public int team;

    void Start()
    {
        if (PV.IsMine)
        {
            PV.RPC(nameof(AssignTeam), RpcTarget.AllBuffered);
            
            PhotonNetwork.NickName = "Player " + team;

            if (team != 0)
            {
                CreatePlayer();
            }
        }

        Debug.Log(team);
    }

    void CreatePlayer()
    {
        Transform spawnpoint = RoomManager.Instance.GetSpawnpoint(team);
        player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", $"PlayerBase {team}"), spawnpoint.position, Quaternion.identity, 0, new object[] { PV.ViewID });
    }

    [PunRPC]
    private void AssignTeam()
    {
        foreach (Transform t in PhotonLauncher.Instance.PlayerListContent)
        {
            PlayerManager[] playerManagers = FindObjectsByType<PlayerManager>(FindObjectsSortMode.None);
            foreach (PlayerManager p in playerManagers)
            {
                if(t.GetComponent<PlayerListItem>().player.NickName.Equals(p.PV.Owner.NickName))
                {
                    p.team = t.GetComponent<PlayerListItem>().team;
                }
            }
        }
    }
}
