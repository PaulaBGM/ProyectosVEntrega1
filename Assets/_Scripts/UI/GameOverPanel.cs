using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameOverPanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject root;      // panel/canvas que se muestra
    [SerializeField] private TMP_Text titleText;   // opcional, por si quieres cambiar el título
    [SerializeField] private Button retryButton;
    [SerializeField] private Button menuButton;

    [Header("Opcional")]
    [SerializeField] private LivesSystem lives;    // opcional: si no lo asignas, siempre permite retry

    private void Awake()
    {
        // que empiece oculto
        if (root != null)
            root.SetActive(false);

        // enganchar botones si están
        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetry);

        if (menuButton != null)
            menuButton.onClick.AddListener(OnMenu);
    }

    /// <summary>
    /// Llamas a esto cuando el jugador muere.
    /// </summary>
    public void Show(string title = "Has perdido")
    {
        if (root != null)
            root.SetActive(true);

        if (titleText != null)
            titleText.text = title;

        // si hay sistema de vidas y no hay vidas, desactivar retry
        bool canRetry = true;
        if (lives != null)
            canRetry = lives.CurrentLives > 0 && !lives.InCooldown;

        if (retryButton != null)
            retryButton.interactable = canRetry;

        // pausar juego
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
        // si hay vidas y no se puede, no hagas nada
        if (lives != null && (lives.CurrentLives <= 0 || lives.InCooldown))
            return;

        // recarga escena actual
        Time.timeScale = 1f;
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    private void OnMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); // menú principal
    }
}
