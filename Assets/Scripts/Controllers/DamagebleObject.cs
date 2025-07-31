using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamagebleObject : MonoBehaviour
{
    [Header("Event system settings")]
    public UnityEvent HitEvent;
    public UnityEvent AttackEvent;
    public UnityEvent BlockEvent;
    public UnityEvent DeathEvent;

    [Header("Sounds")]
    public AudioClip[] HitEventSounds;
    public AudioClip[] AttackEventSounds;
    public AudioClip[] BlockEventSounds;
    public AudioClip[] DeathEventSounds;

    [Header("Object health")]
    [SerializeField] public CharacterStat MaxHealthPoints;
    [SerializeField] public float CurrentHealthPoints;

    public virtual void GetHeal(float Heal)
    {
        Debug.Log("Heal detected");
    }

    public virtual void GetDamage(float damage, float hitDirection)
    {
        Debug.Log("Hit detected");
    }

    public virtual void Death()
    {
        Destroy(this.gameObject);
    }
}
