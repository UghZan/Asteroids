using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Required Elements")]
    [SerializeField] PlayerHealth PlayerHealth;
    [SerializeField] PlayerScore PlayerScore;
    [SerializeField] SpawnManager SpawningManager;
    [SerializeField] PlayerController PlayerControls;

    [Header("UI Elements")]
    [SerializeField] TextMeshProUGUI ScoreText;
    [SerializeField] TextMeshProUGUI LivesText;
    [SerializeField] GameObject PauseMenu;
    [SerializeField] Button PauseMenuContinueButton;
    [SerializeField] TextMeshProUGUI PauseMenuControlButtonText;
    [SerializeField] GameObject GameOverText;
    // Start is called before the first frame update
    void Start()
    {
        PauseMenuContinueButton.interactable = GameSettings.GameStarted;
        PauseMenuControlButtonText.text = "Controls: " + (GameSettings.ControlScheme ? "KB+M" : "KB");
        PlayerHealth.OnDeath.AddListener(GameOver);
    }

    void GameOver()
    {
        GameOverText.SetActive(true);
        GameSettings.GameStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        LivesText.text = "Lives: " + PlayerHealth.Lives;
        ScoreText.text = "Score: " + string.Format("{0:d8}", PlayerScore.Score);

        if (Input.GetKeyDown(KeyCode.Escape))
            Pause();
    }

    public void Pause()
    {
        if (GameSettings.GameStarted || !GameSettings.GamePaused) //first one doesn't allow to exit out of pause menu when game hasn't even started yet
        {                                                         //second one allows to open pause menu if game has ended (game over sequence)
            GameOverText.SetActive(false);
            GameSettings.GamePaused = !GameSettings.GamePaused;

            PauseMenu.SetActive(GameSettings.GamePaused);
            PauseMenuContinueButton.interactable = GameSettings.GameStarted;
        }
    }

    public void NewGame()
    {
        PlayerHealth.gameObject.SetActive(true);
        PlayerScore.ResetToStart();
        PlayerHealth.ResetToStart();
        PlayerControls.ResetToStart();
        SpawningManager.ResetToStart();
        GameSettings.GameStarted = true;
        Pause();
    }

    public void SwitchControlScheme()
    {
        GameSettings.ControlScheme = !GameSettings.ControlScheme;

        PauseMenuControlButtonText.text = "Controls: " + (GameSettings.ControlScheme ? "KB+M" : "KB");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

