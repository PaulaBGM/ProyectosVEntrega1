using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameOverPanel : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject root;     // el Panel o el propio Canvas
    [SerializeField] private Button retryButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private TMP_Text noLivesText;

    [Header("Gameplay")]
    [SerializeField] private LivesSystem lives;   // arrastra tu LivesSystem de la escena

    private void Awake()
    {
        if (root != null)
            root.SetActive(false);
    }

    public void Show()
    {
        if (root != null)
            root.SetActive(true);

        bool canRetry = lives != null && lives.CurrentLives > 0 && !lives.InCooldown;

        if (retryButton) retryButton.interactable = canRetry;
        if (noLivesText) noLivesText.gameObject.SetActive(!canRetry);

        // pausar
        Time.timeScale = 0f;
    }

    public void Hide()
    {
        if (root != null)
            root.SetActive(false);

        Time.timeScale = 1f;
    }

    // BOTONES
    public void OnRetry()
    {
        if (lives == null) return;
        if (lives.CurrentLives <= 0 || lives.InCooldown) return;

        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    public void OnMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); // tu menú principal
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}

