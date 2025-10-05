using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject optionsMenuCanvas;

    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        // Aseguramos estados iniciales
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(true);
        if (optionsMenuCanvas != null) optionsMenuCanvas.SetActive(false);

        // Asignar listeners
        if (playButton != null)
            playButton.onClick.AddListener(OnPlay);

        if (optionsButton != null)
            optionsButton.onClick.AddListener(OpenOptionsMenu);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuit);
    }

    public void OnPlay()
    {
        // Aquí cargas la primera escena del juego
        SceneManager.LoadScene(1);
    }

    public void OpenOptionsMenu()
    {
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(false);
        if (optionsMenuCanvas != null) optionsMenuCanvas.SetActive(true);
    }

    public void CloseOptionsMenu()
    {
        if (optionsMenuCanvas != null) optionsMenuCanvas.SetActive(false);
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(true);
    }

    public void OnQuit()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
