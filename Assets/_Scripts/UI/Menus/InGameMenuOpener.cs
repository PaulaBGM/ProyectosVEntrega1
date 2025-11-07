using UnityEngine;
using UnityEngine.UI;

public class InGameMenuOpener : MonoBehaviour
{
    [SerializeField] private Button inGameOptionsButton;

    private void Start()
    {
        if (inGameOptionsButton)
            inGameOptionsButton.onClick.AddListener(OpenOptionsInGame);
    }

    private void OpenOptionsInGame()
    {
        UIEvents.RequestOpenOptions(true, "InGame");
    }
}
