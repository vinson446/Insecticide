using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    [Header("Combat Parameters")]
    [SerializeField] int ammo;
    public int Ammo { get => ammo; set => ammo = value; }
    [SerializeField] int damage;
    public int Damage { get => damage; set => damage = value; }
    [SerializeField] float fireRate;
    public float FireRate { get => fireRate; set => fireRate = value; }

    [Header("Effects")]
    [SerializeField] GameObject gunFireVFX;
    [SerializeField] AudioClip gunFireSFX;

    [SerializeField] AudioClip noAmmoSFX;

    // references
    protected PlayerCameraController playerCamController;

    protected void Awake()
    {
        playerCamController = GetComponentInParent<PlayerCameraController>();
    }

    public abstract void Shoot();

    protected abstract void PlayGunFireAnimation();

    protected abstract void PlayGunFireVFX();

    protected abstract void PlayGunFireSFX();

    protected abstract void PlayNoAmmoSFX();
}
