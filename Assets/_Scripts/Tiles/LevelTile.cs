using System.Collections.Generic;
using _Scripts.Occupants;
using _Scripts.TileInteractor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.Tiles
{
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
        
        public ITileInteractor TileInteractor { get; set; } = null;
        
        public LevelTileNeighbours TileNeighbours { get; set; }

        public float WidthSize => TilemapMember.cellSize.x;

        public float HeightSize => TilemapMember.cellSize.y;

        public IEnumerable<LevelTile> GetNeighbours()
        {
            yield return TileNeighbours.UpTile;
            yield return TileNeighbours.DownTile;
            yield return TileNeighbours.LeftTile;
            yield return TileNeighbours.RightTile;
        }
    }

    public class LevelTileNeighbours
    {
        public LevelTileNeighbours(LevelTile upTile, LevelTile downTile, LevelTile rightTile, LevelTile leftTile)
        {
            UpTile = upTile;
            DownTile = downTile;
            RightTile = rightTile;
            LeftTile = leftTile;
        }

        public readonly LevelTile UpTile;
        public readonly LevelTile DownTile;
        public readonly LevelTile RightTile;
        public readonly LevelTile LeftTile;
    }
}