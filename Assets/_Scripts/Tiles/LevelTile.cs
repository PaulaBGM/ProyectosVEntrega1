using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelTile
{
    public LevelTile(Vector3Int localPosition,
        Vector3 worldPosition,
        TileBase tileBase,
        Tilemap tilemapMember,
        int heightLayer)
    {
        LocalPosition = localPosition;
        WorldPosition = worldPosition;
        TileBase = tileBase;
        TilemapMember = tilemapMember;
        HeightLayer = heightLayer;
    }

    public readonly Vector3Int LocalPosition;

    public readonly Vector3 WorldPosition;

    public readonly TileBase TileBase;

    public readonly Tilemap TilemapMember;

    public readonly int HeightLayer;

    //Others

    public IOccupant Occupant { get; set; } = null;

    public LevelTileNeighbours TileNeighbours { get; set; }

    public float WidthSize => TilemapMember.cellSize.x;

    public float HeightSize => TilemapMember.cellSize.y;

    // Flags mínimos
    public bool IsLadder { get; set; } = false;

    public bool IsWalkable => LevelTileMetadata.IsWalkable(TileBase);

    public IEnumerable<LevelTile> GetNeightbours()
    {
        if (TileNeighbours.UpTile != null && TileNeighbours.UpTile.IsWalkable) yield return TileNeighbours.UpTile;
        if (TileNeighbours.DownTile != null && TileNeighbours.DownTile.IsWalkable) yield return TileNeighbours.DownTile;
        if (TileNeighbours.LeftTile != null && TileNeighbours.LeftTile.IsWalkable) yield return TileNeighbours.LeftTile;
        if (TileNeighbours.RightTile != null && TileNeighbours.RightTile.IsWalkable) yield return TileNeighbours.RightTile;

        // Verticales (si fueron asignados)
        if (TileNeighbours.UpLevelTile != null && TileNeighbours.UpLevelTile.IsWalkable) yield return TileNeighbours.UpLevelTile;
        if (TileNeighbours.DownLevelTile != null && TileNeighbours.DownLevelTile.IsWalkable) yield return TileNeighbours.DownLevelTile;
    }
}

public class LevelTileNeighbours
{
    public LevelTileNeighbours(LevelTile upTile, LevelTile downTile, LevelTile rightTile, LevelTile leftTile,
                               LevelTile upLevelTile, LevelTile downLevelTile)
    {
        UpTile = upTile;
        DownTile = downTile;
        RightTile = rightTile;
        LeftTile = leftTile;
        UpLevelTile = upLevelTile;
        DownLevelTile = downLevelTile;
    }

    public readonly LevelTile UpTile;
    public readonly LevelTile DownTile;
    public readonly LevelTile RightTile;
    public readonly LevelTile LeftTile;

    // Conexiones verticales entre capas
    public readonly LevelTile UpLevelTile;
    public readonly LevelTile DownLevelTile;
}
