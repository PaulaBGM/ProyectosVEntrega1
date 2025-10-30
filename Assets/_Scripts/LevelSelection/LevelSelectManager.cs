using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class LevelSelectManager : MonoBehaviour
{
    public Transform LevelParent;
    public GameObject LevelButtonPrefab;
    public TextMeshProUGUI LevelHeaderText;
    public HashSet<string> UnlockedLevelIDs = new HashSet<string>();
    private LevelSelectEventSystemHandler _eventSystemHandler;
    public AreaData CurrentArea;
    private Camera _camera;

    private List<GameObject> _buttonObjects = new List<GameObject>();
    private Dictionary<GameObject, Vector3> _buttonLocations = new Dictionary<GameObject, Vector3>();

    private void Awake()
    {
        _camera = Camera.main;
        _eventSystemHandler = GetComponentInChildren<LevelSelectEventSystemHandler>(true);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadUnlockedLevels();
        CreateLevelButtons();
    }

    private void LoadUnlockedLevels() 
    {
        foreach (var level in CurrentArea.levels) 
        {
            if (level.ISUnlockedByDefault) 
                UnlockedLevelIDs.Add(level.LevelID);
        }
    }

    private void CreateLevelButtons() 
    {
        for (int i = 0; i < CurrentArea.levels.Count; i++) 
        {
            GameObject buttonGo = Instantiate(LevelButtonPrefab, LevelParent);
            
            _buttonObjects.Add(buttonGo);
            Debug.Log("button "+i);
            RectTransform buttonRect = buttonGo.GetComponent<RectTransform>();

            buttonGo.name = CurrentArea.levels[i].LevelID;
            CurrentArea.levels[i].LevelButtonObj = buttonGo;

            LevelButton levelButton = buttonGo.GetComponent<LevelButton>();
            levelButton.Setup(CurrentArea.levels[i], UnlockedLevelIDs.Contains(CurrentArea.levels[i].LevelID));
            
            //populate the selectables for the event system
            Selectable selectable = buttonGo.GetComponent<Selectable>();
            _eventSystemHandler.AddSelectable(selectable);
        
        }

        LevelParent.gameObject.SetActive(true);
        _eventSystemHandler.InitSelectables();
        _eventSystemHandler.SetFirstSelected();
    }
}
