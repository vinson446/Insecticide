using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerActionController : MonoBehaviour
{
    [Header("Debugger - Combat")]
    [SerializeField] Gun currentWeap;
    public Gun CurrentWeap => currentWeap;
    [SerializeField] Gun[] weapList;
    int weapIndex;
    public int WeapIndex => weapIndex;
    [SerializeField] float ammo;
    public float Ammo { get => ammo; set => ammo = value; }
    [SerializeField] float damage;
    public float Damage { get => damage; set => damage = value; }
    [SerializeField] float fireRate;
    public float FireRate { get => fireRate; set => fireRate = value; }

    float nextTimeToShoot;
    // add weapon switch animation if there's time
    Coroutine switchWeapCoroutine;

    /*
    [Header("Debugger - Interaction")]
    [SerializeField] Interactable interactable;

    [Header("Interaction Parameters")]
    [SerializeField] float interactDistance = 3;
    [SerializeField] LayerMask interactLayer;

    Vector3 interactRayPoint;
    Interactable currentInteractable;
    bool inFocus;
    */

    // local references
    Player player;
    PlayerCameraController playerCamController;
    PlayerAnimationController playerAnimController;
    PlayerKeybinds playerKeybinds;

    GameUIManager gameUIManager;

    void Awake()
    {
        player = GetComponent<Player>();
        playerCamController = GetComponent<PlayerCameraController>();
        playerAnimController = GetComponent<PlayerAnimationController>();
        playerKeybinds = GetComponent<PlayerKeybinds>();

        gameUIManager = FindObjectOfType<GameUIManager>();

        // interactRayPoint = new Vector3(0.5f, 0.5f, 0);

        currentWeap = weapList[0];
        weapIndex = 0;

        SetWeaponStats();
    }

    void SetWeaponStats()
    {
        ammo = currentWeap.CurrentAmmo;
        damage = currentWeap.Damage;
        fireRate = currentWeap.FireRate;
    }

    // Update is called once per frame
    void Update()
    {
        // combat
        HandleShooting();
        HandleSwitchingWeapon();

        // interaction
        /*
        HandleInteractFocus();
        HandleInteractInput();
        */
    }

    #region Combat
    void HandleSwitchingWeapon()
    {
        if (Input.GetKeyDown(playerKeybinds.Weap1Key))
        {
            if (switchWeapCoroutine != null)
            {
                StopCoroutine(switchWeapCoroutine);
                switchWeapCoroutine = null;
            }

            currentWeap.gameObject.SetActive(false);
            currentWeap = weapList[0];
            weapIndex = 0;

            playerAnimController.SetWeaponIndex(0);

            currentWeap.gameObject.SetActive(true);
            SetWeaponStats();

            gameUIManager.UpdateAmmo();
        }
        else if (Input.GetKeyDown(playerKeybinds.Weap2Key))
        {
            if (switchWeapCoroutine != null)
            {
                StopCoroutine(switchWeapCoroutine);
                switchWeapCoroutine = null;
            }

            currentWeap.gameObject.SetActive(false);
            currentWeap = weapList[1];
            weapIndex = 1;

            playerAnimController.SetWeaponIndex(1);

            currentWeap.gameObject.SetActive(true);
            SetWeaponStats();

            gameUIManager.UpdateAmmo();
        }
        else if (Input.GetKeyDown(playerKeybinds.Weap3Key))
        {
            if (switchWeapCoroutine != null)
            {
                StopCoroutine(switchWeapCoroutine);
                switchWeapCoroutine = null;
            }

            currentWeap.gameObject.SetActive(false);
            currentWeap = weapList[2];
            weapIndex = 2;

            playerAnimController.SetWeaponIndex(2);

            currentWeap.gameObject.SetActive(true);
            SetWeaponStats();

            gameUIManager.UpdateAmmo();
        }
    }

    void HandleShooting()
    {
        if (Input.GetKeyDown(playerKeybinds.ShootKey) && currentWeap.CurrentAmmo <= 0)
        {
            player.PlayNoAmmoSFX();
            return;
        }

        if (Input.GetKey(playerKeybinds.ShootKey) && Time.time >= nextTimeToShoot)
        {
            if (currentWeap.CurrentAmmo <= 0)
            {
                playerAnimController.InShootAnim = false;
                return;
            }

            nextTimeToShoot = Time.time + 1 / fireRate;
            currentWeap.Shoot();
            ammo -= 1;

            playerAnimController.PlayShootingAnimation();

            player.PlayShootEffects(weapIndex);

            gameUIManager.UpdateAmmo();
        }

        if (Input.GetKeyUp(playerKeybinds.ShootKey))
            playerAnimController.InShootAnim = false;
    }
    #endregion

    /*
    #region Interact
    void HandleInteractFocus()
    {
        if (Physics.Raycast(playerCamController.PlayerCam.ViewportPointToRay(interactRayPoint), out RaycastHit hit, interactDistance))
        {
            if (hit.collider.gameObject.layer == 11 && (currentInteractable == null || hit.collider.gameObject.GetInstanceID() != currentInteractable.gameObject.GetInstanceID()))
            {
                hit.collider.TryGetComponent(out currentInteractable);

                if (currentInteractable && !inFocus)
                {
                    interactable = currentInteractable;
                    currentInteractable.OnFocus();
                    inFocus = true;
                }
            }
        }
        else if (currentInteractable)
        {
            interactable = null;
            currentInteractable.OnUnfocus();
            currentInteractable = null;
            inFocus = false;
        }
    }

    void HandleInteractInput()
    {
        if (Input.GetKeyDown(playerKeybinds.InteractKey) && currentInteractable != null && Physics.Raycast(playerCamController.PlayerCam.ViewportPointToRay(interactRayPoint), out RaycastHit hit, interactDistance, interactLayer))
        {
            currentInteractable.OnInteract();
        }
    }
    #endregion
    */
}
