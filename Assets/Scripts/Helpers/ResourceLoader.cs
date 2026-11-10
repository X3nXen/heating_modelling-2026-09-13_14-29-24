using System.Collections.Generic;
using UnityEngine;

public static class ResourceLoader
{
    private static Dictionary<string, Object[]> loadedData;

    public static Object Get(string path)
    {
        if (loadedData == null)
        {
            loadedData = new Dictionary<string, Object[]>();
        }
        if (loadedData.ContainsKey(path))
        {
            return loadedData[path][0];
        }
        Sprite spriteData = Resources.Load<Sprite>(path);
        if(spriteData != null)
        {
            loadedData.Add(path, new Object[1] { spriteData });
            return spriteData;
        }
        Object data = Resources.Load<Object>(path);
        loadedData.Add(path, new Object[1] { data });
        return data;
    }

    public static Object[] GetAll(string path)
    {
        if (loadedData == null)
        {
            loadedData = new Dictionary<string, Object[]>();
        }
        if (loadedData.ContainsKey(path))
        {
            return loadedData[path];
        }
        Object[] data = Resources.LoadAll<Object>(path);
        loadedData.Add(path, data);
        return data;
    }
}