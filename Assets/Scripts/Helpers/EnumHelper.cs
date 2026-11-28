using UnityEngine;

public static class EnumHelper
{
    public static float GetWallMaterialDiffusivity(WallMaterial wallMaterial, bool insulated)
    {
        switch (wallMaterial)
        {
            case WallMaterial.BRICK_COMMON:
                return insulated ? 0.00000012f : 0.00000052f;
            case WallMaterial.BRICK_ADOBE:
                return insulated ? 0.00000009f : 0.00000027f;
            case WallMaterial.CONCRETE:
                return insulated ? 0.00000014f : 0.00000069f;
            default:
                return insulated ? 0.00000012f : 0.00000052f;
        }
    }

    public static string GetWallMaterialName(WallMaterial wallMaterial)
    {
        switch (wallMaterial)
        {
            case WallMaterial.BRICK_COMMON:
                return "Tégla";
            case WallMaterial.BRICK_ADOBE:
                return "Vályogtégla";
            case WallMaterial.CONCRETE:
                return "Beton";
            default:
                return "Tégla";
        }
    }

    public static float GetDoorMaterialDiffusivity(DoorMaterial doorMaterial)
    {
        switch (doorMaterial)
        {
            case DoorMaterial.WOOD:
                return 0.000000082f;
            case DoorMaterial.PLASTIC:
                return 0.00000018f;
            case DoorMaterial.GLASS:
                return 0.00000034f;
            default:
                return 0.000000082f;
        }
    }

    public static string GetDoorMaterialName(DoorMaterial doorMaterial)
    {
        switch (doorMaterial)
        {
            case DoorMaterial.WOOD:
                return "Fa (mahagóni)";
            case DoorMaterial.PLASTIC:
                return "Műanyag (uPVC)";
            case DoorMaterial.GLASS:
                return "Üveg";
            default:
                return "Fa (mahagóni)";
        }
    }

    public static float GetWindowMaterialDiffusivity(WindowMaterial windowMaterial)
    {
        switch (windowMaterial)
        {
            case WindowMaterial.PLASTIC_DOUBLE:
                return 0.00000015f;
            case WindowMaterial.PLASTIC_SINGLE:
                return 0.00000027f;
            case WindowMaterial.WOOD_DOUBLE:
                return 0.00000018f;
            case WindowMaterial.WOOD_SINGLE:
                return 0.00000028f;
            default:
                return 0.00000015f;
        }
    }

    public static string GetWindowMaterialName(WindowMaterial windowMaterial)
    {
        switch (windowMaterial)
        {
            case WindowMaterial.PLASTIC_DOUBLE:
                return "Kétrétegű, műanyag keretes";
            case WindowMaterial.PLASTIC_SINGLE:
                return "Egyrétegű, műanyag keretes";
            case WindowMaterial.WOOD_DOUBLE:
                return "Kétrétegű, fa keretes";
            case WindowMaterial.WOOD_SINGLE:
                return "Egyrétegű, fa keretes";
            default:
                return "Kétrétegű, műanyag keretes";
        }
    }
}
