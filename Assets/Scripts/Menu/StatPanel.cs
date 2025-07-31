using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatPanel : MonoBehaviour
{
    [SerializeField] private StatRow[] statRows;
    [SerializeField] private string[] statNames;

    private CharacterStat[] stats;

    private void OnValidate()
    {
        statRows = GetComponentsInChildren<StatRow>();
        UpdateStatNames();
    }

    public void UpdateStatNames()
    {
        for (int i = 0; i < statNames.Length; i++)
        {
            statRows[i].NameText.text = statNames[i];
        }
    }

    public void UpdateStatValues()
    {
        for (int i = 0; i < stats.Length; i++)
        {
            statRows[i].ValueText.text = stats[i].Value.ToString();
        }
    }

    public void SetStats(CharacterStat[] characterStats)
    {
        stats = characterStats;

        if (stats.Length > statRows.Length)
        {
            Debug.LogError("Not Enough Stat Displays!");
            return;
        }

        for (int i = 0; i < statRows.Length; i++)
        {
            statRows[i].Stat = i < stats.Length ? stats[i] : null;
            statRows[i].gameObject.SetActive(i < stats.Length);
        }
    }
}
