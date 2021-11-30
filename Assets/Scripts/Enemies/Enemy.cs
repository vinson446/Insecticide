using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable<int>, IKillable
{
    [Header("Enemy Parameters")]
    [SerializeField] int currentHealth;
    [SerializeField] int maxHealth;
    [SerializeField] int exp;

    [Header("Combat Parameters")]
    [SerializeField] int damage;
    [SerializeField] float atkSpeed;
    [SerializeField] int moveSpeed;
    bool canAttack;

    [Header("Effects")]
    [SerializeField] AudioClip deathSFX;

    NavMeshAgent agent;
    AudioSource audioSource;

    Player player;

    private void Awake()
    {
        agent = GetComponentInChildren<NavMeshAgent>();
        agent.speed = moveSpeed;
        audioSource = GetComponent<AudioSource>();

        player = FindObjectOfType<Player>();

        currentHealth = maxHealth;
    }

    private void Update()
    {
        MoveTowardsPlayer();
        RotateTowardsPlayer();
    }

    void MoveTowardsPlayer()
    {
        agent.SetDestination(player.transform.position);
    }

    void RotateTowardsPlayer()
    {
        Vector3 dir = transform.position - player.transform.position;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 0.1f);
        }
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

        // play death sound
        if (audioSource != null)
        {
            audioSource.volume = 0.2f;
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(deathSFX);
        }

        player.GainExp(exp);

        Renderer[] rends = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rends)
        {
            r.enabled = false;
        }
        Destroy(gameObject, 3);
    }
}
