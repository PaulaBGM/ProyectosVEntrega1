using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private MenuController menuController;

    [SerializeField] private Button backButton;

    private void Start()
    {
        if (optionsPanel != null) optionsPanel.SetActive(true);

        if (backButton != null)
            backButton.onClick.AddListener(OnBack);
    }

    public void OnBack()
    {
        // Vuelve al menú principal
        if (menuController != null)
            menuController.CloseOptionsMenu();
    }
}
