using UnityEngine;

public interface IDragAndDropObject: IPlaceableObject
{
    LayerMask layerMask { get; set; }
    LayerMask GetLayerMask();
}
