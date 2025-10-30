using System;
using UnityEngine;

public class LivesSystem : MonoBehaviour
{
    [Header("Config")]
    public int maxLives = 3;
    public float cooldownSeconds = 60f;

    public int CurrentLives { get; private set; }
    public bool InCooldown { get; private set; }
    public double CooldownRemaining => InCooldown ? cooldownEnd - Time.unscaledTimeAsDouble : 0;

    public event Action<int, int> OnLivesChanged;
    public event Action<bool, double> OnCooldownChanged;

    private double cooldownEnd;

    void Awake()
    {
        CurrentLives = maxLives;
        NotifyState();
    }

    void Update()
    {
        if (!InCooldown) return;

        double remaining = cooldownEnd - Time.unscaledTimeAsDouble;
        if (remaining <= 0)
        {
            InCooldown = false;
            CurrentLives = maxLives;
            NotifyState();
        }
        else
        {
            OnCooldownChanged?.Invoke(true, remaining);
        }
    }

    public void ConsumeLife()
    {
        if (InCooldown) return;

        CurrentLives--;
        OnLivesChanged?.Invoke(CurrentLives, maxLives);

        if (CurrentLives <= 0)
        {
            InCooldown = true;
            cooldownEnd = Time.unscaledTimeAsDouble + cooldownSeconds;
            OnCooldownChanged?.Invoke(true, CooldownRemaining);
        }
    }

    private void NotifyState()
    {
        OnLivesChanged?.Invoke(CurrentLives, maxLives);
        OnCooldownChanged?.Invoke(InCooldown, CooldownRemaining);
    }
}
