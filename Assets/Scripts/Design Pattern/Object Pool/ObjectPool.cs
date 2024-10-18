using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[System.Serializable]
public class ObjectPool
{
    [SerializeField, ReadOnly] private GameObject[] _prefabs;
    [SerializeField, ReadOnly] private List<GameObject> _inactiveObjects = new();

    public ObjectPool(GameObject[] objPrefabs, int initialSize, Transform parent = null)
    {
        _prefabs = objPrefabs;
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Object.Instantiate(_prefabs[Random.Range(0, _prefabs.Length)]);
            obj.SetActive(false);
            _inactiveObjects.Add(obj);

            if (parent != null)
            {
                obj.transform.SetParent(parent);
            }
        }
    }

    public GameObject GetObject()
    {
        if (_inactiveObjects.Count == 0)
        {
            GameObject obj = Object.Instantiate(_prefabs[Random.Range(0, _prefabs.Length)]);
            obj.SetActive(true);
            return obj;
        }

        GameObject pooledObject = _inactiveObjects[0];
        _inactiveObjects.RemoveAt(0);
        pooledObject.SetActive(true);
        return pooledObject;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        _inactiveObjects.Add(obj);
    }
}