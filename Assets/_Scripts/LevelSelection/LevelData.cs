using UnityEngine;

[CreateAssetMenu(menuName = "Level Data/levels", fileName = "New Level")]
public class LevelData : ScriptableObject
{
    [Header("Level Stats")]
    public string LevelID;
    [Tooltip("For Starting Levels")] public bool ISUnlockedByDefault;
    public SceneField scene;

    [Header("Level Display Information")]
    public string LevelName;

    public GameObject LevelButtonObj {  get; set; }
  
}
