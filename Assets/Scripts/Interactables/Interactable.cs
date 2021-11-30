using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected AudioClip pickUpSFX;
    protected AudioSource audioSource;

    public virtual void Awake()
    {
        gameObject.layer = 11;
        audioSource = GetComponent<AudioSource>();
    }

    /*
    public abstract void OnInteract();
    public abstract void OnFocus();
    public abstract void OnUnfocus();
    */

    public abstract void PickUp(Collider c);
}
