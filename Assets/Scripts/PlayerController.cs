using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using TMPro;
using System.Security.Cryptography;
using static UnityEngine.UI.CanvasScaler;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public int team;
    private PhotonView PV;
    PlayerManager playerManager;
    private BuildingPlacement placement;

    public AudioSource playerAudioSource;
    GameObject voiceChat;

    public Camera cam;
    public LayerMask ground;
    public LayerMask clickable;
    public LayerMask ore;
    public LayerMask UI;
    public Canvas canvas;

    public Transform spawnpoint;

    [SerializeField] RectTransform boxImage;
    [SerializeField] GameObject targetIndicator;

    private Rect selectionBox;
    private Vector2 startPosition;
    private Vector2 endPosition;
    private readonly float dragThreshold = 0.1f;
    private bool isDragging = false;
    private float rayDistance = 500f;

    public int playerAge = 1;

    public List<GameObject> selectedEntities = new();
    public List<Unit> selectedUnits = new();
    public List<Building> myBuildingsList = new();

    PlayerStats playerStats;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        startPosition = Vector2.zero;
        endPosition = Vector2.zero;
        boxImage.sizeDelta = Vector2.zero;
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
        team = playerManager.team;
        placement = GetComponent<BuildingPlacement>();
        playerStats = GetComponent<PlayerStats>();

        if (!cam.GetComponent<PhotonView>().IsMine)
        {
            cam.gameObject.SetActive(false);
        }
        if(!PV.IsMine)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (PV.IsMine)
        {
            SelectByDragging();
            RightClick();
            UpdateTargetIndicator();
            PushToTalk();
            if (placement.buildingPreview == null)
            {
                LeftClick();
            }
        }
        foreach (GameObject go in selectedEntities)
        {
            if(go.TryGetComponent<Unit>(out Unit unit))
            {
                if (!selectedUnits.Contains(unit))
                {
                    selectedUnits.Add(unit);
                }
            }
        }

        List<Unit> willBeRemoved = new List<Unit>();
        foreach (Unit u in selectedUnits)
        {
 
            if (!selectedEntities.Contains(u.gameObject))
            {
                willBeRemoved.Add(u);
            }
        }
        foreach(Unit u in willBeRemoved)
        {
            selectedUnits.Remove(u);
        }
    }

    void FixedUpdate()
    {
        if (PV.IsMine) { }
    }

    public void SpawnPaladin()
    {
        if(playerStats.population < playerStats.populationLimit)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", $"Swordsman {team}"), spawnpoint.position, Quaternion.identity, 0, new object[] { PV.ViewID });
        }
    }

    public void SpawnVillager()
    {
        if (playerStats.population < playerStats.populationLimit)
        {
            int rand = Random.Range(0, 2);
            if (rand == 0)
            {
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", $"Male Peasant {team}"), spawnpoint.position, Quaternion.identity, 0, new object[] { PV.ViewID });
            }
            else
            {
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", $"Female Peasant {team}"), spawnpoint.position, Quaternion.identity, 0, new object[] { PV.ViewID });
            }
        }
    }

    public void SpawnCatfish()
    {

    }

    public void SpawnLizard()
    {

    }

    void LeftClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            ray.origin = cam.transform.position;

            if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, clickable))
            {
                SelectByClicking(hit.collider.gameObject);
            }
            else if (Physics.Raycast(ray, out hit, rayDistance, ore))
            {
                DeselectAll();
                hit.transform.gameObject.GetComponent<Gatherable>().OpenUI();
                selectedEntities.Add(hit.collider.gameObject);
            }
            else
            {
                DeselectAll();
            }
        }
    }

    void RightClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            if (placement.buildingPreview != null)
            {
                Destroy(placement.buildingPreview);
                placement.buildingPreview = null;
                return;
            }

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            ray.origin = cam.transform.position;
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayDistance, ore))
            {
                DeselectNonmovables();
                foreach (GameObject go in selectedEntities)
                {
                    if (go.TryGetComponent<NavMeshAgent>(out var movable))
                    {
                        movable.GetComponent<Unit>().UpdateDestination(hit.point);
                        if (movable.TryGetComponent(out Villager villager)){
                            villager.Workplace = hit.transform;
                        }
                        if (movable.TryGetComponent(out Soldier soldier))
                        {
                            soldier.attackTarget = null;
                            PV.RPC(nameof(soldier.UpdateAttackTarget), RpcTarget.Others, soldier.attackTarget);
                        }
                    }
                }
            }
            else if (Physics.Raycast(ray, out hit, rayDistance, clickable))
            {
                DeselectNonmovables();
                foreach (GameObject go in selectedEntities)
                {
                    if (go.TryGetComponent<NavMeshAgent>(out var movable))
                    {
                        movable.GetComponent<Unit>().UpdateDestination(hit.point);
                        if (movable.TryGetComponent(out Villager villager))
                        {
                            villager.Workplace = null;
                        }
                        if (movable.TryGetComponent(out Soldier soldier))
                        {
                            if (hit.transform.TryGetComponent(out Unit unit))
                            {
                                if (unit.myTeam != team)
                                    soldier.attackTarget = unit.gameObject.transform;
                                    PV.RPC(nameof(soldier.UpdateAttackTarget), RpcTarget.Others, soldier.attackTarget);
                            }
                            else if (hit.transform.TryGetComponent(out Building building))
                            {
                                if (building.myTeam != team)
                                    soldier.attackTarget = building.gameObject.transform;
                                    PV.RPC(nameof(soldier.UpdateAttackTarget), RpcTarget.Others, soldier.attackTarget);
                            }
                        }
                    }
                }
            }
            else if (Physics.Raycast(ray, out hit, rayDistance, ground))
            {
                DeselectNonmovables();
                foreach (GameObject go in selectedEntities)
                {
                    if (go.TryGetComponent<NavMeshAgent>(out var movable))
                    {
                        movable.GetComponent<Unit>().UpdateDestination(hit.point);
                        if (movable.TryGetComponent(out Villager villager))
                        {
                            villager.Workplace = null;
                        }
                        if (movable.TryGetComponent(out Soldier soldier))
                        {
                            soldier.attackTarget = null;
                            PV.RPC(nameof(soldier.UpdateAttackTarget), RpcTarget.Others, soldier.attackTarget);
                        }
                    }
                }
            }
        }
    }

    void SelectByClicking(GameObject gameObject)
    {
        DeselectAll();
        selectedEntities.Add(gameObject);
        if (!gameObject.TryGetComponent<Unit>(out var unit))
        {
            if (gameObject.TryGetComponent<Building>(out var building))
            {
                building.selectionIndicator.SetActive(true);
                building.OpenUI();
            }
        }
        else
        {
            unit.selectionIndicator.SetActive(true);
        }
    }

    void SelectByDragging()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
            selectionBox = new Rect();
        }
        if (Input.GetMouseButton(0) && Vector3.Distance(startPosition, Input.mousePosition) > dragThreshold)
        {
            isDragging = true;
            endPosition = Input.mousePosition;

            Vector2 boxStart = startPosition;
            Vector2 boxEnd = endPosition;
            Vector2 boxCenter = (boxStart + boxEnd) / 2;
            boxImage.position = boxCenter;
            Vector2 boxSize = new(Mathf.Abs(boxStart.x - boxEnd.x), Mathf.Abs(boxStart.y - boxEnd.y));
            boxImage.sizeDelta = boxSize;

            if (startPosition.x < endPosition.x)
            {
                selectionBox.xMin = startPosition.x;
                selectionBox.xMax = endPosition.x;
            }
            else
            {
                selectionBox.xMin = endPosition.x;
                selectionBox.xMax = startPosition.x;
            }
            if (startPosition.y < endPosition.y)
            {
                selectionBox.yMin = startPosition.y;
                selectionBox.yMax = endPosition.y;
            }
            else
            {
                selectionBox.yMin = endPosition.y;
                selectionBox.yMax = startPosition.y;
            }

            foreach (var unit in RoomManager.Instance.allUnitsList)
            {
                Unit go = unit.GetComponent<Soldier>();
                if(go == null)
                {
                    go = unit.GetComponent<Villager>();
                }
                if (selectionBox.Contains(cam.WorldToScreenPoint(unit.transform.position)))
                {
                    if (!selectedEntities.Contains(unit))
                    {
                        selectedEntities.Add(unit);
                        go.selectionIndicator.SetActive(true);
                    }
                }
                else if(isDragging)
                {
                    selectedEntities.Remove(unit);
                    go.selectionIndicator.SetActive(false);
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            startPosition = Vector2.zero;
            endPosition = Vector2.zero;
            boxImage.sizeDelta = Vector2.zero;
        }
    }

    void DeselectAll()
    {
        foreach (var go in selectedEntities)
        {
            if (go == null)
            {
                selectedEntities.Remove(go);
            }
            else if (go.TryGetComponent<Unit>(out var soldier))
            {
                soldier.selectionIndicator.SetActive(false);
            }
            else if(go.TryGetComponent<Building>(out var building))
            {
                building.selectionIndicator.SetActive(false);
                building.CloseUI();
            }
            else if (go.TryGetComponent<Gatherable>(out var gatherable))
            {
                gatherable.CloseUI();
            }
        }
        Debug.Log("selectedunits.clear çalışmask üzere.");
        selectedEntities.Clear();
    }

    void DeselectNonmovables()
    {
        List<GameObject> objectsToRemove = new();
        foreach (GameObject go in selectedEntities)
        {

            if (!go.TryGetComponent<Unit>(out var unit))
            {
                objectsToRemove.Add(go);
                Building building = go.GetComponent<Building>();
                building.selectionIndicator.SetActive(false);
            }
            else if (unit.myTeam != team)
            {
                objectsToRemove.Add(go);
                unit.selectionIndicator.SetActive(false);
            }
        }
        foreach (GameObject go in objectsToRemove)
        {
            selectedEntities.Remove(go);
        }
    }

    void UpdateTargetIndicator()
    {
        if (selectedEntities.Count == 1)
        {
            Unit go = selectedEntities[0].GetComponent<Unit>();
            if (go != null && go.myTeam == team)
            {
                targetIndicator.transform.position = go.gameObject.GetComponent<Unit>().target;
                targetIndicator.SetActive(true);
                if(go.gameObject.GetComponent<NavMeshAgent>().velocity == Vector3.zero)
                {
                    targetIndicator.SetActive(false);
                }
            }
            else
            {
                targetIndicator.SetActive(false);
            }
        }
        else if(selectedEntities.Count > 1)
        {

        }
        else if (selectedEntities.Count == 0)
        {
            targetIndicator.SetActive(false);
        }
    }

    void PushToTalk()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            playerAudioSource.mute = false;
            photonView.RPC(nameof(RPC_PushToTalkOpen), RpcTarget.Others, playerAudioSource.GetComponent<PhotonView>().ViewID);
            voiceChat = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "SpeakerIndicatorPrefab"),
                RoomManager.Instance.VoiceChatIndicator.position,
                RoomManager.Instance.VoiceChatIndicator.rotation,
                0, new object[] { PV.ViewID });

            SetParentOnLocal(voiceChat);
            photonView.RPC(nameof(RPC_SetParent), RpcTarget.Others, voiceChat.GetComponent<PhotonView>().ViewID);

        }
        if (Input.GetKeyUp(KeyCode.T))
        {
            playerAudioSource.mute = true;
            photonView.RPC(nameof(RPC_PushToTalkClose), RpcTarget.Others, playerAudioSource.GetComponent<PhotonView>().ViewID);
            foreach (Transform t in RoomManager.Instance.VoiceChatIndicator)
            {
                if (t.GetComponent<PhotonView>().IsMine){
                    PhotonNetwork.Destroy(t.gameObject);
                }
            }
        }
    }

    [PunRPC]
    void RPC_PushToTalkOpen(int viewID)
    {
        GameObject obj = PhotonView.Find(viewID).gameObject;
        obj.GetComponent<AudioSource>().mute = false;
    }

    [PunRPC]
    void RPC_PushToTalkClose(int viewID)
    {
        GameObject obj = PhotonView.Find(viewID).gameObject;
        obj.GetComponent<AudioSource>().mute = true;
    }

    void SetParentOnLocal(GameObject obj)
    {
        obj.transform.SetParent(RoomManager.Instance.VoiceChatIndicator, false);
        obj.GetComponentInChildren<TextMeshProUGUI>().text = "Player " + team;
    }

    [PunRPC]
    void RPC_SetParent(int viewID)
    {
        GameObject obj = PhotonView.Find(viewID).gameObject;
        SetParentOnLocal(obj);
        obj.GetComponentInChildren<TextMeshProUGUI>().text = "Player " + team;
    }

    void NextAge()
    {
        playerAge += 1;
    }
}
