using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class HighlightTextColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text targetText;
    public Color normalColor = Color.white;
    public Color highlightColor = Color.yellow;

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetText.color = highlightColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetText.color = normalColor;
    }
}
