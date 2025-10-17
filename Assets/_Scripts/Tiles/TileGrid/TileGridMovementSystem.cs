using System.Collections.Generic;
using System.Linq;
using _Scripts.Events;
using UnityEngine;

namespace _Scripts.Tiles.TileGrid
{
    public class TileGridMovementSystem : MediatorClientSystem<TileGridMediator>
    {
        private IEnumerable<LevelTile> _availableTilesToMove;

        private void OnEnable()
        {
            mediator.OnOccupantSelected += PlayerOccupantSelected;
            mediator.OnPlayerOccupantMove += MovePlayerOccupantToTile;
        }
      
        private void PlayerOccupantSelected(
            (LevelTile tileSelected, int maxMovementTiles) eventData)
        {
            _availableTilesToMove = GetMovementTiles(eventData.tileSelected, eventData.maxMovementTiles);
        }

        private IEnumerable<LevelTile> GetMovementTiles(LevelTile initialTile, int maxMovementTiles)
        {
            var visited = new HashSet<LevelTile>();
            var frontier = new Queue<(LevelTile tile, int depth)>();
            var reachable = new List<LevelTile>();

            frontier.Enqueue((initialTile, 0));
            visited.Add(initialTile);

            while (frontier.Count > 0)
            {
                var (current, depth) = frontier.Dequeue();

                if (depth > 0)
                    reachable.Add(current);

                if (depth >= maxMovementTiles)
                    continue;

                foreach (var neightbour in current.GetNeightbours())
                {
                    if (neightbour is null ||
                        visited.Contains(neightbour) ||
                        neightbour.Occupant is not null) 
                        continue;

                    visited.Add(neightbour);
                    frontier.Enqueue((neightbour, depth + 1));
                }
            }

            mediator.MovementTilesSet(reachable);
            return reachable;
        }

        private void MovePlayerOccupantToTile((LevelTile tileToMove, IOccupant occupant) eventData)
        {
            var tileToMove = eventData.tileToMove;
            var occupant = eventData.occupant;

            if (!_availableTilesToMove.Contains(tileToMove) || occupant is not IPlayerOccupant)
                return;

            occupant.AssignTile(tileToMove);
            occupant.Transform.position = tileToMove.TilemapMember.CellToWorld(tileToMove.LocalPosition)
                                          + (Vector3)tileToMove.TilemapMember.cellSize * 0.5f;
            
            EventBus<OnPlayerAction>.Publish(new OnPlayerAction());
        }

        private void OnDisable()
        {
            mediator.OnOccupantSelected -= PlayerOccupantSelected;
            mediator.OnPlayerOccupantMove -= MovePlayerOccupantToTile;
        }
    }
}
