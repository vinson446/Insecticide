using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class Player : MonoBehaviour, IDamageable<int>, IKillable
{
    [Header("Game Parameters")]
    [SerializeField] int level;
    public int Level => level;
    [SerializeField] int exp;
    public int Exp => exp;
    [SerializeField] int maxExp;
    public int MaxExp => maxExp;
    [SerializeField] int baseDamage;
    public int BaseDamage => baseDamage;
    [SerializeField] float baseFireRate;
    public float BaseFireRate => baseFireRate;
    [SerializeField] int damageIncrement;
    [SerializeField] float fireRateIncrement;

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
    [SerializeField] float staminaRecoveryTime;
    bool recoveringStamina;

    [Header("Events")]
    public UnityEvent TakeDamageEvent;

    [Header("Effects - VFX")]
    [SerializeField] VisualEffect[] shootVFXs;
    [SerializeField] GameObject lvlUpVFX;
    [SerializeField] GameObject healVFX;

    [SerializeField] GameObject runVFX;
    bool isRunning;

    [Header("Effects - SFX")]
    [SerializeField] AudioClip shootSFX;
    [SerializeField] AudioClip noAmmoSFX;

    [SerializeField] AudioClip takeDamageSFX;

    [SerializeField] AudioClip lvlUpSFX;

    [SerializeField] AudioClip moveSFX;
    [SerializeField] float walkingStepTimer;
    [SerializeField] float runningStepTimer;
    [SerializeField] AudioClip jumpSound;
    bool takeStep = true;

    [SerializeField] AudioSource playerAudioSource;
    [SerializeField] AudioSource gunAudioSource;

    // local references
    PlayerMovementController playerMovementController;
    PlayerActionController playerActionController;
    public PlayerActionController PlayerActionController => playerActionController;
    PlayerKeybinds playerKeybinds;
    CharacterController charController;

    GameManager gameManager;
    GameUIManager gameUIManager;

    Coroutine footstepsCoroutine;

    private void Awake()
    {
        playerMovementController = GetComponent<PlayerMovementController>();
        playerActionController = GetComponent<PlayerActionController>();
        playerKeybinds = GetComponent<PlayerKeybinds>();
        charController = GetComponent<CharacterController>();
        playerAudioSource = GetComponent<AudioSource>();

        gameManager = FindObjectOfType<GameManager>();
        gameUIManager = FindObjectOfType<GameUIManager>();

        currentHealth = maxHealth;
        currentStamina = maxStamina;

        takeStep = true;
    }

    private void Update()
    {
        if (charController.isGrounded && charController.velocity.magnitude > 2 && takeStep && !Input.GetKey(playerKeybinds.JumpKey))
        {
            PlayFootstepsSound();
        }

        if (playerMovementController.IsSprinting)
            UseStamina();
        else
            RecoverStamina();
    }

    public void GainExp(int xp)
    {
        exp += xp;

        if (exp >= maxExp)
        {
            int remainder = exp - 100;
            exp = remainder;

            LevelUp();
        }

        gameUIManager.UpdateExpSlider();
    }

    void LevelUp()
    {
        level++;
        maxExp += maxExp;

        baseDamage += damageIncrement;
        baseFireRate += fireRateIncrement;

        playerAudioSource.pitch = Random.Range(0.95f, 1.05f);
        playerAudioSource.volume = 1f;
        playerAudioSource.PlayOneShot(lvlUpSFX);

        lvlUpVFX.SetActive(true);
        StartCoroutine(TurnOffEffect(lvlUpVFX));
        
        gameUIManager.UpdateLevelText();
        gameUIManager.UpdateExpSlider();
        gameUIManager.UpdateStats();

        // update animation times
    }

    IEnumerator TurnOffEffect(GameObject o)
    {
        yield return new WaitForSeconds(3);

        o.SetActive(false);
    }

    #region Combat Effects
    public void PlayShootEffects(int weapIndex)
    {
        shootVFXs[weapIndex].Play();

        switch (weapIndex)
        {
            case 0:
                gunAudioSource.pitch = 1;
                gunAudioSource.volume = 0.2f;
                break;
            case 1:
                gunAudioSource.pitch = 0.75f;
                gunAudioSource.volume = 0.2f;
                break;
            case 2:
                gunAudioSource.pitch = 1.25f;
                gunAudioSource.volume = 0.2f;
                break;
        }
        gunAudioSource.PlayOneShot(shootSFX);
    }

    public void PlayNoAmmoSFX()
    {
        gunAudioSource.pitch = 0.5f;
        gunAudioSource.volume = 0.2f;
        gunAudioSource.PlayOneShot(noAmmoSFX);
    }

    public void TakeDamage(int damageTaken)
    {
        currentHealth -= damageTaken;

        TakeDamageEvent.Invoke();
        
        if (currentHealth <= 0)
            Die();

        if (damageTaken < 0)
        {
            healVFX.SetActive(true);
            StartCoroutine(TurnOffEffect(healVFX));
        }
        else
        {
            playerAudioSource.pitch = Random.Range(0.95f, 1.05f);
            playerAudioSource.volume = 0.2f;
            playerAudioSource.PlayOneShot(takeDamageSFX);

            gameUIManager.Hurt();
        }
    }

    public void Die()
    {
        gameUIManager.ShowDeathScreen();

        PlayerCameraController playerCameraController = GetComponent<PlayerCameraController>();
        playerCameraController.enabled = false;

        playerActionController.enabled = false;
        this.enabled = false;
    }

    public void RecoverAmmo(int rifleAmmoAmt, int shotgunAmmoAmt)
    {
        if (playerActionController.WeapIndex == 0)
            playerActionController.CurrentWeap.CurrentAmmo += rifleAmmoAmt;
        else if (playerActionController.WeapIndex == 1)
            playerActionController.CurrentWeap.CurrentAmmo += shotgunAmmoAmt;

        if (playerActionController.CurrentWeap.CurrentAmmo > playerActionController.CurrentWeap.MaxAmmo)
            playerActionController.CurrentWeap.CurrentAmmo = playerActionController.CurrentWeap.MaxAmmo;

        gameUIManager.UpdateAmmo();
    }
    #endregion

    #region Movement Effects
    void UseStamina()
    {
        currentStamina -= staminaUsageMultiplier * Time.deltaTime;

        if (!isRunning)
        {
            isRunning = true;
            runVFX.SetActive(true);
        }

        if (currentStamina < 0)
        {
            if (!recoveringStamina)
                StartCoroutine(RecoveringStaminaCoroutine());

            currentStamina = 0;
        }

        gameUIManager.UpdateStaminaSlider();
    }

    void RecoverStamina()
    {
        if (isRunning)
        {
            isRunning = false;
            runVFX.SetActive(false);
        }

        if (!recoveringStamina)
        {
            currentStamina += staminaRecoveryMultiplier * Time.deltaTime;

            if (currentStamina > maxStamina)
                currentStamina = maxStamina;

            gameUIManager.UpdateStaminaSlider();
        }
    }

    IEnumerator RecoveringStaminaCoroutine()
    {
        recoveringStamina = true;

        yield return new WaitForSeconds(staminaRecoveryTime);

        recoveringStamina = false;
    }

    void PlayFootstepsSound()
    {
        playerAudioSource.clip = moveSFX;
        playerAudioSource.volume = 0.2f;
        playerAudioSource.pitch = Random.Range(0.95f, 1.05f);
        playerAudioSource.Play();

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

    public void PlayJumpSound()
    {
        playerAudioSource.pitch = Random.Range(0.95f, 1.05f);
        playerAudioSource.volume = 1;
        playerAudioSource.PlayOneShot(jumpSound);
    }
    #endregion
}
