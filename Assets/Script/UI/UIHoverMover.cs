using UnityEngine;
using UnityEngine.UI;

public class UIHoverMover : MonoBehaviour
{
	public RectTransform targetPanel;      // �̵��� UI
	public Vector2 hiddenPosition;         // ������ ��ġ
	public Vector2 shownPosition;          // Ƣ��� ��ġ
	public float moveDuration = 0.3f;      // �̵� �ð�

	private bool isOpen = false;
	private bool isMoving = false;
	private float moveStartTime;
	private Vector2 startPos, endPos;

	void Update()
	{
		if (!isMoving || targetPanel == null)
			return;

		float t = (Time.time - moveStartTime) / moveDuration;
		if (t >= 1f)
		{
			targetPanel.anchoredPosition = endPos;
			isMoving = false;
			return;
		}

		targetPanel.anchoredPosition = Vector2.Lerp(startPos, endPos, EaseOutCubic(t));
	}

	public void TogglePanel()
	{
		if (isMoving) return;

		isOpen = !isOpen;
		startPos = targetPanel.anchoredPosition;
		endPos = isOpen ? shownPosition : hiddenPosition;
		moveStartTime = Time.time;
		isMoving = true;
	}

	float EaseOutCubic(float t)
	{
		return 1 - Mathf.Pow(1 - t, 3);
	}
}
