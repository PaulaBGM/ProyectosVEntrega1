using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSelectEventSystemHandler : DynamicEventSystemHandler
{
    private Image _image;
    private LevelButton _levelButton;
    private LevelSelectManager _manager;

    private void Awake()
    {
        // Cachea referencias para evitar buscar cada vez
        _manager = GetComponentInParent<LevelSelectManager>();
        // _image = GetComponent<Image>();
        //_levelButton = GetComponent<LevelButton>();
    }

    public override void OnPointerEnter(BaseEventData eventData) 
    {
    }

    public override void OnPointerExit(BaseEventData eventData) { }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);

        _image = eventData.selectedObject.GetComponent<Image>();
        _levelButton = eventData.selectedObject.GetComponent<LevelButton>();
        if (_levelButton != null) 
        {
            _manager.LevelHeaderText.SetText(_levelButton.levelData.LevelName);

            if (_image != null) 
            {
                _image.color = Color.red;
            }
        }
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);

        if (_levelButton != null) 
        {
            _manager.LevelHeaderText.SetText("");

            if (_image != null) 
            {
                _image.color = _levelButton.ReturnColor;
            }
        }
    }
}
