using _Scripts.Events;
using TMPro;
using UnityEngine;

public class TurnsPanelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turnsText;

    private void OnEnable()
    {
        EventBus<OnTurnsChanged>.Subscribe(HandleTurnsChanged);
    }

    private void Start()
    {
        if (turnsText != null)
            turnsText.text = "Turnos: --";
    }

    private void HandleTurnsChanged(OnTurnsChanged data)
    {
        if (turnsText == null)
            return;

        turnsText.text = $"Turnos: {data.TurnsLeft}";
    }

    private void OnDisable()
    {
        EventBus<OnTurnsChanged>.Unsubscribe(HandleTurnsChanged);
    }
}
