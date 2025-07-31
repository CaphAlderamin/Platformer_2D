using UnityEngine;
using System.Collections;

public class Sensor_HeroKnight : MonoBehaviour {

    public int m_ColCount = 0;

    private float m_DisableTimer;
    private OneWayPlatformDisabler platform;

    private void OnEnable()
    {
        m_ColCount = 0;
    }

    public bool State()
    {
        if (m_DisableTimer > 0)
            return false;
        return m_ColCount > 0;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("OneWayPlatform"))
        {
            platform = other.GetComponent<OneWayPlatformDisabler>();
        }
        else
        {
            platform = null;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground") || other.CompareTag("OneWayPlatform"))
        {
            m_ColCount++;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground") || other.CompareTag("OneWayPlatform"))
        {
            m_ColCount--;
        }
    }

    void Update()
    {
        m_DisableTimer -= Time.deltaTime;
    }

    public OneWayPlatformDisabler Platform()
    {
        if (platform != null)
            return platform;
        else return null;
    }

    public void Disable(float duration)
    {
        m_DisableTimer = duration;
    }
}
