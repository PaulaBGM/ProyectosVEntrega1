using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public static MenuController Instance { get; private set; }

    [Header("UI References (persistentes)")]
    [SerializeField] private GameObject mainMenuCanvas;     // Canvas Menú Principal (persistente)
    [SerializeField] private GameObject optionsMenuCanvas;  // Canvas Opciones (persistente)

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;

    private bool _listenersBound;

    private void Awake()
    {
        // Singleton + persistencia
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Suscribirse para mostrar/ocultar los canvas según escena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        BindListenersOnce();

        // Estado inicial (asumiendo que arrancas en el menú: buildIndex 0)
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(true);
        if (optionsMenuCanvas != null) optionsMenuCanvas.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    private void BindListenersOnce()
    {
        if (_listenersBound) return;

        if (playButton != null)
            playButton.onClick.AddListener(OnPlay);

        if (optionsButton != null)
            optionsButton.onClick.AddListener(OpenOptionsMenu);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuit);

        _listenersBound = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Muestra el menú solo en la escena 0 (menú). Ocúltalo en niveles.
        bool isMenuScene = scene.buildIndex == 0;

        if (mainMenuCanvas != null)
            mainMenuCanvas.SetActive(isMenuScene);

        if (optionsMenuCanvas != null)
            optionsMenuCanvas.SetActive(false); // al entrar a cualquier escena, cierra opciones
    }

    // --- Acciones de botones ---
    public void OnPlay()
    {
        // Carga tu escena de juego (ajusta el índice o usa nombre)
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

    // --- Utilidad: volver al menú desde el juego ---
    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
        // OnSceneLoaded se encargará de mostrar mainMenuCanvas
    }
}
