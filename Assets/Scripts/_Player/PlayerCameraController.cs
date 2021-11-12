using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Camera - Look Parameters")]
    [SerializeField, Range(1, 10)] float lookSpeedX = 2;
    [SerializeField, Range(1, 10)] float lookSpeedY = 2;
    [SerializeField, Range(1, 180)] float upperLookLimit = 80;
    [SerializeField, Range(1, 180)] float lowerLookLimit = 80;

    float rotX = 0;

    [Header("Camera - Zoom Parameters")]
    [SerializeField] bool inZoom;
    public bool InZoom => inZoom;
    [SerializeField] float timeToZoom = 0.3f;
    [SerializeField] float zoomFOV = 30;

    float defaultFOV;
    Coroutine zoomCoroutine;

    // local references
    PlayerKeybinds playerKeybinds;
    Camera playerCam;
    public Camera PlayerCam => playerCam;

    private void Awake()
    {
        playerKeybinds = GetComponent<PlayerKeybinds>();
        playerCam = GetComponentInChildren<Camera>();

        defaultFOV = playerCam.fieldOfView;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleCamera();
        HandleZoom();
    }

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
        if (Input.GetKeyDown(playerKeybinds.ZoomKey))
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
}
