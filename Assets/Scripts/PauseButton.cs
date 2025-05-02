using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    [SerializeField] private UIDocument gameUIDocument;     // Main in-game UI
    [SerializeField] private GameObject pauseUI;            // Pause menu UI (with buttons)
    [SerializeField] private GameObject pauseCat;           // PauseCat canvas (con animaciones)
    [SerializeField] private string backButtonScene;      // Scene name to load for the main menu

    private bool isPaused = false;
    private bool isMuted = false;

    private void OnEnable()
    {
        if (!ValidateReferences()) return;

        var root = gameUIDocument.rootVisualElement;

        var pauseButton = root.Q<UnityEngine.UIElements.Button>("pauseButton");
        if (pauseButton != null)
        {
            pauseButton.clicked += TogglePause;
        }

        // Hide pause UI and PauseCat on start
        pauseUI.SetActive(false);
        if (pauseCat != null)
        {
            pauseCat.SetActive(false);
            DisablePauseCatClicks();
        }
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

                // Forzar que los animators usen tiempo no escalado para que animaciones sigan
                var animators = pauseCat.GetComponentsInChildren<Animator>(true);
                foreach (var animator in animators)
                {
                    animator.updateMode = AnimatorUpdateMode.UnscaledTime;
                }
            }

            StartCoroutine(SetupPauseMenuButtonsNextFrame());
        }
        else
        {
            ResumeGame();
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

        var backButton = pauseRoot.Q<UnityEngine.UIElements.Button>("back");
        if (backButton != null)
            backButton.clicked += GoToMainMenu;

        var playButton = pauseRoot.Q<UnityEngine.UIElements.Button>("play");
        if (playButton != null)
            playButton.clicked += ResumeGame;

        var muteButton = pauseRoot.Q<UnityEngine.UIElements.Button>("mute");
        if (muteButton != null)
            muteButton.clicked += ToggleMute;
    }

    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        pauseUI.SetActive(false);

        if (pauseCat != null)
        {
            pauseCat.SetActive(false);
        }
    }

    private void GoToMainMenu()
    {
        if (!string.IsNullOrEmpty(backButtonScene))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(backButtonScene);
        }
        else
        {
            Debug.LogWarning("Main menu scene name is not set!");
        }
    }

    private void ToggleMute()
    {
        isMuted = !isMuted;
        AudioListener.volume = isMuted ? 0f : 1f;
    }

    private void DisablePauseCatClicks()
    {
        var raycaster = pauseCat.GetComponent<GraphicRaycaster>();
        if (raycaster != null)
        {
            raycaster.enabled = false;
        }
    }
}
