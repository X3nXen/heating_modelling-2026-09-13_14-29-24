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

    public static float GetDoorMaterialDiffusivity(DoorMaterials doorMaterial)
    {
        switch (doorMaterial)
        {
            case DoorMaterials.WOOD:
                return 0.000000082f;
            case DoorMaterials.PLASTIC:
                return 0.00000018f;
            case DoorMaterials.GLASS:
                return 0.00000034f;
            default:
                return 0.000000082f;
        }
    }

    public static string GetDoorMaterialName(DoorMaterials doorMaterial)
    {
        switch (doorMaterial)
        {
            case DoorMaterials.WOOD:
                return "Fa (mahagóni)";
            case DoorMaterials.PLASTIC:
                return "Műanyag (uPVC)";
            case DoorMaterials.GLASS:
                return "Üveg";
            default:
                return "Fa (mahagóni)";
        }
    }

    public static float GetWindowMaterialDiffusivity(WindowMaterials windowMaterial)
    {
        switch (windowMaterial)
        {
            case WindowMaterials.GLASS:
                return 0.00000077f;
            default:
                return 0.00000077f;
        }
    }

    public static string GetWindowMaterialName(WindowMaterials windowMaterial)
    {
        switch (windowMaterial)
        {
            case WindowMaterials.GLASS:
                return "Szóda-mész üveg";
            default:
                return "Szóda-mész üveg";
        }
    }

    public static float GetPipeMaterialDiffusivity(PipeMaterial pipeMaterial)
    {
        switch (pipeMaterial)
        {
            case PipeMaterial.COPPER:
                return 0.00000116f;
            case PipeMaterial.PLASTIC:
                return 0.00000019f;
            case PipeMaterial.STEEL:
                return 0.00000656f;
            default:
                return 0.00000116f;
        }
    }

    public static string GetPipeMaterialName(PipeMaterial pipeMaterial) 
    {
        switch (pipeMaterial)
        {
            case PipeMaterial.COPPER:
                return "Réz";
            case PipeMaterial.PLASTIC:
                return "Műanyag";
            case PipeMaterial.STEEL:
                return "Acél";
            default:
                return "Réz";
        }
    }
}
