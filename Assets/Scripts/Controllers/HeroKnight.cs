using UnityEngine;
using System.Collections;

public class HeroKnight : DamagebleObject, IStatLevelSystemPlayer, IUiPlayer
{
    [Header("Player settings")]
    [SerializeField] private int coinAmount;
    [SerializeField] private CharacterStat  speed;
    [SerializeField] private float  wallSlideSpeed = -1.5f;
    [SerializeField] private float  jumpForceModifier;
    [SerializeField] private float  rollForceModifier;
    [SerializeField] private CharacterStat reliableCoins;
    [SerializeField] private CharacterStat maxJumpInFly;
    [SerializeField] private CharacterStat maxRollInFly;
    [SerializeField] private float timeBtwRoll = 1f;

    [Header("Fighting")]
    [SerializeField] private CharacterStat standartDamage;//6
    [SerializeField] private CharacterStat criticalDamagePercent;//1.5f
    [SerializeField] private CharacterStat criticalChancePercent;//20
    [SerializeField] private CharacterStat attackRange;//0.9f
    [SerializeField] private LayerMask damagebleLayerMask;
    [SerializeField] private float timeStun;
    [SerializeField] private float protectionAfterHitTime = 2.0f;
    [SerializeField] private float HitPushForce;

    [Header("Wall slide particles")]
    [SerializeField] private GameObject slideDust;

    [Header("Player statuses")]
    public bool IsGrounded = false;
    public bool IsRolling = false;
    public bool IsStunned = false;
    public bool IsBlocking = false;
    public bool IsAttacking = false;
    public bool IsDead = false;

    private Animator                animator;
    private Rigidbody2D             rb;
    private new AudioSource             audio;
    private Sensor_HeroKnight       groundSensor;
    private Sensor_HeroKnight       wallSensorR1;
    private Sensor_HeroKnight       wallSensorR2;
    private GameObject              slideDustCln;
    private Transform               attackPoint;
    private Transform               hitParticles;
    private Transform               deathParticles;

    private Vector3                 leftFlip = new Vector3(0, 180, 0);

    private int                     facingDirection = 1;
    private int                     currentAttack = 0;
    private int                     currentJump = 0;
    private int                     currentRoll = 0;
    private float                   timeSinceAttack = 0.0f;
    private float                   timeSinceRoll = 0.0f;
    private float                   timeSinceDamaged = 0.0f;
    private float                   sighPrevFrame;
    private float                   sighCurrFrame;
    private StatLevelSystem         statLevelSystem;
    private readonly float          attack1Duration = 0.4f;
    private readonly float          attack2Duration = 0.4f;
    private readonly float          attack3Duration = 0.5f;
    
    //public static HeroKnight Instance { get; private set; }

    private HeroStates State
    {
        get { return (HeroStates)animator.GetInteger("State"); }
        set { animator.SetInteger("State", (int)value); }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audio = GetComponent<AudioSource>();
        animator = transform.Find("HeroKnightGFX").GetComponent<Animator>();
        attackPoint = transform.Find("AttackPoint").GetComponent<Transform>();
        hitParticles = transform.Find("HitParticles").GetComponent<Transform>();
        deathParticles = transform.Find("DeathParticles").GetComponent<Transform>();
        groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        statLevelSystem = GetComponent<StatLevelSystem>();

        setStats();
    }

    protected void setStats()
    {
        CharacterStat[] stats =
        {
            MaxHealthPoints,
            speed,
            standartDamage,
            attackRange,
            criticalDamagePercent,
            criticalChancePercent,
            reliableCoins,
            maxJumpInFly,
            maxRollInFly
        };

        statLevelSystem.SetCharacterStats(stats);
    }

    private void FixedUpdate()
    {
        ChackGround();

        //Debug.Log(rb.velocity);
    }

    private void Update ()
    {
        timeSinceRoll += Time.deltaTime;
        timeSinceAttack += Time.deltaTime;
        timeSinceDamaged += Time.deltaTime;

        if (IsGrounded && !IsRolling && !IsBlocking && !IsAttacking && !IsDead)
        {
            State = HeroStates.idle;
        }
        if (Input.GetButton("Horizontal") && !IsRolling && !IsAttacking && !IsDead)
        {
            if (!IsGrounded && !IsBlocking && State != HeroStates.jump && wallSensorR1.State() && wallSensorR2.State())
                WallSlide();
            Run();
        }
        if (Input.GetButtonDown("Jump") && !IsRolling && currentJump <= maxJumpInFly.Value && !IsDead)
        {
            Jump();
        }
        if (Input.GetButton("Down") && groundSensor.Platform() != null && !IsRolling && !IsDead)
        {
            DownThroughPlatform();
        }
        if (Input.GetButtonDown("Shift") && !IsRolling && State != HeroStates.slide && ((timeSinceRoll > timeBtwRoll) || (currentRoll <= maxRollInFly.Value)) && !IsDead)
        {
            Roll();
        }
        if (Input.GetButton("Fire1") && timeSinceAttack > 0.25f && !IsAttacking && !IsRolling && !IsStunned && !IsDead)
        {
            Attack();
        }
        if (Input.GetButton("Fire2") && !IsAttacking && !IsRolling && !IsDead)
        {
            BlockIdle();
        } if (Input.GetButtonUp("Fire2")) IsBlocking = false;
    }

    private void Run()
    {
        if (IsGrounded)
            State = HeroStates.run;

        //Vector3 dir = transform.right * Input.GetAxis("Horizontal");
        //transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);
        //sprite.flipX = dir.x < 0.0f;
        //rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, rb.velocity.y);

        rb.velocity = IsBlocking ? 
            new Vector2(Input.GetAxis("Horizontal") * speed.Value / 4, rb.velocity.y) : 
            new Vector2(Input.GetAxis("Horizontal") * speed.Value, rb.velocity.y) ;

        Flip();
    }

    private void Flip()
    {
        sighCurrFrame = rb.velocity.x == 0 ? sighPrevFrame : Mathf.Sign(rb.velocity.x);
        if (sighCurrFrame != sighPrevFrame) 
        {
            facingDirection = (int)sighCurrFrame;
            if (!IsGrounded)
            {
                State = HeroStates.fall;
                wallSensorR1.Disable(0.2f);
                wallSensorR2.Disable(0.2f);
            }
            transform.rotation = Quaternion.Euler(rb.velocity.x < 0 ? leftFlip : Vector3.zero);
        }

        sighPrevFrame = sighCurrFrame;

        //if (Input.GetAxis("Horizontal") < 0)
        //{
        //    facingDirection = -1;
        //    transform.rotation = Quaternion.Euler(leftFlip);
        //}
        //else if (Input.GetAxis("Horizontal") > 0)
        //{
        //    facingDirection = 1;
        //    transform.rotation = Quaternion.Euler(Vector3.zero);
        //}
    }

    private void Jump()
    {
        currentJump++;
        IsBlocking = false;
        IsGrounded = false;
        State = HeroStates.jump;

        rb.velocity = new Vector2(rb.velocity.x, speed.Value * jumpForceModifier);
        groundSensor.Disable(0.2f);
    }

    private void DownThroughPlatform()
    {
        groundSensor.Platform().DisablePlatform();
    }

    private void Roll()
    {
        if (timeSinceRoll > timeBtwRoll)
            currentRoll = 0;

        currentRoll++;
        IsBlocking = false;
        IsRolling = true;
        State = HeroStates.roll;
        rb.velocity = new Vector2(facingDirection * (IsGrounded ? speed.Value * rollForceModifier : speed.Value * rollForceModifier / 1.2f), rb.velocity.y >= 0 ? rb.velocity.y : 0);
        timeSinceRoll = 0;
    }

    private void ChackGround()
    {
        animator.SetFloat("AirSpeedY", rb.velocity.y);


        if (groundSensor.State())
        {
            IsGrounded = true;
            currentJump = 0;
        } else
        {
            IsGrounded = false;
            if (rb.velocity.y < 0 && State != HeroStates.slide)
            {
                State = HeroStates.fall;
            }
        }
    }

    private void WallSlide()
    {
        State = HeroStates.slide;
        IsRolling = false;
        rb.velocity = new Vector2(0, wallSlideSpeed);



        //if (!IsGrounded && !IsAttacking && !IsBlocking && State != HeroStates.jump && wallSensorR1.State() && wallSensorR2.State())
        //{
        //    State = HeroStates.slide;
        //    IsRolling = false;
        //    rb.velocity = new Vector2(0, wallSlideSpeed);
        //}
        //if (!wallSensorR1.State() && !wallSensorR2.State() && !IsGrounded)
        //{
        //    State = HeroStates.fall;
        //    wallSensorR1.Disable(0.2f);
        //    wallSensorR2.Disable(0.2f);
        //}
    }

    private void Attack()
    {
        IsBlocking = false;
        IsAttacking = true;
        State = HeroStates.attack;
        currentAttack++;

        if (currentAttack > 3)
            currentAttack = 1;

        if (timeSinceAttack > 1.0f)
            currentAttack = 1;

        animator.SetTrigger("Attack" + currentAttack);
        timeSinceAttack = 0.0f;

        if (AttackEvent != null && AttackEventSounds != null)
        {
            audio.clip = AttackEventSounds[Random.Range(0, AttackEventSounds.Length)];
            AttackEvent.Invoke();
        }

        StartCoroutine(AttackAnimation(currentAttack));
    }
    IEnumerator AttackAnimation(int currAttack)
    {
        switch (currAttack)
        {
            case 1:
                rb.velocity = new Vector2(facingDirection * 3, rb.velocity.y);
                yield return new WaitForSeconds(attack1Duration);
                break;
            case 2:
                rb.velocity = new Vector2(facingDirection * 3, rb.velocity.y);
                yield return new WaitForSeconds(attack2Duration);
                break;
            case 3:
                rb.velocity = new Vector2(facingDirection * 6, rb.velocity.y);
                yield return new WaitForSeconds(attack3Duration);
                break;
        }
        IsAttacking = false;
    }
    public void OnHeroAttack()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.Find("AttackPoint").position, attackRange.Value, damagebleLayerMask);
        
        foreach (Collider2D enemy in enemies)
        {
            if (Random.Range(0, 100) < criticalChancePercent.Value)
                enemy.GetComponent<DamagebleObject>().GetDamage(standartDamage.Value * (criticalDamagePercent.Value / 100), facingDirection);
            else
                enemy.GetComponent<DamagebleObject>().GetDamage(standartDamage.Value, facingDirection);
        }
    }

    private void BlockIdle()
    {
        IsAttacking = false;
        IsBlocking = true;
        State = HeroStates.block;
    }

    public override void GetDamage(float damage, float hitDirection)
    {
        if (IsBlocking && facingDirection != hitDirection)
        {
            rb.velocity = new Vector2(HitPushForce / 2 * hitDirection, HitPushForce / 2);
            animator.SetTrigger("Block");

            if (BlockEvent != null && BlockEventSounds != null)
            {
                audio.clip = BlockEventSounds[Random.Range(0, BlockEventSounds.Length)];
                BlockEvent.Invoke();
            }
        }
        else if (timeSinceDamaged > protectionAfterHitTime && !IsRolling)
        {
            CurrentHealthPoints = Mathf.Clamp(CurrentHealthPoints - damage, 0, MaxHealthPoints.Value);
            if (CurrentHealthPoints > 0)
            {
                rb.velocity = new Vector2(HitPushForce * hitDirection, HitPushForce);
                animator.SetTrigger("Hurt");

                if (HitEvent != null && HitEventSounds != null)
                {
                    audio.clip = HitEventSounds[Random.Range(0, HitEventSounds.Length)];
                    hitParticles.rotation = Quaternion.Euler(hitDirection > 0 ? new Vector3(0,120,0) : new Vector3(0, 240, 0));
                    HitEvent.Invoke();
                }  
            }
            else 
            {
                rb.velocity = new Vector2(HitPushForce * 3 * hitDirection, HitPushForce * 3);
                Death();

                if (DeathEvent != null && DeathEventSounds != null)
                {
                    audio.clip = DeathEventSounds[Random.Range(0, DeathEventSounds.Length)];
                    deathParticles.rotation = Quaternion.Euler(hitDirection > 0 ? new Vector3(0, 120, 0) : new Vector3(0, 240, 0));
                    DeathEvent.Invoke();
                }
            }

            GlobalEventManager.SendHealth();
            timeSinceDamaged = 0;
        }
    }

    public override void GetHeal(float Heal)
    {
        CurrentHealthPoints = Mathf.Clamp(CurrentHealthPoints + Heal, 0, MaxHealthPoints.Value);

        GlobalEventManager.SendHealth();
    }

    public override void Death()
    {
        IsDead = true;
        State = HeroStates.dead;
        gameObject.layer = 10;
        gameObject.tag = "Decoration";
        GlobalEventManager.SendHeroDeath();
    }

    public void AE_SlideDust()
    {
        slideDustCln = Instantiate(slideDust, transform);
        slideDustCln.transform.parent = null;
    }
    public void AE_ResetRoll()
    {
        IsRolling = false;
    }

    public void AddCoins(int coins)
    {
        coinAmount += coins;
        GlobalEventManager.SendCoins();
    }

    public void GoldAmountToRelibleGold()
    {
        coinAmount = (int)Mathf.Round(coinAmount * (reliableCoins.Value / 100));
    }

    public bool TryBoughtStatUp(int cost)
    {
        if (cost <= coinAmount)
        {
            coinAmount -= cost;
            GlobalEventManager.SendCoins();
            return true;
        }
        else return false;
    }

    public int GetCoinAmount()
    {
        return coinAmount;
    }

    public float GetMaxHealthPoints()
    {
        return MaxHealthPoints.Value;
    }

    public float GetCurrentHealthPoints()
    {
        return CurrentHealthPoints;
    }

    public void increaseCurrentHPWithMaxHP(float HealthPoints)
    {
        GetHeal(HealthPoints);
    }
}

public enum HeroStates
{
    idle,   //State 0
    run,    //State 1
    jump,   //State 2
    fall,   //State 3
    roll,   //State 4
    hurt,   //State 5
    block,  //State 6
    dead,   //State 7
    slide,  //State 8
    attack  //State 9
}