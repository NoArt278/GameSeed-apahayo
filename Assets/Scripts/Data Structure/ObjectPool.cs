using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private readonly GameObject prefab;
    private readonly List<GameObject> inactiveObjects = new();

    public ObjectPool(GameObject objPrefab, int initialSize, Transform parent = null)
    {
        prefab = objPrefab;
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Object.Instantiate(prefab);
            obj.SetActive(false);
            inactiveObjects.Add(obj);

            if (parent != null)
            {
                obj.transform.SetParent(parent);
            }
        }
    }

    public GameObject GetObject()
    {
        if (inactiveObjects.Count == 0)
        {
            GameObject obj = Object.Instantiate(prefab);
            obj.SetActive(true);
            return obj;
        }

        GameObject pooledObject = inactiveObjects[0];
        inactiveObjects.RemoveAt(0);
        pooledObject.SetActive(true);
        return pooledObject;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        inactiveObjects.Add(obj);
    }
}