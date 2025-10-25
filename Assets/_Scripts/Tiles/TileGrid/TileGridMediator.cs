using System;
using System.Collections.Generic;
using _Scripts.Core.Mediator;
using _Scripts.Events;
using _Scripts.Input;
using _Scripts.Occupants;
using UnityEngine;

namespace _Scripts.Tiles.TileGrid
{
    public class TileGridMediator : MonoBehaviour, IMediator
    {
        public event Action<Dictionary<Vector3, LevelTile>> OnWorldTilesSet;

        public event Action<IEnumerable<LevelTile>> OnMovementTilesSet;

        public event Action<LevelTile> OnTileExecuteAction;

        //EventBus or Singletons events

        public event Action<(LevelTile levelTile, int maxMovementTiles)> OnOccupantSelected;

        public event Action<(LevelTile levelTile, IPlayerOccupant playerOccupant)> OnPlayerOccupantMove;

        public event Action<Vector3> OnTileClicked;
        
        public event Action<OnPerformAIAction> OnPerformAIAction;

        private void OnEnable()
        {
            PlayerInputHandler.Instance.OnSelect += TileClicked;
            EventBus<OnPerformAIAction>.Subscribe(PerformAIAction);
        }

        public void WorldTilesSet(Dictionary<Vector3, LevelTile> tiles)
        {
            OnWorldTilesSet?.Invoke(tiles);
            EventBus<OnWorldTilesSet>.Publish(new OnWorldTilesSet { Tiles = tiles.Values });
        }

        public void MovementTilesSet(IEnumerable<LevelTile> movementTiles)
        {
            OnMovementTilesSet?.Invoke(movementTiles);
        }

        public void TileExecuteAction (LevelTile tileSelected)
        {
            OnTileExecuteAction?.Invoke(tileSelected);
        }

        public void OccupantSelected(LevelTile levelTile, int maxMovementTiles)
        {
            OnOccupantSelected?.Invoke((levelTile, maxMovementTiles));
        }

        public void PlayerOccupantMove(LevelTile levelTile, IPlayerOccupant playerOccupant)
        {
            OnPlayerOccupantMove?.Invoke((levelTile, playerOccupant));
        }

        private void TileClicked(Vector3 pointInWorld)
        {
            OnTileClicked?.Invoke(pointInWorld);
        }
        
        private void PerformAIAction(OnPerformAIAction eventData)
        {
            OnPerformAIAction?.Invoke(eventData);
        }

        private void OnDisable()
        {
            PlayerInputHandler.Instance.OnSelect -= TileClicked;
            EventBus<OnPerformAIAction>.Unsubscribe(PerformAIAction);
        }
    }
}