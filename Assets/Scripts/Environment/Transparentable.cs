using DG.Tweening;
using UnityEngine;

public class Transparentable : MonoBehaviour {
    [SerializeField] private MeshRenderer meshRenderer;
    public MeshRenderer MeshRenderer { get => meshRenderer; }
    private Material[] _storedMaterials;
    private Material _opaqueOutline;
    private Material _transparentOutline;

    private void Awake() {
        if (meshRenderer == null) {
            Debug.LogWarning("MeshRenderer is null for " + gameObject.name);
            return;
        }
        _storedMaterials = meshRenderer.sharedMaterials;
        for (int i=0; i<_storedMaterials.Length; i++)
        {
            _storedMaterials[i] = new Material(_storedMaterials[i]);
        }
        meshRenderer.sharedMaterials = _storedMaterials;
        _transparentOutline = Resources.Load<Material>("Materials/outlineTransparent");
        _opaqueOutline = Resources.Load<Material>("Materials/outline");
    }
    public void BeTransparent() {
        for (int i=0; i<_storedMaterials.Length; i++)
        {
            if (_storedMaterials[i].name == "outline")
            {
                _storedMaterials[i] = _transparentOutline;
                meshRenderer.sharedMaterials = _storedMaterials;
                continue;
            }
            _storedMaterials[i].DOFade(0.2f, 0.2f);
            _storedMaterials[i].SetFloat("_ZWrite", 0);
        }
    }
    public void BeOpaque() {
        for (int i = 0; i < _storedMaterials.Length; i++)
        {
            if (_storedMaterials[i].name == "outlineTransparent")
            {
                _storedMaterials[i] = _opaqueOutline;
                meshRenderer.sharedMaterials = _storedMaterials;
                continue;
            }
            _storedMaterials[i].DOFade(1f, 0.2f);
            _storedMaterials[i].SetFloat("_ZWrite", 1);
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < _storedMaterials.Length; i++)
        {
            if (_storedMaterials[i].name == "outlineTransparent")
            {
                _storedMaterials[i] = _opaqueOutline;
                meshRenderer.sharedMaterials = _storedMaterials;
                continue;
            }
            Color currColor = _storedMaterials[i].color;
            currColor.a = 1f;
            _storedMaterials[i].color = currColor;
        }
    }
}