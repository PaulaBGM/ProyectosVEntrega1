using System.Numerics;
using UnityEngine;

public interface IOccupant
{
    LevelTile TileAssigned { get; }
    int MaxMovementTiles { get; }
    Transform Transform { get; }
    void AssignTile(LevelTile levelTile);
}

public interface IPlayerOccupant : IOccupant { }

public interface IAIOccupant : IOccupant { }
