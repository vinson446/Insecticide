
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables
    // functional checks
    bool isSprinting => Input.GetKey(sprintKey);
    bool canJumpNow => charController.isGrounded && Input.GetKey(jumpKey);
    bool canCrouchNow => (!inCrouchAnim && charController.isGrounded && (Input.GetKeyDown(crouchKey)
        || isCrouching && (Input.GetKeyUp(crouchKey))));

    [Header("Debugger - Movement")]
    [SerializeField] float currentSpeed;

    [Header("Debugger - Shooting")]
    [SerializeField] float currentAmmo;
    [SerializeField] float maxAmmo;
    [SerializeField] float damage;
    [SerializeField] float fireRate;

    [Header("Keybinds")]
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] KeyCode shootKey = KeyCode.Mouse0;
    [SerializeField] KeyCode zoomKey = KeyCode.Mouse1;
    [SerializeField] KeyCode interactKey = KeyCode.E;

    [Header("Movement - Speed Parameters")]
    [SerializeField] float walkSpeed = 3;
    [SerializeField] float sprintSpeed = 6;
    [SerializeField] float crouchSpeed = 1;
    [SerializeField] float slideSpeed = 8;

    [Header("Movement - Jumping Parameters")]
    [SerializeField] float jumpForce = 8;
    [SerializeField] float gravity = 30;

    [Header("Movement - Crouching Parameters")]
    [SerializeField] float crouchingHeight = 0.5f;
    [SerializeField] float standingHeight = 2;
    [SerializeField] float timeToCrouch = 0.25f;
    Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    Vector3 standingCenter = new Vector3(0, 0, 0);
    bool isCrouching;
    bool inCrouchAnim;

    [Header("Camera - Look Parameters")]
    [SerializeField, Range(1, 10)] float lookSpeedX = 2;
    [SerializeField, Range(1, 10)] float lookSpeedY = 2;
    [SerializeField, Range(1, 180)] float upperLookLimit = 80;
    [SerializeField, Range(1, 180)] float lowerLookLimit = 80;

    [Header("Camera - Zoom Parameters")]
    [SerializeField] bool inZoom;
    [SerializeField] float timeToZoom = 0.3f;
    [SerializeField] float zoomFOV = 30;
    float defaultFOV;
    Coroutine zoomCoroutine;

    [Header("Interaction Parameters")]
    [SerializeField] float interactDistance;
    [SerializeField] LayerMask interactLayer;
    Vector3 interactRayPoint;
    Interactable currentInteractable;
    bool inFocus;

    // movement 
    Vector2 moveInput;
    Vector3 moveDir;
    float rotX = 0;

    // sliding
    Vector3 hitPointNormal;
    bool isSliding
    {
        get 
        {
            if (charController.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 2))
            {
                hitPointNormal = slopeHit.normal;
                return Vector3.Angle(hitPointNormal, Vector3.up) > charController.slopeLimit;
            }
            else
                return false;
        }
    }

    // shooting
    float nextTimeToShoot;

    // local references
    Camera playerCam;
    CharacterController charController;
    #endregion

    private void Awake()
    {
        playerCam = GetComponentInChildren<Camera>();
        charController = GetComponent<CharacterController>();

        defaultFOV = playerCam.fieldOfView;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // movement
        HandleMovementInput();
        HandleJump();
        HandleCrouch();

        // camera
        HandleCamera();
        HandleZoom();

        // shooting
        HandleShooting();

        // interaction
        HandleInteractCheck();
        HandleInteractInput();
            
        ApplyFinalMovement();
    }

    #region Movement
    void HandleMovementInput()
    {
        currentSpeed = (isCrouching ? crouchSpeed : isSprinting ? sprintSpeed : walkSpeed);
        moveInput = currentSpeed * new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));

        float moveDirY = moveDir.y;
        moveDir = (transform.TransformDirection(Vector3.forward) * moveInput.x) + (transform.TransformDirection(Vector3.right) * moveInput.y);
        moveDir.y = moveDirY;
    }

    void HandleJump()
    {
        if (canJumpNow)
            moveDir.y = jumpForce;
    }

    void HandleCrouch()
    {
        if (canCrouchNow)
            StartCoroutine(CrouchStand());
    }

    IEnumerator CrouchStand()
    {
        // don't let the player stand if there's an obstacle above it while crouching
        if (isCrouching && Physics.Raycast(playerCam.transform.position, Vector3.up, 1))
            yield break;

        inCrouchAnim = true;

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standingHeight : crouchingHeight;
        float currentHeight = charController.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = charController.center;

        while (timeElapsed < timeToCrouch)
        {
            charController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            charController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        charController.height = targetHeight;
        charController.center = targetCenter;

        isCrouching = !isCrouching;

        inCrouchAnim = false;
    }

    void ApplyFinalMovement()
    {
        if (!charController.isGrounded)
            moveDir.y -= gravity * Time.deltaTime;

        if (isSliding)
        {
            currentSpeed = slideSpeed;
            moveDir += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * currentSpeed;
        }

        charController.Move(moveDir * Time.deltaTime);
    }
    #endregion

    #region Camera
    void HandleCamera()
    {
        // looking up and down - rotate cam
        rotX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotX = Mathf.Clamp(rotX, -upperLookLimit, lowerLookLimit);
        playerCam.transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        // turning around - rotate player
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }

    void HandleZoom()
    {
        if (Input.GetKeyDown(zoomKey))
        {
            if (zoomCoroutine != null)
            {
                StopCoroutine(zoomCoroutine);
                zoomCoroutine = null;
            }

            inZoom = !inZoom;
            zoomCoroutine = StartCoroutine(ToggleZoom(inZoom));
        }
    }

    IEnumerator ToggleZoom(bool startZoom)
    {
        float targetFOV = startZoom ? zoomFOV : defaultFOV;
        float startingFOV = playerCam.fieldOfView;
        float timeElapsed = 0;

        while (timeElapsed < timeToZoom)
        {
            playerCam.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, timeElapsed / timeToZoom);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        playerCam.fieldOfView = targetFOV;

        zoomCoroutine = null;
    }
    #endregion

    #region Combat
    void HandleShooting()
    {
        if (Input.GetKey(shootKey) && Time.time >= nextTimeToShoot)
        {
            nextTimeToShoot = Time.time + 1 / fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        print("shoot");
    }

    #endregion

    #region Interact
    void HandleInteractCheck()
    {
        if (Physics.Raycast(playerCam.ViewportPointToRay(interactRayPoint), out RaycastHit hit, interactDistance))
        {
            if (hit.collider.gameObject.layer == 11 && (currentInteractable == null || hit.collider.gameObject.GetInstanceID() != currentInteractable.GetInstanceID()))
            {
                hit.collider.TryGetComponent(out currentInteractable);

                if (currentInteractable && !inFocus)
                {
                    currentInteractable.OnFocus();
                    inFocus = true;
                }
            }
        }
        else if (currentInteractable)
        {
            currentInteractable.OnUnfocus();
            currentInteractable = null;
            inFocus = false;
        }
    }

    void HandleInteractInput()
    {
        if (Input.GetKeyDown(interactKey) && currentInteractable != null && Physics.Raycast(playerCam.ViewportPointToRay(interactRayPoint), out RaycastHit hit, interactDistance, interactLayer))
        {
            currentInteractable.OnInteract();
        }
    }
    #endregion
}
