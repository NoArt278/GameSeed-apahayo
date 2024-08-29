using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class CatArmy : MonoBehaviour
{
    [SerializeField] private Transform follow;
    [SerializeField] private List<ArmyCatBehaviour> cats;
    [SerializeField] private GameObject armyCatPrefab;

    private void Awake() {
        foreach (ArmyCatBehaviour cat in cats) {
            cat.Initialize(follow);
        }
    }
    
    [Button]
    public void AddCat()
    {
        GameObject newCat = Instantiate(armyCatPrefab, transform);
        ArmyCatBehaviour catBehaviour = newCat.GetComponent<ArmyCatBehaviour>();
        catBehaviour.Initialize(follow);
        cats.Add(catBehaviour);
    }
}