using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class Player : MonoBehaviour, IDamageable<int>, IKillable
{
    [Header("Player Parameters")]
    [SerializeField] float currentHealth;
    public float CurrentHealth => currentHealth;
    [SerializeField] float maxHealth;
    public float MaxHealth => maxHealth;

    [SerializeField] float currentStamina;
    public float CurrentStamina => currentStamina;
    [SerializeField] float maxStamina;
    public float MaxStamina => maxStamina;
    [SerializeField] float staminaUsageMultiplier;
    [SerializeField] float staminaRecoveryMultiplier;

    [Header("Events")]
    public UnityEvent TakeDamageEvent;

    [Header("Effects - VFX")]
    [SerializeField] VisualEffect[] shootVFXs;

    [Header("Effects - SFX")]
    [SerializeField] AudioClip shootSFX;
    [SerializeField] AudioClip noAmmoSFX;

    [SerializeField] AudioClip takeDamageSFX;
    [SerializeField] AudioClip deathSFX;

    [SerializeField] AudioClip moveSFX;
    [SerializeField] float walkingStepTimer;
    [SerializeField] float runningStepTimer;
    bool takeStep = true;

    // local references
    PlayerMovementController playerMovementController;
    PlayerActionController playerActionController;
    CharacterController charController;
    AudioSource audioSource;

    GameUIManager gameUIManager;

    Coroutine footstepsCoroutine;

    private void Awake()
    {
        playerMovementController = GetComponent<PlayerMovementController>();
        playerActionController = GetComponent<PlayerActionController>();
        charController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

        gameUIManager = FindObjectOfType<GameUIManager>();

        currentHealth = maxHealth;
        currentStamina = maxStamina;

        takeStep = true;
    }

    private void Update()
    {
        if (charController.isGrounded && charController.velocity.magnitude > 2 && takeStep)
        {
            PlayFootstepsSound();
        }

        if (playerMovementController.IsSprinting)
            UseStamina();
        else
            RecoverStamina();
    }

    #region Combat Effects
    public void PlayShootEffects(int weapIndex)
    {
        shootVFXs[weapIndex].Play();
        audioSource.PlayOneShot(shootSFX);
    }

    public void PlayNoAmmoSFX()
    {
        audioSource.PlayOneShot(noAmmoSFX);
    }

    public void TakeDamage(int damageTaken)
    {
        print($"Player took {damageTaken} damage");

        currentHealth -= damageTaken;

        TakeDamageEvent.Invoke();

        if (currentHealth <= 0)
            Die();
    }

    public void Die()
    {
        print("Player died");
    }
    #endregion

    #region Movement Effects
    void UseStamina()
    {
        currentStamina -= staminaUsageMultiplier * Time.deltaTime;

        if (currentStamina < 0)
            currentStamina = 0;

        gameUIManager.UpdateStaminaSlider();
    }

    void RecoverStamina()
    {
        currentStamina += staminaRecoveryMultiplier * Time.deltaTime;

        if (currentStamina > maxStamina)
            currentStamina = maxStamina;

        gameUIManager.UpdateStaminaSlider();
    }

    void PlayFootstepsSound()
    {
        audioSource.clip = moveSFX;
        audioSource.Play();

        // walking
        if (!playerMovementController.IsSprinting)
        {
            footstepsCoroutine = StartCoroutine(WaitForFootsteps(walkingStepTimer));
        }
        // running
        else
        {
            footstepsCoroutine = StartCoroutine(WaitForFootsteps(runningStepTimer));
        }
    }

    IEnumerator WaitForFootsteps(float walkingStepTimer)
    {
        takeStep = false;

        yield return new WaitForSeconds(walkingStepTimer);

        takeStep = true;
    }
    #endregion
}
