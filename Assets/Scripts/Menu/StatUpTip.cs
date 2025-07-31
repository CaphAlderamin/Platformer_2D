using UnityEngine;
using UnityEngine.UI;


public class StatUpTip : MonoBehaviour
{
	public static StatUpTip Instance;

	[SerializeField] Text statNameText;
	[SerializeField] Text statCostText;
	[SerializeField] Text descriptionText;

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

		gameObject.SetActive(false);
	}

	public void ShowTip(string statName, string statLevelCost, string statDescription)
	{
		gameObject.SetActive(true);

		statNameText.text = statName;
		statCostText.text = "Cost: " + statLevelCost;
		descriptionText.text = statDescription;
	}

	public void HideTip()
	{
		gameObject.SetActive(false);
	}

}
