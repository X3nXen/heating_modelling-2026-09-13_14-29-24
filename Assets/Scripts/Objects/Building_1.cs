using UnityEngine;

public class Building_1: MonoBehaviour, IPlaceableObject
{
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
        return LayerMask.NameToLayer("Default");
    }

    public GameObject GetPrefab()
    {
        return prefab;
    }

    public void Init()
    {
        this.displayName = "Kocka ház";
        this.displayImage = Resources.Load<Sprite>("Images/Boiler_display_icon");

        this.prefab = Resources.Load<GameObject>("Prefabs/Buildings/Box_house");
    }

    private void Awake()
    {
        Init();
    }
}
