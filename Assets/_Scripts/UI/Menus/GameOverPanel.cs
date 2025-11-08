using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using _Scripts.Events;

public class GameOverPanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button menuButton;

    [Header("Opcional")]
    [SerializeField] private LivesSystem lives;          // puede venir de otra escena o no estar
    [SerializeField] private string levelSelectSceneName = "LevelSelectScene";

    private void Awake()
    {
        if (root != null)
            root.SetActive(false);

        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetry);

        if (menuButton != null)
            menuButton.onClick.AddListener(OnMenu);
    }

    private void OnEnable()
    {
        // si no nos lo han asignado en el inspector, intentamos encontrarlo en la escena
        if (lives == null)
            lives = FindFirstObjectByType<LivesSystem>();

        EventBus<OnGameFinished>.Subscribe(HandleGameFinished);
    }

    private void OnDisable()
    {
        EventBus<OnGameFinished>.Unsubscribe(HandleGameFinished);
    }

    private void HandleGameFinished(OnGameFinished data)
    {
        if (!data.IsWin)
        {
            Show("Has perdido");
        }
    }

    public void Show(string title = "Has perdido")
    {
        if (root != null)
            root.SetActive(true);

        if (titleText != null)
            titleText.text = title;

        bool canRetry = CanRetry();

        if (retryButton != null)
            retryButton.interactable = canRetry;

        Time.timeScale = 0f;
    }

    private bool CanRetry()
    {
        // si no hay lives en esta escena, dejamos reintentar
        if (lives == null)
            return true;

        if (lives.InCooldown)
            return false;

        return lives.CurrentLives > 0;
    }

    private void OnRetry()
    {
        if (!CanRetry())
            return;

        Time.timeScale = 1f;
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    private void OnMenu()
    {
        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(levelSelectSceneName))
            SceneManager.LoadScene(levelSelectSceneName);
        else
            SceneManager.LoadScene(0);
    }
}
