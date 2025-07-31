using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private float FOVUpdateSeconds;

    public float radius;
    [Range(0,360)]
    public float angle;

    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstructionMask;

    public bool canSeePlayer = false;

    [NonSerialized]public GameObject player;

    private void Start()
    {
        player = PlayerManager.Instance.Player;
        StartCoroutine(FOVRoutine());
    }
    IEnumerator FOVRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(FOVUpdateSeconds);
            FOVCheck();
        }
    }

    private void FOVCheck()
    {
        Collider2D[] rangeChecks = Physics2D.OverlapCircleAll(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector2.Angle(transform.right, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    canSeePlayer = true;
                else
                    canSeePlayer = false;
            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
            canSeePlayer = false;
    }
}
