using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Place : MonoBehaviour
{
	private PlaceManager placeManager;
	private ItemSaleZone sale_zone_obj; // 판매 구역

	private enum Place_enum
	{
		escape,
		resurrection,
		sale
	}

	[SerializeField] private Place_enum place_enum;
	[SerializeField] private float requiredTime = 2f;
	[SerializeField] public Image holdGauge;

	[HideInInspector] public float sale_max_Time = 10f; // 쿨타임 최대값
	[HideInInspector] public float sale_cu_Time = 0f;

	[HideInInspector] public float contactTime = 0f;
	private bool playerInRange = false;
	private bool registered = false;

	[Header("Escape Settings")]
	[SerializeField] private GameObject warningText; // 탈출 실패 경고 텍스트 (비활성 상태)

	private Coroutine warningCoroutine;

	private void Start()
	{
		placeManager = FindObjectOfType<PlaceManager>();

		Transform saleZoneTransform = transform.Find("Sale_zone");
		if (saleZoneTransform != null)
			sale_zone_obj = saleZoneTransform.GetComponent<ItemSaleZone>();

		if (holdGauge != null)
			holdGauge.gameObject.SetActive(false);

		if (warningText != null)
			warningText.SetActive(false);
	}

	private void Update()
	{
		if (sale_cu_Time > 0) sale_cu_Time -= Time.deltaTime;
		if (registered) return;

		playerInRange = IsPlayerNearby();

		if (playerInRange)
		{
			ActivateGauge();

			contactTime += Time.deltaTime;
			UpdateGaugeFill(contactTime / requiredTime);

			if (contactTime >= requiredTime)
			{
				switch (place_enum)
				{
					case Place_enum.resurrection:
						RegisterResurrection();
						break;

					case Place_enum.escape:
						if (sale_cu_Time <= 0)
						{
							TryEscape();
							sale_cu_Time = requiredTime;
						}
						break;

					case Place_enum.sale:
						if (sale_cu_Time <= 0)
						{
							sale_zone_obj.SellItems();
							sale_cu_Time = requiredTime;
						}
						break;
				}
			}
		}
		else
		{
			contactTime = 0f;
			if (holdGauge != null)
			{
				holdGauge.fillAmount = 0f;
				holdGauge.gameObject.SetActive(false);
			}
		}
	}

	private bool IsPlayerNearby()
	{
		Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1f);
		foreach (var hit in hits)
		{
			if (hit.CompareTag("Player"))
				return true;
		}
		return false;
	}

	private void ActivateGauge()
	{
		if (holdGauge != null && !holdGauge.gameObject.activeSelf)
			holdGauge.gameObject.SetActive(true);
	}

	private void UpdateGaugeFill(float ratio)
	{
		if (holdGauge != null)
			holdGauge.fillAmount = Mathf.Clamp01(ratio);
	}

	private void RegisterResurrection()
	{
		registered = true;
		if (SoundManager.Instance != null)
			SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_resurrection_register"));

		if (holdGauge != null)
			holdGauge.gameObject.SetActive(false);

		placeManager.resurrection = true;
	}

	private void EscapeScene()
	{
		registered = true;
		if (holdGauge != null)
			holdGauge.gameObject.SetActive(false);

		placeManager.Go_to_escape();
	}

	private void TryEscape()
	{
		if (GameManager.Instance.Soul >= GameManager.Instance.N_Day_Cost)
		{
			EscapeScene();
		}
		else
		{
			contactTime = 0f;

			if (holdGauge != null)
			{
				holdGauge.fillAmount = 0f;
				holdGauge.gameObject.SetActive(false);
			}

			if (warningCoroutine != null)
				StopCoroutine(warningCoroutine);

			if (warningText != null)
				warningCoroutine = StartCoroutine(ShowWarningText());
		}
	}

	private IEnumerator ShowWarningText()
	{
		warningText.SetActive(true);

		CanvasGroup canvasGroup = warningText.GetComponent<CanvasGroup>();
		if (canvasGroup == null)
			canvasGroup = warningText.AddComponent<CanvasGroup>();

		canvasGroup.alpha = 1f;

		yield return new WaitForSeconds(3f);

		float fadeDuration = 1f;
		float timer = 0f;

		while (timer < fadeDuration)
		{
			timer += Time.deltaTime;
			canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
			yield return null;
		}

		warningText.SetActive(false);
	}
}
