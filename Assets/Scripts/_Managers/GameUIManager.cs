using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] Slider healthSlider;
    [SerializeField] Slider staminaSlider;

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
    }

    public void UpdateHealthSlider()
    {
        healthSlider.value = player.CurrentHealth;
    }

    public void UpdateStaminaSlider()
    {
        staminaSlider.value = player.CurrentStamina;
    }
}
