using UnityEngine;
using UnityEngine.UI;

public class LivesUI : MonoBehaviour
{
    public LivesSystem lives;          // referencia al sistema de vidas
    public Image[] hearts;             // arrastra las imágenes de corazones
    public GameObject cooldownPanel;   // panel que muestra cuenta atrás
    public TMPro.TMP_Text cooldownText;

    void OnEnable()
    {
        if (lives == null)
            return;

        lives.OnLivesChanged += UpdateHearts;
        lives.OnCooldownChanged += UpdateCooldown;

        // inicializamos con el estado actual
        UpdateHearts(lives.CurrentLives, lives.maxLives);
        UpdateCooldown(lives.InCooldown, lives.CooldownRemaining);
    }

    void OnDisable()
    {
        if (lives == null)
            return;

        lives.OnLivesChanged -= UpdateHearts;
        lives.OnCooldownChanged -= UpdateCooldown;
    }

    void UpdateHearts(int current, int max)
    {
        if (hearts == null) return;

        // mostramos solo tantos corazones como vidas actuales
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null)
                continue;

            bool shouldShow = i < current;
            if (hearts[i].gameObject.activeSelf != shouldShow)
                hearts[i].gameObject.SetActive(shouldShow);
        }
    }

    void UpdateCooldown(bool inCooldown, double remaining)
    {
        if (cooldownPanel != null)
            cooldownPanel.SetActive(inCooldown);

        if (!inCooldown || cooldownText == null)
            return;

        int s = Mathf.CeilToInt((float)remaining);
        cooldownText.text = $"Reponiendo en {s / 60:00}:{s % 60:00}";
    }
}
