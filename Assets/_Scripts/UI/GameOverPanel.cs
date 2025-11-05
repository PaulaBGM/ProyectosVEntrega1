using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using _Scripts.Events;   // <-- el namespace donde tengas el EventBus y el OnGameFinished

public class GameOverPanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button menuButton;

    [Header("Opcional")]
    [SerializeField] private LivesSystem lives;

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
        EventBus<OnGameFinished>.Subscribe(HandleGameFinished);
    }

    private void OnDisable()
    {
        EventBus<OnGameFinished>.Unsubscribe(HandleGameFinished);
    }

    private void HandleGameFinished(OnGameFinished data)
    {
        // si ganó, aquí podrías abrir un panel de victoria
        if (!data.IsWin)
        {
            Show("Has perdido");
        }
        else
        {
            // si quieres pausar también al ganar:
            // Time.timeScale = 0f;
            // o cargar otra escena, etc.
        }
    }

    public void Show(string title = "Has perdido")
    {
        if (root != null)
            root.SetActive(true);

        if (titleText != null)
            titleText.text = title;

        bool canRetry = true;
        if (lives != null)
            canRetry = lives.CurrentLives > 0 && !lives.InCooldown;

        if (retryButton != null)
            retryButton.interactable = canRetry;

        Time.timeScale = 0f;
    }

    private void Hide()
    {
        if (root != null)
            root.SetActive(false);

        Time.timeScale = 1f;
    }

    private void OnRetry()
    {
        if (lives != null && (lives.CurrentLives <= 0 || lives.InCooldown))
            return;

        Time.timeScale = 1f;
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    private void OnMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
