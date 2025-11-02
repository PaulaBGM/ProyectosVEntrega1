using System;

public static class UIEvents
{
    public static event Action<bool, string> OnRequestOpenOptions;
    public static event Action OnOptionsClosed;

    public static event Action<float> OnBrightnessChanged;
    public static event Action<float> OnMusicChanged;
    public static event Action<float> OnSfxChanged;

    public static void RequestOpenOptions(bool pauseGame, string source = null)
        => OnRequestOpenOptions?.Invoke(pauseGame, source);

    public static void NotifyOptionsClosed()
        => OnOptionsClosed?.Invoke();

    public static void BroadcastBrightness(float v)
        => OnBrightnessChanged?.Invoke(v);

    public static void BroadcastMusic(float v)
        => OnMusicChanged?.Invoke(v);

    public static void BroadcastSfx(float v)
        => OnSfxChanged?.Invoke(v);
}
