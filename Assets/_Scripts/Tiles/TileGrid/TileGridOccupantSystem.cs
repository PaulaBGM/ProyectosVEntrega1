using System.Collections.Generic;
using System.Linq;
using _Scripts.Core.Mediator;
using _Scripts.Events;
using _Scripts.Occupants;
using UnityEngine;

namespace _Scripts.Tiles.TileGrid
{
    public class TileGridOccupantSystem : MediatorClientSystem<TileGridMediator>
    {
        [SerializeField]
        private GameObject[] _occupants;

        private IOccupant _occupantSelected;

        private void OnEnable()
        {
            mediator.OnWorldTilesSet += SetOccupantsInTiles;
            mediator.OnTileExecuteAction += ExecuteActionInTile;
        }

        public void SetOccupantsInTiles(Dictionary<Vector3, LevelTile> tiles)
        {
            var tilemap = tiles.First().Value.TilemapMember;

            foreach (var occupant in _occupants)
            {
                if (occupant.TryGetComponent<IOccupant>(out var occup))
                {
                    if (tiles.TryGetValue(tilemap.WorldToCell(occupant.transform.position), out var tile))
                    {
                        occup.AssignTile(tile);
                    }

                    Debug.Log(tilemap.WorldToCell(occupant.transform.position));
                }
                else
                {
                    Debug.LogError("Gameobject: " +  occupant.name + " doesn't implement the " +
                                   "IOccupant interface or misses the script that does");
                }
            }
        }

        public void ExecuteActionInTile(LevelTile tileSelected)
        {
            if (_occupantSelected is IPlayerOccupant playerOccupant)
            {
                HandlePlayerAction(tileSelected, playerOccupant);
                _occupantSelected = null;
                return;
            }
            
            SelectOccupantOnTile(tileSelected);
        }

        private void HandlePlayerAction(LevelTile tileSelected, IPlayerOccupant playerOccupant)
        {
            var occupantOnTile = tileSelected.Occupant;
            
            if (occupantOnTile is IAIOccupant catOccupant && CanCatchCat(catOccupant, playerOccupant))
            {
                catOccupant.Catch();
                EventBus<OnPlayerAction>.Publish(new OnPlayerAction());
            }
            else
            {
                mediator.PlayerOccupantMove(tileSelected, playerOccupant);
            }
        }

        private void SelectOccupantOnTile(LevelTile tileSelected)
        {
            _occupantSelected = tileSelected.Occupant;

            if (_occupantSelected != null)
            {
                mediator.OccupantSelected(tileSelected, _occupantSelected.MaxMovementTiles);
            }
        }


        private bool CanCatchCat(IAIOccupant catOccupant, IPlayerOccupant playerOccupant) =>
            catOccupant.TileAssigned.GetNeightbours().Contains(playerOccupant.TileAssigned);

        private void OnDisable()
        {
            mediator.OnWorldTilesSet -= SetOccupantsInTiles;
            mediator.OnTileExecuteAction -= ExecuteActionInTile;
        }
    }
}
