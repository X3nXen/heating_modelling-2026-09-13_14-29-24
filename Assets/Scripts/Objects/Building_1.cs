using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

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

    public void Init(Action<List<InformationFlag>, AInformationPanelOpener> _)
    {
        this.displayName = "Kocka ház";
        this.displayImage = ResourceLoader.Get("Images/Boiler_display_icon") as Sprite;

        this.prefab = ResourceLoader.Get("Prefabs/Buildings/Box_house") as GameObject;
    }
}
