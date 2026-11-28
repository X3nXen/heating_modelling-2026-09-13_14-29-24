using UnityEngine;

[System.Serializable]
public class ASimulationObjectPrefab
{
    public string objectType;
    public float[] position;
    public float[] rotation;
    public float[] scale;
    public string prefabPath;

    public int wallMaterial;
    public bool isInsulated;
    public bool canDelete;

    public int windowMaterial;

    public float radiationHeat;

    public int doorMaterial;

    public float[] buildingScale;

    public float insideHeat;
    public float outsideHeat;
}
