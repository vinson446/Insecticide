using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssaultRifle : Gun
{
    private new void Awake()
    {
        base.Awake();
    }

    public override void Shoot(int increment)
    {
        if (Physics.Raycast(playerCamController.PlayerCam.transform.position, playerCamController.PlayerCam.transform.forward, out RaycastHit hit, range))
        {
            IDamageable<int> damageable = hit.transform.GetComponentInParent<IDamageable<int>>();

            if (damageable != null && hit.transform.tag != "Player")
                damageable.TakeDamage(Damage + increment);

            if (hitVFX != null)
                Instantiate(hitVFX, hit.point, Quaternion.identity);

            CurrentAmmo -= 1;
        }
    }
}
