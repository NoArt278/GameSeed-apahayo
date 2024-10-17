using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[System.Serializable]
public class ObjectPool
{
    [SerializeField, ReadOnly] private GameObject[] prefabs;
    [SerializeField, ReadOnly] private List<GameObject> inactiveObjects = new();

    public ObjectPool(GameObject[] objPrefabs, int initialSize, Transform parent = null)
    {
        prefabs = objPrefabs;
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Object.Instantiate(prefabs[Random.Range(0, prefabs.Length)]);
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
            GameObject obj = Object.Instantiate(prefabs[Random.Range(0, prefabs.Length)]);
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