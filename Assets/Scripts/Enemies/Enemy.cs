using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable<int>, IKillable
{
    [Header("Enemy Parameters")]
    [SerializeField] int currentHealth;
    [SerializeField] int maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageTaken)
    {
        Debug.Log($"Enemy Ant took {damageTaken} damage", gameObject);

        currentHealth -= damageTaken;

        if (currentHealth <= 0)
            Die();
    }

    public void Die()
    {
        Debug.Log("Enemy Ant died", gameObject);

        gameObject.SetActive(false);
    }
}
