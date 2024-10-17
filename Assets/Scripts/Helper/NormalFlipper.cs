using NaughtyAttributes;
using UnityEngine;

public class NormalFlipper : MonoBehaviour
{
    #if UNITY_EDITOR
    [Button]
    public void FlipNormals()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        Vector3[] normals = mesh.normals;
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = -normals[i];
        }
        mesh.normals = normals;

        int[] triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int temp = triangles[i];
            triangles[i] = triangles[i + 2];
            triangles[i + 2] = temp;
        }
        mesh.triangles = triangles;
    }
    #endif
}
