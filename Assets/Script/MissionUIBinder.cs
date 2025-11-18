using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MissionUIBinder : MonoBehaviour
{
    public TMP_Text missionText;
    public GameObject missionPanel;
    public Mission_System slot1;
    public Mission_System slot2;
    public Mission_System slot3;
    public GameObject selector;
    public RectTransform[] positions;

    void Start()
    {
        MissionManager.Instance.BindUI(this);
    }
}
