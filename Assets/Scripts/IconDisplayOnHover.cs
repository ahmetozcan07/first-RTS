using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class IconDisplayOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject requiredMaterialsPanel;
    public GameObject[] icons;

    public Building building;

    public void OnPointerEnter(PointerEventData eventData)
    {
        requiredMaterialsPanel.SetActive(true);
        ShowRequiredMaterials(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        requiredMaterialsPanel.SetActive(false);
        ShowRequiredMaterials(false);
    }

    private void ShowRequiredMaterials(bool show)
    {
        for (int i = 0; i < building.materialsToBuild.Length; i++)
        {
            if (building.materialsToBuild[i] > 0)
            {
                icons[i].GetComponent<TextMeshProUGUI>().text = building.materialsToBuild[i].ToString();
                icons[i].SetActive(show);
            }

        }
    }
}
