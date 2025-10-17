using System;
using System.Collections.Generic;
using _Scripts.Events;
using UnityEngine;

namespace _Scripts.Tiles.TileGrid
{
    public class TileGridMediator : MonoBehaviour, IMediator
    {
        public event Action<Dictionary<Vector3, LevelTile>> OnWorldTilesSet;

        public event Action<IEnumerable<LevelTile>> OnMovementTilesSet;

        public event Action<LevelTile> OnTileExecuteAction;

        //EventBus

        public event Action<(LevelTile levelTile, int maxMovementTiles)> OnOccupantSelected;

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

        public void OccupantSelected(LevelTile levelTile, int maxMovementTiles)
        {
            OnOccupantSelected?.Invoke((levelTile, maxMovementTiles));
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
}