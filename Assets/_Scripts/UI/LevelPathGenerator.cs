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

            // Instanciamos la línea como hijo del pathParent
            Image path = Instantiate(pathPrefab, pathParent);
            path.name = $"Path_{from.name}_{to.name}";
            RectTransform rt = path.rectTransform;

            // Convertir posiciones de los botones al espacio local del parent de las líneas (esto es lo que hace que se "alineen" de verdad)
            Vector3 p1Local = pathParent.InverseTransformPoint(from.position);
            Vector3 p2Local = pathParent.InverseTransformPoint(to.position);

            // Calcular el punto medio y la distancia en ese mismo espacio
            Vector3 midLocal = (p1Local + p2Local) * 0.5f;
            float dist = Vector2.Distance(p1Local, p2Local);

            // Calcular ángulo en ese espacio
            Vector3 dirLocal = (p2Local - p1Local).normalized;
            float angle = Mathf.Atan2(dirLocal.y, dirLocal.x) * Mathf.Rad2Deg;

            // Aplicar al rect del path
            rt.anchoredPosition = midLocal;                  // porque ahora estamos en el espacio del parent
            rt.sizeDelta = new Vector2(dist, pathThickness); // largo = distancia entre botones
            rt.rotation = Quaternion.Euler(0f, 0f, angle);

            // por si el prefab no lo trae, aseguramos pivote centrado
            rt.pivot = new Vector2(0.5f, 0.5f);

            // Opcional: si quieres animación, añade el LevelPathSegment
            var segment = path.gameObject.AddComponent<LevelPathSegment>();
            segment.fromLevelId = from.name;
            segment.toLevelId = to.name;
            segment.pathImage = path;
        }
    }
}
