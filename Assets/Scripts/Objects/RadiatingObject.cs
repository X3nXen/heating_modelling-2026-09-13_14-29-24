using UnityEngine;

[CreateAssetMenu(fileName = "RadiatingObject", menuName = "Scriptable Objects/RadiatingObject")]
public class RadiatingObject : ScriptableObject
{
    [Header("UI Info")]
    public string displayName;
    public Sprite displayImage;

    [Header("Object Info")]
    public GameObject prefab;

    [Header("Calculation Info")]
    public float surfaceHeat;
}
