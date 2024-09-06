using DG.Tweening;
using UnityEngine;
public class Transparentable : MonoBehaviour {
    [SerializeField] private MeshRenderer meshRenderer;
    public MeshRenderer MeshRenderer { get => meshRenderer; }
    [SerializeField] private Material transparentMaterial;
    private Material[] storedMaterials;
    private Material[] transparents;
    private void Awake() {
        if (meshRenderer == null) {
            Debug.LogWarning("MeshRenderer is null for " + gameObject.name);
            return;
        }
        storedMaterials = meshRenderer.sharedMaterials;
        transparents = new[] { transparentMaterial };
    }
    public void BeTransparent() {
        // Belom nemu cara yang bagus
        // meshRenderer.sharedMaterials = transparents;
        return;

        // transparentMaterial.DOFade(0.2f, 0.2f).OnUpdate(() => {
        //     meshRenderer.sharedMaterials = transparents;
        // });
    }
    public void BeOpaque() {
        // Belom nemu cara yang bagus
        // meshRenderer.sharedMaterials = storedMaterials;
        return;
        // transparentMaterial.DOFade(1f, 0.2f).OnUpdate(() => {
        //     meshRenderer.sharedMaterials = transparents;
        // }).OnComplete(() => {
        //     meshRenderer.sharedMaterials = storedMaterials;
        // });
    }
}