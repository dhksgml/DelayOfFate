using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

	public GameObject Special_text_obj; // 특수 텍스트 오브젝트
	public TMP_Text Special_Text; // 특수 텍스트
	private Coroutine warningCoroutine;

	private void Start()
	{
		placeManager = FindObjectOfType<PlaceManager>();
		registered = 1;
		if (Special_text_obj != null) Special_text_obj.SetActive(false);
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
				if (place_enum == Place_enum.sale)
                {
					if (Special_text_obj != null) Special_text_obj.SetActive(true);
					Player_Item_Use player_Item_Use = other.GetComponent<Player_Item_Use>();
					int item_soul = 0;
					int item_coin = 0;
					for (int i = 0; i < player_Item_Use.quickSlots.Length; i++)
					{
						if (player_Item_Use.quickSlots[i] != null && !string.IsNullOrEmpty(player_Item_Use.quickSlots[i].itemName))
						{
							item_coin += player_Item_Use.quickSlots[i].Coin*2;
							item_soul += player_Item_Use.quickSlots[i].Coin;
						}
					}
					if (Special_text_obj != null) Special_Text.text = string.Format("<sprite=8> +{0} / <sprite=9> +{1}",item_soul,item_coin);
				}
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

			if (Special_text_obj != null) warningCoroutine = StartCoroutine(ShowSpecial_text_obj());
		}
	}
	public void SellItems()
	{
		// 아무것도 없을 때: 투명도 1 → 0으로 서서히 사라지기
		if (Special_text_obj != null)
		{
			// 기존 페이드아웃 코루틴이 있다면 중지
			if (warningCoroutine != null) StopCoroutine(warningCoroutine);
			if (Special_text_obj != null) warningCoroutine = StartCoroutine(ShowSpecial_text_obj());
			Player_Item_Use player_Item_Use = FindObjectOfType<Player_Item_Use>();
			player_Item_Use.Sale("all",0);
		}
		if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_all_sell"));
	}
	private IEnumerator ShowSpecial_text_obj()
	{
		Special_text_obj.SetActive(true);

		CanvasGroup canvasGroup = Special_text_obj.GetComponent<CanvasGroup>();
		if (canvasGroup == null) canvasGroup = Special_text_obj.AddComponent<CanvasGroup>();

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

		Special_text_obj.SetActive(false);
	}
}
