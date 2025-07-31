using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatUpRow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public Text NameText;
	public Text ValueText;
	public Text MaxValueText;
	public Text CostText;

	[NonSerialized] public int statLevel;
	[NonSerialized] public int statLevelCost;
	[NonSerialized] public string statDescription;

	private void OnValidate()
	{
		Text[] texts = GetComponentsInChildren<Text>();
		NameText = texts[0];
		ValueText = texts[1];
		MaxValueText = texts[2];
		CostText = texts[4];
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		StatUpTip.Instance.ShowTip(NameText.text, CostText.text, statDescription);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		StatUpTip.Instance.HideTip();
	}
}
