using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SaveRow : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private MainMenuController mainMenuController;
    [SerializeField] private Text SaveRowText;

    private void Awake()
    {
        mainMenuController = FindObjectOfType<MainMenuController>();
        SaveRowText = GetComponent<Text>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        mainMenuController.SaveFileNameString = SaveRowText.text;
    }
}
