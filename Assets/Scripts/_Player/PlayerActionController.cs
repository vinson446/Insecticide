using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionController : MonoBehaviour
{
    [Header("Debugger - Combat")]
    [SerializeField] Gun currentWeap;
    [SerializeField] Gun[] weapList;
    [SerializeField] float ammo;
    public float Ammo { get => ammo; set => ammo = value; }
    [SerializeField] float damage;
    public float Damage { get => damage; set => damage = value; }
    [SerializeField] float fireRate;
    public float FireRate { get => fireRate; set => fireRate = value; }

    float nextTimeToShoot;
    Coroutine switchWeapCoroutine;

    [Header("Debugger - Interaction")]
    [SerializeField] Interactable interactable;

    [Header("Interaction Parameters")]
    [SerializeField] float interactDistance = 3;
    [SerializeField] LayerMask interactLayer;

    Vector3 interactRayPoint;
    Interactable currentInteractable;
    bool inFocus;

    // local references
    PlayerKeybinds playerKeybinds;
    PlayerCameraController playerCamController;

    void Awake()
    {
        playerKeybinds = GetComponent<PlayerKeybinds>();
        playerCamController = GetComponent<PlayerCameraController>();

        interactRayPoint = new Vector3(0.5f, 0.5f, 0);

        currentWeap = weapList[0];
        SetWeaponStats();
    }

    // Update is called once per frame
    void Update()
    {
        // combat
        HandleShooting();
        HandleSwitchingWeapon();

        // interaction
        HandleInteractFocus();
        HandleInteractInput();
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
        }

        currentWeap.gameObject.SetActive(true);
        SetWeaponStats();
    }

    void SetWeaponStats()
    {
        ammo = currentWeap.Ammo;
        damage = currentWeap.Damage;
        fireRate = currentWeap.FireRate;
    }

    void HandleShooting()
    {
        if (Input.GetKey(playerKeybinds.ShootKey) && Time.time >= nextTimeToShoot)
        {
            nextTimeToShoot = Time.time + 1 / fireRate;
            currentWeap.Shoot();
        }
    }
    #endregion

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
}
