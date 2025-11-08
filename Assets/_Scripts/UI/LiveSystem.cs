using System;
using UnityEngine;
using _Scripts.Events;

public class LivesSystem : MonoBehaviour
{
    public static LivesSystem Instance { get; private set; }

    [Header("Config")]
    public int maxLives = 3;
    public float cooldownSeconds = 60f;

    private const string LIVES_KEY = "LS_CURRENT_LIVES";
    private const string IN_COOLDOWN_KEY = "LS_IN_COOLDOWN";
    private const string COOLDOWN_END_KEY = "LS_COOLDOWN_END_TICKS";

    public int CurrentLives { get; private set; }
    public bool InCooldown { get; private set; }

    private DateTime cooldownEndUtc;

    public double CooldownRemaining => InCooldown
        ? (cooldownEndUtc - DateTime.UtcNow).TotalSeconds
        : 0;

    public event Action<int, int> OnLivesChanged;
    public event Action<bool, double> OnCooldownChanged;

    private void Awake()
    {
        // singleton simple
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadState();
        NotifyState();
    }

    private void OnEnable()
    {
        EventBus<OnGameFinished>.Subscribe(HandleGameFinished);
    }

    private void OnDisable()
    {
        EventBus<OnGameFinished>.Unsubscribe(HandleGameFinished);
    }

    private void HandleGameFinished(OnGameFinished data)
    {
        if (!data.IsWin)
        {
            ConsumeLife();
        }
    }

    private void Update()
    {
        if (!InCooldown)
            return;

        double remaining = (cooldownEndUtc - DateTime.UtcNow).TotalSeconds;
        if (remaining <= 0)
        {
            InCooldown = false;
            CurrentLives = maxLives;
            SaveState();
            NotifyState();
        }
        else
        {
            OnCooldownChanged?.Invoke(true, remaining);
        }
    }

    public void ConsumeLife()
    {
        if (InCooldown)
            return;

        CurrentLives = Mathf.Max(0, CurrentLives - 1);
        OnLivesChanged?.Invoke(CurrentLives, maxLives);
        SaveState();

        if (CurrentLives <= 0)
        {
            InCooldown = true;
            cooldownEndUtc = DateTime.UtcNow.AddSeconds(cooldownSeconds);
            SaveState();
            OnCooldownChanged?.Invoke(true, CooldownRemaining);
        }
    }

    private void NotifyState()
    {
        OnLivesChanged?.Invoke(CurrentLives, maxLives);
        OnCooldownChanged?.Invoke(InCooldown, CooldownRemaining);
    }

    private void LoadState()
    {
        CurrentLives = PlayerPrefs.GetInt(LIVES_KEY, maxLives);
        InCooldown = PlayerPrefs.GetInt(IN_COOLDOWN_KEY, 0) == 1;

        if (InCooldown)
        {
            string ticksStr = PlayerPrefs.GetString(COOLDOWN_END_KEY, string.Empty);
            if (!string.IsNullOrEmpty(ticksStr) && long.TryParse(ticksStr, out long ticks))
            {
                cooldownEndUtc = new DateTime(ticks, DateTimeKind.Utc);
            }
            else
            {
                InCooldown = false;
            }

            if (CooldownRemaining <= 0)
            {
                InCooldown = false;
                CurrentLives = maxLives;
            }
        }
    }

    private void SaveState()
    {
        PlayerPrefs.SetInt(LIVES_KEY, CurrentLives);
        PlayerPrefs.SetInt(IN_COOLDOWN_KEY, InCooldown ? 1 : 0);
        if (InCooldown)
        {
            PlayerPrefs.SetString(COOLDOWN_END_KEY, cooldownEndUtc.Ticks.ToString());
        }
        PlayerPrefs.Save();
    }
}
