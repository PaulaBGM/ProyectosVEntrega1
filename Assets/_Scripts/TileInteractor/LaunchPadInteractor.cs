using System.Linq;
using System.Collections.Generic;
using _Scripts.Occupants;
using _Scripts.Tiles;
using UnityEngine;

namespace _Scripts.TileInteractor
{
    public class LaunchPadInteractor : MonoBehaviour, ITileInteractor
    {
        [SerializeField]
        private Transform _desiredDestinationTilePosition;
        
        private LevelTile _destinationTile;

        public void Interact(IOccupant occupant)
        {
            occupant.AssignTile(_destinationTile);
            occupant.Transform.position = _desiredDestinationTilePosition.position;
        }

        public void SetTileInteractor(Dictionary<Vector3, LevelTile> tiles)
        {
            var tilemap = tiles.First().Value.TilemapMember;

            if (tiles.TryGetValue(tilemap.WorldToCell(_desiredDestinationTilePosition.position), out var tile))
            {
                _destinationTile = tile;
            }
            else
            {
                Debug.LogError($"Destination tile not assigned." +
                               $" Check if desiredPosition matches an existing destination tile");
            }
        }
    }
}