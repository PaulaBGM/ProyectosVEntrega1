// OptionsMenuController.cs (refactor mínima)
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("Buttons")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button creditsBackButton;

    [Header("Sliders (0..1)")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider brightnessSlider;

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

        // Aplicamos vía bus al global
        UIEvents.BroadcastMusic(music);
        UIEvents.BroadcastSfx(sfx);
        UIEvents.BroadcastBrightness(bright);

        if (backButton) backButton.onClick.AddListener(OnBack);
        if (creditsButton) creditsButton.onClick.AddListener(OpenCredits);
        if (creditsBackButton) creditsBackButton.onClick.AddListener(CloseCredits);

        if (musicSlider) musicSlider.onValueChanged.AddListener(OnMusicChanged);
        if (sfxSlider) sfxSlider.onValueChanged.AddListener(OnSfxChanged);
        if (brightnessSlider) brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
    }

    private void OnEnable()
    {
        // al habilitar, reemitimos para asegurar
        float music = musicSlider ? musicSlider.value : PlayerPrefs.GetFloat(PP_MUSIC, 0.8f);
        float sfx = sfxSlider ? sfxSlider.value : PlayerPrefs.GetFloat(PP_SFX, 0.8f);
        float bright = brightnessSlider ? brightnessSlider.value : PlayerPrefs.GetFloat(PP_BRIGHT, 1.0f);

        UIEvents.BroadcastMusic(music);
        UIEvents.BroadcastSfx(sfx);
        UIEvents.BroadcastBrightness(bright);
    }

    public void OnBack()
    {
        // le decimos al global que cierre y que avise
        GlobalUIRoot.Instance?.CloseOptionsFromGlobal();
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

    private void OnMusicChanged(float v)
    {
        PlayerPrefs.SetFloat(PP_MUSIC, v);
        UIEvents.BroadcastMusic(v);
    }

    private void OnSfxChanged(float v)
    {
        PlayerPrefs.SetFloat(PP_SFX, v);
        UIEvents.BroadcastSfx(v);
    }

    private void OnBrightnessChanged(float v)
    {
        PlayerPrefs.SetFloat(PP_BRIGHT, v);
        UIEvents.BroadcastBrightness(v);
    }

    private void OnDestroy() => PlayerPrefs.Save();
}
