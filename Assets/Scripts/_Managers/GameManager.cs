using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Current")]
    [SerializeField] int stageNum = 1;
    public int StageNum => stageNum;
    [SerializeField] int score;
    public int Score => score;

    [Header("Debugger")]
    [SerializeField] int neededScoreForThisStage = 500;

    SpawnManager spawnManager;

    void Awake()
    {
        spawnManager = GetComponent<SpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            IncreaseStageNum();
    }

    public void IncreaseScore(int points)
    {
        score += points;

        if (score >= neededScoreForThisStage)
        {
            IncreaseStageNum();
        }
    }

    void IncreaseStageNum()
    {
        stageNum++;
        spawnManager.SpawnRate++;

        int tmp = neededScoreForThisStage;
        neededScoreForThisStage = stageNum * tmp;
    }
}
