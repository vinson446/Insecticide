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
    [SerializeField] int chanceToDrop;
    [SerializeField] GameObject[] pickups;
    bool canAttack;
    float nextTimeToAttack;

    [Header("Movement Parameters")]
    [SerializeField] int moveSpeed;
    [SerializeField] float rotSpeed;
    [SerializeField] float stopRange;
    bool isInMovingAnim;

    [Header("Effects")]
    [SerializeField] AudioClip attackSFX;
    [SerializeField] AudioClip deathSFX;

    Animator animator;
    NavMeshAgent agent;
    AudioSource audioSource;

    GameManager gameManager;
    SpawnManager spawnManager;
    Player player;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponentInChildren<NavMeshAgent>();
        agent.speed = moveSpeed;
        audioSource = GetComponent<AudioSource>();

        gameManager = FindObjectOfType<GameManager>();
        spawnManager = FindObjectOfType<SpawnManager>();
        player = FindObjectOfType<Player>();

        IncreaseStats();

        currentHealth = maxHealth;
    }

    private void FixedUpdate()
    {
        if (!(Vector3.Distance(transform.position, player.transform.position) <= stopRange))
        {
            agent.speed = moveSpeed;

            MoveTowardsPlayer();
            RotateTowardsPlayer();
        }
        else
        {
            isInMovingAnim = false;
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

        if (!isInMovingAnim)
        {
            animator.CrossFadeInFixedTime("Move", 0.1f);
            isInMovingAnim = true;
        }
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

            audioSource.volume = 0.2f;
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(attackSFX);
            animator.CrossFadeInFixedTime("Attack", 0.1f);
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
        spawnManager.NumEnemiesRightNow--;

        int drop = Random.Range(0, 100);
        if (drop < chanceToDrop)
        {
            int pick = Random.Range(0, 2);
            Instantiate(pickups[pick], transform.position, transform.rotation);
        }

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
