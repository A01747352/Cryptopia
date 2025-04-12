using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseButton : MonoBehaviour
{
    [SerializeField] private UIDocument gameUIDocument;     // Main in-game UI
    [SerializeField] private GameObject pauseUI;            // Pause menu UI (with buttons)
    [SerializeField] private GameObject pauseCat;           // UI-based animated cat (Image under Canvas)

    private bool isPaused = false;
    private bool isMuted = false;

    private void OnEnable()
    {
        if (!ValidateReferences()) return;

        var root = gameUIDocument.rootVisualElement;

        var pauseButton = root.Q<Button>("pauseButton");
        if (pauseButton != null)
        {
            pauseButton.clicked += TogglePause;
        }

        // Hide pause UI and cat on start
        pauseUI.SetActive(false);
        if (pauseCat != null) pauseCat.SetActive(false);
    }

    private bool ValidateReferences()
    {
        return gameUIDocument != null && pauseUI != null;
    }

    private void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            pauseUI.SetActive(true);

            if (pauseCat != null)
            {
                pauseCat.SetActive(true);
                StartCoroutine(PlayCatAnimationDelayed());
            }

            StartCoroutine(SetupPauseMenuButtonsNextFrame());
        }
        else
        {
            ResumeGame();
        }
    }

    private IEnumerator PlayCatAnimationDelayed()
    {
        yield return null; // Wait one frame so Animator reinitializes

        var animator = pauseCat.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Rebind();             // Reset animator state
            animator.Update(0f);           // Apply immediately
            animator.Play("Cat", 0, 0f);   // Replace "Cat" if your state has a different name
        }
    }

    private IEnumerator SetupPauseMenuButtonsNextFrame()
    {
        yield return null;
        SetupPauseMenuButtons();
    }

    private void SetupPauseMenuButtons()
    {
        var pauseUIDocument = pauseUI.GetComponent<UIDocument>();
        var pauseRoot = pauseUIDocument.rootVisualElement;

        var backButton = pauseRoot.Q<Button>("back");
        if (backButton != null)
            backButton.clicked += GoToMainMenu;

        var playButton = pauseRoot.Q<Button>("play");
        if (playButton != null)
            playButton.clicked += ResumeGame;

        var muteButton = pauseRoot.Q<Button>("mute");
        if (muteButton != null)
            muteButton.clicked += ToggleMute;
    }

    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        pauseUI.SetActive(false);
        if (pauseCat != null) pauseCat.SetActive(false);
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;

        if (pauseCat != null) pauseCat.SetActive(false);
        SceneManager.LoadScene("MainMenu");
    }

    private void ToggleMute()
    {
        isMuted = !isMuted;
        AudioListener.volume = isMuted ? 0f : 1f;
    }
}
