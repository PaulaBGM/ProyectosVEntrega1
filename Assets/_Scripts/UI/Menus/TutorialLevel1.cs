using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialLevel1 : MonoBehaviour
{
    [Header("Configuraci�n")]
    [Tooltip("Asigna aqu� el panel del tutorial (GameObject con CanvasGroup o Panel).")]
    public GameObject tutorialPanel;

    [Tooltip("Usar build index para identificar el nivel en vez del nombre de la escena.")]
    public bool usarBuildIndex = true;

    [Tooltip("Si usarBuildIndex es true, este es el index del nivel 1 en Build Settings.")]
    public int levelBuildIndex = 1;

    [Tooltip("Si usarBuildIndex es false, se comparar� con este nombre de escena.")]
    public string levelName = "Level1";

    [Tooltip("Si true, el juego se pausar� (Time.timeScale = 0) mientras el panel est� abierto.")]
    public bool pausarJuegoMientrasAbierto = true;

    // Key en PlayerPrefs para marcar que ya se mostr� el tutorial de nivel 1
    private const string PlayerPrefsKey = "Tutorial_Level1_Shown_v1";

    void Awake()
    {
        if (tutorialPanel == null)
        {
            Debug.LogWarning("[TutorialLevel1] tutorialPanel no est� asignado en el inspector.");
            return;
        }
    }

    void Start()
    {
        // Comprueba si estamos en el nivel 1 seg�n la configuraci�n
        bool estamosEnLevel1 = false;
        Scene current = SceneManager.GetActiveScene();

        if (usarBuildIndex)
            estamosEnLevel1 = (current.buildIndex == levelBuildIndex);
        else
            estamosEnLevel1 = (current.name == levelName);

        if (!estamosEnLevel1) return;

        // Si ya se mostr� antes, no hacemos nada
        if (PlayerPrefs.GetInt(PlayerPrefsKey, 0) == 1)
        {
            // ya mostrado previamente -> no abrir
            return;
        }

        // Mostrar el panel tutorial y pausar (si procede)
        OpenTutorial();
    }

    /// <summary>
    /// Abre el panel de tutorial.
    /// </summary>
    private void OpenTutorial()
    {
        tutorialPanel.SetActive(true);

        if (pausarJuegoMientrasAbierto)
        {
            Time.timeScale = 0f;
        }
    }

    /// <summary>
    /// M�todo p�blico para cerrar el tutorial (conectar al bot�n UI).
    /// Marca en PlayerPrefs que ya se mostr� para que no vuelva a aparecer.
    /// </summary>
    public void CloseTutorial()
    {
        gameObject.SetActive(false);

        if (pausarJuegoMientrasAbierto)
        {
            Time.timeScale = 1f;
        }

        // Marca como mostrado
        PlayerPrefs.SetInt(PlayerPrefsKey, 1);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// M�todo �til para desarrollo: elimina la marca y permite volver a ver el tutorial.
    /// Llamar desde men� de debug o con un bot�n para testing.
    /// </summary>
    public static void ResetTutorialShown()
    {
        PlayerPrefs.DeleteKey(PlayerPrefsKey);
    }
}
