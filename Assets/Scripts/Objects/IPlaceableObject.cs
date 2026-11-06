using UnityEngine;
public interface IPlaceableObject
{
    string displayName { get; set; }
    Sprite displayImage { get; set; }
    GameObject prefab { get; set; }

    GameObject GetPrefab();

    void Init();

    string GetDisplayName();

    Sprite GetDisplayImage();
}
