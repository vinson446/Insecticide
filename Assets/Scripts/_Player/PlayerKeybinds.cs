using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKeybinds : MonoBehaviour
{
    [Header("Keybinds - Movement")]
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode SprintKey => sprintKey;
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    public KeyCode JumpKey => jumpKey;
    [SerializeField] KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode CrouchKey => crouchKey;

    [Header("Keybinds - Combat/Interaction")]
    [SerializeField] KeyCode shootKey = KeyCode.Mouse0;
    [SerializeField] KeyCode zoomKey = KeyCode.Mouse1;
    public KeyCode ZoomKey => zoomKey;
    public KeyCode ShootKey => shootKey;
    [SerializeField] KeyCode weap1Key = KeyCode.Alpha1;
    public KeyCode Weap1Key => weap1Key;
    [SerializeField] KeyCode weap2Key = KeyCode.Alpha2;
    public KeyCode Weap2Key => weap2Key;
    [SerializeField] KeyCode weap3Key = KeyCode.Alpha3;
    public KeyCode Weap3Key => weap3Key;

    [SerializeField] KeyCode interactKey = KeyCode.E;
    public KeyCode InteractKey => interactKey;
}
