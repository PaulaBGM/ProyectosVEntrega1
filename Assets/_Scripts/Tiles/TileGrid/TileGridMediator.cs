using System;
using System.Collections.Generic;
using UnityEngine;

public class TileGridMediator : MonoBehaviour, IMediator
{
    public event Action<Dictionary<Vector3, LevelTile>> OnWorldTilesSet;

    public event Action<IEnumerable<LevelTile>> OnMovementTilesSet;

    public event Action<LevelTile> OnTileExecuteAction;

    //EventBus

    public event Action<(LevelTile levelTile, int maxMovementTiles)> OnPlayerOccupantSelected;

    public event Action<(LevelTile levelTile, IOccupant occupant)> OnPlayerOccupantMove;

    public event Action<OnTileClicked> OnTileClicked;

    private void OnEnable()
    {
        EventBus<OnTileClicked>.Subscribe(TileClicked);
    }

    public void WorldTilesSet(Dictionary<Vector3, LevelTile> tiles)
    {
        OnWorldTilesSet?.Invoke(tiles);
    }

    public void MovementTilesSet(IEnumerable<LevelTile> movementTiles)
    {
        OnMovementTilesSet?.Invoke(movementTiles);
    }

    public void TileExecuteAction (LevelTile tileSelected)
    {
        OnTileExecuteAction?.Invoke(tileSelected);
    }

    public void PlayerOccupantSelected(LevelTile levelTile, int maxMovementTiles)
    {
        OnPlayerOccupantSelected?.Invoke((levelTile, maxMovementTiles));
    }

    public void PlayerOccupantMove(LevelTile levelTile, IOccupant occupant)
    {
        OnPlayerOccupantMove?.Invoke((levelTile, occupant));
    }

    public void TileClicked(OnTileClicked eventData)
    {
        OnTileClicked?.Invoke(eventData);
    }

    private void OnDisable()
    {
        EventBus<OnTileClicked>.Unsubscribe(TileClicked);
    }
}