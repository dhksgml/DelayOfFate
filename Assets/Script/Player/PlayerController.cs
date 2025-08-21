using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    public Transform fieldOfView;

    public float moveSpeed = 4f;
    public float runSpeed = 7f;
    public float currentMoveSpeed;
    public float speedMultiplier = 1f;
    public float rayCastDistance = 2f;

    [SerializeField] private float hpRecoveryDuration = 10f;
    //[SerializeField] private float mpRecoveryDuration = 10f;
    [SerializeField] private float spRecoveryDuration = 7.5f;

    const float runThreshold = 10f; //�޸��⿡ �ʿ��� �ּ� sp

    public float attackDamage = 1;
    public float attackCoolTime;

    //public float originalMaxHp = 100;
    public float extraHp;
    public float currentExtraHp;

    public float maxHp = 100; //�ִ� ü��
    public float maxMp = 100; //�ִ� ���ŷ�
    public float maxSp = 100; //�ִ� ���

    public float currentHp; //���� ü��
    public float currentMp; //���� ���ŷ�
    public float currentSp; //���� ���

    public bool isFreeze;
    //�Է��� Ű�� ������� Ȯ��
    int inputKey = 0;
    [SerializeField] private float minMoveSpeed = 0.4f;  //���� �߰�

    public bool isRecovering = false;
    public bool isAutoSPRegen = true;

    public bool isPickUpableItem = false;   //������ �ֿ� �� �ִ��� ����
    public bool isHavingFlashLight = false; //������ ȹ�� ����

    public int flashLightLevel = 1;
    public GameObject flashLightObject;
    public GameObject lightCircleObject;
    public float flashLightDistance = 3f;

    public bool isUseItem = false;
    public float Player_Usage_cu_cool_down = 0;//�÷��̾� ������ ���� ��ٿ�
    private Coroutine currentItemUseCoroutine = null;

    private float walkTimer = 0f;
    [SerializeField] private float walkThreshold = 1f;

    private Vector3 aimDir;
    private Vector3 mousePosition;
    private float angle;

    private bool isMoveAble = true;
    private bool isRun = false;
    public PlayerState currentState = PlayerState.Idle;

    private float x;
    private float y;
    [HideInInspector] public bool isMoving;
    private Vector2 lastMoveDirection;

    SpriteRenderer spriteRenderer;
    Animator animator;
    PlaceManager placeManager;
    Vector3 moveVelocity;

    private Vector2 moveInput;
    private Vector2 clickLookDirection = Vector2.down;
    private float clickLookTimer = 0f;
    private float clickLookDuration = 0.3f; // Ŭ�� �� 0.2�ʰ� �ش� �������� ����
    private Camera mainCamera;

    public GameObject Effect_pr;//����Ʈ ������Ʈ
    private float effectTimer = 0f; //����Ʈ ��Ÿ��

    Player_Item_Use player_Item_Use;
    public GameObject corpse; // �÷��̾� ��ü

    private bool isActing = false; //�ݱ�, ������ �ִϸ��̼� ���� �� ����
    private bool isPicking = false; //�ݱ�, ������ � �� ������ �� 
    public bool IsPicking { get => isPicking; set => isPicking = value; }

    private Coroutine recoveryCoroutine;

    private NearestItemFinder nearestItemFinder; //����� ������ Ž��
    public NearestItemFinder NearestItemFinder => nearestItemFinder;

    public enum PlayerState
    {
        Idle,
        Walk,
        Run,
        Recovery,
        Resting,
        GettingUp
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        placeManager = FindObjectOfType<PlaceManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player_Item_Use = GetComponent<Player_Item_Use>();
        nearestItemFinder = GetComponent<NearestItemFinder>();
        mainCamera = Camera.main;

        lightCircleObject.SetActive(true);
        flashLightObject.SetActive(true);
        Init();
    }

    private void OnEnable()
    {
        GameEvents.OnPickupItem += HandlePickupItem;
        GameEvents.OnDropItem += HandleDropItem;
        GameEvents.OnTimeAngleUnit18 += HadleTimeAngleUnit18;
    }

    private void OnDisable()
    {
        GameEvents.OnPickupItem -= HandlePickupItem;
        GameEvents.OnDropItem -= HandleDropItem;
        GameEvents.OnTimeAngleUnit18 -= HadleTimeAngleUnit18;
    }

    public void Init()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.playerData != null)
            {
                PlayerData playerData = GameManager.Instance.playerData;
                extraHp = playerData.extraHp;
                currentExtraHp = extraHp;

                currentHp = playerData.maxHp;

                currentMp = playerData.maxMp;

                flashLightLevel = playerData.flashLightLevel;
                UpdateFlashLight();


                if (PassiveItemManager.Instance != null && PassiveItemManager.Instance.HasEffect("Soul_Add_5_2"))
                {
                    PassiveItemManager.Instance.DoPassive_5_2();
                    speedMultiplier = GameManager.Instance.playerData.speedMultiplier;
                }
            }
        }
        else
        {
            currentHp = maxHp;
            currentExtraHp = extraHp;

            currentMp = maxMp;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (clickLookTimer > 0f)
        {
            clickLookTimer -= Time.deltaTime;
            clickLookTimer = Mathf.Clamp(clickLookTimer, 0f, clickLookDuration);
        }

        if (isFreeze)
        {
            FreezingCancle();
        }
        UpdateItemCooldown();

        if (currentState == PlayerState.Resting) //����Ʈ ����
        {
            if (currentHp + currentExtraHp < maxHp + extraHp || currentSp < maxSp)
            {
                effectTimer -= Time.deltaTime;
                if (effectTimer <= 0f)
                {
                    Effect_cr("hp", transform.position, -1f);
                    effectTimer = 0.25f; // ���� �ߵ����� �ð� ����
                }
            }
        }

        if (isAutoSPRegen && (!isFreeze && !IsRun)) HandleSpRegen(); //�ڵ� ȸ��

        if ((currentState == PlayerState.Recovery || currentState == PlayerState.Resting) && Input.anyKeyDown)
        {
            // ȸ�� �� �Է��� ������ ȸ�� ���

            Debug.Log("ȸ�� �ߴܵ�");
            if (recoveryCoroutine != null)
                StopCoroutine(HandleGetUp()); // recoveryCoroutine ���� �ʿ�

            recoveryCoroutine = StartCoroutine(HandleGetUp());
            return;
        }
        HandleInputAndState();
        HandleFlashlight();
        HandleSpSpend();

        HandleMouseClick(); // Ŭ�� �� ���� ����
        PlayerAnimation();
    }
    private IEnumerator HandleGetUp()
    {
        if (currentState != PlayerState.Recovery && currentState != PlayerState.Resting)
            yield break;

        currentState = PlayerState.GettingUp;
        animator.SetTrigger("Rest_out");

        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Rest_out") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f
        );

        isRecovering = false;
        EndRecovery(); // ���� ����
    }

    public void OnPickUpStart(bool isPickup)
    {
        if (isActing) return;

        isActing = true;
        isPicking = isPickup;
        isMoveAble = false;

        //�ִϸ��̼� ����
        animator.SetTrigger("Pickup");
    }

    public void OnPickupOrDropAnimationEvent()
    {
        if (isPicking)
        {
            //HandleItemPickup();
            player_Item_Use.PickUpItem();
        }
        else
        {
            //HandleItemDrop();
            player_Item_Use.DropItem();
        }
    }
    public void OnPickUpFinished()
    {
        isActing = false;
        isMoveAble = true;
        isPicking = false;
    }
    void HandleMouseClick()
    {
        if (isRecovering) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mouseWorldPos - transform.position;

            float absX = Mathf.Abs(direction.x);
            float absY = Mathf.Abs(direction.y);

            if (absX > absY)
                clickLookDirection = new Vector2(direction.x > 0 ? 1 : -1, 0);
            else
                clickLookDirection = new Vector2(0, direction.y > 0 ? 1 : -1);

            animator.SetFloat("MouseDirX", direction.x);
            animator.SetFloat("MouseDirY", direction.y);

            clickLookTimer = clickLookDuration; // Ÿ�̸� �ʱ�ȭ
        }
    }

    //���� ������ ����ϴ��� ����
    public void SetUseItem(bool isUseItem)
    {
        if (isUseItem)
        {
            isMoveAble = false;
        }
        else
        {
            isMoveAble = true;
        }
    }

    private void UpdateItemCooldown()
    {
        if (Player_Usage_cu_cool_down > 0f)
        {
            Player_Usage_cu_cool_down -= Time.deltaTime;
            if (Player_Usage_cu_cool_down <= 0f)
            {
                Player_Usage_cu_cool_down = 0f;
                isUseItem = false;
            }
        }
    }

    public void SetUseItemCooltime(float coolTime)
    {
        Player_Usage_cu_cool_down = coolTime;
        currentItemUseCoroutine = StartCoroutine(EndItemUseAfterDelay(coolTime));
    }

    private IEnumerator EndItemUseAfterDelay(float delay)
    {
        Debug.Log("������ ����߽��ϴ�.");
        SetUseItem(true);
        yield return new WaitForSeconds(delay);
        SetUseItem(false);
        currentItemUseCoroutine = null;
    }

    private void HandleInputAndState()
    {
        if (isRecovering || currentState == PlayerState.Recovery) return;

        PlayerInput();

        if (isMoveAble && isMoving)
        {
            if ((currentSp > 0) && isRun)
            {
                currentState = PlayerState.Run;
                currentMoveSpeed = runSpeed;
            }
            else
            {
                currentState = PlayerState.Walk;
                currentMoveSpeed = moveSpeed;
            }
        }
        else
        {
            currentState = PlayerState.Idle;
            isRun = false;
        }
    }


    private void HandleFlashlight()
    {
        if (flashLightObject != null)
        {
            if (isRecovering) flashLightObject.SetActive(false);
            //else if (flashLightLevel < 3 && isRun && !isEmptySP) flashLightObject.SetActive(false);
            else flashLightObject.SetActive(isHavingFlashLight);
            SetflashLightPosition();
        }
        ;
    }

    private void UpdateFlashLight()
    {
        Light2D lightCircleLight = lightCircleObject.GetComponent<Light2D>();
        Light2D flashLight = flashLightObject.GetComponent<Light2D>();

        if (flashLightLevel == 1)
        {
            lightCircleLight.intensity = 0.05f;
            lightCircleLight.pointLightOuterRadius = 3;
            lightCircleLight.falloffIntensity = 0f;

            flashLight.intensity = 0.5f;
            flashLight.pointLightOuterRadius = 10;
            flashLight.falloffIntensity = 0.3f;
        }
        else if (flashLightLevel == 2)
        {
            lightCircleLight.intensity = 0.5f;
            lightCircleLight.pointLightOuterRadius = 3;
            lightCircleLight.falloffIntensity = 0f;

            flashLight.intensity = 1;
            flashLight.pointLightOuterRadius = 13;
            flashLight.falloffIntensity = 0.2f;
        }
        else if (flashLightLevel == 3)
        {
            lightCircleLight.intensity = 0.5f;
            lightCircleLight.pointLightOuterRadius = 3;
            lightCircleLight.falloffIntensity = 0f;

            flashLight.intensity = 1.5f;
            flashLight.pointLightOuterRadius = 16;
            flashLight.falloffIntensity = 0.1f;
        }
    }

    private void FixedUpdate()
    {
        if (isMoveAble && !isFreeze) Move();

        //LookMousePlayer();
    }

    void PlayerInput()
    {
        if (!isMoveAble || isUseItem)
        {
            HandleBlockedInput();
            return;
        }

        HandleMovementInput();
        HandleRunInput();

        if (Input.GetKeyDown(KeyCode.C)) DoRecovery();
    }
    void HandleBlockedInput()
    {
        x = 0;
        y = 0;
        isMoving = false;
    }
    void HandleMovementInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");

        isMoving = x != 0 || y != 0;
        if (isMoving)
        {
            lastMoveDirection = new Vector2(x, y);
        }
    }
    void HandleRunInput()
    {
        if ((currentSp > runThreshold))
        {
            isEmptySP = false;
        }
        if (currentSp <= 0)
        {
            isEmptySP = true;
            isRun = false;
        }
        else if(!isEmptySP)
        {
            isRun = Input.GetKey(KeyCode.LeftShift) && isMoving;
        }
    }

    void LookMousePlayer()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        aimDir = (transform.position - mousePosition).normalized;
        angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        fieldOfView.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    void SetflashLightPosition()
    {
        if (!isMoveAble || flashLightObject == null) return;

        Vector2 boxSize = new Vector2(0.2f, 0.2f);
        Vector3 dir = lastMoveDirection.normalized;

        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxSize, 0f, dir, flashLightDistance, LayerMask.GetMask("Wall"));
        Debug.DrawRay(transform.position, dir * flashLightDistance, Color.red);

        Vector3 targetPosition;
        float offsetFromWall = 0.1f;

        if (hit.collider != null)
            targetPosition = (Vector3)hit.point - dir * offsetFromWall;
        else
            targetPosition = transform.position + dir * flashLightDistance;

        // ��/�Ʒ� ������ ���� ��� �̵�
        bool isVertical = Mathf.Abs(dir.y) > Mathf.Abs(dir.x);

        if (isVertical)
        {
            flashLightObject.transform.position = targetPosition;
        }
        else
        {
            float flashlightSmoothSpeed = 15f;
            flashLightObject.transform.position = Vector3.Lerp(
                flashLightObject.transform.position,
                targetPosition,
                Time.deltaTime * flashlightSmoothSpeed
            );
        }
    }

    bool CanMove(Vector3 moveDir)
    {
        Vector2 boxSize = new Vector2(0.2f, 0.2f);
        float totalDistance = flashLightDistance;

        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxSize, 0f, moveDir, totalDistance, LayerMask.GetMask("Wall"));
        return hit.collider == null;
    }

    void TraceflashLightPosition()
    {
        if (flashLightObject == null) return;

        // ���콺 ��ġ �� ���� ��ǥ
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector3 dir = (mouseWorldPos - transform.position).normalized;

        // Raycast ���
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, flashLightDistance, LayerMask.GetMask("Wall"));

        Vector3 targetPosition;

        float offsetFromWall = 0.1f;

        if (hit.collider != null)
        {
            // ���� ��Ҵٸ�, �浹 ��������
            targetPosition = (Vector3)hit.point - dir * offsetFromWall;
        }
        else
        {
            // ���� �� ������, �÷��̾� ���� �ִ� �Ÿ�������
            float distance = Vector3.Distance(transform.position, mouseWorldPos);
            float clampedDistance = Mathf.Min(distance, flashLightDistance);
            targetPosition = transform.position + dir * clampedDistance;
        }

        targetPosition.z = 0f; // Z ����
        flashLightObject.transform.position = targetPosition;
    }

    void Move()
    {
        Vector3 moveDir = new Vector3(x, y, 0).normalized;
        if (moveDir != Vector3.zero && CanMove(moveDir))
        {
            UpdateMoveSpeedByWeight(); // �߰�
            transform.position += moveDir * currentMoveSpeed * speedMultiplier * Time.fixedDeltaTime;

            if (nearestItemFinder != null && GameManager.Instance != null && GameManager.Instance.playerData.isFindNearestItem)
                nearestItemFinder.FindNearestItem();
        }
    }
    void UpdateMoveSpeedByWeight()
    {
        if (player_Item_Use != null)
        {
            float currentWeight = player_Item_Use.GetTotalItemWeight();
            float penalty = currentWeight * 0.02f;

            if (PassiveItemManager.Instance != null && PassiveItemManager.Instance.HasEffect("Soul_Add_1_1"))//õ����� ������
            {
                penalty = 0f;
            }
            currentMoveSpeed = Mathf.Max(currentMoveSpeed - penalty, minMoveSpeed);
        }
    }
    void PlayerAnimation()
    {
        Vector2 direction;

        // if(Moving)�տ� �� �κ� �߰�
        if (clickLookTimer > 0f)
        {
            direction = clickLookDirection;
        }
        else if (isMoving)
        {
            direction = new Vector2(x, y);
            lastMoveDirection = direction;
        }
        else
        {
            direction = lastMoveDirection;
        }

        animator.SetFloat("DirectionX", direction.x);
        animator.SetFloat("DirectionY", direction.y);

        if (direction.x != 0)
            spriteRenderer.flipX = direction.x > 0;


        switch (currentState)
        {
            case PlayerState.Run:
                if (SoundManager.Instance != null) SoundManager.Instance.StopSFX(Resources.Load<AudioClip>("SFX/sfx_slow_move"));
                if (SoundManager.Instance != null) SoundManager.Instance.Play_stop_ok_SFX(Resources.Load<AudioClip>("SFX/sfx_run_move"));
                animator.SetBool("isWalk", false);
                animator.SetBool("isRun", true);
                break;
            case PlayerState.Idle:
                if (SoundManager.Instance != null) SoundManager.Instance.StopSFX(Resources.Load<AudioClip>("SFX/sfx_slow_move"));
                if (SoundManager.Instance != null) SoundManager.Instance.StopSFX(Resources.Load<AudioClip>("SFX/sfx_run_move"));
                animator.SetBool("isWalk", false);
                animator.SetBool("isRun", false);
                break;
            case PlayerState.Walk:
                if (SoundManager.Instance != null) SoundManager.Instance.StopSFX(Resources.Load<AudioClip>("SFX/sfx_run_move"));
                if (SoundManager.Instance != null) SoundManager.Instance.Play_stop_ok_SFX(Resources.Load<AudioClip>("SFX/sfx_slow_move"));
                animator.SetBool("isWalk", true);
                animator.SetBool("isRun", false);
                break;
            case PlayerState.Recovery:
                if (SoundManager.Instance != null) SoundManager.Instance.StopSFX(Resources.Load<AudioClip>("SFX/sfx_slow_move"));
                if (SoundManager.Instance != null) SoundManager.Instance.StopSFX(Resources.Load<AudioClip>("SFX/sfx_run_move"));
                animator.SetBool("isRun", false);
                animator.SetBool("isWalk", false);
                break;
        }
    }

    public void DoRecovery()
    {
        //if (currentState != PlayerState.Recovery && !isRecovering)
        if (currentState == PlayerState.Idle || currentState == PlayerState.Walk || currentState == PlayerState.Run)
        {
            isRun = false;
            currentState = PlayerState.Recovery;
            if (SoundManager.Instance != null) SoundManager.Instance.Play_stop_ok_SFX(Resources.Load<AudioClip>("SFX/sfx_player_breathing"));
            if (SoundManager.Instance != null) SoundManager.Instance.PauseBGM();
            StartCoroutine(RecoverOverTime());
        }
    }

    private void EndRecovery()
    {
        isMoveAble = true;
        isRecovering = false;
        if (SoundManager.Instance != null) SoundManager.Instance.StopSFX(Resources.Load<AudioClip>("SFX/sfx_player_breathing"));
        if (SoundManager.Instance != null) SoundManager.Instance.UnPause();
        currentState = PlayerState.Idle;
    }

    void FreezingCancle()
    {
        //a �Ǵ� D ��ư�� ���� 
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            //���� �ö�
            inputKey++;
        }


        if (inputKey >= 10)
        {
            isFreeze = false;
            inputKey = 0;
        }
    }
    private IEnumerator RecoverOverTime()
    {
        isRecovering = true;
        isMoveAble = false;

        currentState = PlayerState.Recovery;
        animator.SetTrigger("Rest_in");

        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Rest_in") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f
        );

        animator.SetTrigger("Resting");
        currentState = PlayerState.Resting;
        yield return null;

        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Resting") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f
        );

        float totalMaxHp = maxHp + extraHp;
        float hpPerSecond = totalMaxHp / hpRecoveryDuration;
        //float mpPerSecond = maxMp / mpRecoveryDuration;

        while ((currentHp + currentExtraHp < totalMaxHp || currentSp < maxSp) && isRecovering)
        {
            float delta = Time.deltaTime;

            float totalHp = currentHp + currentExtraHp;
            totalHp += hpPerSecond * delta;
            totalHp = Mathf.Min(totalHp, totalMaxHp);


            if (totalHp <= maxHp)
            {
                currentHp = totalHp;
                currentExtraHp = 0;
            }
            else
            {
                currentHp = maxHp;
                currentExtraHp = totalHp - maxHp;
            }

            yield return null;
        }

        yield return new WaitUntil(() => Input.anyKeyDown);
        StartCoroutine(HandleGetUp());
    }

    #region MP
    public void SpendMp(float value)
    {
        //ü�� ���� �� �߰�������
        float hpRatio = currentHp / maxHp;
        float damageMultiplier = Mathf.Lerp(1, 2, 1 - hpRatio);
        currentMp -= value * damageMultiplier;

        if (currentMp <= 0)
        {
            currentMp = 0;
            SpendAllMp();
        }
    }

    public void SpendAllMp()
    {
        Debug.Log("Spend All Mp");
    }

    #endregion
    public void DamagedHP(float value)
    {
        float totalDamage = value;
        if (PassiveItemManager.Instance != null && PassiveItemManager.Instance.HasEffect("Soul_Add_4_1"))
        {
            totalDamage *= GameManager.Instance.playerData.damageTakenMultiplier;
        }

        Effect_cr("e_at", transform.position, 0);//����Ʈ

        if (currentExtraHp > 0)
        {
            float damageToExtra = Mathf.Min(currentExtraHp, totalDamage);
            currentExtraHp -= damageToExtra;
            totalDamage -= damageToExtra;
        }

        if (totalDamage > 0)
        {
            currentHp = Mathf.Max(currentHp - totalDamage, 0);
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_player_hit"));
        }

        if (currentHp <= 0)
        {
            Die();
        }
    }

    public void DamagedMP(float value)
    {
        //ü�� ���� �� �߰�������
        float hpRatio = currentHp / maxHp;
        float damageMultiplier = Mathf.Lerp(1, 2, 1 - hpRatio);

        currentMp -= value * damageMultiplier;

        if (currentMp <= 0)
        {
            currentMp = 0;
            Die();
        }
    }
    public void Die()
    {
        //�׾��� �� �ൿ
        Debug.Log("Player Die..");
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_player_die"));
        StartCoroutine(ReviveRoutine(Vector3.zero));
    }
    void Effect_cr(string ty, Vector3 basePos, float offset)
    {

        float offsetX = Random.Range(-offset, offset);
        float offsetY = Random.Range(-offset, offset);

        // ���� ��ġ
        Vector3 spawnPos = basePos + new Vector3(offsetX, offsetY, 0f);

        // ����Ʈ ����
        GameObject effectObj = Instantiate(Effect_pr, spawnPos, Quaternion.identity);

        // Effect_sc�� ty �� ����
        Effect_sc effectScript = effectObj.GetComponent<Effect_sc>();
        effectScript.ty = ty;
    }
    IEnumerator ReviveRoutine(Vector3 revivePosition)
    {
        Instantiate(corpse, gameObject.transform.position, Quaternion.identity);

        //�����ϻ� Ȱ��ȭ ��
        if (GameManager.Instance != null && !GameManager.Instance.playerData.isDropWhenRevive)
            player_Item_Use.Drop_All_Item();

        yield return new WaitForSeconds(0.1f);

        if (placeManager.resurrection) // ��Ȱ�� �����ϴٸ�
        {
            Revive();
            placeManager.resurrection = false;
        }
        else
        {
            placeManager.Go_to_escape(); //��� �� �� �̵�
        }
    }

    public void Revive()
    {
        //��ü ���� �ڵ� �ʿ�
        SetPosition(placeManager.resurrection_pos);// ��Ȱ ��ҷ� ���� �̵� �ϱ�
        currentHp = maxHp;
        currentMp = maxMp;
        currentSp = maxSp;
        isFreeze = false;
    }

    public void SetPosition(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }

    public Vector3? GetNearestItemDir()
    {
        if (nearestItemFinder != null && nearestItemFinder.nearestItem != null)
            return nearestItemFinder.nearestItem.position;

        return null;
    }

    private void HandleDropItem()
    {
        if (PassiveItemManager.Instance != null)
        {
            if (PassiveItemManager.Instance.HasEffect("Soul_Add_5_2"))
            {
                speedMultiplier = GameManager.Instance.playerData.speedMultiplier;
            }

        }
    }

    private void HandlePickupItem()
    {
        if (PassiveItemManager.Instance != null)
        {
            if (PassiveItemManager.Instance.HasEffect("Soul_Add_5_2"))
            {
                speedMultiplier = GameManager.Instance.playerData.speedMultiplier;
            }
        }
    }

    private void HadleTimeAngleUnit18()
    {
        if (PassiveItemManager.Instance != null)
        {
            if (PassiveItemManager.Instance.HasEffect("Soul_Add_7_2"))
            {
                speedMultiplier = GameManager.Instance.playerData.speedMultiplier;
            }
        }
    }
}
