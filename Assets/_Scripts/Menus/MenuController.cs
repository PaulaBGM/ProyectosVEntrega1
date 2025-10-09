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
        mainMenuCanvas?.SetActive(true);
        optionsMenuCanvas?.SetActive(false);
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

        playButton?.onClick.AddListener(OnPlay);
        optionsButton?.onClick.AddListener(OpenOptionsMenu);
        quitButton?.onClick.AddListener(OnQuit);

        _listenersBound = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Muestra el menú solo en la escena 0 (menú)
        bool isMenuScene = scene.buildIndex == 0;

        mainMenuCanvas?.SetActive(isMenuScene);

        optionsMenuCanvas?.SetActive(false); // al entrar a cualquier escena, cierra opciones
    }

    // --- Acciones de botones ---
    public void OnPlay()
    {
        // Carga la escena de juego 
        SceneManager.LoadScene(1);
    }

    public void OpenOptionsMenu()
    {
        mainMenuCanvas?.SetActive(false);
        optionsMenuCanvas?.SetActive(true);
    }

    public void CloseOptionsMenu()
    {
        optionsMenuCanvas?.SetActive(false);
        mainMenuCanvas?.SetActive(true);
    }

    public void OnQuit()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
        // OnSceneLoaded se encargará de mostrar mainMenuCanvas
    }
}
