using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private MenuController menuController;

    [Header("Buttons")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button creditsBackButton;

    [Header("Sliders (0..1)")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider brightnessSlider;

    [Header("Audio")]
    [SerializeField] private AudioMixer masterMixer; // expone "MusicVol" y "SFXVol"
    private const string MIXER_MUSIC_PARAM = "MusicVol";
    private const string MIXER_SFX_PARAM = "SFXVol";

    [Header("Visuals")]
    [SerializeField] private Image brightnessOverlay; // Image negra fullscreen

    private const string PP_MUSIC = "opt_music";
    private const string PP_SFX = "opt_sfx";
    private const string PP_BRIGHT = "opt_brightness";

    private void Start()
    {
        if (optionsPanel) optionsPanel.SetActive(true);
        if (creditsPanel) creditsPanel.SetActive(false);

        float music = PlayerPrefs.GetFloat(PP_MUSIC, 0.8f);
        float sfx = PlayerPrefs.GetFloat(PP_SFX, 0.8f);
        float bright = PlayerPrefs.GetFloat(PP_BRIGHT, 1.0f);

        if (musicSlider) musicSlider.SetValueWithoutNotify(music);
        if (sfxSlider) sfxSlider.SetValueWithoutNotify(sfx);
        if (brightnessSlider) brightnessSlider.SetValueWithoutNotify(bright);

        ApplyMusicVolume(music);
        ApplySfxVolume(sfx);
        ApplyBrightness(bright);

        if (backButton) backButton.onClick.AddListener(OnBack);
        if (creditsButton) creditsButton.onClick.AddListener(OpenCredits);
        if (creditsBackButton) creditsBackButton.onClick.AddListener(CloseCredits);

        if (musicSlider) musicSlider.onValueChanged.AddListener(OnMusicChanged);
        if (sfxSlider) sfxSlider.onValueChanged.AddListener(OnSfxChanged);
        if (brightnessSlider) brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
    }

    private void OnEnable()
    {
        float music = musicSlider ? musicSlider.value : PlayerPrefs.GetFloat(PP_MUSIC, 0.8f);
        float sfx = sfxSlider ? sfxSlider.value : PlayerPrefs.GetFloat(PP_SFX, 0.8f);
        float bright = brightnessSlider ? brightnessSlider.value : PlayerPrefs.GetFloat(PP_BRIGHT, 1.0f);

        ApplyMusicVolume(music);
        ApplySfxVolume(sfx);
        ApplyBrightness(bright);
    }

    // --- Navegación de paneles ---
    public void OnBack()
    {
        if (menuController) menuController.CloseOptionsMenu();
    }

    public void OpenCredits()
    {
        if (creditsPanel) creditsPanel.SetActive(true);
        if (optionsPanel) optionsPanel.SetActive(false);
    }

    private void CloseCredits()
    {
        if (creditsPanel) creditsPanel.SetActive(false);
        if (optionsPanel) optionsPanel.SetActive(true);
    }

    // --- Callbacks de sliders ---
    private void OnMusicChanged(float v)
    {
        ApplyMusicVolume(v);
        PlayerPrefs.SetFloat(PP_MUSIC, v);
    }

    private void OnSfxChanged(float v)
    {
        ApplySfxVolume(v);
        PlayerPrefs.SetFloat(PP_SFX, v);
    }

    private void OnBrightnessChanged(float v)
    {
        ApplyBrightness(v);
        PlayerPrefs.SetFloat(PP_BRIGHT, v);
    }

    private void OnDestroy() => PlayerPrefs.Save();

    // --- Aplicadores ---
    private void ApplyMusicVolume(float value01)
    {
        if (!masterMixer) return;
        float dB = (value01 <= 0.0001f) ? -80f : Mathf.Log10(Mathf.Clamp(value01, 0.0001f, 1f)) * 20f;
        masterMixer.SetFloat(MIXER_MUSIC_PARAM, dB);
    }

    private void ApplySfxVolume(float value01)
    {
        if (!masterMixer) return;
        float dB = (value01 <= 0.0001f) ? -80f : Mathf.Log10(Mathf.Clamp(value01, 0.0001f, 1f)) * 20f;
        masterMixer.SetFloat(MIXER_SFX_PARAM, dB);
    }

    private void ApplyBrightness(float value01)
    {
        if (!brightnessOverlay) return;
        float alpha = Mathf.Lerp(0.85f, 0f, Mathf.Clamp01(value01)); // 0=>oscuro, 1=>claro
        var c = brightnessOverlay.color; c.a = alpha; brightnessOverlay.color = c;
    }
}
