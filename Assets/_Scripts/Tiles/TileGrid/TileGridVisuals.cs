using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGridVisuals : MediatorClientSystem<TileGridMediator>
{
    IEnumerable<LevelTile> tilesHighlighted = new List<LevelTile>();

    private void OnEnable()
    {
        mediator.OnMovementTilesSet += HighlightMovementTiles;
        mediator.OnPlayerOccupantMove += HideHighlightMovementTiles;
    }

    private void HighlightMovementTiles(IEnumerable<LevelTile> movementTiles)
    {
        tilesHighlighted = movementTiles;

        foreach (var tile in movementTiles)
        {
            tile.TilemapMember.SetTileFlags(tile.LocalPosition, TileFlags.LockTransform);
            tile.TilemapMember.SetColor(tile.LocalPosition, Color.blue);
        }
    }

    private void HideHighlightMovementTiles((LevelTile tile, IOccupant occupant) _)
    {
        if (!tilesHighlighted.Any())
            return;

        foreach (var tile in tilesHighlighted)
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
