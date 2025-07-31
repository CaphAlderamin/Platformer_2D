using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class StatTip : MonoBehaviour
{
	public static StatTip Instance;

	[SerializeField] Text statNameText;
	[SerializeField] Text finalValueText;
	[SerializeField] Text modifiersListText;

	private StringBuilder sb = new StringBuilder();
	private RectTransform rect;
	private Vector2 standartRect;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(this);
		}

		rect = GetComponent<RectTransform>();
		standartRect = rect.sizeDelta;

		gameObject.SetActive(false);
	}

	public void ShowTip(CharacterStat stat, string statName)
	{
		gameObject.SetActive(true);

		statNameText.text = statName;
		finalValueText.text = GetValueText(stat);
		modifiersListText.text = GetModifiersText(stat);
	}

	public void HideTip()
	{
		gameObject.SetActive(false);
	}

	private string GetValueText(CharacterStat stat)
	{
		sb.Length = 0;

		sb.Append(stat.Value);
		sb.Append(" (");
		sb.Append(stat.BaseValue);
		sb.Append(" + ");
		sb.Append((float)System.Math.Round(stat.Value - stat.BaseValue, 4));
		sb.Append(")");

		return sb.ToString();
	}

	private string GetModifiersText(CharacterStat stat)
	{
		sb.Length = 0;

		for (int i = 0; i < stat.StatModifiers.Count; i++)
		{
			StatModifier mod = stat.StatModifiers[i];

			sb.Append(mod.Source);
			sb.Append(": ");

			if (mod.Value > 0)
			{
				sb.Append("+");
			}

			if (mod.Type == StatModType.Flat)
			{
				sb.Append(mod.Value);
			}
			else
			{
				sb.Append(mod.Value * 100);
				sb.Append("%");
			}

			if (i < stat.StatModifiers.Count - 1)
			{
				sb.AppendLine();
			}
		}

		if (stat.StatModifiers.Count > 4)
			rect.sizeDelta = standartRect + new Vector2(0, (stat.StatModifiers.Count - 4) * 25f);
		else
			rect.sizeDelta = standartRect;
		return sb.ToString();
	}
}
