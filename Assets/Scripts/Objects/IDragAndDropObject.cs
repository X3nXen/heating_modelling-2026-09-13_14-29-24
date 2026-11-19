using UnityEngine;

public interface IDragAndDropObject: IPlaceableObject
{
    LayerMask layerMask { get; set; }
    bool directional { get; set; }

    float heightOffset { get; set; }
    float widthOffset { get; set; }
    LayerMask GetLayerMask();
    bool GetDirectional();

    float GetHeightOffset();
    float GetWidthOffset();
}
