using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GlobalEventManager : MonoBehaviour
{
    public static UnityEvent OnHealthUpdate = new UnityEvent();

    public static UnityEvent OnCoinsUpdate = new UnityEvent();

    public static UnityEvent OnHeroDeath = new UnityEvent();

    public static UnityEvent OnEnemyDeath = new UnityEvent();


    public static void SendHealth()
    {
        OnHealthUpdate.Invoke();
    }

    public static void SendCoins()
    {
        OnCoinsUpdate.Invoke();
    }

    public static void SendHeroDeath()
    {
        OnHeroDeath.Invoke();
    }

    public static void SendEnemyDeath()
    {
        OnEnemyDeath.Invoke();
    }
}
