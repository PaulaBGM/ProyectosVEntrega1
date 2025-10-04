using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameTiles : MonoBehaviour
{
    [SerializeField]
    private Tilemap[] _tilemapsFromTopToBottom;

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
        int layerCounter = 0;

        foreach (var tilemap  in _tilemapsFromTopToBottom)
        {
            foreach (Vector3Int localPosition in tilemap.cellBounds.allPositionsWithin)
            {
                if ((!tilemap.HasTile(localPosition)) || 
                    Tiles.ContainsKey(tilemap.CellToWorld(localPosition)))
                        continue;

                var tile = new LevelTile()
                {
                    LocalPosition = localPosition,
                    WorldPosition = tilemap.CellToWorld(localPosition),
                    TileBase = tilemap.GetTile(localPosition),
                    TilemapMember = tilemap,
                    Height = layerCounter
                };

                Tiles.Add(tile.WorldPosition, tile);
            }

            layerCounter++;
        }
    }
}