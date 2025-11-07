using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using _Scripts.Events;

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
    public Vector2 PlayerPositionOffsetPerLevel = new Vector2(0f, 0f);

    private List<GameObject> _buttonObjects = new List<GameObject>();
    private Dictionary<string, Vector3> _buttonLocationsById = new Dictionary<string, Vector3>();

    public GameObject PlayerObj { get; set; }
    public bool _playerIsFacingRight;
    private LevelPathSegment[] _pathSegments;

    private bool _playerSpawned = false;
    private Tween _playerMoveTween;

    private void Awake()
    {
        _camera = Camera.main;
        _eventSystemHandler = GetComponentInChildren<LevelSelectEventSystemHandler>(true);
        _pathSegments = GetComponentsInChildren<LevelPathSegment>(true);
    }

    private void Start()
    {
        LoadUnlockedLevels();
        CreateLevelButtons();
        StartCoroutine(ShowPathsAfterFrame());

        string pending = PlayerPrefs.GetString("PendingLevelUnlock", "");
        if (!string.IsNullOrEmpty(pending))
        {
            UnlockLevelFromBridge(pending);
            PlayerPrefs.DeleteKey("PendingLevelUnlock");
        }
    }

    private IEnumerator ShowPathsAfterFrame()
    {
        yield return null;
        ShowPathsForAlreadyUnlocked();
    }

    private void ShowPathsForAlreadyUnlocked()
    {
        if (_pathSegments == null) return;

        foreach (var seg in _pathSegments)
        {
            if (string.IsNullOrEmpty(seg.toLevelId)) continue;

            if (UnlockedLevelIDs.Contains(seg.toLevelId))
            {
                seg.Play();
                PlacePathBetweenPreviousAndThis(seg.toLevelId, seg.transform);
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
            levelButton.levelSelectManager = this;
            levelButton.Setup(CurrentArea.levels[i], UnlockedLevelIDs.Contains(CurrentArea.levels[i].LevelID));

            Selectable selectable = buttonGo.GetComponent<Selectable>();
            _eventSystemHandler.AddSelectable(selectable);

            StartCoroutine(AddLocationAfterDelay(CurrentArea.levels[i].LevelID, buttonRect));

            if (!_playerSpawned)
            {
                StartCoroutine(SpawnInPlayerAfterDelay(buttonRect, WorldSpaceCanvasRect));
                _playerSpawned = true;
            }
        }

        LevelParent.gameObject.SetActive(true);
        _eventSystemHandler.InitSelectables();
        _eventSystemHandler.SetFirstSelected();
    }

    private IEnumerator AddLocationAfterDelay(string levelId, RectTransform buttonRect)
    {
        yield return null;
        Vector2 buttonScreenPoint = RectTransformUtility.WorldToScreenPoint(_camera, buttonRect.position);
        Vector3 buttonWorldPos = _camera.ScreenToWorldPoint(new Vector3(buttonScreenPoint.x, buttonScreenPoint.y, _camera.nearClipPlane));
        buttonWorldPos.z = 0;

        _buttonLocationsById[levelId] = buttonWorldPos;
    }

    #region HelperMethods
    private void UnlockLevelFromBridge(string levelId)
    {
        for (int i = 0; i < CurrentArea.levels.Count; i++)
        {
            if (CurrentArea.levels[i].LevelID == levelId)
            {
                GameObject btnObj = CurrentArea.levels[i].LevelButtonObj;
                if (btnObj != null)
                {
                    var levelBtn = btnObj.GetComponent<LevelButton>();
                    UnlockLevel(levelId, levelBtn);
                }
                break;
            }
        }
    }

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
                    PlacePathBetweenPreviousAndThis(levelID, seg.transform);
                }
            }
        }
    }

    [ContextMenu("Test Level Unlock")]
    public void UnlockLevelTwoExample()
    {
        if (_buttonObjects.Count > 1)
        {
            LevelButton levelButton = _buttonObjects[1].GetComponent<LevelButton>();
            string levelToUnlock = levelButton.levelData.LevelID;
            UnlockLevel(levelToUnlock, levelButton);
        }
        else
        {
            Debug.LogWarning("No hay suficiente botones creados aún para hacer el test de desbloqueo.");
        }
    }

    private void PlacePathBetweenPreviousAndThis(string levelId, Transform pathTransform)
    {
        if (pathTransform == null) return;

        int idx = -1;
        for (int i = 0; i < CurrentArea.levels.Count; i++)
        {
            if (CurrentArea.levels[i].LevelID == levelId)
            {
                idx = i;
                break;
            }
        }
        if (idx == -1) return;
        if (idx == 0) return;

        string prevLevelId = CurrentArea.levels[idx - 1].LevelID;
        string currLevelId = CurrentArea.levels[idx].LevelID;

        if (!_buttonLocationsById.TryGetValue(prevLevelId, out Vector3 prevPos)) return;
        if (!_buttonLocationsById.TryGetValue(currLevelId, out Vector3 currPos)) return;

        Vector3 mid = (prevPos + currPos) * 0.5f;
        mid.z = pathTransform.position.z;

        pathTransform.position = mid;
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
        PlayerObj.transform.SetAsLastSibling();

        RectTransform playerRect = PlayerObj.GetComponent<RectTransform>();

        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(_camera, screenSpaceUIObject.position);
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(worldSpaceUIObject, screenPosition, _camera, out Vector2 localPoint))
        {
            playerRect.anchoredPosition = localPoint + PlayerPositionOffsetPerLevel;
        }

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
        if (player == null) return;

        if (targetWorldPosition.x < player.transform.position.x && isFacingRight)
        {
            player.transform.Rotate(0f, 180f, 0f);
            isFacingRight = false;
        }
        else if (targetWorldPosition.x > player.transform.position.x && !isFacingRight)
        {
            player.transform.Rotate(0f, 180f, 0f);
            isFacingRight = true;
        }
    }

    public void MovePlayerToButton(GameObject playerUI, RectTransform targetButton, RectTransform worldSpaceUIObject)
    {
        if (playerUI == null || targetButton == null || _camera == null) return;

        RectTransform playerRect = playerUI.GetComponent<RectTransform>();

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(_camera, targetButton.position);
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(worldSpaceUIObject, screenPoint, _camera, out Vector2 localPoint))
        {
            Vector2 endPos = localPoint + PlayerPositionOffsetPerLevel;

            if (_playerMoveTween != null && _playerMoveTween.IsActive())
            {
                _playerMoveTween.Kill();
            }

            _playerMoveTween = playerRect.DOAnchorPos(endPos, 0.11f);
        }

        Vector3 targetWorld = _camera.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, _camera.nearClipPlane));
        CheckForRightOrLeftTurn(playerUI, ref _playerIsFacingRight, targetWorld);
    }

    private void OnDestroy()
    {
        if (_playerMoveTween != null && _playerMoveTween.IsActive())
        {
            _playerMoveTween.Kill();
        }
    }

    #endregion
}
