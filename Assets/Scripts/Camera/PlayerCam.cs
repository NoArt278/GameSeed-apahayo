using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    Transform player;
    [SerializeField] private Material transparentMaterial;
    private List<Material> prevMaterials;
    private List<MeshRenderer> blockingObjectsRenderer;

    private void Start()
    {
        CinemachineVirtualCamera vcam = GetComponentInChildren<CinemachineVirtualCamera>();
        player = vcam.Follow;
        blockingObjectsRenderer = new List<MeshRenderer>();
        prevMaterials = new List<Material>();
    }

    private void Update()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward);
        bool isBlocked = false;

        foreach (var hit in hits)
        {
            if (!hit.collider.CompareTag("Player") && !hit.collider.CompareTag("Cat") && !hit.collider.CompareTag("Hide")) // Camera obstructed
            {
                if (Vector3.Distance(transform.position, player.position) > Vector3.Distance(transform.position, hit.point))
                {
                    isBlocked = true;
                    // Save previous material
                    MeshRenderer blockingObjRenderer = hit.collider.GetComponent<MeshRenderer>();
                    Material currMaterial = blockingObjRenderer.material;
                    if (!blockingObjectsRenderer.Contains(blockingObjRenderer))
                    {
                        blockingObjectsRenderer.Add(blockingObjRenderer);
                        prevMaterials.Add(currMaterial);

                        Material newTransMaterial = new Material(transparentMaterial);
                        Color transMaterialColor = currMaterial.color;
                        transMaterialColor.a = 0.2f;
                        newTransMaterial.color = transMaterialColor;
                        blockingObjRenderer.material = newTransMaterial;
                    }
                }
            }
        }

        if (!isBlocked && blockingObjectsRenderer.Count > 0)
        {
            int blockerCount = blockingObjectsRenderer.Count;
            for (int i = 0; i < blockerCount; i++)
            {
                blockingObjectsRenderer[0].material = prevMaterials[0];
                blockingObjectsRenderer.RemoveAt(0);
                prevMaterials.RemoveAt(0);
            }
        }
    }
}
