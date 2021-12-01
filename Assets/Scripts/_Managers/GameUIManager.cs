using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameUIManager : MonoBehaviour
{
    [Header("Bot Right")]
    [SerializeField] Slider healthSlider;
    [SerializeField] Slider staminaSlider;
    [SerializeField] Text ammoText;
    [SerializeField] Image ammoImage;
    [SerializeField] Sprite[] ammoImages;

    [Header("Top Right")]
    [SerializeField] Text levelText;
    [SerializeField] Slider expSlider;
    [SerializeField] Text damageText;
    [SerializeField] Text fireRateText;

    [Header("Top Center")]
    [SerializeField] TextMeshProUGUI stageText;
    [SerializeField] TextMeshProUGUI scoreText;

    [Header("Game Over")]
    [SerializeField] GameObject endPanel;
    [SerializeField] Image hurtImage;

    Player player;

    void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    private void Start()
    {
        healthSlider.maxValue = player.MaxHealth;
        healthSlider.value = player.CurrentHealth;

        staminaSlider.maxValue = player.MaxStamina;
        staminaSlider.value = player.CurrentStamina;

        UpdateAmmo();
    }

    public void Hurt()
    {
        StartCoroutine(HurtCoroutine());
    }

    IEnumerator HurtCoroutine()
    {
        hurtImage.DOFade(0.75f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        hurtImage.DOFade(0, 0.5f);
    }

    public void UpdateHealthSlider()
    {
        healthSlider.value = player.CurrentHealth;
    }

    public void UpdateStaminaSlider()
    {
        staminaSlider.value = player.CurrentStamina;
    }

    public void UpdateAmmo()
    {
        ammoText.text = player.PlayerActionController.CurrentWeap.CurrentAmmo.ToString();
    }

    public void UpdateAmmoImage(int weapIndex)
    {
        ammoImage.sprite = ammoImages[weapIndex];
    }

    public void UpdateLevelText()
    {
        levelText.text = player.Level.ToString();
    }

    public void UpdateExpSlider()
    {
        expSlider.maxValue = player.MaxExp;
        expSlider.value = player.Exp;
    }

    public void UpdateStats()
    {
        damageText.text = (player.BaseDamage + player.PlayerActionController.CurrentWeap.Damage).ToString();
        fireRateText.text = (player.BaseFireRate + player.PlayerActionController.CurrentWeap.FireRate).ToString();
    }

    public void UpdateStageText(int stageNum)
    {
        stageText.text = "STAGE: " + stageNum.ToString();
    }

    public void UpdateScoreText(int score)
    {
        scoreText.text = score.ToString();
    }

    public void ShowDeathScreen()
    {
        Time.timeScale = 0;

        endPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
