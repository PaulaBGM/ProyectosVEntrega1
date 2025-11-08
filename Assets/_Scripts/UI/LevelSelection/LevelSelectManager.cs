using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using _Scripts.Events;

public class LevelSelectManager : MonoBehaviour
{
    [Header("Levels")]
    public Transform LevelParent;
    public GameObject LevelButtonPrefab;
    public TextMeshProUGUI LevelHeaderText;
    public AreaData CurrentArea;

    // niveles desbloqueados en esta sesi�n / por defecto
    public HashSet<string> UnlockedLevelIDs = new HashSet<string>();

    private LevelSelectEventSystemHandler _eventSystemHandler;
    private Camera _camera;

    [Header("Player References")]
    public GameObject PlayerUIPrefab;
    public RectTransform WorldSpaceCanvasRect;
    public Vector2 PlayerPositionOffsetPerLevel = new Vector2(0f, 0f);

    [Header("Paths")]
    [SerializeField] private Image pathPrefab;            // prefab de la l�nea
    [SerializeField] private RectTransform pathParent;    // contenedor donde van las l�neas
    [SerializeField] private float pathThickness = 10f;   // grosor de la l�nea

    private List<GameObject> _buttonObjects = new List<GameObject>();

    // posiciones de cada bot�n por levelId (mundo)
    private Dictionary<string, Vector3> _buttonLocationsById = new Dictionary<string, Vector3>();

    // paths ya creados, indexados por "toLevelId"
    private Dictionary<string, LevelPathSegment> _pathByToLevelId = new Dictionary<string, LevelPathSegment>();

    public GameObject PlayerObj { get; set; }
    public bool _playerIsFacingRight;

    private bool _playerSpawned = false;
    private Tween _playerMoveTween;

    private const string UnlockedPrefix = "UnlockedLevel_";

    // guardamos el pending para aplicarlo cuando ya est�n las posiciones
    private string _pendingLevelId = "";

    private void Awake()
    {
        _camera = Camera.main;
        _eventSystemHandler = GetComponentInChildren<LevelSelectEventSystemHandler>(true);

        // si hay paths ya colocados en la escena, los registramos
        var preExistingSegments = GetComponentsInChildren<LevelPathSegment>(true);
        if (preExistingSegments != null)
        {
            foreach (var seg in preExistingSegments)
            {
                if (!string.IsNullOrEmpty(seg.toLevelId) && !_pathByToLevelId.ContainsKey(seg.toLevelId))
                    _pathByToLevelId.Add(seg.toLevelId, seg);
            }
        }
    }

    private void Start()
    {
        LoadUnlockedLevels();
        CreateLevelButtons();
        
        // puente: si venimos de un nivel que dej� un desbloqueo pendiente
        string pending = PlayerPrefs.GetString("PendingLevelUnlock", "");
        if (!string.IsNullOrEmpty(pending))
        {
            UnlockLevelFromBridge(pending);
            PlayerPrefs.DeleteKey("PendingLevelUnlock");
        }
        
        MusicManager.Instance?.PlayLevelSelectMusic();
        // leer pending
        _pendingLevelId = PlayerPrefs.GetString("PendingLevelUnlock", "");

        StartCoroutine(InitAfterFrames());
    }

    private IEnumerator InitAfterFrames()
    {
        // esperamos a que:
        // 1) se creen los botones
        // 2) AddLocationAfterDelay guarde las posiciones
        yield return null;
        yield return null;

        // ya podemos mostrar paths de los que estaban desbloqueados
        ShowPathsForAlreadyUnlocked();

        // y ahora s� aplicar el pending
        if (!string.IsNullOrEmpty(_pendingLevelId))
        {
            UnlockLevelFromBridge(_pendingLevelId);
            PlayerPrefs.DeleteKey("PendingLevelUnlock");
            PlayerPrefs.Save();
            _pendingLevelId = "";
        }
    }

    private void LoadUnlockedLevels()
    {
        // aqu� usamos SOLO tu arquitectura: los que vienen en el AreaData
        foreach (var level in CurrentArea.levels)
        {
            bool isDefault = level.ISUnlockedByDefault;
            bool wasUnlockedThisSession = PlayerPrefs.GetInt(UnlockedPrefix + level.LevelID, 0) == 1;

            if (isDefault || wasUnlockedThisSession)
            {
                UnlockedLevelIDs.Add(level.LevelID);
            }
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

            // guardamos su posici�n un frame despu�s
            StartCoroutine(AddLocationAfterDelay(CurrentArea.levels[i].LevelID, buttonRect));

            // spawn del player en el primer bot�n
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
        // esperamos 1 frame para que la UI est� colocada
        yield return null;

        Vector2 buttonScreenPoint = RectTransformUtility.WorldToScreenPoint(_camera, buttonRect.position);
        Vector3 buttonWorldPos = _camera.ScreenToWorldPoint(
            new Vector3(buttonScreenPoint.x, buttonScreenPoint.y, _camera.nearClipPlane)
        );
        buttonWorldPos.z = 0;

        _buttonLocationsById[levelId] = buttonWorldPos;
    }

    private void ShowPathsForAlreadyUnlocked()
    {
        for (int i = 0; i < CurrentArea.levels.Count; i++)
        {
            var level = CurrentArea.levels[i];
            if (!UnlockedLevelIDs.Contains(level.LevelID))
                continue;

            // intenta crear/colocar el path de este nivel con el anterior
            TryCreatePathForLevelIndex(i, animate: false);
        }
    }

    #region Unlock

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

        PlayerPrefs.SetInt(UnlockedPrefix + levelID, 1);
        PlayerPrefs.Save();

        int idx = GetLevelIndex(levelID);
        if (idx != -1)
        {
            if (!TryCreatePathForLevelIndex(idx, animate: true))
            {
                // si a�n no estaban las posiciones, lo intentamos al siguiente frame
                StartCoroutine(PlacePathNextFrame(idx));
            }
        }
    }

    private IEnumerator PlacePathNextFrame(int levelIndex)
    {
        if (_buttonObjects.Count > 1)
        {
            LevelButton levelButton = _buttonObjects[1].GetComponent<LevelButton>();
            string levelToUnlock = levelButton.levelData.LevelID;
            UnlockLevel(levelToUnlock, levelButton);
        }
        else
        {
            Debug.LogWarning("No hay suficiente botones creados a�n para hacer el test de desbloqueo.");
        }
        yield return null;
        TryCreatePathForLevelIndex(levelIndex, animate: true);
    }

    private int GetLevelIndex(string levelId)
    {
        for (int i = 0; i < CurrentArea.levels.Count; i++)
        {
            if (CurrentArea.levels[i].LevelID == levelId)
                return i;
        }
        return -1;
    }

    /// <summary>
    /// Intenta crear (o recolocar) el path entre el nivel anterior y este �ndice.
    /// Devuelve true si pudo.
    /// </summary>
    private bool TryCreatePathForLevelIndex(int idx, bool animate)
    {
        // el primero no tiene path
        if (idx <= 0) return false;

        string prevLevelId = CurrentArea.levels[idx - 1].LevelID;
        string currLevelId = CurrentArea.levels[idx].LevelID;

        // necesitamos que ambos botones tengan su posici�n ya registrada
        if (!_buttonLocationsById.TryGetValue(prevLevelId, out Vector3 prevPos)) return false;
        if (!_buttonLocationsById.TryGetValue(currLevelId, out Vector3 currPos)) return false;

        // si ya existe un segmento para este "toLevel", solo lo recolocamos
        if (_pathByToLevelId.TryGetValue(currLevelId, out LevelPathSegment existingSeg))
        {
            var existingRt = existingSeg.GetComponent<RectTransform>();
            if (existingRt != null)
            {
                PositionPathSegment(existingRt, prevPos, currPos);
            }

            if (animate) existingSeg.Play();
            else
            {
                existingSeg.pathImage.fillAmount = 1f;
                var c = existingSeg.pathImage.color;
                c.a = 1f;
                existingSeg.pathImage.color = c;
            }

            return true;
        }

        // si no existe, lo creamos
        if (pathPrefab == null || pathParent == null)
        {
            Debug.LogWarning("No hay prefab de path o parent asignado en el LevelSelectManager.");
            return false;
        }

        Image path = Instantiate(pathPrefab, pathParent);
        path.name = $"Path_{prevLevelId}_{currLevelId}";
        RectTransform rt = path.rectTransform;

        PositionPathSegment(rt, prevPos, currPos);

        // a�adir / configurar el LevelPathSegment
        LevelPathSegment seg = path.GetComponent<LevelPathSegment>();
        if (seg == null)
            seg = path.gameObject.AddComponent<LevelPathSegment>();

        seg.fromLevelId = prevLevelId;
        seg.toLevelId = currLevelId;
        seg.pathImage = path;

        _pathByToLevelId[currLevelId] = seg;

        if (animate)
        {
            seg.Play();
        }
        else
        {
            path.fillAmount = 1f;
            var c = path.color;
            c.a = 1f;
            path.color = c;
        }

        return true;
    }

    private void PositionPathSegment(RectTransform rt, Vector3 prevWorld, Vector3 currWorld)
    {
        // pasamos las posiciones de mundo al espacio local del parent de los paths
        Vector3 p1Local = pathParent.InverseTransformPoint(prevWorld);
        Vector3 p2Local = pathParent.InverseTransformPoint(currWorld);

        Vector3 midLocal = (p1Local + p2Local) * 0.5f;
        float dist = Vector2.Distance(p1Local, p2Local);
        Vector3 dir = (p2Local - p1Local).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        rt.anchoredPosition = midLocal;
        rt.sizeDelta = new Vector2(dist, pathThickness);
        rt.rotation = Quaternion.Euler(0f, 0f, angle);
        rt.pivot = new Vector2(0.5f, 0.5f);
    }

    #endregion

    private void OnApplicationQuit()
    {
        // esto es tu l�gica original
        foreach (var level in CurrentArea.levels)
        {
            PlayerPrefs.DeleteKey(UnlockedPrefix + level.LevelID);
        }
        PlayerPrefs.DeleteKey("PendingLevelUnlock");
        PlayerPrefs.Save();
    }

    #region Player movement in map

    private IEnumerator SpawnInPlayerAfterDelay(RectTransform screenSpaceButton, RectTransform worldSpaceCanvas)
    {
        yield return null;
        SpawnInPlayer(screenSpaceButton, worldSpaceCanvas);
    }

    private void SpawnInPlayer(RectTransform screenSpaceUIObject, RectTransform worldSpaceUIObject)
    {
        _playerIsFacingRight = true;
        PlayerObj = Instantiate(PlayerUIPrefab, worldSpaceUIObject);

        RectTransform playerRect = PlayerObj.GetComponent<RectTransform>();

        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(_camera, screenSpaceUIObject.position);
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(worldSpaceUIObject, screenPosition, _camera, out Vector2 localPoint))
        {
            playerRect.anchoredPosition = localPoint + PlayerPositionOffsetPerLevel;
        }

        // mirar hacia el segundo bot�n, si hay
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
