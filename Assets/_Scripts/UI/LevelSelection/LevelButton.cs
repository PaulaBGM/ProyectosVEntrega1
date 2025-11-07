using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LevelButton : MonoBehaviour, ISelectHandler
{
    [SerializeField] private TextMeshProUGUI _levelNameText;

    public LevelData levelData { get; set; }

    private Button _button;
    private Image _image;
    public Color ReturnColor { get; set; }

    [HideInInspector] public LevelSelectManager levelSelectManager;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _image = GetComponent<Image>();
        ReturnColor = Color.grey;
    }

    public void Setup(LevelData level, bool isUnlocked)
    {
        levelData = level;
        _levelNameText.SetText(level.LevelID);

        _button.interactable = isUnlocked;

        if (isUnlocked)
        {
            _button.onClick.AddListener(LoadLevel);
            ReturnColor = Color.white;
            _image.color = ReturnColor;
        }
        else
        {
            ReturnColor = Color.grey;
            _image.color = ReturnColor;
        }
    }

    public void Unlock()
    {
        _button.interactable = true;
        _button.onClick.AddListener(LoadLevel);
        ReturnColor = Color.white;
        _image.color = ReturnColor;
    }

    private void LoadLevel()
    {
        SceneManager.LoadScene(levelData.scene);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (levelSelectManager != null && levelSelectManager.PlayerObj != null)
        {
            levelSelectManager.MovePlayerToButton(
                levelSelectManager.PlayerObj,
                GetComponent<RectTransform>(),
                levelSelectManager.WorldSpaceCanvasRect
            );
        }
    }
}
