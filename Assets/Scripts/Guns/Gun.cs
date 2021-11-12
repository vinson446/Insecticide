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

    [Header("References")]
    [SerializeField] GameObject bulletObj;
    protected GameObject BulletObj => bulletObj;
    [SerializeField] Transform bulletSpawnTrans;
    protected Transform BulletSpawnTrans { get => bulletSpawnTrans; set => bulletSpawnTrans = value; }
    [SerializeField] int bulletSpeed;
    protected int BulletSpeed { get => bulletSpeed; set => bulletSpeed = value; }

    // references
    protected PlayerCameraController playerCamController;

    protected void Awake()
    {
        playerCamController = GetComponentInParent<PlayerCameraController>();
    }

    public abstract void Shoot();
}
