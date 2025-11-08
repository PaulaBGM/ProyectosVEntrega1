using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LivesUI : MonoBehaviour
{
    public LivesSystem lives;
    public Image[] hearts;
    public GameObject cooldownPanel;
    public TMP_Text cooldownText;

    private void OnEnable()
    {
        if (lives == null)
            lives = LivesSystem.Instance;

        if (lives == null)
            return;

        lives.OnLivesChanged += UpdateHearts;
        lives.OnCooldownChanged += UpdateCooldown;

        UpdateHearts(lives.CurrentLives, lives.maxLives);
        UpdateCooldown(lives.InCooldown, lives.CooldownRemaining);
    }

    private void OnDisable()
    {
        if (lives == null)
            return;

        lives.OnLivesChanged -= UpdateHearts;
        lives.OnCooldownChanged -= UpdateCooldown;
    }

    private void UpdateHearts(int current, int max)
    {
        if (hearts == null) return;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null)
                continue;

            bool show = i < current;
            hearts[i].gameObject.SetActive(show);
        }
    }

    private void UpdateCooldown(bool inCooldown, double remaining)
    {
        if (cooldownPanel != null)
            cooldownPanel.SetActive(inCooldown);

        if (!inCooldown || cooldownText == null)
            return;

        int s = Mathf.CeilToInt((float)remaining);
        cooldownText.text = $"Reponiendo en {s / 60:00}:{s % 60:00}";
    }
}
