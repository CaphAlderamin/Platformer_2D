using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatUpPanel : MonoBehaviour
{
    [SerializeField] private StatUpRow[] statUpRows;
    [SerializeField] private string[] statNames;
    [TextArea(3, 10)]
    [SerializeField] private string[] statDescription;

    private int[] statLevels;
    private int[] statLevelsCost;
    private int[] statMaxLevels;
    private float[] perLevelStatModifier;

    private void OnValidate()
    {
        statUpRows = GetComponentsInChildren<StatUpRow>();
        UpdateStatNames();
    }

    public void UpdateStatNames()
    {
        for (int i = 0; i < statNames.Length; i++)
        {
            statUpRows[i].NameText.text = statNames[i];
        }
    }

    public void UpdateStatLevels()
    {
        for (int i = 0; i < statLevels.Length; i++)
        {
            statUpRows[i].ValueText.text = statLevels[i].ToString();
        }
    }

    public void UpdateStatMaxLevels()
    {
        for (int i = 0; i < statMaxLevels.Length; i++)
        {
            if (statMaxLevels[i].ToString().Length < 2)
                statUpRows[i].MaxValueText.text = "/" + statMaxLevels[i].ToString() + " ";
            else
                statUpRows[i].MaxValueText.text = "/" + statMaxLevels[i].ToString();
        }
    }

    public void UpdateStatLevelsCost()
    {
        for (int i = 0; i < statLevelsCost.Length; i++)
        {
            statUpRows[i].CostText.text = statLevelsCost[i].ToString();
        }
    }

    public void SetLevels(int[] StatLevels, int[] StatLevelsCost, int[]StatMaxLevels, float[] PerLevelStatModifier)
    {
        statLevels = StatLevels;
        statLevelsCost = StatLevelsCost;
        statMaxLevels = StatMaxLevels;
        perLevelStatModifier = PerLevelStatModifier;  

        if (statLevels.Length > statUpRows.Length)
        {
            Debug.LogError("Not Enough Stat Displays!");
            return;
        }

        for (int i = 0; i < statDescription.Length; i++)
        {
            statDescription[i] +=
                "\n" +
                "\nPer Level: +" + perLevelStatModifier[i].ToString() +
                "\nMaximum: +" + (perLevelStatModifier[i] * statMaxLevels[i]).ToString();
        }

        for (int i = 0; i < statUpRows.Length; i++)
        {
            statUpRows[i].statLevel = i < statLevels.Length ? statLevels[i] : 0;
            statUpRows[i].statLevelCost = i < statLevelsCost.Length ? statLevelsCost[i] : 0;
            statUpRows[i].statDescription = i < statDescription.Length ? statDescription[i] : null;
            statUpRows[i].gameObject.SetActive(i < statLevels.Length);
        }
    }
}
