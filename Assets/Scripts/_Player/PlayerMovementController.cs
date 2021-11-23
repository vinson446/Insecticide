using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    // functional checks
    bool isSprinting => Input.GetKey(playerKeybinds.SprintKey);
    public bool IsSprinting => isSprinting;
    bool canJumpNow => charController.isGrounded && Input.GetKey(playerKeybinds.JumpKey);
    bool canCrouchNow => (!inCrouchAnim && charController.isGrounded && (Input.GetKeyDown(playerKeybinds.CrouchKey)
        || isCrouching && (Input.GetKeyUp(playerKeybinds.CrouchKey))));

    [Header("Debugger - Movement")]
    [SerializeField] float currentSpeed;

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

    // movement 
    Vector2 moveInput;
    Vector3 moveDir;

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

    // local references
    PlayerCameraController playerCamController;
    PlayerAnimationController playerAnimController;
    PlayerKeybinds playerKeybinds;
    CharacterController charController;

    private void Awake()
    {
        playerCamController = GetComponent<PlayerCameraController>();
        playerAnimController = GetComponent<PlayerAnimationController>();
        playerKeybinds = GetComponent<PlayerKeybinds>();
        charController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovementInput();
        HandleJump();
        HandleCrouch();
            
        ApplyFinalMovement();
    }

    void HandleMovementInput()
    {
        currentSpeed = isCrouching || playerCamController.InZoom ? crouchSpeed : isSprinting ? sprintSpeed : walkSpeed;
        moveInput = (isCrouching || playerCamController.InZoom ? crouchSpeed : isSprinting ? sprintSpeed : walkSpeed) * new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));

        float moveDirY = moveDir.y;
        moveDir = (transform.TransformDirection(Vector3.forward) * moveInput.x) + (transform.TransformDirection(Vector3.right) * moveInput.y);
        moveDir.y = moveDirY;

        // animation
        if (moveInput == Vector2.zero)
            playerAnimController.PlayIdleAnimation();
        else if (currentSpeed == sprintSpeed)
            playerAnimController.PlayRunningAnimation();
        else if (currentSpeed == walkSpeed)
            playerAnimController.PlayWalkingAnimation();
    }

    void HandleJump()
    {
        if (canJumpNow)
            moveDir.y = jumpForce;
    }

    void HandleCrouch()
    {
        if (canCrouchNow)
            StartCoroutine(CrouchStandCoroutine());
    }

    IEnumerator CrouchStandCoroutine()
    {
        // don't let the player stand if there's an obstacle above it while crouching
        if (isCrouching && Physics.Raycast(playerCamController.PlayerCam.transform.position, Vector3.up, 1))
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
            moveDir += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slideSpeed;
        }

        charController.Move(moveDir * Time.deltaTime);
    }
}
