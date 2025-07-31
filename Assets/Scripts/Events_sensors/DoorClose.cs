using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorClose : MonoBehaviour
{
    [SerializeField] private Transform triggerPoint;

    Transform player;
    Rigidbody2D rb;

    private void Start()
    {
        player = PlayerManager.Instance.Player.transform;
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (player.position.x >= triggerPoint.position.x)
        {
            rb.gravityScale = 1;
            GetComponent<DoorClose>().enabled = false;
        }
    }
}
