using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackTrigger : MonoBehaviour
{
    SphereCollider coll;

    Enemy enemy;

    void Awake()
    {
        enemy = GetComponentInParent<Enemy>();

        coll = GetComponentInChildren<SphereCollider>();
        coll.radius = enemy.AtkRange;

        enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            enemy.Attack();
        }
    }
}
