using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private GameObject projectileGFX;
    [SerializeField] private ParticleSystem projectileParticles;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask interactableLayers;

    [SerializeField] private float speed;
    [SerializeField] private float lineFlyTime;
    [SerializeField] private float liveTime;

    float standartDamage;
    float facingDirection;
    bool hit;

    new CapsuleCollider2D  collider;
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<CapsuleCollider2D>();
    }

    private void FixedUpdate()
    {
        if (hit) return;

        TrajectoryUpdate();
    }

    private void TrajectoryUpdate()
    {
        Vector2 direction = rb.velocity;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.gameObject.layer);
        //if (collision.IsTouching(playerLayer)
        //{
        //    Hit();
        //}
        //else if (collision.IsTouchingLayers(playerLayer))
        //{
        //    Hit();
        //    collision.GetComponent<HeroKnight>().GetDamage(standartDamage, facingDirection);
        //}

        if (collision.tag != "Enemy" && collision is Collider2D && (collision.tag == "Player" || collision.tag == "Ground"))
        {
            if (collision.tag == "Ground")
            {
                Hit();
                return;
            }

            if (collision.tag == "Player")
            {
                HeroKnight player = collision.GetComponent<HeroKnight>();
                if (player.IsRolling == false)
                {
                    Hit();
                    collision.GetComponent<HeroKnight>().GetDamage(standartDamage, facingDirection);
                    return;
                }
            }
        }
    }
    private void Hit()
    {
        hit = true;
        collider.enabled = false;
        projectileParticles.Play();
        projectileGFX.SetActive(false);
        StartCoroutine(DesableAfterTime());
    }
    IEnumerator DesableAfterTime()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false); 
        projectileGFX.SetActive(true);
        collider.enabled = true;
    }



    public void ShootProjectile(Vector2 shootDirection, float _standartDamage)
    {
        standartDamage = _standartDamage;

        gameObject.SetActive(true); 
        hit = false;
        rb.gravityScale = 0;

        transform.right = shootDirection;
        if (shootDirection.x < 0)
            facingDirection = -1;
        else
            facingDirection = 1;
        Vector2 shootForce = transform.right * speed;
        rb.AddForce(shootForce);

        StartCoroutine(LineFlyTime());  
    }
    IEnumerator LineFlyTime()
    {
        yield return new WaitForSeconds(lineFlyTime);
        rb.gravityScale = 1;
        StartCoroutine(LiveTime());
    }
    IEnumerator LiveTime()
    {
        yield return new WaitForSeconds(liveTime);
        if (gameObject.activeSelf == true)
        {
            Hit();
        }
    }
}
