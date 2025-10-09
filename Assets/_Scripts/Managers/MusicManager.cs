using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Mixer (opcional)")]
    [SerializeField] AudioMixer masterMixer;
    [SerializeField] AudioMixerGroup musicGroup, sfxGroup;

    [Header("UI SFX")]
    [SerializeField] AudioClip uiHover, uiClick;
    [SerializeField, Range(0f, 1f)] float uiVolume = 1f;
    [SerializeField] Vector2 uiPitchRange = new(1f, 1f);

    [Header("Fade")]
    [SerializeField, Min(0f)] float defaultFade = 0.25f;

    AudioSource music, sfx;
    Coroutine fadeCo;

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this; DontDestroyOnLoad(gameObject);
        music = CreateSource("Music", true, musicGroup);
        sfx = CreateSource("SFX", false, sfxGroup);
    }

    AudioSource CreateSource(string n, bool loop, AudioMixerGroup g)
    {
        var go = new GameObject(n); go.transform.SetParent(transform, false);
        var a = go.AddComponent<AudioSource>(); a.playOnAwake = false; a.loop = loop; a.spatialBlend = 0f; a.outputAudioMixerGroup = g; return a;
    }

    // -------- Música --------
    public void PlayMusic(AudioClip clip, float fade = -1f, bool loop = true, float startTime = 0f)
    {
        if (!clip) return;
        if (music.isPlaying && music.clip == clip) return;
        float f = (fade < 0f) ? defaultFade : fade;
        if (fadeCo != null) StopCoroutine(fadeCo);
        fadeCo = StartCoroutine(FadeSwap(clip, f, loop, Mathf.Clamp(startTime, 0f, Mathf.Max(0f, clip.length - .01f))));
    }

    public void StopMusic(float fade = -1f)
    {
        float f = (fade < 0f) ? defaultFade : fade;
        if (!music.isPlaying || f == 0f) { music.Stop(); return; }
        if (fadeCo != null) StopCoroutine(fadeCo);
        fadeCo = StartCoroutine(FadeOut(f));
    }

    IEnumerator FadeSwap(AudioClip next, float dur, bool loop, float start)
    {
        float startVol = music.volume;
        for (float t = 0; t < dur; t += Time.unscaledDeltaTime) { music.volume = Mathf.Lerp(startVol, 0f, t / dur); yield return null; }
        music.clip = next; music.loop = loop; music.time = start; music.Play();
        for (float t = 0; t < dur; t += Time.unscaledDeltaTime) { music.volume = Mathf.Lerp(0f, 1f, t / dur); yield return null; }
        music.volume = 1f; fadeCo = null;
    }

    IEnumerator FadeOut(float dur)
    {
        float startVol = music.volume;
        for (float t = 0; t < dur; t += Time.unscaledDeltaTime) { music.volume = Mathf.Lerp(startVol, 0f, t / dur); yield return null; }
        music.Stop(); music.volume = 1f; fadeCo = null;
    }

    // -------- SFX --------
    public void PlayUIHover() => PlaySFX(uiHover, uiVolume, RandPitch(uiPitchRange));
    public void PlayUIClick() => PlaySFX(uiClick, uiVolume, RandPitch(uiPitchRange));
    public void PlaySFX(AudioClip clip, float vol = 1f, float pitch = 1f)
    {
        if (!clip) return; sfx.pitch = Mathf.Clamp(pitch, .1f, 3f); sfx.PlayOneShot(clip, Mathf.Clamp01(vol));
    }

    float RandPitch(Vector2 r) => (r.x == r.y) ? r.x : Random.Range(Mathf.Min(r.x, r.y), Mathf.Max(r.x, r.y));

    // Opcional: actualizar mixer/grupos en runtime
    public void SetMixer(AudioMixer mixer, AudioMixerGroup musicGrp, AudioMixerGroup sfxGrp)
    { masterMixer = mixer; musicGroup = musicGrp; sfxGroup = sfxGrp; if (music) music.outputAudioMixerGroup = musicGroup; if (sfx) sfx.outputAudioMixerGroup = sfxGroup; }
}
