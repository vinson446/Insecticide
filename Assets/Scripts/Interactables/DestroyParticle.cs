using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticle : MonoBehaviour
{
    [SerializeField] float destroyTime;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, destroyTime);
    }
}
