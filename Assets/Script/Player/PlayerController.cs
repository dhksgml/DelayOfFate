using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public Transform fieldOfView;

    public float moveSpeed = 4f;
    public float runSpeed = 7f;
    public float currentMoveSpeed;
    public float speedMultiplier = 1f;
    public float rayCastDistance = 2f;

    [SerializeField] private CapsuleCollider2D collider;
    [SerializeField] private float hpRecoveryDuration = 10f;

    const float runThreshold = 10f; //????? ????? ??? sp

    public float currentAttackDamage = 1f;
    public float attackDamageMultiplier = 1f;
    public float attackCoolTime;

    //public float originalMaxHp = 100;
    public float extraHp;
    public float currentExtraHp;

    public float maxHp = 100; //??? u??
    public float maxMp = 100; //??? ?????

    public float currentHp; //???? u??
    public float currentMp; //???? ?????

    public bool isFreeze;
    [HideInInspector] public float freezeTime;
    //????? ??? ??????? ???
    int inputKey = 0;
    [SerializeField] private float minMoveSpeed = 0.4f;  //???? ???

    public bool isRecovering = false;
    private bool isDie = false;

    public bool isPickUpableItem = false;   //?????? ??? ?? ????? ????
    public bool isHavingFlashLight = false; //?????? ??? ????

    [Header("Flashlight")]
    public int flashLightLevel = 1;
    public GameObject flashLightObject;
    public GameObject lightCircleObject;
    public float flashLightDistance = 3f;
    public float refillFlashlightCooltime = 0.5f; //호롱 회복 속도
    public float refillFlashlightAmount = 0.25f; //호롱 회복량

    private Light2D flashLight;
    //private float flashlightRadius;
    private float startRadius = 10f; //최대 시야 크기
    private float minRadius = 0f; //최소 시야 크기
    // 그슨대 때문에 public으로 돌려줌
    [HideInInspector] public float currentRadius; //현재 시야 크기
    private float decreaseRate = 0.15f; //시야 감소폭
    private bool isOn = true; //플래시라이트 상태
    private Coroutine refillCoroutine;

    [Header("ItemCooltime")]
    public float Player_Usage_cu_cool_down = 0;//?÷???? ?????? ???? ????
    private Coroutine currentItemUseCoroutine = null;
    private bool isUseItem = false;

    private float walkTimer = 0f;
    [SerializeField] private float walkThreshold = 1f;

    private Vector3 aimDir;
    private Vector3 mousePosition;
    private float angle;

    public bool isMoveAble { private get; set; } = true;
    private bool isRun = false;
    public PlayerState currentState = PlayerState.Idle;

    private float x;
    private float y;
    [HideInInspector] public bool isMoving;
    [HideInInspector] public Vector2 lastMoveDirection;

    SpriteRenderer spriteRenderer;
    Animator animator;
    PlaceManager placeManager;
    private Player_Item_p player_Item_P;
    Vector3 moveVelocity;
    private QuickSlotUI quickSlotUI;

    private Vector2 moveInput;
    private Vector2 clickLookDirection = Vector2.down;
    private float clickLookTimer = 0f;
    private float clickLookDuration = 0.3f; // ??? ?? 0.2??? ??? ???????? ????
    private Camera mainCamera;

    public GameObject Effect_pr;//????? ???????
    private float effectTimer = 0f; //????? ?????

    Player_Item_Use player_Item_Use;
    public GameObject corpse; // ?÷???? ??u

    private bool isActing = false; //???, ?????? ??????? ???? ?? ????
    private bool isPicking = false; //???, ?????? ?? ?? ?????? ?? 
    public bool IsPicking { get => isPicking; set => isPicking = value; }

    private Coroutine recoveryCoroutine;

    private NearestItemFinder nearestItemFinder; //????? ?????? ???
    public NearestItemFinder NearestItemFinder => nearestItemFinder;

    [SerializeField] private GameObject damageFX;

    public bool isKill2Heal;

    // 11.09 추가, 저주인형을 위함
    [HideInInspector] public bool isHealBan;
    [HideInInspector] public float healBanTime;

    public enum PlayerState
    {
        Idle,
        Walk,
        Run,
        Recovery,
        Resting,
        GettingUp,
        Dead
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        placeManager = FindObjectOfType<PlaceManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player_Item_Use = GetComponent<Player_Item_Use>();
        nearestItemFinder = GetComponent<NearestItemFinder>();
        player_Item_P = FindObjectOfType<Player_Item_p>();
        quickSlotUI = FindObjectOfType<QuickSlotUI>();
        mainCamera = Camera.main;

        lightCircleObject.SetActive(true);
        flashLightObject.SetActive(true);
        Init();
        lastMoveDirection = Vector2.down;
        if (flashLightObject)
        {
            flashLight = flashLightObject.GetComponent<Light2D>();
            //flashlightRadius = flashLight.pointLightOuterRadius;
            currentRadius = startRadius;
            flashLight.enabled = isOn;
            GameEvents.CallClickLenton(isOn);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnEnemyDie += HandleEnemyDie;
        GameEvents.OnPickupItem += HandlePickupItem;
        GameEvents.OnDropItem += HandleDropItem;
        GameEvents.OnTimeAngleUnit18 += HandleTimeAngleUnit18;
    }

    private void OnDisable()
    {
        GameEvents.OnEnemyDie -= HandleEnemyDie;
        GameEvents.OnPickupItem -= HandlePickupItem;
        GameEvents.OnDropItem -= HandleDropItem;
        GameEvents.OnTimeAngleUnit18 -= HandleTimeAngleUnit18;
    }

    private float CalculateDamageMulitplier()
    {
        float baseDamage = GameManager.Instance.playerData.damageMultiplier;
        float bonus = 0f;

        if (PassiveItemManager.Instance.HasEffect("Soul_Add_1_1"))
        {
            int heldItemCount = player_Item_Use.quickSlots.Length - player_Item_Use.CheckEmptySlotsCount();
            bonus += 0.1f * heldItemCount;
        }

        if (PassiveItemManager.Instance.HasEffect("Soul_Add_6_1"))
        {
            if (GameManager.Instance.Day >= 4)
                bonus += 0.3f;
        }

        //일취월장
        if(PassiveItemManager.Instance.HasEffect("Soul_Add_6_4"))
        {
            bonus += Mathf.Clamp(Mathf.FloorToInt(GameManager.Instance.Soul / 300), 0, 7) * 0.1f;
        }

        //무아지경
        if (PassiveItemManager.Instance.HasEffect("Soul_Add_7_3"))
        {
            if ((currentHp / maxHp) <= 0.5f)
                bonus += 0.5f;
        }

        // 다른 패시브들 계산
        return baseDamage * (1f + bonus);
    }

    private float CalculateSpeedMultiplier()
    {

        float baseSpeed = GameManager.Instance.playerData.speedMultiplier;
        float bonus = 0f;

        if (!isOn) bonus += 0.3f;

        if (PassiveItemManager.Instance.HasEffect("Soul_Add_3_2"))
            bonus += Mathf.Clamp(Mathf.FloorToInt(GameManager.Instance.Gold / 200), 0, 3) * 0.1f;

        if (PassiveItemManager.Instance.HasEffect("Soul_Add_5_2"))
            bonus += 0.1f * player_Item_Use.CheckEmptySlotsCount();

        if(PassiveItemManager.Instance.HasEffect("Soul_Add_6_1"))
        {
            if (GameManager.Instance.Day >= 4)
                bonus += 0.3f;
        }

        if (PassiveItemManager.Instance.HasEffect("Soul_Add_7_1"))
        {
            float healthRatio = currentHp / maxHp;

            if(healthRatio <= 0.3f)
                bonus += 0.3f;
        }
            

        if (PassiveItemManager.Instance.HasEffect("Soul_Add_7_2") && quickSlotUI.angleUnit >= 18)
            bonus += 0.5f;

        if (player_Item_P != null && player_Item_P.item_p_count != null)
        {
            if (player_Item_P.item_p[13]) { bonus += 0.3f; }
            bonus -= 0.1f * player_Item_P.item_p_count[14]; // 14번: -10% × 개수
            bonus += 0.1f * player_Item_P.item_p_count[17]; // 17번: +10% × 개수
        }

        // 다른 패시브들 계산
        return baseSpeed * (1f + bonus);
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
                maxHp = playerData.maxHp;
                currentHp = playerData.currentHp;
                currentMp = playerData.maxMp;
                flashLightLevel = playerData.flashLightLevel;
                UpdateFlashLight();

                // 이속 배율 계산
                UpdateSpeed();
                UpdateDamage();

                if (PassiveItemManager.Instance.HasEffect("Soul_Add_6_2"))
                    isKill2Heal = true;
            }
        }
        else
        {
            currentHp = maxHp;
            currentExtraHp = extraHp;
            currentMp = maxMp;
            speedMultiplier = 1.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (flashLightObject != null)
        {
            if (!PassiveItemManager.Instance.HasEffect("Soul_Add_5_3"))
            {
                HandleFlashlightInput();
                if (isOn)
                {
                    flashLightObject.GetComponent<CircleCollider2D>().enabled = true;
                    SpendBattery();
                    //flashLight.pointLightOuterRadius = currentRadius;
                    //flashLightObject.GetComponent<CircleCollider2D>().radius = currentRadius;
                }
                else
                {
                    flashLightObject.GetComponent<CircleCollider2D>().enabled = false;
                    RefillFlashlight(refillFlashlightAmount);
                }
            }
        }

        if (clickLookTimer > 0f)
        {
            clickLookTimer -= Time.deltaTime;
            clickLookTimer = Mathf.Clamp(clickLookTimer, 0f, clickLookDuration);
        }

        if (isHealBan)
        {
            healBanTime -= Time.deltaTime;

            if (healBanTime <= 0) { isHealBan = false; }

        }
        UpdateItemCooldown();

        if (currentState == PlayerState.Resting) //????? ????
        {
            if (currentHp + currentExtraHp < maxHp + extraHp)
            {
                effectTimer -= Time.deltaTime;
                if (effectTimer <= 0f)
                {
                    Effect_cr("hp", transform.position, -1f);
                    effectTimer = 0.25f; // ???? ??????? ?ð? ????
                }
            }
        }

        if ((currentState == PlayerState.Recovery || currentState == PlayerState.Resting) && Input.anyKeyDown)
        {
            // ??? ?? ????? ?????? ??? ???

            Debug.Log("??? ????");
            if (recoveryCoroutine != null)
                StopCoroutine(HandleGetUp()); // recoveryCoroutine ???? ???

            recoveryCoroutine = StartCoroutine(HandleGetUp());
            return;
        }
        HandleInputAndState();
        HandleFlashlight();

        //HandleMouseClick(); // ??? ?? ???? ????
        PlayerAnimation();

        
    }

    public float GetCurrentBattery()
    {
        return currentRadius / startRadius;
    }

    private void HandleFlashlightInput()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            isOn = !isOn;
            flashLight.enabled = isOn;
            GameEvents.CallClickLenton(isOn);
        }
    }

private void SpendBattery()
{
    if(currentRadius > minRadius)
    {
        currentRadius -= decreaseRate * Time.deltaTime;
        currentRadius = Mathf.Max(currentRadius, minRadius);
    }
    else
    {
        isOn = false;
        flashLight.enabled = false;
        GameEvents.CallClickLenton(isOn);
    }
    
    float batteryPercent = currentRadius / startRadius; // 0~1
    float actualRadius;
    
    if (batteryPercent <= 0)
    {
        actualRadius = 0; // 배터리 0%면 완전히 꺼짐
    }
    else
    {
        // 기본 50 + 배터리 비율에 따른 보너스 50
        actualRadius = startRadius * 0.5f + (startRadius * 0.5f * batteryPercent);
    }
    
    flashLight.pointLightOuterRadius = actualRadius;
    flashLightObject.GetComponent<CircleCollider2D>().radius = actualRadius / 2;
}

    public void RefillFlashlight(float amount)
    {
        if (refillCoroutine == null)
            refillCoroutine = StartCoroutine(RefillFlashlightRoutine(amount));
        
    }

    private IEnumerator RefillFlashlightRoutine(float amount)
    {
        currentRadius += amount;
        currentRadius = Mathf.Min(currentRadius, startRadius);
        yield return new WaitForSeconds(refillFlashlightCooltime);
        refillCoroutine = null;
    }

    private void DecreaseFlashlight(float amount)
    {
        if (!isOn) return;
        currentRadius -= amount;
        currentRadius = Mathf.Max(currentRadius, minRadius);
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
        EndRecovery(); // ???? ????
    }

    public void OnPickUpStart(bool isPickup)
    {
        if (isActing) return;

        isActing = true;
        isPicking = isPickup;
        isMoveAble = false;

        //??????? ????
        animator.SetTrigger("Pickup");
    }

    public void OnPickupOrDropAnimationEvent()
    {
        if (isPicking)
        {
            //HandleItemPickup();
            player_Item_Use.PickUpItem();
        }
        //else
        //{
        //    //HandleItemDrop();
        //    player_Item_Use.DropItem();
        //}
    }
    public void OnPickUpFinished()
    {
        isActing = false;
        isMoveAble = true;
        isPicking = false;
    }

    #region 마우스 방향 애니메이션
    /*
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

            clickLookTimer = clickLookDuration; // ???? ????
        }
    }
    */
    #endregion

    //???? ?????? ???????? ????
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
        SetUseItem(true);
        yield return new WaitForSeconds(delay);
        SetUseItem(false);
        currentItemUseCoroutine = null;
    }

    private void HandleInputAndState()
    {
        if (isDie || currentState == PlayerState.Dead) return;
        if (isRecovering || currentState == PlayerState.Recovery) return;

        PlayerInput();

        if (isMoveAble && isMoving)
        {
            if (isRun)
            {
                currentState = PlayerState.Run;
                currentMoveSpeed = runSpeed * speedMultiplier;
            }
            else
            {
                currentState = PlayerState.Walk;
                currentMoveSpeed = moveSpeed * speedMultiplier;
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
        // 처녀귀신 리메이크
        if (isFreeze) { freezeTime -= Time.deltaTime; }
        if (freezeTime <= 0) { isFreeze = false; }

        float baseSpeed = isRun ? runSpeed : moveSpeed;
        UpdateSpeed();
        currentMoveSpeed = baseSpeed * speedMultiplier;

        float baseDamage = 1f;
        UpdateDamage();
        currentAttackDamage = baseDamage * attackDamageMultiplier;

        if (isMoveAble && !isFreeze) Move();

        //LookMousePlayer();
    }

    void PlayerInput()
    {
        if (isDie) return;

        if (!isMoveAble || isUseItem)
        {
            HandleBlockedInput();
            return;
        }

        HandleMovementInput();
    }
    void HandleBlockedInput()
    {
        x = 0;
        y = 0;
        isMoving = false;
    }
    void HandleMovementInput()
    {
        // 방향키 입력만 받기
        x = 0f;
        y = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))  x = -1f;
        else if (Input.GetKey(KeyCode.RightArrow))  x = 1f;

        if (Input.GetKey(KeyCode.UpArrow)) y = 1f;
        else if (Input.GetKey(KeyCode.DownArrow)) y = -1f;

        isMoving = x != 0 || y != 0;

        if (isMoving)
        {
            lastMoveDirection = new Vector2(x, y);

            // 달리기 관련
            walkTimer = Mathf.Clamp(walkTimer + Time.deltaTime, 0, walkThreshold);
            if (walkTimer >= walkThreshold)
                isRun = true;
        }
        else
        {
            walkTimer = 0;
            isRun = false;
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

        // ??/??? ?????? ???? ??? ???
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

        // ???콺 ??? ?? ???? ???
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector3 dir = (mouseWorldPos - transform.position).normalized;

        // Raycast ???
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, flashLightDistance, LayerMask.GetMask("Wall"));

        Vector3 targetPosition;

        float offsetFromWall = 0.1f;

        if (hit.collider != null)
        {
            // ???? ?????, ?浹 ????????
            targetPosition = (Vector3)hit.point - dir * offsetFromWall;
        }
        else
        {
            // ???? ?? ??????, ?÷???? ???? ??? ?????????
            float distance = Vector3.Distance(transform.position, mouseWorldPos);
            float clampedDistance = Mathf.Min(distance, flashLightDistance);
            targetPosition = transform.position + dir * clampedDistance;
        }

        targetPosition.z = 0f; // Z ????
        flashLightObject.transform.position = targetPosition;
    }

    void Move()
    {
        if (isDie) return;

        Vector3 moveDir = new Vector3(x, y, 0).normalized;
        if (moveDir != Vector3.zero && CanMove(moveDir))
        {

            transform.position += moveDir * currentMoveSpeed * Time.fixedDeltaTime;

            if (nearestItemFinder != null && GameManager.Instance != null && GameManager.Instance.playerData.isFindNearestItem)
                nearestItemFinder.FindNearestItem();
        }
    }
    /*void UpdateMoveSpeedByWeight()
    {
        if (player_Item_Use != null)
        {
            float currentWeight = player_Item_Use.GetTotalItemWeight();
            float penalty = currentWeight * 0.02f;

            if (PassiveItemManager.Instance != null && PassiveItemManager.Instance.HasEffect("Soul_Add_1_1"))//o????? ??????
            {
                penalty = 0f;
            }
            currentMoveSpeed = Mathf.Max(currentMoveSpeed - penalty, minMoveSpeed);
        }
    }*/
    void PlayerAnimation()
    {
        Vector2 direction;

        // if(Moving)??? ?? ?κ? ???
        if (isMoving)
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
            case PlayerState.Dead:
                if (SoundManager.Instance != null) SoundManager.Instance.StopSFX(Resources.Load<AudioClip>("SFX/sfx_slow_move"));
                if (SoundManager.Instance != null) SoundManager.Instance.StopSFX(Resources.Load<AudioClip>("SFX/sfx_run_move"));
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

    // 처녀귀신 전버전
    //void FreezingCancle()
    //{
    //    //a ??? D ????? ???? 
    //    if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
    //    {
    //        //???? ?o?
    //        inputKey++;
    //    }


    //    if (inputKey >= 10)
    //    {
    //        isFreeze = false;
    //        inputKey = 0;
    //    }
    //}

    public void Hp_add(float healing)
    {
        //배수진
        if(PassiveItemManager.Instance.HasEffect("Soul_Add_7_4"))
        {
            float maxAllowedHp = maxHp * 0.5f;

            if (currentHp >= maxAllowedHp)
                return;

            currentHp += healing * Hp_add_magnification();

            if (currentHp > maxAllowedHp)
                currentHp = maxAllowedHp;

            return;
        }   
        
        currentHp += healing * Hp_add_magnification();

        // 최대 채력 넘어가면 방지
        if (currentHp >= maxHp)
        {
            currentHp = maxHp;
        }
    }
    public float Hp_add_magnification()
    {
        float bonus = 0f;

        if (player_Item_P != null && player_Item_P.item_p_count != null)
        {
            // 2번 아이템: 2할 증가 (중첩 가능)
            if (player_Item_P.item_p[2])
            {
                bonus += 0.2f * player_Item_P.item_p_count[2];
            }

            // 12번 아이템: 2할 감소 (중첩 가능)
            if (player_Item_P.item_p[12])
            {
                bonus -= 0.2f * player_Item_P.item_p_count[12];
            }
        }

        return 1.0f + bonus; // 기본 100% + 보너스
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
        float baseHpPerSecond = totalMaxHp / hpRecoveryDuration;
        float hpPerSecond = baseHpPerSecond ;
        //float mpPerSecond = maxMp / mpRecoveryDuration;

        while ((currentHp + currentExtraHp < totalMaxHp) && isRecovering)
        {
            float delta = Time.deltaTime;

            float totalHp = currentHp + currentExtraHp;
            totalHp += hpPerSecond * delta;
            totalHp = Mathf.Min(totalHp, totalMaxHp);
            if (!player_Item_P.item_p[2]) { }

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
        //u?? ???? ?? ?????????
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
        StartCoroutine(mainCamera.GetComponent<CameraShake>().Shake());
        float totalDamage = value;

        // 11번 아이템 체크 및 피해 감소
        if (player_Item_P != null && player_Item_P.item_p[11])
        {
            totalDamage *= 0.5f; // 피해량 절반

            // 인벤토리에서 11번 아이템 찾아서 제거 (낮은 인덱스부터)
            for (int i = 0; i < player_Item_Use.quickSlots.Length; i++)
            {
                Item item = player_Item_Use.quickSlots[i];
                if (item != null && item.id == 11)
                {
                    player_Item_Use.quickSlots[i] = null;
                    // 여기에 아이템 제거 코드 추가 예정
                    break; // 하나만 제거하고 중단
                }
            }
        }

        if (PassiveItemManager.Instance != null && PassiveItemManager.Instance.HasEffect("Soul_Add_4_1"))
        {
            totalDamage *= GameManager.Instance.playerData.damageTakenMultiplier;
        }

        Effect_cr("e_at", transform.position, 0);

        if (currentExtraHp > 0)
        {
            float damageToExtra = Mathf.Min(currentExtraHp, totalDamage);
            currentExtraHp -= damageToExtra;
            totalDamage -= damageToExtra;
        }

        if (totalDamage > 0)
        {
            currentHp = Mathf.Max(currentHp - totalDamage, 0);
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_player_hit"));
        }

        if (damageFX)
            Instantiate(damageFX, transform.position, Quaternion.identity);

        if (currentHp <= 0 && !isDie)
        {
            currentHp = 0;
            Die();
        }
    }

    public void DamagedMP(float value)
    {
        StartCoroutine(mainCamera.GetComponent<CameraShake>().Shake());

        //u?? ???? ?? ?????????
        float hpRatio = currentHp / maxHp;
        float damageMultiplier = Mathf.Lerp(1, 2, 1 - hpRatio);

        currentMp -= value * damageMultiplier;

        if (currentMp <= 0 && !isDie)
        {
            isDie = !isDie;
            currentMp = 0;
            Die();
        }
    }
    public void Die()
    {
        isMoveAble = false;
        isMoving = false;
        isDie = true;
        currentState = PlayerState.Dead;
        collider.enabled = false;
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_player_die"));
        StartCoroutine(DieAnimation());
    }

    public IEnumerator DieAnimation()
    {
        animator.SetTrigger("isDie");
        var cameraShake = mainCamera.GetComponent<CameraShake>();
        StartCoroutine(cameraShake.Shake());
        yield return new WaitUntil(() => 
            animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Rest_in") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        StartCoroutine(ReviveRoutine(Vector3.zero));
    }

    public IEnumerator ReviveAnimation()
    {
        animator.SetTrigger("isRevive");
        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Rest_out") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        Debug.Log("i am revive");
    }

    void Effect_cr(string ty, Vector3 basePos, float offset)
    {

        float offsetX = Random.Range(-offset, offset);
        float offsetY = Random.Range(-offset, offset);

        // ???? ???
        Vector3 spawnPos = basePos + new Vector3(offsetX, offsetY, 0f);

        // ????? ????
        GameObject effectObj = Instantiate(Effect_pr, spawnPos, Quaternion.identity);

        // Effect_sc?? ty ?? ????
        Effect_sc effectScript = effectObj.GetComponent<Effect_sc>();
        effectScript.ty = ty;
    }
    IEnumerator ReviveRoutine(Vector3 revivePosition)
    {
        //Instantiate(corpse, gameObject.transform.position, Quaternion.identity);

        //??????? ???? ??
        if (GameManager.Instance != null && !GameManager.Instance.playerData.isDropWhenRevive)
            player_Item_Use.Drop_All_Item();

        yield return new WaitForSeconds(0.1f);

        if (placeManager != null && placeManager.resurrection) // 부활
        {
            SetPosition(placeManager.resurrection_pos);
            placeManager.Resurrection();
            yield return StartCoroutine(ReviveAnimation());
            Revive();
            placeManager.Resurrection();
        }
        else
        {
            
            Debug.Log("I am die..");
            //placeManager.Go_to_escape(); //로비로
            SceneManager.LoadScene("Gameover_Scene");
        }
    }

    public void Revive()
    {
        currentHp = maxHp;
        currentMp = maxMp;
        isFreeze = false;
        collider.enabled = true;
        isDie = false;
        isMoving = true;
        isMoveAble = true;
        currentState = PlayerState.Idle;

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
        UpdateSpeed();
    }

    private void HandlePickupItem()
    {
        UpdateSpeed();
    }

    private void HandleTimeAngleUnit18()
    {
        UpdateSpeed();
    }

    private void HandleEnemyDie()
    {
        if(PassiveItemManager.Instance.HasEffect("Soul_Add_6_2"))
        {
            //힐
            currentHp = Mathf.Clamp(currentHp + 5, 0, maxHp);
            currentMp = Mathf.Clamp(currentMp + 3, 0, maxMp);
        }
    }

    #region 아이템 패시브 관련

    public void AddMoveSpeedMultiplier(float amount)
    {
        speedMultiplier += amount;
    }

    #endregion

    private void UpdateSpeed()
    {
        speedMultiplier = CalculateSpeedMultiplier();
    }

    private void UpdateDamage()
    {
        attackDamageMultiplier = CalculateDamageMulitplier();
    }
}
