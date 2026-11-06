using System;
using Unity.VisualScripting;
using UnityEngine;

public class Boiler : MonoBehaviour, IRadiatingObject, IDragAndDropObject
{
    public float radiationHeat { get ; set; }
    public string displayName { get; set; }
    public Sprite displayImage { get; set; }
    public GameObject prefab { get; set; }
    public LayerMask layerMask { get; set; }

    public Sprite GetDisplayImage()
    {
        return displayImage;
    }

    public string GetDisplayName()
    {
        return displayName;
    }

    public LayerMask GetLayerMask()
    {
        return this.layerMask;
    }

    public GameObject GetPrefab()
    {
        return prefab;
    }

    public void Init()
    {
        this.displayName = "Kazán";
        this.displayImage = Resources.Load<Sprite>("Images/Boiler_display_icon");

        this.prefab = Resources.Load<GameObject>("Prefabs/Heaters/Boiler_prefab");
        this.layerMask = LayerMask.GetMask("House_layer");

        this.radiationHeat = 35;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "GridPoint")
        {
            try
            {
                GridPoint gridPoint = other.GetComponentInParent<GridPoint>();
                gridPoint.SetShouldUpdate(false);
                gridPoint.SetCurrentHeat(this.radiationHeat);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    private void Awake()
    {
        Init();
    }
}
