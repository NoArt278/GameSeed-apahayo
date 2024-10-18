using UnityEngine;

public class Arena : MonoBehaviour {
    [SerializeField] private Vector2Int _gridSpan = new(1, 1);

    [Tooltip("The maximum number of this arena can be generated in the scene.")]
    [SerializeField] private int _generationSlot = 7;
    [SerializeField] private bool _mustAppear = false;

    // Getter
    public Vector2Int GridSpan => _gridSpan;
    public int GenerationSlot => _generationSlot;
    public bool MustAppear => _mustAppear;

    public Vector2Int GridCoordinate { get; private set; }

    public void Init(Vector2Int gridCoordinate) {
        GridCoordinate = gridCoordinate;
    }
}