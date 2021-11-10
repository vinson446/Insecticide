using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    public float currentAmmo { get; private set; }
    public float maxAmmo { get; private set; }
    public float damage { get; private set; }
    public float fireRate { get; private set; }


    public abstract void Shoot();
    public abstract void Reload();
}
