using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject optionsPanel;           // Panel principal de opciones
    [SerializeField] private GameObject creditsPanel;           // Panel de créditos
    [SerializeField] private MenuController menuController;     // Referencia al menú principal

    [Header("Buttons")]
    [SerializeField] private Button backButton;                 // Volver al menú principal
    [SerializeField] private Button creditsButton;              // Abrir créditos
    [SerializeField] private Button creditsBackButton;          // Volver desde créditos

    [Header("Sliders")]
    [SerializeField] private Slider musicSlider;                // 0..1
    [SerializeField] private Slider sfxSlider;                  // 0..1
    [SerializeField] private Slider brightnessSlider;           // 0..1

    [Header("Audio")]
    [SerializeField] private AudioMixer masterMixer;            // Debe exponer "MusicVol" y "SFXVol"
    private const string MIXER_MUSIC_PARAM = "MusicVol";
    private const string MIXER_SFX_PARAM = "SFXVol";

    [Header("Visuals")]
    [SerializeField] private Image brightnessOverlay;           // Image negra fullscreen (alpha controla oscuridad)

    // PlayerPrefs keys
    private const string PP_MUSIC = "opt_music";
    private const string PP_SFX = "opt_sfx";
    private const string PP_BRIGHT = "opt_brightness";

    private void Start()
    {
        // Estados iniciales de paneles
        if (optionsPanel != null) optionsPanel.SetActive(true);
        if (creditsPanel != null) creditsPanel.SetActive(false);

        // Cargar valores guardados (o por defecto)
        float music = PlayerPrefs.GetFloat(PP_MUSIC, 0.8f);
        float sfx = PlayerPrefs.GetFloat(PP_SFX, 0.8f);
        float bright = PlayerPrefs.GetFloat(PP_BRIGHT, 1.0f);

        if (musicSlider != null) musicSlider.SetValueWithoutNotify(music);
        if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(sfx);
        if (brightnessSlider != null) brightnessSlider.SetValueWithoutNotify(bright);

        // Aplicar inmediatamente los valores cargados
        ApplyMusicVolume(music);
        ApplySfxVolume(sfx);
        ApplyBrightness(bright);

        // Listeners
        if (backButton != null) backButton.onClick.AddListener(OnBack);
        if (creditsButton != null) creditsButton.onClick.AddListener(OpenCredits);
        if (creditsBackButton != null) creditsBackButton.onClick.AddListener(CloseCredits);

        if (musicSlider != null) musicSlider.onValueChanged.AddListener(OnMusicChanged);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(OnSfxChanged);
        if (brightnessSlider != null) brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
    }

    // --- Panel nav ---
    public void OnBack()
    {
        if (menuController != null)
            menuController.CloseOptionsMenu();
    }

    public void OpenCredits()
    {
        if (creditsPanel != null) creditsPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    private void CloseCredits()
    {
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(true);
    }

    // --- Sliders callbacks ---
    private void OnMusicChanged(float value)
    {
        ApplyMusicVolume(value);
        PlayerPrefs.SetFloat(PP_MUSIC, value);
    }

    private void OnSfxChanged(float value)
    {
        ApplySfxVolume(value);
        PlayerPrefs.SetFloat(PP_SFX, value);
    }

    private void OnBrightnessChanged(float value)
    {
        ApplyBrightness(value);
        PlayerPrefs.SetFloat(PP_BRIGHT, value);
    }

    private void OnDestroy()
    {
        PlayerPrefs.Save();
    }

    // --- Aplicadores ---
    private void ApplyMusicVolume(float value01)
    {
        // Convierte 0..1 en dB: 0 -> -80 dB (mute), 1 -> 0 dB
        if (masterMixer == null) return;
        float dB = (value01 <= 0.0001f) ? -80f : Mathf.Log10(Mathf.Clamp(value01, 0.0001f, 1f)) * 20f;
        //masterMixer.SetFloat(MIXER_MUSIC_PARAM, dB);
    }

    private void ApplySfxVolume(float value01)
    {
        if (masterMixer == null) return;
        float dB = (value01 <= 0.0001f) ? -80f : Mathf.Log10(Mathf.Clamp(value01, 0.0001f, 1f)) * 20f;
        //masterMixer.SetFloat(MIXER_SFX_PARAM, dB);
    }

    private void ApplyBrightness(float value01)
    {
        // value01 = 0 (muy oscuro) ... 1 (brillante)
        if (brightnessOverlay == null) return;

        // Usamos overlay negro: alpha inverso al brillo
        // brillo 1 => alpha 0 (sin oscurecer). brillo 0 => alpha 0.85 (oscuro)
        float maxDarkAlpha = 0.85f;
        float alpha = Mathf.Lerp(maxDarkAlpha, 0f, Mathf.Clamp01(value01));

        Color c = brightnessOverlay.color;
        c.a = alpha;
        brightnessOverlay.color = c;
    }
}
