using TMPro;
using UnityEngine;

public class XPTest : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private XPManager xpManager;

    private void Update() {
        xpText.text = "Remaining XP To Level Up: " + (xpManager.XPToLevelUp - xpManager.CurrentXP);
    }
}
