using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling.Memory.Experimental;

public class TileGridMovementSystem : MediatorClientSystem<TileGridMediator>
{
    private Dictionary<Vector3, LevelTile> _tiles;

    private IEnumerable<LevelTile> _availableTilesToMove;

    private void OnEnable()
    {
        mediator.OnPlayerOccupantSelected += PlayerOccupantSelected;
        mediator.OnPlayerOccupantMove += MoveToTile;
    }
      
    private void PlayerOccupantSelected(OnPlayerOccupantSelected eventData)
    {
        _availableTilesToMove = GetMovementTiles(eventData.tileSelected, eventData.MaxMovementTiles);
    }

    public IEnumerable<LevelTile> GetMovementTiles(LevelTile initialTile, int maxMovementTiles)
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
                if (neightbour is null || visited.Contains(neightbour)) 
                    continue;

                visited.Add(neightbour);
                frontier.Enqueue((neightbour, depth + 1));
            }
        }

        mediator.MovementTilesSet(reachable);
        return reachable;
    }

    public void MoveToTile(OnPlayerOccupantMove eventData)
    {
        var tileToMove = eventData.tileToMove;
        var occupant = eventData.Occupant;

        if (!_availableTilesToMove.Contains(tileToMove))
            return;

        occupant.AssignTile(tileToMove);
        occupant.Transform.position = tileToMove.TilemapMember.CellToWorld(tileToMove.LocalPosition)
                     + (Vector3)tileToMove.TilemapMember.cellSize * 0.5f;
    }

    private void OnDisable()
    {
        mediator.OnPlayerOccupantSelected -= PlayerOccupantSelected;
    }
}
