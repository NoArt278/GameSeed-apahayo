using System.Collections.Generic;
using UnityEngine;

public class TrashBinManager : MonoBehaviour
{
    private List<Transform> binPosList;
    [SerializeField] private GameObject binPrefab, binPosListContainer;
    public int maxBinCount = 4;
    private void Awake()
    {
        binPosList = new List<Transform>();
        binPosList.AddRange(binPosListContainer.GetComponentsInChildren<Transform>());
        binPosList.RemoveAt(0);

        for (int i = 0; i < maxBinCount; i++)
        {
            int chosenIndex = Random.Range(-1, binPosList.Count);
            if (chosenIndex >= 0)
            {
                Transform chosenPos = binPosList[chosenIndex];
                Instantiate(binPrefab, chosenPos.position, chosenPos.rotation, transform);
                binPosList.Remove(chosenPos);
            }
        }
    }
}
