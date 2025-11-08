using UnityEngine;
using System.Collections;                 // <-- Necesario para IEnumerator no genérico
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.Events;
public class MenuEventSystemHandler : MonoBehaviour
{
    [Header("References")]
    public List<Selectable> Selectables = new List<Selectable>();
    [SerializeField] protected Selectable _firstSelected;

    [Header("Controls")]
    [SerializeField] protected InputActionReference _navigationReference;

    [Header("Animations")]
    [SerializeField] protected float _selectedAnimationScale = 1.1f;
    [SerializeField] protected float _scaleDuration = 0.25f;
    [SerializeField] protected List<GameObject> _animationExclusions = new List<GameObject>();

    [Header("Sounds")]
    [SerializeField] protected UnityEvent SoundEvent;

    protected readonly Dictionary<Selectable, Vector3> _scales = new Dictionary<Selectable, Vector3>();

    protected Selectable _lastSelected;

    protected Tween _scaleUpTween;
    protected Tween _scaleDownTween;

    public virtual void Awake()
    {
        // Cache de escalas y registro de eventos
        for (int i = 0; i < Selectables.Count; i++)
        {
            var selectable = Selectables[i];
            if (selectable == null) continue;

            AddSelectionListeners(selectable);
            if (!_scales.ContainsKey(selectable))
                _scales.Add(selectable, selectable.transform.localScale);
        }
    }

    public virtual void OnEnable()
    {
        if (_navigationReference != null && _navigationReference.action != null)
        {
            _navigationReference.action.performed += OnNavigate; // <-- Firma corregida
            _navigationReference.action.Enable();
        }

        // Reset de escalas
        for (int i = 0; i < Selectables.Count; i++)
        {
            var sel = Selectables[i];
            if (sel == null) continue;
            if (_scales.TryGetValue(sel, out var original))
                sel.transform.localScale = original;
        }

        StartCoroutine(SelectedAfterDelay());
    }

    protected virtual IEnumerator SelectedAfterDelay()
    {
        yield return null; // esperar un frame para que el EventSystem esté listo

        if (_firstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(_firstSelected.gameObject);
            _lastSelected = _firstSelected;
        }
        else if (Selectables.Count > 0 && Selectables[0] != null)
        {
            EventSystem.current.SetSelectedGameObject(Selectables[0].gameObject);
            _lastSelected = Selectables[0];
        }
    }

    public virtual void OnDisable()
    {
        if (_navigationReference != null && _navigationReference.action != null)
        {
            _navigationReference.action.performed -= OnNavigate;
            _navigationReference.action.Disable();
        }

        _scaleUpTween?.Kill(true);
        _scaleDownTween?.Kill(true);
        _scaleUpTween = null;
        _scaleDownTween = null;
    }

    protected virtual void AddSelectionListeners(Selectable selectable)
    {
        if (selectable == null) return;

        // Reutiliza el EventTrigger si ya existe; si no, crea uno
        EventTrigger trigger = selectable.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = selectable.gameObject.AddComponent<EventTrigger>();

        // SELECT
        var selectEntry = new EventTrigger.Entry { eventID = EventTriggerType.Select };
        selectEntry.callback.AddListener(OnSelect);
        trigger.triggers.Add(selectEntry);

        // DESELECT
        var deselectEntry = new EventTrigger.Entry { eventID = EventTriggerType.Deselect };
        deselectEntry.callback.AddListener(OnDeselect);
        trigger.triggers.Add(deselectEntry);

        // POINTER ENTER
        var pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnter.callback.AddListener(OnPointerEnter);
        trigger.triggers.Add(pointerEnter);

        // POINTER EXIT
        var pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExit.callback.AddListener(OnPointerExit);
        trigger.triggers.Add(pointerExit);
    }

    public void OnSelect(BaseEventData eventData)
    {
        SoundEvent?.Invoke();
        _lastSelected = eventData.selectedObject.GetComponent<Selectable>();

        if (_animationExclusions.Contains(eventData.selectedObject))
            return;
        if (eventData?.selectedObject == null) return;

        var sel = eventData.selectedObject.GetComponent<Selectable>();
        if (sel == null) return;

        _lastSelected = sel;

        // Escala hacia arriba
        var t = eventData.selectedObject.transform;
        _scaleUpTween?.Kill(true);
        Vector3 newScale = t.localScale * _selectedAnimationScale;
        _scaleUpTween = t.DOScale(newScale, _scaleDuration);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (_animationExclusions.Contains(eventData.selectedObject)) return;

        if (eventData?.selectedObject == null) return;

        var sel = eventData.selectedObject.GetComponent<Selectable>();
        if (sel == null) return;

        // Vuelve a su escala original
        if (_scales.TryGetValue(sel, out var original))
        {
            var t = eventData.selectedObject.transform;
            _scaleDownTween?.Kill(true);
            _scaleDownTween = t.DOScale(original, _scaleDuration);
        }
    }

    public void OnPointerEnter(BaseEventData eventData)
    {
        var pointerEventData = eventData as PointerEventData;
        if (pointerEventData == null) return;

        // Busca un Selectable en el objeto o en sus padres/hijos inmediatos
        var go = pointerEventData.pointerEnter;
        if (go == null) return;

        Selectable sel = go.GetComponentInParent<Selectable>();
        if (sel == null)
            sel = go.GetComponentInChildren<Selectable>();

        if (sel != null && sel.IsActive() && sel.IsInteractable())
        {
            EventSystem.current.SetSelectedGameObject(sel.gameObject);
            _lastSelected = sel;
        }
    }

    public void OnPointerExit(BaseEventData eventData)
    {
        // No anulamos la selección aquí para evitar dejar el EventSystem sin selección.
        // Si quisieras limpiar selección al salir, asegúrate de que haya otra por defecto.
    }

    // Firma compatible con InputAction.performed
    protected virtual void OnNavigate(InputAction.CallbackContext ctx)
    {
        // Mantener la selección si se estaba animando/teniendo foco
        if (EventSystem.current.currentSelectedGameObject == null && _lastSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(_lastSelected.gameObject);
        }
        else
        {
            // Si hay selección, la recordamos como última conocida
            var current = EventSystem.current.currentSelectedGameObject;
            if (current != null)
            {
                var sel = current.GetComponent<Selectable>();
                if (sel != null) _lastSelected = sel;
            }
        }
    }
}
