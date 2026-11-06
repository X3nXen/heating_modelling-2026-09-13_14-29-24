using UnityEngine;

public class Wall: ADiffuseMaterial
{
    private void Awake()
    {
        this.diffusivity = 0.00000052f;
    }
}
