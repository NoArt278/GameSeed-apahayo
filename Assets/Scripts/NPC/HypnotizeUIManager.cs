using UnityEngine;
using UnityEngine.UI;

public class HypnotizeUIManager : MonoBehaviour
{
    [SerializeField] private Image hypnoBar;

    private void Start() {
        hypnoBar.fillAmount = 0;
        DisableHypnoBar();
    }

    public void EnableHypnoBar(){
        hypnoBar.gameObject.SetActive(true);
    }

    public void DisableHypnoBar(){
        hypnoBar.gameObject.SetActive(false);
    }

    public void UpdateHypnoBar(float t){
        hypnoBar.fillAmount = t;
    }

    public void SetupHypnoBar(Canvas canvas){
        hypnoBar.transform.SetParent(canvas.transform);
    }
}
