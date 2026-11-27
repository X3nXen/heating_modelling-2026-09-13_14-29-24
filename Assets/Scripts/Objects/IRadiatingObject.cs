using System;
using UnityEngine;

public interface IRadiatingObject
{
    float radiationHeat { get; set; }

    void SetRadiationHeat(float radiationHeat);
}
