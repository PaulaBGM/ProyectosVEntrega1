// MenuController.cs (versi�n escena 0)
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        // este ya no es singleton ni DontDestroyOnLoad
        if (mainMenuCanvas) mainMenuCanvas.SetActive(true);
    }

    private void Start()
    {
        if (playButton) playButton.onClick.AddListener(OnPlay);
        if (optionsButton) optionsButton.onClick.AddListener(OnOpenOptions);
        if (quitButton) quitButton.onClick.AddListener(OnQuit);
        
        MusicManager.Instance?.PlayMenuMusic();
    }

    private void OnPlay()
    {
        // si tu escena de selecci�n es la 1:
        SceneManager.LoadScene(1);
    }

    private void OnOpenOptions()
    {
        // no pausamos porque estamos en el men�
        UIEvents.RequestOpenOptions(false, "MainMenu");
        // ocultar nuestro canvas si quieres
        if (mainMenuCanvas) mainMenuCanvas.SetActive(false);

        // cuando se cierren las opciones, queremos volver a mostrarlo:
        UIEvents.OnOptionsClosed += HandleOptionsClosed;
    }

    private void HandleOptionsClosed()
    {
        if (mainMenuCanvas) mainMenuCanvas.SetActive(true);
        UIEvents.OnOptionsClosed -= HandleOptionsClosed;
    }

    private void OnQuit()
    {
        Application.Quit();
    }
}
