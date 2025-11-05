using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelPathGenerator : MonoBehaviour
{
    [Header("Referencias")]
    public RectTransform levelParent;     // contenedor de botones
    public Image pathPrefab;              // prefab del path (una Image con Fill o Stretch)
    public Transform pathParent;          // contenedor donde van los paths (Canvas/Panel)

    [Header("Opciones")]
    public float pathThickness = 10f;

    private void Start()
    {
        GeneratePaths();
    }

    private void GeneratePaths()
    {
        if (levelParent == null || pathPrefab == null || pathParent == null) return;

        // Todos los botones hijos de LevelParent
        List<RectTransform> buttons = new List<RectTransform>();
        foreach (Transform child in levelParent)
        {
            var rt = child as RectTransform;
            if (rt != null)
                buttons.Add(rt);
        }

        // Crear un path entre cada par consecutivo
        for (int i = 0; i < buttons.Count - 1; i++)
        {
            RectTransform from = buttons[i];
            RectTransform to = buttons[i + 1];

            // Instanciamos la línea
            Image path = Instantiate(pathPrefab, pathParent);
            path.name = $"Path_{from.name}_{to.name}";

            RectTransform rt = path.rectTransform;

            // Colocar en medio
            Vector3 p1 = from.position;
            Vector3 p2 = to.position;
            Vector3 mid = (p1 + p2) / 2f;
            float dist = Vector3.Distance(p1, p2);
            Vector3 dir = (p2 - p1).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            rt.position = mid;
            rt.sizeDelta = new Vector2(dist, pathThickness);
            rt.rotation = Quaternion.Euler(0f, 0f, angle);

            // Opcional: si quieres animación, añade el LevelPathSegment
            var segment = path.gameObject.AddComponent<LevelPathSegment>();
            segment.toLevelId = to.name;
            segment.pathImage = path;
        }
    }
}
