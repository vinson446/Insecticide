using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] AudioClip buttonClick;

    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ButtonClick()
    {
        audioSource.PlayOneShot(buttonClick);
    }

    public void GoToGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
