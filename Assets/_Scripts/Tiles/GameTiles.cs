using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameTiles : MonoBehaviour
{
    [SerializeField]
    private Tilemap _tilemap;

    public Dictionary<Vector3, LevelTile> Tiles { get; private set; } = new();

    private void Awake()
    {
        GetWorldTiles();
    }

    private void Start()
    {
        EventBus<OnGameTilesAssigned>.Publish(new OnGameTilesAssigned
        {
            GameTiles = this
        });
    }

    private void GetWorldTiles()
    {
        Tiles.Clear();
        foreach (Vector3Int localPosition in _tilemap.cellBounds.allPositionsWithin)
        {
            if (!_tilemap.HasTile(localPosition)) continue;
            var tile = new LevelTile()
            {
                LocalPosition = localPosition,
                WorldPosition = _tilemap.CellToWorld(localPosition),
                TileBase = _tilemap.GetTile(localPosition),
                TilemapMember = _tilemap
            };

            Tiles.Add(tile.WorldPosition, tile);
        }
    }
}