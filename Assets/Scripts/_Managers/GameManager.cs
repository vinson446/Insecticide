using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Current")]
    [SerializeField] int stageNum = 1;
    public int StageNum => stageNum;
    [SerializeField] int score;
    public int Score => score;

    [Header("Debugger")]
    [SerializeField] int neededScoreForThisStage = 500;

    GameUIManager gameUIManager;
    SpawnManager spawnManager;

    void Awake()
    {
        gameUIManager = GetComponent<GameUIManager>();
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
        gameUIManager.UpdateScoreText(score);

        if (score >= neededScoreForThisStage)
        {
            IncreaseStageNum();
        }
    }

    void IncreaseStageNum()
    {
        stageNum++;
        spawnManager.SpawnRate *= stageNum;

        neededScoreForThisStage *= stageNum;

        gameUIManager.UpdateStageText(stageNum);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ReplayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
