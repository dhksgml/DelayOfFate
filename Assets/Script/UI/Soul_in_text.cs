using UnityEngine;
using UnityEngine.EventSystems; // ������ �̺�Ʈ�� ����Ϸ��� �ʿ�

public class Soul_in_text : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject mouse_text; // Ȱ��ȭ/��Ȱ��ȭ�� ������Ʈ

    // ���콺�� �÷��� ��
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (mouse_text != null)
            mouse_text.SetActive(true);
    }

    // ���콺�� ������ ��
    public void OnPointerExit(PointerEventData eventData)
    {
        if (mouse_text != null)
            mouse_text.SetActive(false);
    }
}
