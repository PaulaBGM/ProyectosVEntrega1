using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameTiles : MonoBehaviour
{
    [SerializeField]
    private Tilemap[] _tilemapsFromTopToBottom;

    public Dictionary<Vector3, LevelTile> Tiles { get; private set; } = new();

    public void GetWorldTiles()
    {
        Tiles.Clear();
        int layerCounter = 0;

        foreach (var tilemap in _tilemapsFromTopToBottom)
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
                    HeightLayer = layerCounter
                };

                Tiles.Add(tile.WorldPosition, tile);
            }

            layerCounter++;
        }
    }

    public void SetOccupantsInTiles(IEnumerable<GameObject> occupants)
    {
        var tilemap = _tilemapsFromTopToBottom.First();

        foreach (var occupant in occupants)
        {
            if (occupant.TryGetComponent<IOccupant>(out var occup))
            {
                if (Tiles.TryGetValue(tilemap.WorldToCell(occupant.transform.position), out var tile))
                {
                    occup.AssignTile(tile);
                }

                Debug.Log(tilemap.WorldToCell(occupant.transform.position));
            }
        }
    }
}