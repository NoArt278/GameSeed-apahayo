using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class CatArmy : MonoBehaviour
{
    [SerializeField] private Transform follow;
    [ReadOnly] [SerializeField] private List<ArmyCatBehaviour> cats;
}