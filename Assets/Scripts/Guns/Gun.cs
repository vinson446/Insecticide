using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    [Header("Combat Parameters")]
    [SerializeField] int currentAmmo;
    public int CurrentAmmo { get => currentAmmo; set => currentAmmo = value; }
    [SerializeField] int maxAmmo;
    public int MaxAmmo { get => maxAmmo; set => maxAmmo = value; }
    [SerializeField] int damage;
    public int Damage { get => damage; set => damage = value; }
    [SerializeField] float fireRate;
    public float FireRate { get => fireRate; set => fireRate = value; }
    [SerializeField] protected float range;

    [Header("Effects")]
    [SerializeField] protected GameObject hitVFX;
    [SerializeField] protected AudioClip hitSFX;

    // references
    protected PlayerCameraController playerCamController;

    protected void Awake()
    {
        playerCamController = GetComponentInParent<PlayerCameraController>();
    }

    public abstract void Shoot(int increment);
}
