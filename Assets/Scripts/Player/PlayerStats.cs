using UnityEngine;

public class PlayerStats : MonoBehaviour {
    [SerializeField] private float _walkSpeed = 4f;
    [SerializeField] private float _sprintSpeed = 8f;
    [SerializeField] private int _maxArmyCapacity = 10;

    public float WalkSpeed => _walkSpeed;
    public float SprintSpeed => _sprintSpeed;
    public int MaxArmyCapacity => _maxArmyCapacity;
}