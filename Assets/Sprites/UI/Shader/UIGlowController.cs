using UnityEngine;
using UnityEngine.UI;

public class UIGlowController : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    [SerializeField] private Image targetFlashlightUIImage;
    private Material mat;

    private void Awake()
    {
        mat = targetImage.material;
    }

    public void SetGlow(bool active)
    {
        mat.SetFloat("_GlowStrength", active ? 1f : 0f);
    }

    public void SetImageColor(bool active)
    {
        targetFlashlightUIImage.color = active ? Color.white : new Color(0.5f, 0.5f, 0.5f);
    }

    private void OnEnable()
    {
        GameEvents.OnClickLenton += SetGlow;
        GameEvents.OnClickLenton += SetImageColor;
    }

    private void OnDisable()
    {
        GameEvents.OnClickLenton -= SetGlow;
        GameEvents.OnClickLenton -= SetImageColor;
    }
}