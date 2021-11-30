using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : Interactable
{
    [SerializeField] int rifleAmmoAmt;
    [SerializeField] int shotgunAmmoAmt;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PickUp(other);

            Renderer[] rend = GetComponentsInChildren<Renderer>();
            foreach (Renderer r in rend)
            {
                r.enabled = false;
            }
            Destroy(gameObject, 3);
        }
    }

    public override void PickUp(Collider other)
    {
        Player player = other.GetComponent<Player>();
        player.RecoverAmmo(rifleAmmoAmt, shotgunAmmoAmt);

        audioSource.PlayOneShot(pickUpSFX);
    }
}
