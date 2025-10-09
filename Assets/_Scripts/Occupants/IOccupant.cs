using System.Numerics;
using UnityEngine;

public interface IOccupant
{
    LevelTile TileAssigned { get; set; }
    void AssignTile(LevelTile levelTile);
    void MoveTo(LevelTile levelTile);
}
