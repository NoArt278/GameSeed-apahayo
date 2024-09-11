using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
public class Transparentable : MonoBehaviour {
    [SerializeField] private MeshRenderer meshRenderer;
    public MeshRenderer MeshRenderer { get => meshRenderer; }
    private Material[] storedMaterials;
    private Material opaqueOutline, transparentOutline, transparentMaterial;

    private void Awake() {
        if (meshRenderer == null) {
            Debug.LogWarning("MeshRenderer is null for " + gameObject.name);
            return;
        }
        storedMaterials = meshRenderer.sharedMaterials;
        for (int i=0; i<storedMaterials.Length; i++)
        {
            storedMaterials[i] = new Material(storedMaterials[i]);
        }
        meshRenderer.sharedMaterials = storedMaterials;
        transparentOutline = Resources.Load<Material>("Materials/outlineTransparent");
        opaqueOutline = Resources.Load<Material>("Materials/outline");
    }
    public void BeTransparent() {
        for (int i=0; i<storedMaterials.Length; i++)
        {
            if (storedMaterials[i].name == "outline")
            {
                storedMaterials[i] = transparentOutline;
                meshRenderer.sharedMaterials = storedMaterials;
                continue;
            }
            storedMaterials[i].DOFade(0.2f, 0.2f);
        }
    }
    public void BeOpaque() {
        for (int i = 0; i < storedMaterials.Length; i++)
        {
            if (storedMaterials[i].name == "outlineTransparent")
            {
                storedMaterials[i] = opaqueOutline;
                meshRenderer.sharedMaterials = storedMaterials;
                continue;
            }
            storedMaterials[i].DOFade(1f, 0.2f);
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < storedMaterials.Length; i++)
        {
            if (storedMaterials[i].name == "outlineTransparent")
            {
                storedMaterials[i] = opaqueOutline;
                meshRenderer.sharedMaterials = storedMaterials;
                continue;
            }
            Color currColor = storedMaterials[i].color;
            currColor.a = 1f;
            storedMaterials[i].color = currColor;
        }
    }
}