using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName ="Level Data/Areas", fileName="New Area")]

public class AreaData : ScriptableObject
{
    public string areaName;
    public List<LevelData> levels = new List<LevelData>();
    

  
}
