using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawner Parameters")]
    [SerializeField] int numEnemies;
    [SerializeField] int numEnemiesToSpawn;

    GameManager gameManager;

    void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnWave()
    {

    }
}
