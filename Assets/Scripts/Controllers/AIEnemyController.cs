using System.Collections;
using System;
using UnityEngine;
using Pathfinding;

public class AIEnemyController : DamagebleObject
{
    public static AIEnemyController Instance { get; private set; }

    [Header("Pathfinding")]
    //[SerializeField] private float angryDistance = 8f;
    [SerializeField] private float nextWaypointDistance = 2f;
    [SerializeField] private float minDistanceToTarget = 2f;
    [SerializeField] private float pathUpdateSeconds = 0.5f;

    [Header("Physics")]
    [SerializeField] private float speed = 200f;
    [SerializeField] private float jumpNodeHeightRequirement = 0.8f;
    [SerializeField] private float jumpModifier = 0.3f;

    [Header("Fighting")]
    [SerializeField] private float angryTime;
    [SerializeField] private float standartDamage;
    [SerializeField] private float attackRange;
    [SerializeField] private float closeAttackRange;
    [SerializeField] private LayerMask damagebleLayerMask;
    [SerializeField] private float attack1Duration;
    [SerializeField] private float attack2Duration;
    [SerializeField] private float timeAttackChill;
    [SerializeField] private int maxStunCount;
    [SerializeField] private float stunProtectionTime;
    [SerializeField] private float timeStun;
    [SerializeField] private float hitPushForce;

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
    Transform target;
    Transform attackPoint;
    Seeker seeker;
    Path path;

    int currentWaypoint = 0;
    int stunCount;
    float timeSinceStunned;
    float timeSinceAttack;
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

    private EnemyStates State
    {
        get { return (EnemyStates)animator.GetInteger("State"); }
        set { animator.SetInteger("State", (int)value); }
    }

    public void Start()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        //target = PlayerManager.instance.Player.transform;
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
            target = PlayerManager.Instance.Player.transform;
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
        angry = PlayerInFOV();

        if (IsIdleing)
        {
            State = EnemyStates.idle;
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
        if (angry && !IsAttacking && !IsStunned && followEnabled)
        {
            PathFollow();
        }
        if (angry && !IsAttacking && !IsAttackChilling && !IsStunned && attackEnabled)
        {
            Attack();
        }
    }

    private void Update()
    {
        timeSinceStunned += Time.deltaTime;
        timeSinceAttack += Time.deltaTime;
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

        // See if colliding with anything
        //Vector3 startOffset = transform.position - new Vector3(0f, GetComponent<Collider2D>().bounds.extents.y + jumpCheckOffset);
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
        if (Vector2.Distance(rb.position, target.position) > minDistanceToTarget)
        {
            //rb.AddForce(force);
            rb.velocity = new Vector2(force.x, rb.velocity.y);
            State = EnemyStates.run;
            //rb.AddForce(new Vector2(force.x, 0 ));
        }
        else if (!angry)
        {
            StartCoroutine(waitOnPatrolPoint());
        }

        // Next Waypoint
        if (Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        if (directionLookEnabled)
            Flip(force);
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

    private void Flip(Vector2 force)
    {
        // Direction Graphics Handling
        sighCurrFrame = force.x == 0 ? sighPrevFrame : Mathf.Sign(force.x);
        if (sighCurrFrame != sighPrevFrame)
        {
            facingDirection = (int)sighCurrFrame;
            transform.rotation = Quaternion.Euler(force.x < 0 ? leftFlip : Vector3.zero);
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

        //if (rb.velocity.x <= 0.01f && force.x < 0)
        //{
        //    //transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        //    facingDirection = -1;
        //    transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        //}
        //else if (rb.velocity.x >= 0.01f && force.x > 0)
        //{
        //    //transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        //    facingDirection = 1;
        //    transform.rotation = Quaternion.Euler(Vector3.zero);
        //}
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
            dir = (target.position - transform.position).normalized;
            rb.velocity = new Vector2(dir.x, rb.velocity.y);
        }
        else if (target.position.x < transform.position.x && facingDirection != -1)
        {
            dir = (target.position - transform.position).normalized;
            rb.velocity = new Vector2(dir.x, rb.velocity.y);
        }

        Collider2D[] players = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, damagebleLayerMask);
        foreach (Collider2D player in players)
        {
            IsAttacking = true;
            currentAttack++;

            if (currentAttack > 2)
                currentAttack = 1;

            if (currentAttack == 2)
                StartCoroutine("AttackChill");

            if (timeSinceAttack > 1.5f)
                currentAttack = 1;

            animator.SetTrigger("Attack" + currentAttack);
            timeSinceAttack = 0.0f;

            if (AttackEvent != null && AttackEventSounds != null)
            {
                audio.clip = AttackEventSounds[UnityEngine.Random.Range(0, AttackEventSounds.Length)];
                AttackEvent.Invoke();
            }

            StartCoroutine(AttackAnimation(currentAttack));
        }
    }
    IEnumerator AttackChill()
    {
        IsAttackChilling = true;
        yield return new WaitForSeconds(timeAttackChill);
        IsAttackChilling = false;
    }
    IEnumerator AttackAnimation(int currAttack)
    {
        if (Vector2.Distance(target.position, transform.position) > closeAttackRange)
            rb.velocity = new Vector2(facingDirection * 3, rb.velocity.y);
        switch (currAttack)
        {
            case 1:
                yield return new WaitForSeconds(attack1Duration);
                break;
            case 2:
                yield return new WaitForSeconds(attack2Duration);
                break;
        }
        IsAttacking = false;
    }
    public void OnEnemyAttack()
    {
        Collider2D[] players = Physics2D.OverlapCircleAll(transform.Find("AttackPoint").position, attackRange, damagebleLayerMask);

        foreach (Collider2D player in players)
        {
            player.GetComponent<DamagebleObject>().GetDamage(standartDamage, facingDirection);
        }
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
        State = EnemyStates.dead;
        gameObject.layer = 10;
        sr.sortingLayerName = "DestroyedObjects";
        GlobalEventManager.SendEnemyDeath();
        StartCoroutine(DestroyEnemy());
    }
    IEnumerator DestroyEnemy()
    {
        yield return new WaitForSeconds(2f);
        rb.gravityScale = 0f;
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

public enum EnemyStates
{
    idle,   //State 0
    run,    //State 1
    dead,   //State 2
}



//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Pathfinding;

//public class AIEnemyController : MonoBehaviour
//{

//    private Transform Player;

//    public float Speed;
//    public float NextWaypointDistance;

//    private Path path;
//    private int currantWaypoint = 0;
//    private bool reachedEndOfPath = false;

//    private Seeker seeker;
//    private Rigidbody2D rb;


//    void Start()
//    {
//        Player = PlayerManager.instance.Player.transform;
//        seeker = GetComponent<Seeker>();
//        rb = GetComponent<Rigidbody2D>();

//        InvokeRepeating("UpdatingPath", 0f, 0.5f);
//    }

//    private void UpdatingPath()
//    {
//        if (seeker.IsDone())
//            seeker.StartPath(rb.position, Player.position, OnPathComplete);
//    }

//    private void OnPathComplete(Path p)
//    {
//        if (!p.error)
//        {
//            path = p;
//            currantWaypoint = 0;
//        }
//    }

//    void Update()
//    {
//        if (path == null)
//            return;

//        if (currantWaypoint >= path.vectorPath.Count)
//        {
//            reachedEndOfPath = true;
//            return;
//        }
//        else
//        {
//            reachedEndOfPath = false;
//        }

//        Vector2 direction = ((Vector2)path.vectorPath[currantWaypoint] - rb.position).normalized;
//        Vector2 force = direction * Speed * Time.deltaTime;

//        //rb.AddForce(force);
//        //rb.velocity = force;
//        rb.velocity = new Vector2(force.x, rb.velocity.y);

//        float distance = Vector2.Distance(rb.position, path.vectorPath[currantWaypoint]);

//        if (distance < NextWaypointDistance)
//        {
//            currantWaypoint++;
//        }

//    }
//}
