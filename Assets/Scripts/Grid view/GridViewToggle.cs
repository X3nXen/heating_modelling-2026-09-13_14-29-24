using UnityEngine;

public class GridViewToggle : MonoBehaviour
{
    public void ToggleGridView()
    {
        GameObject grid = this.gameObject;
        if (grid != null)
        {
            grid.SetActive(!grid.activeSelf);
        }
    }   
}
