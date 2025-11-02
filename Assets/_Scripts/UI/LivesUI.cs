using UnityEngine;
using UnityEngine.UI;

public class LivesUI : MonoBehaviour
{
    public LivesSystem lives;          // referencia al sistema de vidas
    public Image[] hearts;             // arrastra las imágenes de corazones
    public Sprite heartFull;           // sprite corazón lleno
    public Sprite heartEmpty;          // sprite corazón vacío
    public GameObject cooldownPanel;   // panel que muestra cuenta atrás
    public TMPro.TMP_Text cooldownText;

    void OnEnable()
    {
        lives.OnLivesChanged += UpdateHearts;
        lives.OnCooldownChanged += UpdateCooldown;
        UpdateHearts(lives.CurrentLives, lives.maxLives);
        UpdateCooldown(lives.InCooldown, lives.CooldownRemaining);
    }

    void OnDisable()
    {
        lives.OnLivesChanged -= UpdateHearts;
        lives.OnCooldownChanged -= UpdateCooldown;
    }

    void UpdateHearts(int current, int max)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].sprite = (i < current) ? heartFull : heartEmpty;
        }
    }

    void UpdateCooldown(bool inCooldown, double remaining)
    {
        cooldownPanel.SetActive(inCooldown);
        if (!inCooldown) return;

        int s = Mathf.CeilToInt((float)remaining);
        cooldownText.text = $"Reponiendo en {s / 60:00}:{s % 60:00}";
    }
}
