using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable<int>, IKillable
{
    [Header("Player Parameters")]
    [SerializeField] int currentHealth;
    [SerializeField] int maxHealth;
    [SerializeField] int currentStamina;
    [SerializeField] int maxStamina;

    private void Awake()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    public void TakeDamage(int damageTaken)
    {
        print($"Player took {damageTaken} damage");

        currentHealth -= damageTaken;

        if (currentHealth <= 0)
            Die();
    }

    public void Die()
    {
        print("Player died");
    }
}
