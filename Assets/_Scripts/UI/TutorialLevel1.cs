using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialLevel1 : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Asigna aquí el panel del tutorial (GameObject con CanvasGroup o Panel).")]
    public GameObject tutorialPanel;

    [Tooltip("Usar build index para identificar el nivel en vez del nombre de la escena.")]
    public bool usarBuildIndex = true;

    [Tooltip("Si usarBuildIndex es true, este es el index del nivel 1 en Build Settings.")]
    public int levelBuildIndex = 1;

    [Tooltip("Si usarBuildIndex es false, se comparará con este nombre de escena.")]
    public string levelName = "Level1";

    [Tooltip("Si true, el juego se pausará (Time.timeScale = 0) mientras el panel esté abierto.")]
    public bool pausarJuegoMientrasAbierto = true;

    // Key en PlayerPrefs para marcar que ya se mostró el tutorial de nivel 1
    private const string PlayerPrefsKey = "Tutorial_Level1_Shown_v1";

    void Awake()
    {
        if (tutorialPanel == null)
        {
            Debug.LogWarning("[TutorialLevel1] tutorialPanel no está asignado en el inspector.");
            return;
        }

        // Aseguramos que el panel esté oculto al inicio
        tutorialPanel.SetActive(false);
    }

    void Start()
    {
        // Comprueba si estamos en el nivel 1 según la configuración
        bool estamosEnLevel1 = false;
        Scene current = SceneManager.GetActiveScene();

        if (usarBuildIndex)
            estamosEnLevel1 = (current.buildIndex == levelBuildIndex);
        else
            estamosEnLevel1 = (current.name == levelName);

        if (!estamosEnLevel1) return;

        // Si ya se mostró antes, no hacemos nada
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
    /// Método público para cerrar el tutorial (conectar al botón UI).
    /// Marca en PlayerPrefs que ya se mostró para que no vuelva a aparecer.
    /// </summary>
    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);

        if (pausarJuegoMientrasAbierto)
        {
            Time.timeScale = 1f;
        }

        // Marca como mostrado
        PlayerPrefs.SetInt(PlayerPrefsKey, 1);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Método útil para desarrollo: elimina la marca y permite volver a ver el tutorial.
    /// Llamar desde menú de debug o con un botón para testing.
    /// </summary>
    public static void ResetTutorialShown()
    {
        PlayerPrefs.DeleteKey(PlayerPrefsKey);
    }
}
