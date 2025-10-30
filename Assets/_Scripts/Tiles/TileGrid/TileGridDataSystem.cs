using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGridDataSystem : MediatorClientSystem<TileGridMediator>
{
    [SerializeField]
    private Tilemap[] _tilemapsFromTopToBottom;

    public Dictionary<TileKey, LevelTile> Tiles { get; private set; } = new();

    public void SetWorldTiles()
    {
        Tiles.Clear();
        int layerCounter = 0;

        foreach (var tilemap in _tilemapsFromTopToBottom)
        {
            foreach (Vector3Int localPosition in tilemap.cellBounds.allPositionsWithin)
            {
                if (!tilemap.HasTile(localPosition))
                    continue;

                var key = new TileKey(localPosition, layerCounter);
                if (Tiles.ContainsKey(key))
                    continue;

                var tileBase = tilemap.GetTile(localPosition);

                var tile = new LevelTile(
                    localPosition: localPosition,
                    worldPosition: tilemap.CellToWorld(localPosition),
                    tileBase: tileBase,
                    tilemapMember: tilemap,
                    heightLayer: layerCounter);

                // Flags básicos (puedes conectar con tu pipeline)
                tile.IsLadder = LevelTileMetadata.IsLadder(tileBase);

                Tiles.Add(key, tile);
            }

            layerCounter++;
        }

        AssignNeighbours();
        mediator.WorldTilesSet(Tiles);
    }

    private void AssignNeighbours()
    {
        foreach (var kv in Tiles)
        {
            var key = kv.Key;
            var tile = kv.Value;

            // Laterales en la misma capa
            var up = TryGet(key.Cell + new Vector3Int(0, 1, 0), key.Layer);
            var down = TryGet(key.Cell + new Vector3Int(0, -1, 0), key.Layer);
            var right = TryGet(key.Cell + new Vector3Int(1, 0, 0), key.Layer);
            var left = TryGet(key.Cell + new Vector3Int(-1, 0, 0), key.Layer);

            // Verticales entre capas: requiere escalera en ambos extremos
            LevelTile upLevel = null, downLevel = null;

            var upLayerTile = TryGet(key.Cell, key.Layer + 1);
            var downLayerTile = TryGet(key.Cell, key.Layer - 1);

            if (tile.IsLadder && upLayerTile != null && upLayerTile.IsLadder)
                upLevel = upLayerTile;

            if (tile.IsLadder && downLayerTile != null && downLayerTile.IsLadder)
                downLevel = downLayerTile;

            tile.TileNeighbours = new LevelTileNeighbours(up, down, right, left, upLevel, downLevel);
        }

        LevelTile TryGet(Vector3Int cell, int layer)
            => Tiles.TryGetValue(new TileKey(cell, layer), out var t) ? t : null;
    }
}
