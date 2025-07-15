using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCorpse : MonoBehaviour
{
    [Header("Corpse Stat")]
    public string corpseName;
    public int corpseGold;
    public int corpseHeight;

    [Header("Reference")]
    public TextMeshProUGUI corpseNameText;

    void LateUpdate()
    {
        corpseNameText.text = corpseName;
    }
}
