using System;
using System.Collections.Generic;
using UnityEngine;

public class Window : ASimulationObject, IDragAndDropObject
{
    private WindowMaterial material;

    public override void Init(Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> informationPanelOpener)
    {
        this.displayName = "Ablak";
        this.displayImage = ResourceLoader.Get("Images/window") as Sprite;

        this.prefab = ResourceLoader.Get("Prefabs/Fenestration/Window_prefab") as GameObject;
        this.layerMask = LayerMask.GetMask("Wall_layer");

        this.material = WindowMaterial.PLASTIC_DOUBLE;
        this.diffusivity = EnumHelper.GetWindowMaterialDiffusivity(material);
        this.editableDefaultValues = new Dictionary<InformationFlag, string>();
        this.editableDefaultValues.Add(InformationFlag.WINDOW_MATERIAL, ((int)this.material).ToString());
        this.directional = true;
        this.heightOffset = 0f;
        this.widthOffset = 0.5f;

        this.originalScale = this.prefab.transform.localScale;
        this.currentScale = new Vector3(1, 1, 1);
        InitInfoPanel(new List<InformationFlag>() { InformationFlag.DIFFUSIVITY, InformationFlag.WINDOW_MATERIAL, InformationFlag.DELETE_ACTION, InformationFlag.RESIZE_ACTION }, informationPanelOpener);
    }

    public override void UpdateInformation(Action<InformationFlag, string> panelInfoUpdater)
    {
        panelInfoUpdater(InformationFlag.OBJECT_NAME, "Ablak");
        panelInfoUpdater(InformationFlag.DIFFUSIVITY, this.diffusivity.ToString());
    }

    public void SetMaterial(WindowMaterial material)
    {
        this.material = material;
        this.diffusivity = EnumHelper.GetWindowMaterialDiffusivity(material);
        if (this.editableDefaultValues != null)
        {
            this.editableDefaultValues[InformationFlag.WINDOW_MATERIAL] = ((int)this.material).ToString();
        }
    }

    public override void SetEqualTo(ASimulationObject simulationObject)
    {
        this.diffusivity = ((Window)simulationObject).diffusivity;
        this.informationFlags = simulationObject.informationFlags;
        this.panelOpener = simulationObject.panelOpener;
        this.displayImage = simulationObject.displayImage;
        this.displayName = simulationObject.displayName;
        this.prefab = simulationObject.prefab;
        this.layerMask = ((Window)simulationObject).layerMask;
        this.editableDefaultValues = new Dictionary<InformationFlag, string>(simulationObject.GetEditableValues());
        this.originalScale = simulationObject.GetOriginalScale();
        this.currentScale = simulationObject.GetCurrentScale();
    }

    public WindowMaterial GetWindowMaterial()
    {
        return this.material;
    }
}
