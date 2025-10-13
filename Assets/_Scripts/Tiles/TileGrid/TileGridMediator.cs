using System;
using System.Collections.Generic;
using UnityEngine;

public class TileGridMediator : MonoBehaviour, IMediator
{
    public event Action<Dictionary<Vector3, LevelTile>> OnWorldTilesSet;

    public event Action<IEnumerable<LevelTile>> OnMovementTilesSet;

    //EventBus

    public event Action<OnPlayerOccupantSelected> OnPlayerOccupantSelected;

    public event Action<OnPlayerOccupantMove> OnPlayerOccupantMove;

    private void OnEnable()
    {
        EventBus<OnPlayerOccupantSelected>.Subscribe(PlayerOccupantSelected);
        EventBus<OnPlayerOccupantMove>.Subscribe(PlayerOccupantMove);
    }

    public void WorldTilesSet(Dictionary<Vector3, LevelTile> tiles)
    {
        OnWorldTilesSet?.Invoke(tiles);
    }

    public void MovementTilesSet(IEnumerable<LevelTile> movementTiles)
    {
        OnMovementTilesSet?.Invoke(movementTiles);
    }

    public void PlayerOccupantSelected(OnPlayerOccupantSelected eventData)
    {
        OnPlayerOccupantSelected?.Invoke(eventData);
    }

    public void PlayerOccupantMove(OnPlayerOccupantMove eventData)
    {
        OnPlayerOccupantMove?.Invoke(eventData);
    }

    private void OnDisable()
    {
        EventBus<OnPlayerOccupantSelected>.Unsubscribe(PlayerOccupantSelected);
        EventBus<OnPlayerOccupantMove>.Unsubscribe(PlayerOccupantMove);
    }
}