using UnityEngine;

public static class EnumHelper
{
    public static float GetWallMaterialDiffusivity(WallMaterial wallMaterial)
    {
        switch (wallMaterial)
        {
            case WallMaterial.BRICK_COMMON:
                return 0.00000052f;
            case WallMaterial.BRICK_ADOBE:
                return 0.00000027f;
            case WallMaterial.CONCRETE:
                return 0.00000069f;
            default:
                return 0.00000052f;
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

    public static WallMaterial GetWallMaterialFromName(string name)
    {
        switch (name)
        {
            case "Tégla":
                return WallMaterial.BRICK_COMMON;
            case "Vályogtégla":
                return WallMaterial.BRICK_ADOBE;
            case "Beton":
                return WallMaterial.CONCRETE;
            default:
                return WallMaterial.BRICK_COMMON;
        }
    }
}
