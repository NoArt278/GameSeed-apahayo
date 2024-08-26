using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PerksManager : MonoBehaviour {
    [SerializeField] private PerkUpgrade[] perks;
    [SerializeField] private PlayerStats playerStats;

    // Doesn't work sadge
    // #if UNITY_EDITOR
    // [Button]
    // public void RefreshPerkUpgradesFromAssets() {
    //     List<PerkUpgrade> perkUpgrades = new();
        
    //     string[] guids = AssetDatabase.FindAssets("t:ScriptableObject");
    //     foreach (string guid in guids)
    //     {
    //         string path = AssetDatabase.GUIDToAssetPath(guid);
    //         ScriptableObject obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            
    //         if (obj is PerkUpgrade perkUpgrade)
    //         {
    //             perkUpgrades.Add(perkUpgrade);
    //         }
    //     }
    // }
    // #endif

    public PerkUpgrade[] PickThreeRandomUpgrade() {
        if (perks.Length < 3) {
            Debug.LogWarning("Not enough perks to pick from.");
            return null;
        }

        PerkUpgrade[] pickedPerks = new PerkUpgrade[3];
        List<PerkUpgrade> remainingPerks = new List<PerkUpgrade>(perks);

        for (int i = 0; i < 3; i++) {
            int randomIndex = Random.Range(0, remainingPerks.Count);
            pickedPerks[i] = remainingPerks[randomIndex];
            remainingPerks.RemoveAt(randomIndex);
        }

        return pickedPerks;
    }

    public void ApplyUpgrade(PerkUpgrade perkUpgrade) {
        perkUpgrade.ApplyUpgrade(playerStats);
    }
}