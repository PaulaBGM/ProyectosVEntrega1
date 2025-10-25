using System.Collections.Generic;
using System.Linq;
using _Scripts.Core.Mediator;
using _Scripts.Events;
using _Scripts.Occupants;
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
            mediator.OnTileClicked += HideHighlightMovementTiles;
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

        private void HideHighlightMovementTiles(Vector3 _)
        {
            foreach (var tile in _tilesHighlighted)
            {
                tile.TilemapMember.SetTileFlags(tile.LocalPosition, TileFlags.LockTransform);
                tile.TilemapMember.SetColor(tile.LocalPosition, Color.white);
            }
        }

        private void OnDisable()
        {
            mediator.OnMovementTilesSet -= HighlightMovementTiles;
        }
    }
}
