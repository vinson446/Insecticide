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
    [SerializeField] int score;

    [Header("Combat Parameters")]
    [SerializeField] int damage;
    [SerializeField] float atkSpeed;
    [SerializeField] float atkRange;
    public float AtkRange => atkRange;
    [SerializeField] Transform atkTransformPoint;
    bool canAttack;
    float nextTimeToAttack;

    [Header("Movement Parameters")]
    [SerializeField] int moveSpeed;
    [SerializeField] float rotSpeed;
    [SerializeField] float stopRange;

    [Header("Effects")]
    [SerializeField] AudioClip attackSFX;
    [SerializeField] AudioClip deathSFX;

    Animator animator;
    NavMeshAgent agent;
    AudioSource audioSource;

    GameManager gameManager;
    Player player;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponentInChildren<NavMeshAgent>();
        agent.speed = moveSpeed;
        audioSource = GetComponent<AudioSource>();

        gameManager = FindObjectOfType<GameManager>();
        player = FindObjectOfType<Player>();

        IncreaseStats();

        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (!(Vector3.Distance(transform.position, player.transform.position) <= stopRange))
        {
            agent.speed = moveSpeed;

            MoveTowardsPlayer();
            RotateTowardsPlayer();
        }
        else
        {
            agent.speed = 0;
        }
    }

    void IncreaseStats()
    {
        maxHealth *= gameManager.StageNum;
        exp *= gameManager.StageNum;
        damage *= gameManager.StageNum;
        moveSpeed *= gameManager.StageNum;
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
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotSpeed * Time.deltaTime * 0.1f);
        }
    }

    public void TakeDamage(int damageTaken)
    {
        currentHealth -= damageTaken;

        if (currentHealth <= 0)
            Die();
    }

    public void Attack()
    {
        if (Time.time >= nextTimeToAttack)
        {
            nextTimeToAttack = Time.time + 1 / atkSpeed;
            player.TakeDamage(damage);
        }
    }

    public void Die()
    {
        // play death sound
        if (audioSource != null)
        {
            audioSource.volume = 0.2f;
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(deathSFX);
        }

        player.GainExp(exp);
        gameManager.IncreaseScore(score);

        Collider[] colls = GetComponentsInChildren<Collider>();
        foreach (Collider c in colls)
        {
            c.enabled = false;
        }
        animator.CrossFadeInFixedTime("Death", 0.1f);

        Destroy(gameObject, 3);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(atkTransformPoint.position, atkRange);
    }
}
