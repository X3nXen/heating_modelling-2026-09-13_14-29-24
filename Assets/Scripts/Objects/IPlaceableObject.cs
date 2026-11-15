using System;
using System.Collections.Generic;
using UnityEngine;
public interface IPlaceableObject
{
    string displayName { get; set; }
    Sprite displayImage { get; set; }
    GameObject prefab { get; set; }

    GameObject GetPrefab();

    void Init(Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> infoPanelOpen);

    string GetDisplayName();

    Sprite GetDisplayImage();
}
