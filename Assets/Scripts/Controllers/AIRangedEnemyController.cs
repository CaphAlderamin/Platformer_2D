using System.Collections;
using System;
using UnityEngine;
using Pathfinding;

public class AIRangedEnemyController : DamagebleObject
{
    public static AIRangedEnemyController Instance { get; private set; }

    [Header("Pathfinding")]
    //[SerializeField] private float angryDistance = 8f;
    [SerializeField] private float nextWaypointDistance = 2f;
    [SerializeField] private float minDistanceToTarget = 2f;
    [SerializeField] private float pathUpdateSeconds = 0.5f;

    [Header("Physics")]
    [SerializeField] private float speed = 200f;
    [SerializeField] private float jumpNodeHeightRequirement = 0.8f;
    [SerializeField] private float jumpModifier = 0.3f;
    [SerializeField] private float rollForceModifier = 0.3f;

    [Header("Fighting")]
    [SerializeField] private float angryTime;
    [SerializeField] private float standartDamage;
    [SerializeField] private float attackRange;
    [SerializeField] private float attack1Duration;
    [SerializeField] private float timeAttackChill;
    [SerializeField] private int maxStunCount;
    [SerializeField] private float stunProtectionTime;
    [SerializeField] private float timeStun;
    [SerializeField] private float hitPushForce;
    [SerializeField] private GameObject[] Projectiles;
    [SerializeField] private float timeBtwRoll;
    [SerializeField] private float rollBackDistance;

    [Header("Materials")]
    [SerializeField] private Material originalMaterial;
    [SerializeField] private Material onHitMaterial;

    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float waitOnPatrolPointTime;

    [Header("Status")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool IsStunProtected;
    [SerializeField] private bool IsStunned;
    [SerializeField] private bool IsDead;
    [SerializeField] private bool IsAttacking;
    [SerializeField] private bool IsAttackChilling;
    [SerializeField] private bool IsIdleing;
    [SerializeField] private bool IsRolling;

    [Header("Custom Behaviors")]
    [SerializeField] private bool followEnabled = true;
    [SerializeField] private bool patrolEnabled = true;
    [SerializeField] private bool jumpEnabled = true;
    [SerializeField] private bool directionLookEnabled = true;
    [SerializeField] private bool attackEnabled = true;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator animator;
    new AudioSource audio;
    FieldOfView fov;
    Sensor_HeroKnight groundSensor;
    Transform player;
    Transform target;
    Transform attackPoint;
    Seeker seeker;
    Path path;

    float timeSinceRoll;
    
    int currentWaypoint = 0;
    int stunCount;
    float timeSinceStunned;
    float timeSinceCanSeePlayer;
    int facingDirection;
    int currentAttack;
    int currentPatrolPoint = 0;
    float sighPrevFrame;
    float sighCurrFrame;
    bool newPatrolPoint = true;
    bool angry = false;

    Vector2 dir;
    Vector3 leftFlip = new Vector3(0, 180, 0);

    private RangedEnemyStates State
    {
        get { return (RangedEnemyStates)animator.GetInteger("State"); }
        set { animator.SetInteger("State", (int)value); }
    }

    public void Start()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        player = PlayerManager.Instance.Player.transform;
        fov = GetComponent<FieldOfView>();
        audio = GetComponent<AudioSource>();
        target = patrolPoints[0];
        attackPoint = transform.Find("AttackPoint").GetComponent<Transform>();
        groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        timeSinceCanSeePlayer = 10f;
        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }
    private void UpdatePath()
    {
        if (PlayerInFOV() && followEnabled && seeker.IsDone() && !IsDead)
        {
            target = player;
            seeker.StartPath(rb.position, target.position, OnPathComplete);
            newPatrolPoint = true;
        }
        else if (!PlayerInFOV() && patrolEnabled && newPatrolPoint && seeker.IsDone() && !IsDead)
        {
            target = patrolPoints[currentPatrolPoint];
            seeker.StartPath(rb.position, target.position, OnPathComplete);
            newPatrolPoint = false;
        }
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }


    private void FixedUpdate()
    {
        if (IsDead) return;

        timeSinceRoll += Time.deltaTime;
        angry = PlayerInFOV();

        if (IsIdleing)
        {
            State = RangedEnemyStates.idle;
            if (angry)
            {
                StopCoroutine(waitOnPatrolPoint());
                newPatrolPoint = true;
                IsIdleing = false;
            }
            return;
        }
        if (!angry)
        {
            PathFollow();
            return;
        }
        if (angry && !IsRolling && !IsAttacking && !IsStunned && followEnabled)
        {
            PathFollow();
        }
        if (angry && !IsAttacking && !IsRolling && Vector2.Distance(player.position, transform.position) < rollBackDistance && timeSinceRoll > timeBtwRoll)
        { 
            RollBack();
        }
        if (angry && !IsRolling && !IsAttacking && !IsAttackChilling && !IsStunned && attackEnabled)
        {
            Attack();
        }
        
    }

    private void Update()
    {
        timeSinceStunned += Time.deltaTime;
        timeSinceCanSeePlayer += Time.deltaTime;
    }

    private void PathFollow()
    {
        if (path == null)
        {
            //Debug.Log("Path Is NULL");
            return;
        }

        // Reached end of path
        if (currentWaypoint >= path.vectorPath.Count)
        {
            //Debug.Log("currentWaypoint Is grater");
            return;
        }

        isGrounded = groundSensor.State();

        // Direction Calculation
        Vector2 direction = ((Vector2)path.vectorPath[path.vectorPath.Count - currentWaypoint > 1 ? currentWaypoint + 1 : currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        // Jump
        if (jumpEnabled && isGrounded)
        {
            if (direction.y > jumpNodeHeightRequirement)
            {
                rb.AddForce(Vector2.up * speed * jumpModifier);
            }
        }

        // Movement
        float _minDistanceToTarget;
        if (target == player && fov.canSeePlayer)
            _minDistanceToTarget = minDistanceToTarget;
        else
            _minDistanceToTarget = nextWaypointDistance;
        if (Vector2.Distance(rb.position, target.position) > _minDistanceToTarget)
        {
            //rb.AddForce(force);
            rb.velocity = new Vector2(force.x, rb.velocity.y);
            State = RangedEnemyStates.run;
            //rb.AddForce(new Vector2(force.x, 0 ));
        }
        else if (Vector2.Distance(rb.position, target.position) <= _minDistanceToTarget)
        {
            State = RangedEnemyStates.idle;
            if (!angry)
            {
                StartCoroutine(waitOnPatrolPoint());
            }
        }

        // Next Waypoint
        if (Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        // Rotation to direction
        if (directionLookEnabled && !IsRolling && !IsAttacking && !IsStunned)
            Flip();
    }
    IEnumerator waitOnPatrolPoint()
    {
        IsIdleing = true;
        yield return new WaitForSeconds(waitOnPatrolPointTime);
        if (patrolPoints.Length > 1)
        {
            PatrolPointSwitcher();
        }
        newPatrolPoint = true;
        IsIdleing = false;
    }

    private void PatrolPointSwitcher()
    {
        currentPatrolPoint++;

        if (currentPatrolPoint >= patrolPoints.Length)
        {
            Array.Reverse(patrolPoints, 0, patrolPoints.Length);
            currentPatrolPoint = 1;
        }
    }

    private void RollBack()
    {
        if (timeSinceRoll > timeBtwRoll)

        IsRolling = true;
        Flip(-facingDirection);
        State = RangedEnemyStates.roll;
        rb.velocity = new Vector2(facingDirection * speed * rollForceModifier, rb.velocity.y >= 0 ? rb.velocity.y : 0);
        timeSinceRoll = 0;
    }
    public void AE_ResetRoll()
    {
        State = RangedEnemyStates.idle;
        //Flip(-facingDirection);
        IsRolling = false;
    }

    private void Flip()
    {
        // Direction Graphics Handling
        sighCurrFrame = rb.velocity.x == 0 ? sighPrevFrame : Mathf.Sign(rb.velocity.x);
        if (sighCurrFrame != sighPrevFrame)
        {
            facingDirection = (int)sighCurrFrame;
            transform.rotation = Quaternion.Euler(rb.velocity.x < 0 ? leftFlip : Vector3.zero);
        }
        sighPrevFrame = sighCurrFrame;

        //float dir = transform.position.x - target.transform.position.x;
        //if (dir > 0)
        //{
        //    facingDirection = -1;
        //    transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        //}
        //else if (dir < 0)
        //{
        //    facingDirection = 1;
        //    transform.rotation = Quaternion.Euler(Vector3.zero);
        //}

        //if (rb.velocity.x <= 0.01f)
        //{
        //    //transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        //    facingDirection = -1;
        //    transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        //}
        //else if (rb.velocity.x >= 0.01f)
        //{
        //    //transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        //    facingDirection = 1;
        //    transform.rotation = Quaternion.Euler(Vector3.zero);
        //}
    }
    private void Flip(float direction)
    {
        if (direction < 0)
        {
            facingDirection = -1;
            transform.rotation = Quaternion.Euler(leftFlip);
        }
        else
        {
            facingDirection = 1;
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
    }

    private bool PlayerInFOV()
    {
        //return (Vector2.Distance(transform.position, PlayerManager.Instance.Player.transform.position) < angryDistance && !PlayerManager.Instance.Player.GetComponent<HeroKnight>().IsDead);
        if (fov.canSeePlayer)
        {
            timeSinceCanSeePlayer = 0;
            return true;
        }
        else if (timeSinceCanSeePlayer < angryTime)
        {
            return true;
        }
        else
            return false;
    }


    public void Attack()
    {
        if (target.position.x > transform.position.x && facingDirection != 1)
        {
            Flip(1);
        }
        else if (target.position.x < transform.position.x && facingDirection != -1)
        {
            Flip(-1);
        }

        if (Vector3.Distance(transform.position, player.position) <= fov.radius && fov.canSeePlayer)
        {
            IsAttacking = true;
            StartCoroutine("AttackChill");
            
            animator.SetTrigger("Attack1");

            StartCoroutine(AttackAnimation(currentAttack));
        }

        //Collider2D[] players = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, damagebleLayerMask);
        //foreach (Collider2D player in players)
        //{
        //    IsAttacking = true;
        //    StartCoroutine("AttackChill");

        //    animator.SetTrigger("Attack1");

        //    if (AttackEvent != null && AttackEventSounds != null)
        //    {
        //        audio.clip = AttackEventSounds[UnityEngine.Random.Range(0, AttackEventSounds.Length)];
        //        AttackEvent.Invoke();
        //    }

        //    StartCoroutine(AttackAnimation(currentAttack));
        //}
    }
    IEnumerator AttackChill()
    {
        IsAttackChilling = true;
        yield return new WaitForSeconds(timeAttackChill);
        IsAttackChilling = false;
    }
    IEnumerator AttackAnimation(int currAttack)
    {
        yield return new WaitForSeconds(attack1Duration);
        IsAttacking = false;
    }
    public void OnEnemyAttack()
    {
        if (AttackEvent != null && AttackEventSounds != null)
        {
            audio.clip = AttackEventSounds[UnityEngine.Random.Range(0, AttackEventSounds.Length)];
            AttackEvent.Invoke();
        }

        Projectiles[FindProjectile()].transform.position = attackPoint.position;

        Vector2 direction;
        if (fov.canSeePlayer)
            direction = (Vector2)player.position - rb.position;
        else
            direction = Vector2.right * facingDirection;

        Projectiles[FindProjectile()].GetComponent<Projectile>().ShootProjectile(direction, standartDamage);
    }

    private int FindProjectile()
    {
        for(int i = 0; i < Projectiles.Length; i++)
        {
            if (!Projectiles[i].activeInHierarchy)
                return i;
        }
        return 0;
    }

    public override void GetDamage(float damage, float hitDirection)
    {
        Debug.Log(damage);
        CurrentHealthPoints = Mathf.Clamp(CurrentHealthPoints - damage, 0, MaxHealthPoints.Value);

        timeSinceCanSeePlayer = 0;

        if (CurrentHealthPoints > 0)
        {
            rb.velocity = new Vector2(hitPushForce * hitDirection, hitPushForce);

            if (stunCount < maxStunCount && IsStunProtected == false)
            {
                stunCount++;

                animator.SetTrigger("HurtStun");

                StopCoroutine(Stun());
                IsStunned = true;
                StartCoroutine(Stun());
            }
            else if (stunCount < maxStunCount && IsStunProtected == true)
            {
                StartCoroutine(HitWithoutAnim());
            }

            if (stunCount >= maxStunCount)
            {
                stunCount = 0;
                StartCoroutine(StunProtectionWait());
            }
            if (timeSinceStunned > 3f)
            {
                stunCount = 0;
                timeSinceStunned = 0f;
            }

            if (HitEvent != null && HitEventSounds != null)
            {
                audio.clip = HitEventSounds[UnityEngine.Random.Range(0, HitEventSounds.Length)];
                HitEvent.Invoke();
            }
        }
        else
        {
            rb.velocity = new Vector2(hitPushForce * 1.5f * hitDirection, hitPushForce * 1.5f);
            GetComponent<DamagebleObject>().Death();

            if (DeathEvent != null && DeathEventSounds != null)
            {
                audio.clip = DeathEventSounds[UnityEngine.Random.Range(0, DeathEventSounds.Length)];
                DeathEvent.Invoke();
            }
        }
    }
    IEnumerator Stun()
    {
        yield return new WaitForSeconds(timeStun);
        IsStunned = false;
    }
    IEnumerator StunProtectionWait()
    {
        IsStunProtected = true;
        yield return new WaitForSeconds(stunProtectionTime);
        IsStunProtected = false;
    }
    IEnumerator HitWithoutAnim()
    {
        sr.material = onHitMaterial;
        yield return new WaitForSeconds(0.15f);
        sr.material = originalMaterial;
    }

    public override void Death()
    {
        IsDead = true;
        State = RangedEnemyStates.dead;
        gameObject.layer = 10;
        sr.sortingLayerName = "DestroyedObjects";
        GlobalEventManager.SendEnemyDeath();
        StartCoroutine(DestroyEnemy());
    }
    IEnumerator DestroyEnemy()
    {
        yield return new WaitForSeconds(5f);
        //rb.gravityScale = 0.005f;
        //GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(10f);
        Destroy(transform.parent.gameObject);
        
        //for (int i = 0; i < patrolPoints.Length; i++)
        //{
        //    Destroy(patrolPoints[i].gameObject);
        //}
        //Destroy(gameObject);
    }
}

public enum RangedEnemyStates
{
    idle,   //State 0
    run,    //State 1
    roll,   //State 2
    dead,   //State 3
}
