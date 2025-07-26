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

    [SerializeField] private float hpRecoveryDuration = 20f;
    [SerializeField] private float mpRecoveryDuration = 10f;
    [SerializeField] private float spRecoveryDuration = 10f;

    const float runThreshold = 1f; //달리기에 필요한 최소 sp

    public float attackDamage = 1;
    public float attackCoolTime;

    //public float originalMaxHp = 100;
    public float extraHp;
    public float currentExtraHp;

    public float maxHp = 100; //최대 체력
    public float maxMp = 100; //최대 정신력
    public float maxSp = 100; //최대 기력

    public float currentHp; //현재 체력
    public float currentMp; //현재 정신력
    public float currentSp; //현재 기력

    public bool isFreeze;
    //입력한 키가 몇번인지 확인
    int inputKey = 0;
    [SerializeField] private float minMoveSpeed = 0.4f;  //변수 추가

    public bool isSpendingSp = false;
    public bool isRecovering = false;

    public bool isPickUpableItem = false;   //아이템 주울 수 있는지 여부
    public bool isHavingFlashLight = false; //손전등 획득 유무

    public int flashLightLevel = 1;
    public GameObject flashLightObject;
    public GameObject lightCircleObject;
    public float flashLightDistance = 3f;

    public bool isUseItem = false;
    public float Player_Usage_cu_cool_down = 0;//플레이어 아이템 현재 쿨다운
    private Coroutine currentItemUseCoroutine = null;

    private float spSpendTimer = 0f;
    [SerializeField] private float spSpendThreshold = 1f;
    [SerializeField] private int spSpendAmount = 1;

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
    private float clickLookDuration = 0.2f; // 클릭 후 0.2초간 해당 방향으로 고정
    private Camera mainCamera;

    public bool IsRun => isRun;

    Player_Item_Use player_Item_Use;
    public GameObject corpse; // 플레이어 시체

    private bool isActing = false; //줍기, 버리기 애니메이션 진행 중 여부
    private bool isPicking = false; //줍기, 버리기 어떤 걸 실행할 지 
    public bool IsPicking { get => isPicking; set => isPicking = value; }

    private Coroutine recoveryCoroutine;

    private NearestItemFinder nearestItemFinder; //가까운 아이템 탐지

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
        Init();
        animator = GetComponent<Animator>();
        placeManager = FindObjectOfType<PlaceManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player_Item_Use = GetComponent<Player_Item_Use>();
        nearestItemFinder = GetComponent<NearestItemFinder>();
        mainCamera = Camera.main;
    }

    public void Init()
    {
        if(GameManager.Instance != null)
        {
            if (GameManager.Instance.playerData != null)
            {
                PlayerData playerData = GameManager.Instance.playerData;
                extraHp = playerData.extraHp;
                currentExtraHp = extraHp;

                currentHp = playerData.maxHp;

                currentMp = playerData.maxMp;
                currentSp = playerData.maxSp;

                flashLightLevel = playerData.flashLightLevel;
                UpdateFlashLight();
            }
        }
        else
        {
            currentHp = maxHp;
            currentExtraHp = extraHp;

            currentMp = maxMp;
            currentSp = maxSp;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (isFreeze)
        {
            FreezingCancle();
        }
        UpdateItemCooldown();

        if (!isFreeze && !IsRun) HandleSpRegen(); //자동 회복

        if ((currentState == PlayerState.Recovery || currentState == PlayerState.Resting) && Input.anyKeyDown)
        {
            // 회복 중 입력이 들어오면 회복 취소

            Debug.Log("회복 중단됨");
            if (recoveryCoroutine != null)
                StopCoroutine(HandleGetUp()); // recoveryCoroutine 변수 필요

            recoveryCoroutine = StartCoroutine(HandleGetUp());
            return;
        }
        HandleInputAndState();
        HandleFlashlight();
        HandleSpSpend();

        HandleMouseClick(); // 클릭 시 방향 갱신
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
        EndRecovery(); // 상태 복구
    }

    public void OnPickUpStart(bool isPickup)
    {
        if (isActing) return;

        isActing = true;
        isPicking = isPickup;
        isMoveAble = false;

        //애니메이션 실행
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

    private void HandleSpSpend()
    {
        if (isRecovering) return;

        if (isMoving && isRun && currentSp > 0)
        {
            spSpendTimer += Time.deltaTime;

            if (spSpendTimer >= spSpendThreshold)
            {
                SpendSp(spSpendAmount); // SP 감소
                spSpendTimer = 0f;

                if (currentSp <= 0)
                {
                    currentSp = 0;
                    //EnterRecoveryMode();
                }
            }
        }
    }

    void HandleMouseClick()
    {
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

            clickLookTimer = clickLookDuration; // 타이머 초기화
        }
    }

    //유저 아이템 사용하는지 여부
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
        Debug.Log("아이템 사용했습니다.");
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
            if (currentSp > 0 && isRun)
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
            else if (flashLightLevel < 3 && isRun) flashLightObject.SetActive(false);
            else flashLightObject.SetActive(isHavingFlashLight);
            SetflashLightPosition();
        };
    }

    private void UpdateFlashLight()
    {
        Light2D lightCircleLight = lightCircleObject.GetComponent<Light2D>();
        Light2D flashLight = flashLightObject.GetComponent<Light2D>();

        if (flashLightLevel == 1)
        {
            lightCircleLight.intensity = 0.05f;
            lightCircleLight.pointLightOuterRadius = 3;
            lightCircleLight.falloffIntensity = 0.2f;

            flashLight.intensity = 0.5f;
            flashLight.pointLightOuterRadius = 9;
            flashLight.falloffIntensity = 0.5f;
        }
        else if (flashLightLevel == 2)
        {
            lightCircleLight.intensity = 0.5f;
            lightCircleLight.pointLightOuterRadius = 3;
            lightCircleLight.falloffIntensity = 0f;

            flashLight.intensity = 1;
            flashLight.pointLightOuterRadius = 12;
            flashLight.falloffIntensity = 0.5f;
        }
        else if (flashLightLevel == 3)
        {
            lightCircleLight.intensity = 0.5f;
            lightCircleLight.pointLightOuterRadius = 3;
            lightCircleLight.falloffIntensity = 0f;

            flashLight.intensity = 1.5f;
            flashLight.pointLightOuterRadius = 15;
            flashLight.falloffIntensity = 0.5f;
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
        if (currentSp < runThreshold)
        {
            isRun = false;
            return;
        }
        else if (isRun == false)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
                isRun = true;
        }
        else
        {
            isRun = Input.GetKey(KeyCode.LeftShift);
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

        // 위/아래 방향일 경우는 즉시 이동
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

        // 마우스 위치 → 월드 좌표
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector3 dir = (mouseWorldPos - transform.position).normalized;

        // Raycast 쏘기
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, flashLightDistance, LayerMask.GetMask("Wall"));

        Vector3 targetPosition;

        float offsetFromWall = 0.1f;

        if (hit.collider != null)
        {
            // 벽에 닿았다면, 충돌 지점까지
            targetPosition = (Vector3)hit.point - dir * offsetFromWall;
        }
        else
        {
            // 벽에 안 닿으면, 플레이어 기준 최대 거리까지만
            float distance = Vector3.Distance(transform.position, mouseWorldPos);
            float clampedDistance = Mathf.Min(distance, flashLightDistance);
            targetPosition = transform.position + dir * clampedDistance;
        }

        targetPosition.z = 0f; // Z 고정
        flashLightObject.transform.position = targetPosition;
    }

    void Move()
    {
        Vector3 moveDir = new Vector3(x, y, 0).normalized;
        if (moveDir != Vector3.zero && CanMove(moveDir))
        {
            UpdateMoveSpeedByWeight(); // 추가
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

            if (PassiveItemManager.Instance != null && PassiveItemManager.Instance.HasEffect("Soul_Add_1_1"))//천하장사 보유시
            {
                penalty = 0f;
            }
            currentMoveSpeed = Mathf.Max(currentMoveSpeed - penalty, minMoveSpeed);
        }
    }
    void PlayerAnimation()
    {
        Vector2 direction;

        // if(Moving)앞에 이 부분 추가
        if (clickLookTimer > 0f)
        {
            clickLookTimer -= Time.deltaTime;
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
        //a 또는 D 버튼을 눌러 
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            //값이 올라감
            inputKey++;
        }


        if (inputKey >= 10)
        {
            isFreeze = false;
            inputKey = 0;
        }
    }

    // 기력 자동회복
    private void HandleSpRegen()
    {
        if (currentSp >= maxSp) return;

        float baseRegenPerSecond = maxSp / spRecoveryDuration;
        float mpRatio = currentMp / maxMp;
        float mpMultiplier = Mathf.Lerp(0.1f, 1.0f, mpRatio);

        float speedMultiplier = isRecovering ? 1f : 0.5f;

        float regenRate = baseRegenPerSecond * mpMultiplier * speedMultiplier;

        currentSp += regenRate * Time.deltaTime;
        currentSp = Mathf.Min(currentSp, maxSp);
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
        float mpPerSecond = maxMp / mpRecoveryDuration;

        while ((currentHp + currentExtraHp < totalMaxHp || currentMp < maxMp || currentSp < maxSp) && isRecovering)
        {
            float delta = Time.deltaTime;

            float totalHp = currentHp + currentExtraHp;
            totalHp += hpPerSecond * delta;
            totalHp = Mathf.Min(totalHp, totalMaxHp);

            if(totalHp <= maxHp)
            {
                currentHp = totalHp;
                currentExtraHp = 0;
            }
            else
            {
                currentHp = maxHp;
                currentExtraHp = totalHp - maxHp;
            }
            //currentHp += hpPerSecond * delta;
            currentMp += mpPerSecond * delta;

            //currentHp = Mathf.Min(currentHp, maxHp);
            currentMp = Mathf.Min(currentMp, maxMp);

            yield return null;
        }


        yield return new WaitUntil(() => Input.anyKeyDown);

        StartCoroutine(HandleGetUp());

    }

    #region MP
    public void SpendMp(float value)
    {
        //체력 적을 때 추가데미지
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

    #region SP function


    public void SpendSp(float value)
    {
        currentSp -= value;
        currentSp = Mathf.Max(currentSp, 0f);
    }

    #endregion

    public void DamagedHP(float value)
    {
        //기력 적을 때 추가데미지
        float spRatio = currentSp / maxSp;
        float damageMultiplier = Mathf.Lerp(1, 2, 1 - spRatio);

        float totalDamage = value * damageMultiplier;

        if (currentExtraHp > 0)
        {
            float damageToExtra = Mathf.Min(currentExtraHp, totalDamage);
            currentExtraHp -= damageToExtra;
            value -= damageToExtra;
        }

        if (value > 0)
        {
            currentHp = Mathf.Max(currentHp - value, 0);
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_player_die"));
        }

        if (currentHp <= 0)
        {
            Die();
        }
    }

    public void DamagedMP(float value)
    {
        //체력 적을 때 추가데미지
        float hpRatio = currentHp / maxHp;
        float damageMultiplier = Mathf.Lerp(1, 2, 1 - hpRatio);

        currentMp -= value * damageMultiplier;

        if (currentMp <= 0)
        {
            currentMp = 0;
        }
    }

    public void DamagedSP(float value)
    {
        currentSp -= value;

        if (currentSp <= 0)
        {
            currentSp = 0;
        }
    }

    public void Die()
    {
        //죽었을 때 행동
        Debug.Log("Player Die..");
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_player_die"));
        StartCoroutine(ReviveRoutine(Vector3.zero));
    }

    IEnumerator ReviveRoutine(Vector3 revivePosition)
    {
        Instantiate(corpse, gameObject.transform.position, Quaternion.identity);

        //구사일생 활성화 시
        if (GameManager.Instance != null && !GameManager.Instance.playerData.isDropWhenRevive)
            player_Item_Use.Drop_All_Item();

        yield return new WaitForSeconds(0.1f);

        if (placeManager.resurrection) // 부활이 가능하다면
        {
            Revive();
            placeManager.resurrection = false;
        }
        else
        {
            placeManager.Go_to_escape(); //사망 후 씬 이동
        }
    }

    public void Revive()
    {
        //시체 생성 코드 필요
        SetPosition(placeManager.resurrection_pos);// 부활 장소로 순간 이동 하기
        currentHp = maxHp;
        currentMp = maxMp;
        currentSp = maxSp;
        isFreeze = false;
    }

    public void SetPosition(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }


}
