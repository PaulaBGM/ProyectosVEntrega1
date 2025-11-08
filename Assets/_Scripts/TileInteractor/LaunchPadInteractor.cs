using System.Linq;
using System.Collections.Generic;
using _Scripts.Occupants;
using _Scripts.Tiles;
using UnityEngine;
using Random = System.Random;

namespace _Scripts.TileInteractor
{
    public class LaunchPadInteractor : MonoBehaviour, ITileInteractor
    {
        [SerializeField]
        private Transform _desiredDestinationTilePosition;
        
        private LevelTile _destinationTile;

        public void Interact(IOccupant occupant)
        {
            if (_destinationTile.Occupant == null)
            {
                occupant.AssignTile(_destinationTile);
                occupant.Transform.position = _desiredDestinationTilePosition.position;
            }
            else
            {
                var random = new Random();

                var neighbours = new List<LevelTile>();
                foreach (var neighbour in _destinationTile.GetNeighbours())
                {
                    if (neighbour is not null)
                        neighbours.Add(neighbour);
                }
                    
                var newDestinationTile =  neighbours[random.Next(neighbours.Count)];
                
                occupant.AssignTile(newDestinationTile);
                occupant.Transform.position = newDestinationTile.WorldPositionCenter;
            }
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