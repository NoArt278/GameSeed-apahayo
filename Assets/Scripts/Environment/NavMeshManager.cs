using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshManager : MonoBehaviour {
    public static NavMeshManager Instance;
    [SerializeField] private NavMeshSurface catDogSurface;
    [SerializeField] private NavMeshSurface npcSurface;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    public void BuildNavMesh() {
        catDogSurface.BuildNavMesh();
        npcSurface.BuildNavMesh();
    }
}