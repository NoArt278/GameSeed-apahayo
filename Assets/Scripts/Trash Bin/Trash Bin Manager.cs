using System.Collections.Generic;
using UnityEngine;

public class TrashBinManager : MonoBehaviour
{
    private List<Transform> binPosList;
    [SerializeField] private GameObject binPrefab, binPosListContainer;
    public int binCount = 4;
    private void Awake()
    {
        binPosList = new List<Transform>();
        binPosList.AddRange(binPosListContainer.GetComponentsInChildren<Transform>());
        binPosList.RemoveAt(0);

        for (int i = 0; i < binCount; i++)
        {
            Transform chosenPos = binPosList[Random.Range(0, binPosList.Count)];
            Instantiate(binPrefab, chosenPos.position, binPrefab.transform.rotation, transform);
            binPosList.Remove(chosenPos);
        }
    }
}
