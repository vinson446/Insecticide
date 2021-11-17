using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Gun
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
}
