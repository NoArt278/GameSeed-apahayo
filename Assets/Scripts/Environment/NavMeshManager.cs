using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshManager : SingletonMB<NavMeshManager> {
    [SerializeField] private NavMeshSurface _catDogSurface;
    [SerializeField] private NavMeshSurface _npcSurface;

    public void BuildNavMesh() {
        _catDogSurface.BuildNavMesh();
        _npcSurface.BuildNavMesh();
    }
}