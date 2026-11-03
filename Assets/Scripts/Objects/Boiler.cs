using Unity.VisualScripting;
using UnityEngine;

public class Boiler : ARadiatingObject
{
    public override void Init()
    {
        this.displayName = "Kazán";
        this.displayImage = Resources.Load("Images/Boiler_display_icon") as Sprite;

        this.prefab = Resources.Load("Prefabs/Boiler_prefab") as GameObject;

        this.radiationHeat = 35;

        this.raycastLayerMask = LayerMask.GetMask("House_layer");
    }

    private void Awake()
    {
        Init();
    }
}
