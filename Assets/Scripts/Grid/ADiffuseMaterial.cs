using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ADiffuseMaterial: AInformationPanelOpener
{
    [SerializeField]
    protected float diffusivity;

    public abstract void Init(Action<List<InformationFlag>, AInformationPanelOpener> informationPanelOpener);

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "GridPoint")
        {
            try
            {
                GridPoint gridPoint = other.GetComponent<GridPoint>();
                gridPoint.SetDiffusivity(diffusivity);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
