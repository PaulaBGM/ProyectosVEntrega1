using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MenuController : MonoBehaviour 
{ 
    public static MenuController Instance { get; private set; } 
    [Header("UI References (persistentes)")]
    [SerializeField] private GameObject mainMenuCanvas; 
    
    // Canvas Menú Principal (persistente)
    [SerializeField] private GameObject optionsMenuCanvas; 
    
    // Canvas Opciones (persistente)
    [Header("Buttons")] 
    [SerializeField] private Button playButton; 
    [SerializeField] private Button optionsButton; 
    [SerializeField] private Button quitButton;

    [Header("In-Game Menu")]
    [SerializeField] private GameObject inGameMenuCanvas;   // Canvas con botón de opciones en juego
    [SerializeField] private Button inGameOptionsButton;    // Botón para abrir opciones dentro del juego
    [Header("Select Level Menu")]
    [SerializeField] private GameObject SelectLevelCanvas;   // Canvas con botón de opciones en juego

    private bool _listenersBound;
    private bool _pausedByOptions = false;
    private float _prePauseTimeScale = 1f;
    

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
        bool isMenuScene = SceneManager.GetActiveScene().buildIndex == 0;
        bool isSelectScene = SceneManager.GetActiveScene().buildIndex == 1;

        inGameMenuCanvas?.SetActive(!isMenuScene);
        SelectLevelCanvas?.SetActive(!isMenuScene);
        SelectLevelCanvas?.SetActive(isSelectScene);

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
        if (inGameOptionsButton != null)
            inGameOptionsButton.onClick.AddListener(OpenOptionsMenuInGame);

    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
    { 
      // Muestra el menú solo en la escena 0 (menú)
      bool isMenuScene = scene.buildIndex == 0;
      bool isSelectScene = SceneManager.GetActiveScene().buildIndex == 1;

        mainMenuCanvas?.SetActive(isMenuScene); 
      optionsMenuCanvas?.SetActive(false);
      inGameMenuCanvas?.SetActive(!isMenuScene);
        SelectLevelCanvas?.SetActive(isSelectScene);
        // Asegurar que el juego se reanude al cargar escena
        ResumeIfPausedByMenu();
    } 
    
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
    public void CloseOptionsMenu() {
        bool isMenuScene = SceneManager.GetActiveScene().buildIndex == 0;
        bool isSelectScene = SceneManager.GetActiveScene().buildIndex == 1;

        optionsMenuCanvas?.SetActive(false);
        
        if (isMenuScene)
            mainMenuCanvas?.SetActive(true);
        else
            inGameMenuCanvas?.SetActive(true);
    } 
    public void OnQuit() 
    { 
        Debug.Log("Saliendo del juego..."); 
        Application.Quit(); 
    }
    public void OpenOptionsMenuInGame()
    {
        optionsMenuCanvas?.SetActive(true);

        if (!_pausedByOptions)
        {
            _prePauseTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            _pausedByOptions = true;
        }
    }
    private void ResumeIfPausedByMenu()
    {
        if (_pausedByOptions)
        {
            Time.timeScale = (_prePauseTimeScale <= 0f) ? 1f : _prePauseTimeScale;
            _pausedByOptions = false;
        }
    }
    public void ReturnToMenu() { 
        SceneManager.LoadScene(0); 
    }
 }
