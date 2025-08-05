using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections; // Coroutine 사용을 위한 네임스페이스 추가

public class ItemSaleZone : MonoBehaviour
{
	[SerializeField] private LayerMask itemLayer;
	private Collider2D saleArea;
	public Place sale_place;
	public Image saleGaugeImage;
	public Image item_image;
	public TMP_Text item_text;

	private Coroutine fadeOutCoroutine;

	private void Awake()
	{
		saleArea = GetComponent<Collider2D>();

		// 평소에는 item_text를 완전히 안 보이게
		if (item_text != null)
		{
			Color c = item_text.color;
			c.a = 0f;
			item_text.color = c;
			item_image.color = c;
		}
	}

	public void SellItems()
	{
		Vector2 center = saleArea.bounds.center;
		Vector2 size = saleArea.bounds.size;

		Collider2D[] itemColliders = Physics2D.OverlapBoxAll(center, size, 0f, itemLayer);

		if (itemColliders.Length == 0)
		{
			// 아무것도 없을 때: 투명도 1 → 0으로 서서히 사라지기
			if (item_text != null)
			{
				// 기존 페이드아웃 코루틴이 있다면 중지
				if (fadeOutCoroutine != null) StopCoroutine(fadeOutCoroutine);
				fadeOutCoroutine = StartCoroutine(ShowAndFadeText());
			}
			return;
		}
		if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_all_sell"));
		foreach (Collider2D collider in itemColliders)
		{
			ItemObject itemObject = collider.GetComponent<ItemObject>();
			if (itemObject.itemData.isUsable) continue;
			itemObject.Sale("all");

			sale_place.sale_cu_Time = sale_place.sale_max_Time;
			sale_place.contactTime = 0f;
			if (sale_place.holdGauge != null)
			{
				sale_place.holdGauge.fillAmount = 0f;
				sale_place.holdGauge.gameObject.SetActive(false);
			}
		}
	}

	private IEnumerator ShowAndFadeText()
	{
		// 텍스트: 검은색, 배경: 흰색 (투명도는 동일하게 1부터 시작)
		Color textColor = item_text.color;
		Color imageColor = item_image.color;

		textColor = new Color(0f, 0f, 0f, 1f); // 검정색 텍스트
		imageColor = new Color(1f, 1f, 1f, 1f); // 흰색 배경

		item_text.color = textColor;
		item_image.color = imageColor;

		// 1초 유지
		yield return new WaitForSeconds(1f);

		// 0.5초 동안 알파값 감소
		float duration = 0.5f;
		float elapsed = 0f;

		while (elapsed < duration)
		{
			elapsed += Time.deltaTime;
			float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);

			textColor.a = alpha;
			imageColor.a = alpha;

			item_text.color = textColor;
			item_image.color = imageColor;

			yield return null;
		}

		// 최종 알파값 0으로
		textColor.a = 0f;
		imageColor.a = 0f;

		item_text.color = textColor;
		item_image.color = imageColor;
	}

}
