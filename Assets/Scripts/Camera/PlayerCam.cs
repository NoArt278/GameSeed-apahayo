using Cinemachine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    Transform player;
    // [SerializeField] private Material transparentMaterial;
    // private List<Material> prevMaterials;
    // private List<MeshRenderer> blockingObjectsRenderer;
    private List<Transparentable> transparentables;
    private List<MeshRenderer> blockingObjectsRenderer;

    private void Start()
    {
        CinemachineVirtualCamera vcam = GetComponentInChildren<CinemachineVirtualCamera>();
        player = vcam.Follow.GetComponentInParent<PlayerMovement>().transform;
        transparentables = new List<Transparentable>();
        blockingObjectsRenderer = new List<MeshRenderer>();
    }

    private void Update()
    {
        Vector3 direction = player.position - transform.position;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, Mathf.Infinity, LayerMask.GetMask("Obstacle"));
        bool isBlocked = false;

        foreach (var hit in hits)
        {
            if (Vector3.Distance(transform.position, player.position) < Vector3.Distance(transform.position, hit.point)) continue;

            isBlocked = true;
            if (hit.collider.TryGetComponent(out Transparentable transparentable))
            {
                if (transparentables.Contains(transparentable) || blockingObjectsRenderer.Contains(transparentable.MeshRenderer)) continue;
                blockingObjectsRenderer.Add(transparentable.MeshRenderer);
                transparentables.Add(transparentable);
                transparentable.BeTransparent();
            }
        }

        if (!isBlocked && transparentables.Count > 0)
        {
            foreach (var transparentable in transparentables)
            {
                transparentable.BeOpaque();
            }
            transparentables.Clear();
            blockingObjectsRenderer.Clear();
        }
    }

    private void OnDisable()
    {
        DOTween.KillAll();
    }
}
