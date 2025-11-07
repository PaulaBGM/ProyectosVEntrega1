using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelPathSegment : MonoBehaviour
{
    [Header("IDs de niveles")]
    public string fromLevelId;   
    public string toLevelId;     

    [Header("Visual")]
    [SerializeField] public Image pathImage;
    [SerializeField] private float animDuration = 0.4f;

    private void Awake()
    {
        if (pathImage == null)
            pathImage = GetComponent<Image>();

        if (pathImage != null)
        {
            pathImage.fillAmount = 0f;
            var c = pathImage.color;
            c.a = 0f;
            pathImage.color = c;
        }
    }

    public void Play()
    {
        if (pathImage == null) return;
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

            pathImage.fillAmount = k;         // se va “llenando” de izq a der
            c.a = Mathf.Lerp(0f, 1f, k);      // va apareciendo
            pathImage.color = c;

            yield return null;
        }

        pathImage.fillAmount = 1f;
        c.a = 1f;
        pathImage.color = c;
    }
}
