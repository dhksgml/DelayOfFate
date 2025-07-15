using UnityEngine;

public class GhostAI : MonoBehaviour
{
    [Header("�̵� �ӵ�")]
    public float moveSpeed = 2f;

    [Header("�÷��̾� ���� ����")]
    public float detectRange = 3f;

    [Header("���� ���� ����(���� ���� �Ÿ��� �� ��)")]
    public float loseRangeMultiplier = 2f;

    [Tooltip("������ ����ġ�� ������ �������� �ּ� �Ÿ�")]
    public float returnThreshold = 0.1f;  // �ּ� �Ÿ�

    [Tooltip("���� ���� �Ÿ� ���")]
    public float chaseDistanceMultiplier = 2f;  // ���� ���� �Ÿ� ���

    [SerializeField] private SpriteRenderer spriteRenderer;  // ������ ��������Ʈ ������

    public Vector3 startPosition;  // ������ ���� ��ġ
    private Transform playerTransform;  // �÷��̾��� Transform
    public bool isChasing = false;  // ���� ������ ����
    private PlayerController player;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        // ���� ���� ��ġ ����
        startPosition = transform.position;
        // �÷��̾� ��ġ ã�� (�÷��̾� ������Ʈ�� Tag�� "Player"��� ����)
        playerTransform = player.transform;
    }

    private void Update()
    {
        FlipSpriteBasedOnPlayerPosition(); // �÷��̾� ��ġ�� ������� ��������Ʈ ������
        // ������ ���� ���̶��
        if (isChasing)
        {
            MoveToPlayer();
            CheckDistanceToPlayer();
        }
        else
        {
            // ������ ���۵��� �ʾҴٸ�
            CheckForPlayerDetection();
        }
    }

    // �÷��̾ ���� ���� ������ ���� ����
    private void CheckForPlayerDetection()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= detectRange)
        {
            StartChasing();
        }
    }

    // ������ �����ϸ� ���� ���� ��ȯ
    private void StartChasing()
    {
        isChasing = true;
    }

    // ���� �߿��� �÷��̾�� ���� �̵�
    private void MoveToPlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    // ���� ��, �÷��̾���� �Ÿ��� �־����� ���� ����
    private void CheckDistanceToPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > detectRange * chaseDistanceMultiplier)
        {
            StopChasing();
        }
    }

    // ������ ���߰� ���� �ڸ��� ���ư��� ����
    private void StopChasing()
    {
        isChasing = false;
        StartCoroutine(ReturnToStartPosition());
    }
    void FlipSpriteBasedOnPlayerPosition()
    {
        // ������ x��ǥ�� �÷��̾��� x��ǥ���� ������ ���ʿ� �ִٴ� ��
        if (transform.position.x < playerTransform.position.x)
        {
            // ��������Ʈ�� ���� ��������
            spriteRenderer.flipX = false;
        }
        else
        {
            // ��������Ʈ�� ������
            spriteRenderer.flipX = true;
        }
    }

    // ������ ���߸� ����ġ�� ���ư��� �ڷ�ƾ
    private System.Collections.IEnumerator ReturnToStartPosition()
    {
        while (Vector3.Distance(transform.position, startPosition) > returnThreshold)
        {
            Vector3 direction = (startPosition - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            yield return null; // �� ������ ���
        }
        transform.position = startPosition; // ��Ȯ�� ����ġ�� �����ϸ� ��ġ ����
    }
}
