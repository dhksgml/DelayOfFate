using UnityEngine;
using UnityEngine.UI;

public class MoneyEffect : MonoBehaviour
{
	public string ty;

	public Sprite[] soulSprites;
	public Sprite[] coinSprites;

	private int flyPower;
	private Vector2 flyDirection;
	private float spinSpeed;

	private Rigidbody2D rb;
	private Image image;
	private float timer;

	private enum State { Flying, Waiting, Homing }
	private State currentState = State.Flying;

	private float stopTime = 0.5f;
	private float homingDelay;
	private int homingSpeed;

	private Transform targetIcon;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		image = GetComponent<Image>();

		// 타입별 스프라이트 랜덤 지정 및 타겟 설정
		if (ty == "Soul")
		{
			if (soulSprites.Length > 0)
				image.sprite = soulSprites[Random.Range(0, soulSprites.Length)];
			targetIcon = GameObject.Find("Soul_Image_Icon")?.transform;
		}
		else if (ty == "Coin")
		{
			if (coinSprites.Length > 0)
				image.sprite = coinSprites[Random.Range(0, coinSprites.Length)];
			targetIcon = GameObject.Find("Coin_Image_Icon")?.transform;
		}
		homingDelay = Random.Range(0.25f, 0.3f);
		flyPower = Random.Range(800, 1000);
		homingSpeed = Random.Range(1500, 2000);
		float angle = Random.Range(0f, 360f);
		flyDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
		spinSpeed = Random.Range(360f*2, 360f*4); // 초당 회전속도

		// 초기 발사
		rb.velocity = flyDirection * flyPower;
	}

	private void Update()
	{
		timer += Time.deltaTime;

		switch (currentState)
		{
			case State.Flying:
				transform.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);
				if (timer >= stopTime)
				{
					rb.velocity = Vector2.zero;
					spinSpeed = 0f;
					currentState = State.Waiting;
					timer = 0f;
				}
				break;

			case State.Waiting:
				if (timer >= homingDelay)
				{
					currentState = State.Homing;
					timer = 0f;
				}
				break;

			case State.Homing:
				if (targetIcon == null)
				{
					Destroy(gameObject);
					break;
				}

				// 위치 보간 이동
				Vector2 targetPos = targetIcon.position;
				transform.position = Vector2.MoveTowards(transform.position, targetPos, homingSpeed * Time.deltaTime);

				// 방향 회전
				Vector2 moveDir = (targetPos - (Vector2)transform.position).normalized;
				float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.Euler(0f, 0f, angle);

				// 도착 시 제거
				if (Vector2.Distance(transform.position, targetPos) < 1f)
				{
					Destroy(gameObject);
				}
				break;
		}
	}
}
