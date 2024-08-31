using UnityEngine;

public class Arena : MonoBehaviour {
    [SerializeField] private Vector2Int gridSpan = new(1, 1);

    [Tooltip("The maximum number of this arena can be generated in the scene.")]
    [SerializeField] private int generationSlot = 1;
    [SerializeField] private bool mustAppear = false;

    // Getter
    public Vector2Int GridSpan => gridSpan;
    public int GenerationSlot => generationSlot;
    public bool MustAppear => mustAppear;

    public Vector2Int GridCoordinate { get; private set; }

    public void Init(Vector2Int gridCoordinate) {
        GridCoordinate = gridCoordinate;
    }
}