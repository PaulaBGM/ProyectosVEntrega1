using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    [SerializeField] Sprite cursorNormal, cursorClick;
    SpriteRenderer sr; Camera cam;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>() ?? gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = cursorNormal;
        Cursor.visible = false; Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        if (!cam) cam = Camera.main;
        if (!cam) return;

        var p = cam.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(p.x, p.y, 0);

        if (Input.GetMouseButtonDown(0)) sr.sprite = cursorClick;
        else if (Input.GetMouseButtonUp(0)) sr.sprite = cursorNormal;
    }

    void OnDisable() { Cursor.visible = true; Cursor.lockState = CursorLockMode.None; }
}
