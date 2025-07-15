using UnityEngine;

public class GhostAI : MonoBehaviour
{
    [Header("이동 속도")]
    public float moveSpeed = 2f;

    [Header("플레이어 감지 범위")]
    public float detectRange = 3f;

    [Header("추적 포기 범위(추적 시작 거리의 두 배)")]
    public float loseRangeMultiplier = 2f;

    [Tooltip("유령이 원위치에 도달할 때까지의 최소 거리")]
    public float returnThreshold = 0.1f;  // 최소 거리

    [Tooltip("추적 포기 거리 배수")]
    public float chaseDistanceMultiplier = 2f;  // 추적 시작 거리 배수

    [SerializeField] private SpriteRenderer spriteRenderer;  // 유령의 스프라이트 렌더러

    public Vector3 startPosition;  // 유령의 시작 위치
    private Transform playerTransform;  // 플레이어의 Transform
    public bool isChasing = false;  // 추적 중인지 여부
    private PlayerController player;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        // 유령 시작 위치 저장
        startPosition = transform.position;
        // 플레이어 위치 찾기 (플레이어 오브젝트에 Tag가 "Player"라고 가정)
        playerTransform = player.transform;
    }

    private void Update()
    {
        FlipSpriteBasedOnPlayerPosition(); // 플레이어 위치를 기반으로 스프라이트 뒤집기
        // 유령이 추적 중이라면
        if (isChasing)
        {
            MoveToPlayer();
            CheckDistanceToPlayer();
        }
        else
        {
            // 추적이 시작되지 않았다면
            CheckForPlayerDetection();
        }
    }

    // 플레이어가 범위 내에 들어오면 추적 시작
    private void CheckForPlayerDetection()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= detectRange)
        {
            StartChasing();
        }
    }

    // 추적을 시작하면 추적 모드로 전환
    private void StartChasing()
    {
        isChasing = true;
    }

    // 추적 중에는 플레이어로 향해 이동
    private void MoveToPlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    // 추적 중, 플레이어와의 거리가 멀어지면 추적 포기
    private void CheckDistanceToPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > detectRange * chaseDistanceMultiplier)
        {
            StopChasing();
        }
    }

    // 추적을 멈추고 원래 자리로 돌아가기 시작
    private void StopChasing()
    {
        isChasing = false;
        StartCoroutine(ReturnToStartPosition());
    }
    void FlipSpriteBasedOnPlayerPosition()
    {
        // 유령의 x좌표가 플레이어의 x좌표보다 작으면 왼쪽에 있다는 것
        if (transform.position.x < playerTransform.position.x)
        {
            // 스프라이트를 원래 방향으로
            spriteRenderer.flipX = false;
        }
        else
        {
            // 스프라이트를 뒤집음
            spriteRenderer.flipX = true;
        }
    }

    // 추적을 멈추면 원위치로 돌아가는 코루틴
    private System.Collections.IEnumerator ReturnToStartPosition()
    {
        while (Vector3.Distance(transform.position, startPosition) > returnThreshold)
        {
            Vector3 direction = (startPosition - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            yield return null; // 한 프레임 대기
        }
        transform.position = startPosition; // 정확히 원위치에 도달하면 위치 설정
    }
}
