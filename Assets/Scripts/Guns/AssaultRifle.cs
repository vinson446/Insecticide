using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssaultRifle : Gun
{
    private new void Awake()
    {
        base.Awake();
    }

    public override void Shoot()
    {
        if (Physics.Raycast(playerCamController.PlayerCam.transform.position, playerCamController.PlayerCam.transform.forward, out RaycastHit hit, 1000))
        {
            IDamageable<int> damageable = hit.transform.GetComponentInParent<IDamageable<int>>();

            if (damageable != null)
                damageable.TakeDamage(Damage);
        }
    }

    protected override void PlayGunFireAnimation()
    {
        throw new System.NotImplementedException();
    }

    protected override void PlayGunFireVFX()
    {
        throw new System.NotImplementedException();
    }

    protected override void PlayGunFireSFX()
    {
        throw new System.NotImplementedException();
    }

    protected override void PlayNoAmmoSFX()
    {
        throw new System.NotImplementedException();
    }
}
