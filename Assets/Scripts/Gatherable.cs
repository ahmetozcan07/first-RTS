using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Gatherable : MonoBehaviourPunCallbacks
{
    public int maxAmount;
    public int amount;

    private PhotonView PV;

    public TextMeshProUGUI tmp;
    public GameObject canvas;

    void Start()
    {
        amount = maxAmount;
        PV = GetComponent<PhotonView>();
        tmp.text = amount.ToString();
    }

    private void Update()
    {
        Collapse();
    }

    public void Gather(int portable)
    {
        PV.RPC(nameof(UpdateAmount), RpcTarget.All, portable);
    }

    [PunRPC]
    void UpdateAmount(int portable)
    {
        amount -= portable;
        tmp.text = amount.ToString();
    }

    public void OpenUI()
    {
        if (canvas != null)
        {
            canvas.SetActive(true);
        }
    }

    public void CloseUI()
    {
        canvas.SetActive(false);
    }

    void Collapse()
    {
        if (amount <= 0)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
