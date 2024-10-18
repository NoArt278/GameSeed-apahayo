using Cinemachine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    private Transform _player;
    private List<Transparentable> _transparentables;
    private List<MeshRenderer> _blockingObjectsRenderer;

    private void Start()
    {
        CinemachineVirtualCamera vcam = GetComponentInChildren<CinemachineVirtualCamera>();
        _player = vcam.Follow;
        _transparentables = new List<Transparentable>();
        _blockingObjectsRenderer = new List<MeshRenderer>();
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.InGame) return;
        Vector3 direction = _player.position - transform.position;
        RaycastHit[] hits = Physics.RaycastAll(transform.position - direction * 3, direction, Mathf.Infinity, LayerMask.GetMask("Obstacle"));
        bool isBlocked = false;

        foreach (var hit in hits)
        {
            if (Vector3.Distance(transform.position, _player.position) < Vector3.Distance(transform.position, hit.point)) continue;

            isBlocked = true;
            if (hit.collider.TryGetComponent(out Transparentable transparentable))
            {
                if (_transparentables.Contains(transparentable) || _blockingObjectsRenderer.Contains(transparentable.MeshRenderer)) continue;
                _blockingObjectsRenderer.Add(transparentable.MeshRenderer);
                _transparentables.Add(transparentable);
                transparentable.BeTransparent();
            }
        }

        if (!isBlocked && _transparentables.Count > 0)
        {
            foreach (var transparentable in _transparentables)
            {
                transparentable.BeOpaque();
            }
            _transparentables.Clear();
            _blockingObjectsRenderer.Clear();
        }
    }

    private void OnDisable()
    {
        DOTween.KillAll();
    }
}
