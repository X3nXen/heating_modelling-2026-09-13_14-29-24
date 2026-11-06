using System;
using UnityEngine;

public interface IRadiatingObject
{
    float radiationHeat { get; set; }

    void OnTriggerEnter(Collider other);
}
