using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class BuildingPlacement : MonoBehaviourPunCallbacks
{
    PlayerController controller;
    PlayerStats playerStats;
    PhotonView PV;

    public GameObject[] buildings;
    public GameObject buildingPreview;
    public Renderer previewRenderer;

    public Material validPlacementMaterial;
    public Material invalidPlacementMaterial;

    public Vector3 buildingPosition;
    private bool isBuilding = false;
    public int buildingIndex;


    void Start()
    {
        controller = GetComponent<PlayerController>();
        PV = GetComponent<PhotonView>();
        playerStats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (isBuilding)
        {
            Ray ray = controller.cam.ScreenPointToRay(Input.mousePosition);
            ray.origin = controller.cam.transform.position;
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 500, controller.ground))
            {
                buildingPosition = hit.point;
            }

            if (buildingPreview != null)
            {
                buildingPreview.transform.position = buildingPosition;

                Collider previewCollider = buildingPreview.GetComponent<BoxCollider>();

                Collider[] overlappingColliders = Physics.OverlapBox(
                    previewCollider.bounds.center,
                    previewCollider.bounds.extents,
                    buildingPreview.transform.rotation);

                if( overlappingColliders.Length > 2)
                {
                    MakeTransparent(previewRenderer, invalidPlacementMaterial);

                    if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject())
                    {
                        isBuilding = false;
                        Destroy(buildingPreview);
                        buildingPreview = null;
                        return;
                    }
                }
                else
                {
                    MakeTransparent(previewRenderer, validPlacementMaterial);
                }
                if (Input.GetMouseButtonDown(0) && overlappingColliders.Length <= 2) // 2 because of object itself and terrain
                {
                    if (EventSystem.current.IsPointerOverGameObject())
                    {
                        isBuilding = false;
                        Destroy(buildingPreview);
                        buildingPreview = null;
                        return;
                    }

                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", buildings[buildingIndex].name), buildingPosition, Quaternion.identity, 0, new object[] { PV.ViewID });
                    
                    isBuilding = false;
                    Destroy(buildingPreview);
                    buildingPreview = null;

                    for(int i = 0; i < playerStats.inventory.Length; i++)
                    {
                        playerStats.inventory[i] -= buildings[buildingIndex].GetComponent<Building>().materialsToBuild[i];
                    }
                }
            }
        }
    }

    public void setBuildingIndex(int index)
    {
        buildingIndex = index;
        for (int i = 0; i < playerStats.inventory.Length; i++)
        {
            if (playerStats.inventory[i] < buildings[buildingIndex].GetComponent<Building>().materialsToBuild[i])
            {
                Debug.Log("not enough materials");
                return;
            }
        }
        isBuilding = true;
        buildingPreview = Instantiate(buildings[buildingIndex], buildingPosition, Quaternion.identity);
        buildingPreview.GetComponent<BoxCollider>().isTrigger = true;
        buildingPreview.GetComponentInChildren<NavMeshObstacle>().enabled = false;

        previewRenderer = buildingPreview.GetComponent<Renderer>();
    }

    void MakeTransparent(Renderer renderer, Material material)
    {
        material.color = new Color(material.color.r, material.color.g, material.color.b, 0.5f);
        renderer.material = material;
    }
}
