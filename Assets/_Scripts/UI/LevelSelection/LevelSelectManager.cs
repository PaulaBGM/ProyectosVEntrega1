using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class LevelSelectManager : MonoBehaviour
{
    public Transform LevelParent;
    public GameObject LevelButtonPrefab;
    public TextMeshProUGUI LevelHeaderText;
    public HashSet<string> UnlockedLevelIDs = new HashSet<string>();
    private LevelSelectEventSystemHandler _eventSystemHandler;
    public AreaData CurrentArea;
    private Camera _camera;

    [Header("Player References")]
    public GameObject PlayerUIPrefab;
    public RectTransform WorldSpaceCanvasRect;
    public Vector2 PlayerPositionOffsetPerLevel = new Vector2(0.02f, -0.5f);

    private List<GameObject> _buttonObjects = new List<GameObject>();
    private Dictionary<GameObject, Vector3> _buttonLocations = new Dictionary<GameObject, Vector3>();

    public GameObject PlayerObj { get; set; }
    public bool _playerIsFacingRight;
    private LevelPathSegment[] _pathSegments;

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
        ShowPathsForAlreadyUnlocked();

    }
    private void ShowPathsForAlreadyUnlocked()
    {
        if (_pathSegments == null) return;

        foreach (var seg in _pathSegments)
        {
            if (string.IsNullOrEmpty(seg.toLevelId)) continue;

            // si el destino de este camino ya está desbloqueado
            if (UnlockedLevelIDs.Contains(seg.toLevelId))
            {
                seg.Play();
            }
        }
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
            RectTransform buttonRect = buttonGo.GetComponent<RectTransform>();

            buttonGo.name = CurrentArea.levels[i].LevelID;
            CurrentArea.levels[i].LevelButtonObj = buttonGo;

            LevelButton levelButton = buttonGo.GetComponent<LevelButton>();
            levelButton.Setup(CurrentArea.levels[i], UnlockedLevelIDs.Contains(CurrentArea.levels[i].LevelID));
            
            //populate the selectables for the event system
            Selectable selectable = buttonGo.GetComponent<Selectable>();
            _eventSystemHandler.AddSelectable(selectable);
            StartCoroutine(SpawnInPlayerAfterDelay(buttonRect, WorldSpaceCanvasRect));
        }
        
        LevelParent.gameObject.SetActive(true);
        _eventSystemHandler.InitSelectables();
        _eventSystemHandler.SetFirstSelected();
    }

    private IEnumerator AddLocationAfterDelay (GameObject buttonGo, RectTransform buttonRect) 
    {
        yield return null;
        Vector2 buttonScreenPoint = RectTransformUtility.WorldToScreenPoint(_camera, buttonRect.position);
        Vector3 buttonWorldPos = _camera.ScreenToWorldPoint(new Vector3(buttonScreenPoint.x, buttonScreenPoint.y, _camera.nearClipPlane));
        buttonWorldPos.z = 0;
        
        _buttonLocations.Add(buttonGo, buttonWorldPos);
    }
    //private IEnumerator DelayedLineSetup

    #region HelperMethods

    public void UnlockLevel(string levelID, LevelButton levelButton)
    {
        UnlockedLevelIDs.Add(levelID);
        levelButton.Unlock();

        if (_pathSegments != null)
        {
            foreach (var seg in _pathSegments)
            {
                if (seg.toLevelId == levelID)
                {
                    seg.Play();
                }
            }
        }
    }

    #endregion

    #region Player

    private IEnumerator SpawnInPlayerAfterDelay(RectTransform screenSpaceButton, RectTransform worldSpaceCanvas) 
    { 
        yield return null; 
        SpawnInPlayer(screenSpaceButton, worldSpaceCanvas);
    }
    private void SpawnInPlayer(RectTransform screenSpaceUIObject, RectTransform worldSpaceUIObject)
    {
        _playerIsFacingRight = true;
        PlayerObj = Instantiate(PlayerUIPrefab, worldSpaceUIObject);

        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(_camera, screenSpaceUIObject.position);
        Vector3 worldPosition = _camera.ScreenToWorldPoint(new Vector3(screenPosition.x,screenPosition.y,_camera.nearClipPlane));
        worldPosition.z = worldSpaceUIObject.position.z;
        Vector3 offsetPosition = worldPosition + (Vector3)PlayerPositionOffsetPerLevel;

        PlayerObj.transform.position = offsetPosition;
        if (_buttonObjects.Count > 1) 
        {  
            Vector2 secondScreenPoint = RectTransformUtility.WorldToScreenPoint(_camera, _buttonObjects[1].GetComponent<RectTransform>().position);
            Vector3 secondWorldPoint = _camera.ScreenToWorldPoint(new Vector3(secondScreenPoint.x, secondScreenPoint.y, _camera.nearClipPlane));
            secondWorldPoint.z = worldSpaceUIObject.position.z;
            
            CheckForRightOrLeftTurn(PlayerObj, ref _playerIsFacingRight, secondWorldPoint);
        }
    }

    private void CheckForRightOrLeftTurn(GameObject player, ref bool isFacingRight, Vector3 targetWorldPosition) 
    {
        if (isFacingRight) 
        {
            player.transform.Rotate(0f, 180f, 0f);
            isFacingRight = false;
        }
        else
        {
            if (targetWorldPosition.x > player.transform.position.x) 
            {
                player.transform.Rotate(0f, -180f, 0f);
                isFacingRight = true;
            }  
        }
    }

    public void MovePlayerToButton(GameObject playerUI, RectTransform targetButton, RectTransform worldSpaceUIObject) 
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(_camera,targetButton.position);
        Vector3 worldPosition = _camera.ScreenToWorldPoint(new Vector3(screenPoint.x,screenPoint.y,_camera.nearClipPlane));
        worldPosition.z = worldSpaceUIObject.position.z;

        Vector3 endPosition = worldPosition + (Vector3)PlayerPositionOffsetPerLevel;

        CheckForRightOrLeftTurn(playerUI, ref _playerIsFacingRight, worldPosition);

        playerUI.transform.DOMove(endPosition, 0.11f);
    }
    
    #endregion
}
