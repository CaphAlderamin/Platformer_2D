using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StatLevelSystem : MonoBehaviour
{
    private IStatLevelSystemPlayer statLevelSystemPlayer;
    
    [SerializeField] private int standartCost;
    [SerializeField] private float perLevelStatCostModifier;
    [SerializeField] private int[] statMaxLevels;
    [SerializeField] private float[] perLevelStatModifier;
    
    private int[] statLevels; //need save
    private int[] statLevelsCost;
    private CharacterStat[] stats;

    private StatUpPanel statUpPanel;
    private StatPanel statPanel;
    private readonly string thisObject = "Level";

    private void Awake()
    {
        statLevelSystemPlayer = GetComponent<IStatLevelSystemPlayer>();

        statUpPanel = PlayerManager.Instance.StatUpPanel;
        statPanel = PlayerManager.Instance.StatPanel;
    }

    public void SetCharacterStats(CharacterStat[] characterStats)
    {
        stats = characterStats;
        statPanel.SetStats(characterStats);
        Debug.Log("HeroKnight SetStats Success");
    }

    public void SetSaveStats(int[] levels)
    {
        statLevels = new int[levels.Length];
        statLevelsCost = new int[levels.Length];
        for (int i = 0; i < levels.Length; i++)
        {
            statLevels[i] = levels[i];

            if (statMaxLevels[i] == 10)
            {
                statLevelsCost[i] = (int)Mathf.Round(standartCost * Mathf.Pow(perLevelStatCostModifier, statLevels[i]));
            }
            else if (statMaxLevels[i] == 1)
            {
                statLevelsCost[i] = (int)Mathf.Round(standartCost * Mathf.Pow(perLevelStatCostModifier, 10));
            }
            else
                statLevelsCost[i] = standartCost;

            stats[i].RemoveAllModifiersFromSourse(thisObject);
            stats[i].AddModifier(new StatModifier(statLevels[i] * perLevelStatModifier[i], StatModType.Flat, thisObject));

            if (i == 0)
            {
                statLevelSystemPlayer.increaseCurrentHPWithMaxHP(perLevelStatModifier[i] * statLevels[i]);
                GlobalEventManager.SendHealth();
            }
        }
        statUpPanel.SetLevels(statLevels, statLevelsCost, statMaxLevels, perLevelStatModifier);
        statUpPanel.UpdateStatLevels();
        statUpPanel.UpdateStatMaxLevels();
        statUpPanel.UpdateStatLevelsCost();
        statPanel.UpdateStatValues();
        GlobalEventManager.SendHealth();
        Debug.Log("Load SetStats Success");
    }

    public void SetStandartStats()
    {
        statLevels = new int[statMaxLevels.Length];
        statLevelsCost = new int[statMaxLevels.Length];

        for (int i = 0; i < statMaxLevels.Length; i++)
        {
            statLevels[i] = 0;

            if (statMaxLevels[i] == 10)
            {
                statLevelsCost[i] = (int)Mathf.Round(standartCost * Mathf.Pow(perLevelStatCostModifier, statLevels[i]));
            }
            else if (statMaxLevels[i] == 1)
            {
                statLevelsCost[i] = (int)Mathf.Round(standartCost * Mathf.Pow(perLevelStatCostModifier, 10));
            }
            else
                statLevelsCost[i] = standartCost;
        }
        statUpPanel.SetLevels(statLevels, statLevelsCost, statMaxLevels, perLevelStatModifier);
        statUpPanel.UpdateStatLevels();
        statUpPanel.UpdateStatMaxLevels();
        statUpPanel.UpdateStatLevelsCost();
        statPanel.UpdateStatValues();
        Debug.Log("Standart SetStats Success");
    }

    public void SetStats(int[] levels, params CharacterStat[] characterStats)
    {
        stats = characterStats;
        statLevels = levels;
    }

    public void AddLevelStatModifier(int id)
    {
        if (statLevels[id] < statMaxLevels[id])
        {
            if (statLevelSystemPlayer.TryBoughtStatUp(statLevelsCost[id]))
            {
                statLevels[id]++;

                stats[id].RemoveAllModifiersFromSourse(thisObject);

                stats[id].AddModifier(new StatModifier(statLevels[id] * perLevelStatModifier[id], StatModType.Flat, thisObject));
                statLevelsCost[id] = (int)Mathf.Round(statLevelsCost[id] * 1.5f);
                statUpPanel.UpdateStatLevels();
                statUpPanel.UpdateStatLevelsCost();
                statPanel.UpdateStatValues();

                if (id == 0)
                {
                    statLevelSystemPlayer.increaseCurrentHPWithMaxHP(perLevelStatModifier[id]);
                    GlobalEventManager.SendHealth();
                }
                    

                Debug.Log("can buy this");
            }
            else Debug.Log("cant buy this");
        }
    }

    public int[] GetStatLevels()
    {
        return statLevels;
    }
    //public int[] GetStatLevelsCost()
    //{
    //    return statLevelsCost;
    //}
    //public CharacterStat[] GetStats()
    //{
    //    return  stats;
    //}

}
