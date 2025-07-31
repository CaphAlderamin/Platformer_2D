using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroKnightGFX : MonoBehaviour
{
    public HeroKnight Player;

    public void OnHeroAttack()
    {
       Player.OnHeroAttack();
    }

    public void AE_SlideDust()
    {
        Player.AE_SlideDust();
    }

    public void AE_ResetRoll()
    {
        Player.AE_ResetRoll();
    }
}
