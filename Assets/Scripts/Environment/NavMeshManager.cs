using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshManager : SingletonMB<NavMeshManager> {
    [SerializeField] private NavMeshSurface catDogSurface;
    [SerializeField] private NavMeshSurface npcSurface;

    public void BuildNavMesh() {
        catDogSurface.BuildNavMesh();
        npcSurface.BuildNavMesh();
    }
}