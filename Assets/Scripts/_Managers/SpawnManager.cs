using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawner Parameters")]
    [SerializeField] float spawnRate;
    public float SpawnRate { get => spawnRate; set => spawnRate = value; }
    [SerializeField] int bossChance;
    [SerializeField] int spawnCap;
    public int SpawnCap { get => spawnCap; set => spawnCap = value; }
    [SerializeField] int numEnemiesRightNow;
    public int NumEnemiesRightNow { get => numEnemiesRightNow; set => numEnemiesRightNow = value; }
    float nextTimeToSpawn;
    [SerializeField] float spawnCheckDistance;

    [Header("Spawner References")]
    [SerializeField] Transform[] spawnLocations;
    [SerializeField] GameObject[] enemyObjs;

    GameManager gameManager;
    Player player;

    void Awake()
    {
        gameManager = GetComponent<GameManager>();
        player = FindObjectOfType<Player>();
    }

    private void Start()
    {
        InvokeRepeating("CheckDistance", 1, 3);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextTimeToSpawn && numEnemiesRightNow < spawnCap)
        {
            nextTimeToSpawn = Time.time + 1 / (spawnRate * gameManager.StageNum);
            SpawnEnemy();
        }
    }

    void CheckDistance()
    {
        foreach (Transform t in spawnLocations)
        {
            if (Vector3.Distance(t.position, player.transform.position) <= spawnCheckDistance)
            {
                t.gameObject.SetActive(true);
            }
            else
            {
                t.gameObject.SetActive(false);
            }
        }
    }

    void SpawnEnemy()
    {
        bossChance = Random.Range(0, 100);

        int rand = Random.Range(0, spawnLocations.Length);

        if (bossChance == 1)
        {
            if (spawnLocations[rand].gameObject.activeInHierarchy)
                Instantiate(enemyObjs[1], spawnLocations[rand].position, transform.rotation);
        }
        else
        {
            if (spawnLocations[rand].gameObject.activeInHierarchy)
                Instantiate(enemyObjs[0], spawnLocations[rand].position, transform.rotation);
        }

        numEnemiesRightNow++;
    }
}
