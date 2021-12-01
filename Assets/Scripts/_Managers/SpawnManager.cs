using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawner Parameters")]
    [SerializeField] float spawnRate;
    [SerializeField] int bossChance;
    float nextTimeToSpawn;

    [Header("Spawner References")]
    [SerializeField] Transform[] spawnLocations;
    [SerializeField] GameObject[] enemyObjs;

    GameManager gameManager;

    void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextTimeToSpawn)
        {
            nextTimeToSpawn = Time.time + 1 / (spawnRate * gameManager.StageNum);
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        bossChance = Random.Range(0, 100);

        if (bossChance == 1)
            Instantiate(enemyObjs[1], spawnLocations[Random.Range(0, spawnLocations.Length)].position, transform.rotation);
        else
            Instantiate(enemyObjs[0], spawnLocations[Random.Range(0, spawnLocations.Length)].position, transform.rotation);
    }
}
