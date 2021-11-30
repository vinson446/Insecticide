using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Current")]
    [SerializeField] int stageNum;
    public int StageNum => stageNum;
    [SerializeField] int score;
    public int Score => score;

    [Header("Debugger")]
    [SerializeField] int neededScoreForNextStage;
    [SerializeField] int stageMultiplier;

    SpawnManager spawnManager;

    void Awake()
    {
        spawnManager = GetComponent<SpawnManager>();

        neededScoreForNextStage = stageNum * stageMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreaseScore(int points)
    {
        score += points;

        if (score >= neededScoreForNextStage)
        {
            IncreaseStageNum();
        }
    }

    void IncreaseStageNum()
    {
        stageNum++;
        neededScoreForNextStage = stageNum * stageMultiplier;
    }
}
