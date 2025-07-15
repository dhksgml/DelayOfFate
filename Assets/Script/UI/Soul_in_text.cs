using UnityEngine;
using UnityEngine.EventSystems; // 포인터 이벤트를 사용하려면 필요

public class Soul_in_text : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject mouse_text; // 활성화/비활성화할 오브젝트

    // 마우스를 올렸을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (mouse_text != null)
            mouse_text.SetActive(true);
    }

    // 마우스를 내렸을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        if (mouse_text != null)
            mouse_text.SetActive(false);
    }
}
