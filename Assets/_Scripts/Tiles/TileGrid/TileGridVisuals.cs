using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.Tiles.TileGrid
{
    public class TileGridVisuals : MediatorClientSystem<TileGridMediator>
    {
        private IEnumerable<LevelTile> _tilesHighlighted = new List<LevelTile>();

        private void OnEnable()
        {
            mediator.OnMovementTilesSet += HighlightMovementTiles;
            mediator.OnPlayerOccupantMove += HideHighlightMovementTiles;
        }

        private void HighlightMovementTiles(IEnumerable<LevelTile> movementTiles)
        {
            var tilesHighlighted = movementTiles as LevelTile[] ?? movementTiles.ToArray();
            _tilesHighlighted = tilesHighlighted;

            foreach (var tile in tilesHighlighted)
            {
                tile.TilemapMember.SetTileFlags(tile.LocalPosition, TileFlags.LockTransform);
                tile.TilemapMember.SetColor(tile.LocalPosition, Color.blue);
            }
        }

        private void HideHighlightMovementTiles((LevelTile tile, IOccupant occupant) _)
        {
            if (!_tilesHighlighted.Any())
                return;

            foreach (var tile in _tilesHighlighted)
            {
                tile.TilemapMember.SetTileFlags(tile.LocalPosition, TileFlags.LockTransform);
                tile.TilemapMember.SetColor(tile.LocalPosition, Color.white);
            }
        }

        private void OnDisable()
        {
            mediator.OnMovementTilesSet -= HighlightMovementTiles;
            mediator.OnPlayerOccupantMove -= HideHighlightMovementTiles;
        }
    }
}
