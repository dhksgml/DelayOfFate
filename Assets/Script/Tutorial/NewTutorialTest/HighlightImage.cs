using UnityEngine;
using UnityEngine.UI;

public class HighlightImage : MonoBehaviour
{
    public Image glowImage;   // Glow Sprite
    public float speed = 2f;  // ±ôºýÀÌ´Â ¼Óµµ

    private Color baseColor;

    void Start()
    {
        baseColor = glowImage.color;
    }

    void Update()
    {
        float alpha = (Mathf.Sin(Time.time * speed) + 1) / 2f; // 0~1 ¹Ýº¹
        glowImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
    }
}
