using System.Collections.Generic;
using System.Linq;
using _Scripts.Core.Mediator;
using _Scripts.Events;
using _Scripts.Occupants;
using UnityEngine;

namespace _Scripts.Tiles.TileGrid
{
    public class TileGridMovementSystem : MediatorClientSystem<TileGridMediator>
    {
        private IEnumerable<LevelTile> _availableTilesToMove;

        private void OnEnable()
        {
            mediator.OnOccupantSelected += OccupantSelected;
            mediator.OnPlayerOccupantMove += MovePlayerOccupantToTile;
            mediator.OnPerformAIAction += MoveAIOccupantToTile;
        }
      
        private void OccupantSelected(
            (LevelTile tileSelected, int maxMovementTiles) eventData)
        {
            _availableTilesToMove = GetMovementTiles(eventData.tileSelected, eventData.maxMovementTiles);
            mediator.MovementTilesSet(_availableTilesToMove);
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
            
            return reachable;
        }

        private void MovePlayerOccupantToTile((LevelTile tileToMove, IPlayerOccupant playerOccupant) eventData)
        {
            var tileToMove = eventData.tileToMove;
            var occupant = eventData.playerOccupant;

            if (!_availableTilesToMove.Contains(tileToMove))
                return;

            occupant.AssignTile(tileToMove);
            occupant.Transform.position = tileToMove.TilemapMember.CellToWorld(tileToMove.LocalPosition)
                                          + tileToMove.TilemapMember.cellSize * 0.5f;
            
            EventBus<OnPlayerAction>.Publish(new OnPlayerAction());
        }
        
        private void MoveAIOccupantToTile(OnPerformAIAction eventData)
        {
            var aiOccupant = eventData.AiOccupant;
            
            var tileToMove = GetAiMovementTile(aiOccupant);
            
            aiOccupant.AssignTile(tileToMove);
            aiOccupant.Transform.position = tileToMove.TilemapMember.CellToWorld(tileToMove.LocalPosition)
                                          + tileToMove.TilemapMember.cellSize * 0.5f;
        }
        
        private LevelTile GetAiMovementTile(IAIOccupant aiOccupant)
        {
            var availableTiles = GetMovementTiles(
                    aiOccupant.TileAssigned, aiOccupant.MaxMovementTiles)
                .Where(tile =>
                    tile.GetNeightbours().Where(neighbour => neighbour is not null)
                        .All(neighbour => neighbour.Occupant is not IPlayerOccupant))
                .ToArray();
            
            int randomIndex = UnityEngine.Random.Range(0, availableTiles.Count());
            return availableTiles[randomIndex];
        }
        
        private void OnDisable()
        {
            mediator.OnOccupantSelected -= OccupantSelected;
            mediator.OnPlayerOccupantMove -= MovePlayerOccupantToTile;
            mediator.OnPerformAIAction -= MoveAIOccupantToTile;
        }
    }
}
