using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;    

public class HUDController : MonoBehaviour
{
    [Header("HPBar")]
    [SerializeField] private Text HpText;
    [SerializeField] private Image HpFill;
    [SerializeField] private Image HpFillBack;
    [SerializeField] private float hpDecreasingSpeed;

    [Header("CoinsBar")]
    [SerializeField] private Text CoinsText;

    private IUiPlayer uiPlayer;

    float MaxHealthPoints;
    float CurrentHealthPoints;
    HeroKnight Player;

    private void Start()
    {
        GlobalEventManager.OnHealthUpdate.AddListener(TextBarUpdate);
        GlobalEventManager.OnCoinsUpdate.AddListener(CoinsBarUpdate);

        Player = PlayerManager.Instance.Player.GetComponent<HeroKnight>();
        uiPlayer = PlayerManager.Instance.Player.GetComponent<IUiPlayer>();
        if (uiPlayer == null) Debug.Log("interface not found");
        //MaxHealthPoints = Player.MaxHealthPoints.Value;
        //CurrentHealthPoints = Player.CurrentHealthPoints;

        MaxHealthPoints = uiPlayer.GetMaxHealthPoints();
        CurrentHealthPoints = uiPlayer.GetCurrentHealthPoints();

        HpText.text = MaxHealthPoints.ToString() + " / " + CurrentHealthPoints.ToString();
        CoinsText.text = uiPlayer.GetCoinAmount().ToString();
    }

    private void Update()
    {
        if (HpFill.fillAmount != HpFillBack.fillAmount)
        {
            HpFillBack.fillAmount = Mathf.Lerp(HpFillBack.fillAmount, CurrentHealthPoints / MaxHealthPoints, hpDecreasingSpeed * Time.deltaTime);
        }
    }

    private void TextBarUpdate()
    {
        MaxHealthPoints = uiPlayer.GetMaxHealthPoints();
        CurrentHealthPoints = uiPlayer.GetCurrentHealthPoints();

        HpText.text = CurrentHealthPoints.ToString() + " / " + MaxHealthPoints.ToString();
        HpFill.fillAmount = CurrentHealthPoints / MaxHealthPoints;
    }

    private void CoinsBarUpdate()
    {
        CoinsText.text = uiPlayer.GetCoinAmount().ToString();
    }

}
