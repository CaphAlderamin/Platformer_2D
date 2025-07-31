using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    //public float PlayerMaxHealthPoints;
    public float PlayerCurrentHealthPoints;
    public int PlayerCoinAmount;

    public int[] StatLevels;
    //public int[] StatLevelsCost;
    ////Stats[]
    //public float[] values;
    //public StatModType[] types;
    //public int[] orders;
    //public object[] sources;

    public PlayerData(HeroKnight Player, StatLevelSystem StatLevelSystem)
    {
        PlayerCurrentHealthPoints = Player.CurrentHealthPoints;
        PlayerCoinAmount = Player.GetCoinAmount();
        StatLevels = StatLevelSystem.GetStatLevels();




        //PlayerMaxHealthPoints = Player.MaxHealthPoints;

        //StatLevelsCost = statLevelSystem.GetStatLevelsCost();
        ////stats
        //CharacterStat[] stats = statLevelSystem.GetStats();
        //for (int i = 0; i < stats.Length; i++)
        //    for (int j = 0; j < stats[i].StatModifiers.Count; j++)
        //    {
        //        StatModifier statModifiers = stats[i].StatModifiers[j];
        //        if ((string)statModifiers.Source == "Level")
        //        {
        //            values[i] = statModifiers.Value;
        //            types[i] = statModifiers.Type;
        //            orders[i] = statModifiers.Order;
        //            sources[i] = statModifiers.Source;
        //        }
        //    }
    }

}
