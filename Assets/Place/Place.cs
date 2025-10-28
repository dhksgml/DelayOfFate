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
	public Sprite[] sprite_ch_spr;//장소가 변경했을때의 이미지 장소마다 집적 다르게 변경하기
    private int registered; // 횟수 제한(패시브 없으면 1회)

	public GameObject warningText; // 탈출 실패 경고 텍스트 (비활성 상태)

	private Coroutine warningCoroutine;
	private SpriteRenderer spriteRenderer;

	private void Start()
	{
		placeManager = FindObjectOfType<PlaceManager>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		registered = 1;
		if (warningText != null) warningText.SetActive(false);
		if (key_UI_iamge != null) key_UI_iamge.gameObject.SetActive(false);
	}

    private void Update()
    {
		if (Input.GetKeyDown(KeyCode.C))
		{
			if (registered > 0)
            {
				if (ui_on) Interaction(); //모든 장소는 1회용
			}
		}
		if (place_enum == Place_enum.resurrection)//부활장소고
        {
			if (registered == 0)//부활찬스도 썻고
            {
				if (placeManager.resurrection == false)//부활 한것도 확인 되었으면 
                {
					spriteRenderer.sprite = sprite_ch_spr[1]; //스프라이트 변경
				}
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
			if (key_UI_iamge != null) key_UI_iamge.gameObject.SetActive(false);
			other.GetComponent<PlayerController>().isPickUpableItem = false;
			ui_on = false;
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
		MissionManager.Instance.OnPlaceInteracted();
		spriteRenderer.sprite = sprite_ch_spr[0];
		SoundManager.Instance?.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_resurrection_register"));
		placeManager.resurrection = true;
		placeManager.resurrection_pos = transform.position;
	}

	private void TryEscape()
	{
		if (GameManager.Instance.Soul >= GameManager.Instance.N_Day_Cost) //약값을 낼 돈이 있어야 탈출 가능
		{
			registered -= 1;
			MissionManager.Instance.OnPlaceInteracted();
			Player_Item_Use player_Item_Use = FindObjectOfType<Player_Item_Use>();
			if (player_Item_Use != null)
			{
				// 인벤토리의 모든 슬롯 체크
				foreach (Item item in player_Item_Use.quickSlots)
				{
					// 아이템이 있으면 회수 카운트 증가
					if (item != null && !string.IsNullOrEmpty(item.itemName))
					{
						MissionManager.Instance.OnItemRecovered();
					}
				}
			}
			placeManager.Go_to_escape();
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
			registered -= 1;
			MissionManager.Instance.OnPlaceInteracted();
			spriteRenderer.sprite = sprite_ch_spr[0];
			player_Item_Use.Sale("all", player_Item_Use.quickSlots[0]);//[0]을 지정하지만 어차피 큰 의미는 없음
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
