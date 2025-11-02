using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GlobalUIRoot : MonoBehaviour
{
    public static GlobalUIRoot Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject optionsMenuCanvas;
    [SerializeField] private GameObject brightnessCanvas;
    [SerializeField] private Image brightnessOverlay;

    [Header("Audio")]
    [SerializeField] private AudioMixer masterMixer;
    private const string MIXER_MUSIC_PARAM = "MusicVol";
    private const string MIXER_SFX_PARAM = "SFXVol";

    private bool _pausedByOptions;
    private float _prePauseTimeScale = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        UIEvents.OnRequestOpenOptions += HandleOpenOptionsRequested;
        UIEvents.OnBrightnessChanged += HandleBrightnessChanged;
        UIEvents.OnMusicChanged += HandleMusicChanged;
        UIEvents.OnSfxChanged += HandleSfxChanged;

        if (optionsMenuCanvas) optionsMenuCanvas.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            UIEvents.OnRequestOpenOptions -= HandleOpenOptionsRequested;
            UIEvents.OnBrightnessChanged -= HandleBrightnessChanged;
            UIEvents.OnMusicChanged -= HandleMusicChanged;
            UIEvents.OnSfxChanged -= HandleSfxChanged;
            Instance = null;
        }
    }

    private void HandleOpenOptionsRequested(bool pauseGame, string source)
    {
        if (optionsMenuCanvas)
            optionsMenuCanvas.SetActive(true);

        if (pauseGame && !_pausedByOptions)
        {
            _prePauseTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            _pausedByOptions = true;
        }
    }

    public void CloseOptionsFromGlobal()
    {
        if (optionsMenuCanvas)
            optionsMenuCanvas.SetActive(false);

        if (_pausedByOptions)
        {
            Time.timeScale = (_prePauseTimeScale <= 0f) ? 1f : _prePauseTimeScale;
            _pausedByOptions = false;
        }

        UIEvents.NotifyOptionsClosed();
    }

    private void HandleBrightnessChanged(float v)
    {
        if (!brightnessOverlay) return;
        float alpha = Mathf.Lerp(0.85f, 0f, Mathf.Clamp01(v));
        var c = brightnessOverlay.color;
        c.a = alpha;
        brightnessOverlay.color = c;
    }

    private void HandleMusicChanged(float v)
    {
        if (!masterMixer) return;
        float dB = (v <= 0.0001f) ? -80f : Mathf.Log10(Mathf.Clamp(v, 0.0001f, 1f)) * 20f;
        masterMixer.SetFloat(MIXER_MUSIC_PARAM, dB);
    }

    private void HandleSfxChanged(float v)
    {
        if (!masterMixer) return;
        float dB = (v <= 0.0001f) ? -80f : Mathf.Log10(Mathf.Clamp(v, 0.0001f, 1f)) * 20f;
        masterMixer.SetFloat(MIXER_SFX_PARAM, dB);
    }
}
