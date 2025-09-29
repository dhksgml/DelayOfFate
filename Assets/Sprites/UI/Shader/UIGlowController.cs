using UnityEngine;
using UnityEngine.UI;

public class UIGlowController : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    private Material mat;

    private void Awake()
    {
        mat = targetImage.material;
    }

    public void SetGlow(bool active)
    {
        mat.SetFloat("_GlowStrength", active ? 1f : 0f);
    }

    private void OnEnable()
    {
        GameEvents.OnClickLenton += SetGlow;
    }

    private void OnDisable()
    {
        GameEvents.OnClickLenton -= SetGlow;
    }
}