using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelNameText;

    public LevelData levelData { get; set; }

    private Button _button;
    private Image _image;
    public Color ReturnColor {  get; set; }

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
    }
    private void LoadLevel() 
    {
        SceneManager.LoadScene(levelData.scene);
    }
}
