using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatRow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public Text NameText;
	public Text ValueText;

	[NonSerialized] public CharacterStat Stat;

	private void OnValidate()
	{
		Text[] texts = GetComponentsInChildren<Text>();
		NameText = texts[0];
		ValueText = texts[1];
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		StatTip.Instance.ShowTip(Stat, NameText.text);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		StatTip.Instance.HideTip();
	}
}
