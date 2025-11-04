using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelPathSegment : MonoBehaviour
{
    [Header("Destino")]
    public string toLevelId;  // mantenemos el nombre para no liarnos

    [Header("Visual")]
    [SerializeField] private Image pathImage;
    [SerializeField] private float animDuration = 0.4f;

    private void Awake()
    {
        if (!pathImage)
            pathImage = GetComponent<Image>();

        if (pathImage)
        {
            pathImage.fillAmount = 0f;
            var c = pathImage.color;
            c.a = 0f;
            pathImage.color = c;
        }
    }

    public void Play()
    {
        if (!gameObject.activeInHierarchy) return;
        if (!pathImage) return;

        StopAllCoroutines();
        StartCoroutine(AnimatePath());
    }

    private IEnumerator AnimatePath()
    {
        float t = 0f;
        Color c = pathImage.color;

        while (t < animDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / animDuration);

            pathImage.fillAmount = k;
            c.a = Mathf.Lerp(0f, 1f, k);
            pathImage.color = c;

            yield return null;
        }

        pathImage.fillAmount = 1f;
        c.a = 1f;
        pathImage.color = c;
    }

    // útil si quieres forzar el estado final sin animación
    public void SetFilledInstant()
    {
        if (!pathImage) return;
        pathImage.fillAmount = 1f;
        var c = pathImage.color;
        c.a = 1f;
        pathImage.color = c;
    }
}
