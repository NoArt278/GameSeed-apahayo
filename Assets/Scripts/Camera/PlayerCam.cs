using Cinemachine;
using DG.Tweening;
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
            if (!hit.collider.CompareTag("Player") && !hit.collider.CompareTag("Cat") && !hit.collider.CompareTag("Dog") && !hit.collider.CompareTag("Hide")) // Camera obstructed
            {
                if (Vector3.Distance(transform.position, player.position) > Vector3.Distance(transform.position, hit.point))
                {
                    isBlocked = true;
                    // Save previous material
                    MeshRenderer blockingObjRenderer = hit.collider.GetComponent<MeshRenderer>();
                    Material currMaterial = blockingObjRenderer.material;
                    if (!blockingObjectsRenderer.Contains(blockingObjRenderer) && blockingObjectsRenderer != null)
                    {
                        blockingObjectsRenderer.Add(blockingObjRenderer);
                        prevMaterials.Add(currMaterial);

                        blockingObjRenderer.material.DOKill();
                        Material newTransMaterial = new Material(transparentMaterial);
                        Color transMaterialColor = currMaterial.color;
                        newTransMaterial.color = transMaterialColor;
                        blockingObjRenderer.material = newTransMaterial;
                        newTransMaterial.DOFade(0.2f, 0.2f).onUpdate += () =>
                        {
                            blockingObjRenderer.material = newTransMaterial;
                        };
                    }
                }
            }
        }

        if (!isBlocked && blockingObjectsRenderer.Count > 0)
        {
            int blockerCount = blockingObjectsRenderer.Count;
            for (int i = 0; i < blockerCount; i++)
            {
                MeshRenderer blockObjRenderer = blockingObjectsRenderer[0];
                Material transMaterial = blockingObjectsRenderer[0].material;
                Material opaqueMaterial = prevMaterials[0];
                Color finColor = transMaterial.color;
                finColor.a = 1;
                blockingObjectsRenderer.RemoveAt(0);
                prevMaterials.RemoveAt(0);
                transMaterial.DOKill();
                Tween changeTween = transMaterial.DOFade(1f, 0.2f);
                changeTween.onUpdate += () =>
                {
                    blockObjRenderer.material = transMaterial;
                };
                changeTween.onComplete += () =>
                {
                    blockObjRenderer.material = opaqueMaterial;
                    blockObjRenderer.material.color = finColor;
                };
                changeTween.onKill += () =>
                {
                    blockObjRenderer.material = opaqueMaterial;
                    blockObjRenderer.material.color = finColor;
                };
            }
        }
    }

    private void OnDisable()
    {
        DOTween.KillAll();
    }
}
