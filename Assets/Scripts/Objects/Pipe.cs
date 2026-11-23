using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * ASimulationObject-et kiterjesztő osztály, amely a csövek működését írja le
 */
public class Pipe: ASimulationObject, IRadiatingObject, IDragAndDropObject
{
    private PipeMaterial material;
    public float radiationHeat { get; set; }
    private Vector3 lastMainNormal = Vector3.zero;
    private float accumulatedYRotation = 0f;

    /**
     * Inicializáló metódus ami a ASimulationObject absztrakt osztályból öröklődik. Hívásra beállítja a default értékeit a cső objektumnak.
     * param - informationPanelOpener: az információs panelt megnyitó metódus
     */
    public override void Init(Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> informationPanelOpener)
    {
        this.material = PipeMaterial.COPPER;
        this.diffusivity = EnumHelper.GetPipeMaterialDiffusivity(material);
        this.editableDefaultValues = new Dictionary<InformationFlag, string>();
        this.displayName = "Cső";
        this.displayImage = ResourceLoader.Get("Images/pipe_icon") as Sprite;
        this.radiationHeat = 35;

        this.prefab = ResourceLoader.Get("Prefabs/Drawobjects/Pipe") as GameObject;
        this.layerMask = LayerMask.GetMask("Wall_layer");
        this.directional = true;
        this.heightOffset = 0f;
        this.widthOffset = 1f;

        this.originalScale = this.prefab.transform.localScale;
        this.currentScale = new Vector3(1, 1, 1);
        this.editableDefaultValues.Add(InformationFlag.PIPE_MATERIAL, ((int)this.material).ToString());
        this.editableDefaultValues.Add(InformationFlag.OBJECT_TEMPERATURE, this.radiationHeat.ToString());
        InitInfoPanel(new List<InformationFlag>() { InformationFlag.DIFFUSIVITY, InformationFlag.TEMPERATURE, InformationFlag.OBJECT_TEMPERATURE, InformationFlag.PIPE_MATERIAL, InformationFlag.DELETE_ACTION, InformationFlag.RESIZE_ACTION }, informationPanelOpener);
    }

    public override void UpdateInformation(Action<InformationFlag, string> panelInfoUpdater)
    {
        panelInfoUpdater(InformationFlag.OBJECT_NAME, "Cső");
        panelInfoUpdater(InformationFlag.DIFFUSIVITY, this.diffusivity.ToString());
        panelInfoUpdater(InformationFlag.TEMPERATURE, this.radiationHeat.ToString());
    }

    public void SetMaterial(PipeMaterial material)
    {
        this.material = material;
        this.diffusivity = EnumHelper.GetPipeMaterialDiffusivity(material);
        if (this.editableDefaultValues != null)
        {
            this.editableDefaultValues[InformationFlag.PIPE_MATERIAL] = ((int)this.material).ToString();
        }
    }

    public override void SetEqualTo(ASimulationObject simulationObject)
    {
        this.informationFlags = simulationObject.informationFlags;
        this.layerMask = ((IDragAndDropObject)simulationObject).layerMask;
        this.panelOpener = simulationObject.panelOpener;
        this.displayImage = simulationObject.displayImage;
        this.displayName = simulationObject.displayName;
        this.prefab = simulationObject.prefab;
        this.editableDefaultValues = simulationObject.GetEditableValues();
        this.diffusivity = simulationObject.GetDiffusivity();
        this.radiationHeat = ((Pipe)simulationObject).radiationHeat;
        this.heightOffset = ((Pipe)simulationObject).GetHeightOffset();
        this.widthOffset = ((Pipe)simulationObject).GetWidthOffset();
        this.directional = ((Pipe)simulationObject).GetDirectional();
        this.material = ((Pipe)simulationObject).GetPipeMaterial();
        this.originalScale = simulationObject.GetOriginalScale();
        this.currentScale = simulationObject.GetCurrentScale();
    }
    public override void UpdateItemPosition(PointerEventData eventData)
    {
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, this.layerMask, QueryTriggerInteraction.Collide))
        {
            Vector3 currentMainNormal = (Math.Abs(hit.normal.x) > Math.Abs(hit.normal.z) ? new Vector3(Math.Abs(hit.normal.x), 0, 0) : new Vector3(0, 0, Math.Abs(hit.normal.z)));

            if (lastMainNormal != Vector3.zero && currentMainNormal != lastMainNormal)
            {
                accumulatedYRotation += 90f;
            }

            lastMainNormal = currentMainNormal;

            this.transform.rotation = Quaternion.Euler(0f, accumulatedYRotation, 0f);
            Vector3 rotatedWidthOffset = this.transform.rotation * itemWidthOffset;
            Vector3 safeInnerPosition = hit.point + (hit.normal * 0.05f);
            this.transform.position = safeInnerPosition + itemHeightOffset + rotatedWidthOffset;
        }
    }
    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "GridPoint")
        {
            try
            {
                GridPoint gridPoint = other.GetComponent<GridPoint>();
                gridPoint.SetDiffusivity(diffusivity);
                gridPoint.SetCurrentHeat(this.radiationHeat);
                gridPoint.SetIsLocked(true);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    public void SetRadiationHeat(float radiationHeat)
    {
        this.radiationHeat = radiationHeat;
        this.editableDefaultValues[InformationFlag.OBJECT_TEMPERATURE] = this.radiationHeat.ToString();
    }
    public PipeMaterial GetPipeMaterial()
    {
        return this.material;
    }

}
