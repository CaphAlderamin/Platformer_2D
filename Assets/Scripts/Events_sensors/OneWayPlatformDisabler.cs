using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatformDisabler : MonoBehaviour
{
    [SerializeField] private float disableTime;

    PlatformEffector2D effector;

    private void Start()
    {
        effector = GetComponent<PlatformEffector2D>();
    }

    public void DisablePlatform()
    {
        effector.rotationalOffset = 180;
        StartCoroutine(EnableAfterTime());
    }
    IEnumerator EnableAfterTime()
    {
        yield return new WaitForSeconds(disableTime);
        effector.rotationalOffset = 0;
    }
}
