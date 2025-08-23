using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Place : MonoBehaviour
{
	private PlaceManager placeManager;
	private bool ui_on = false; // 닿고 있는지 여부
	private enum Place_enum
	{
		escape,
		resurrection,
		sale
	}

	[SerializeField] private Place_enum place_enum;
	public GameObject key_UI_iamge;
	private int registered; // 횟수 제한(패시브 없으면 1회)

	public GameObject warningText; // 탈출 실패 경고 텍스트 (비활성 상태)

	private Coroutine warningCoroutine;

	private void Start()
	{
		placeManager = FindObjectOfType<PlaceManager>();
		registered = 1;
		if (warningText != null) warningText.SetActive(false);
		if (key_UI_iamge != null) key_UI_iamge.gameObject.SetActive(false);
	}

    private void Update()
    {
		if (Input.GetKeyDown(KeyCode.E))
		{
			if (registered > 0)
            {
				if (ui_on) Interaction(); //모든 장소는 1회용
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			if (registered > 0) //횟수 제한이 있어야함
            {
				if (key_UI_iamge != null) key_UI_iamge.gameObject.SetActive(true);
				other.GetComponent<PlayerController>().isPickUpableItem = true;
				ui_on = true;
			}
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			if (registered > 0) //횟수 제한이 있어야함
			{
				if (key_UI_iamge != null) key_UI_iamge.gameObject.SetActive(false);
				other.GetComponent<PlayerController>().isPickUpableItem = false;
				ui_on = false;
			}
		}
	}


	public void Interaction()
    {
		switch (place_enum)
		{
			case Place_enum.resurrection:
				RegisterResurrection();
				break;

			case Place_enum.escape:
				TryEscape();
				break;

			case Place_enum.sale:
				SellItems();
				break;
		}
	}

	private void RegisterResurrection()
	{
		registered -= 1;
		SoundManager.Instance?.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_resurrection_register"));
		placeManager.resurrection = true;
	}

	private void EscapeScene()
	{
		registered -= 1;
		placeManager.Go_to_escape();
	}

	private void TryEscape()
	{
		if (GameManager.Instance.Soul >= GameManager.Instance.N_Day_Cost) //약값을 낼 돈이 있어야 탈출 가능
		{
			EscapeScene();
		}
		else
		{
			if (warningCoroutine != null) StopCoroutine(warningCoroutine);

			if (warningText != null) warningCoroutine = StartCoroutine(ShowWarningText());
		}
	}
	public void SellItems()
	{
		// 아무것도 없을 때: 투명도 1 → 0으로 서서히 사라지기
		if (warningText != null)
		{
			// 기존 페이드아웃 코루틴이 있다면 중지
			if (warningCoroutine != null) StopCoroutine(warningCoroutine);
			if (warningText != null) warningCoroutine = StartCoroutine(ShowWarningText());
			Player_Item_Use player_Item_Use = FindObjectOfType<Player_Item_Use>();
			player_Item_Use.Sale("all",0);
		}
		if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_all_sell"));
	}
	private IEnumerator ShowWarningText()
	{
		warningText.SetActive(true);

		CanvasGroup canvasGroup = warningText.GetComponent<CanvasGroup>();
		if (canvasGroup == null) canvasGroup = warningText.AddComponent<CanvasGroup>();

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
