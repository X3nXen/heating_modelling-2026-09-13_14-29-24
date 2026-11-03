using System;
using UnityEngine;

public class GridPointCollide : MonoBehaviour
{
    private Material pointMaterial;

    private void Awake()
    {
        pointMaterial = Resources.Load("Materials/Invisible") as Material;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Building")
        {
            this.gameObject.GetComponent<MeshRenderer>().SetMaterials(new System.Collections.Generic.List<Material>() { pointMaterial });
        }
    }
}
