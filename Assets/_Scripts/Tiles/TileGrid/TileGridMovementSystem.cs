using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Core.Mediator;
using _Scripts.Events;
using _Scripts.Occupants;
using _Scripts.Pathfinding;
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
            (LevelTile tileSelected, IOccupant occupant) eventData)
        {
            _availableTilesToMove = GetMovementTiles(eventData.tileSelected, eventData.occupant);
            mediator.MovementTilesSet(_availableTilesToMove);
        }

        private IEnumerable<LevelTile> GetMovementTiles(LevelTile initialTile, IOccupant occupant)
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

                if (depth >= occupant.MaxMovementTiles)
                    continue;

                foreach (var neighbour in current.GetNeighbours())
                {
                    if (neighbour is null ||
                        visited.Contains(neighbour) ||
                        neighbour.Occupant is IPlayerOccupant ||
                        neighbour.Occupant is IAIOccupant && occupant is IAIOccupant) 
                        continue;

                    visited.Add(neighbour);
                    frontier.Enqueue((neighbour, depth + 1));
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
            StartCoroutine(StartPlayerMovement(occupant, tileToMove));
        }

        private IEnumerator StartPlayerMovement(IPlayerOccupant playerOccupant, LevelTile tileToMove)
        {
            playerOccupant.TriggerUnstick();
            yield return new WaitUntil(() => playerOccupant.IsUnstick);
            playerOccupant.Transform.position = tileToMove.TilemapMember.CellToWorld(tileToMove.LocalPosition)
                                                + tileToMove.TilemapMember.cellSize * 0.5f;
            
            playerOccupant.TriggerMove();
            
            EventBus<OnPlayerAction>.Publish(new OnPlayerAction());
            TryExecuteTileInteractor(tileToMove, playerOccupant);
        }
        
        private void MoveAIOccupantToTile(OnPerformAIAction eventData)
        {
            var aiOccupant = eventData.AiOccupant;
            
            var tileToMove = GetAiMovementTile(aiOccupant);

            if (tileToMove is null)
            {
                aiOccupant.IsAIActionFinished = true;
                return;
            }

            var enumerablePath = GeneratePath(aiOccupant.TileAssigned, tileToMove);
            var path = enumerablePath.ToArray();
            
            aiOccupant.AssignTile(tileToMove);

            StartCoroutine(MoveAiOccupantAlongPath(aiOccupant, path));
        }
        
        private LevelTile GetAiMovementTile(IAIOccupant aiOccupant)
        {
            var availableTiles = GetMovementTiles(
                    aiOccupant.TileAssigned, aiOccupant)
                .Where(tile =>
                    tile.GetNeighbours().Where(neighbour => neighbour is not null)
                        .All(neighbour => neighbour.Occupant is not IPlayerOccupant))
                .ToArray();
            
            if (availableTiles.Length <= 0)
                return null;
            
            int randomIndex = UnityEngine.Random.Range(0, availableTiles.Count());
            return availableTiles[randomIndex];
        }

        private static IEnumerable<Vector3> GeneratePath(LevelTile startTile, LevelTile endTile)
        {
            var pathNodeClosedList = new HashSet<LevelTile>();
            var pathNodeOpenList = new HashSet<PathNode>();
            var nodeMap = new Dictionary<LevelTile, PathNode>();

            var startNode = new PathNode(
                startTile, null, 0, GetLocalDistanceBetweenTiles(startTile, endTile));
            pathNodeOpenList.Add(startNode);
            nodeMap[startTile] = startNode;
            
            while (pathNodeOpenList.Count > 0)
            {
                var currentNode = pathNodeOpenList.OrderBy(f => f.FCost).First();
                pathNodeOpenList.Remove(currentNode);
                pathNodeClosedList.Add(currentNode.LevelTile);
                
                if (currentNode.LevelTile == endTile)
                    break;
                
                foreach (var neighbour in currentNode.LevelTile.GetNeighbours())
                {
                    if (neighbour == null ||
                        neighbour.Occupant is IAIOccupant ||
                        pathNodeClosedList.Contains(neighbour))
                        continue;

                    var tentativeG = currentNode.GCost + 1;

                    if (!nodeMap.TryGetValue(neighbour, out var neighbourNode))
                    {
                        neighbourNode = new PathNode(neighbour, currentNode, tentativeG,
                            GetLocalDistanceBetweenTiles(neighbour, endTile));
                        pathNodeOpenList.Add(neighbourNode);
                        nodeMap[neighbour] = neighbourNode;
                    }
                    else if (tentativeG < neighbourNode.GCost)
                    {
                        neighbourNode.GCost = tentativeG;
                        neighbourNode.Parent = currentNode;
                    }
                }
            }
            
            List<Vector3> path = new List<Vector3>();
            if (nodeMap.TryGetValue(endTile, out var endNode))
            {
                var node = endNode;
                while (node != null)
                {
                    path.Add(node.LevelTile.WorldPosition + new Vector3(0.5f, 0.5f, 0));
                    node = node.Parent;
                }
                path.Reverse();
            }

            return path;
        }

        
        private static int GetLocalDistanceBetweenTiles(LevelTile startTile, LevelTile endTile)
        {
            int distanceX = Math.Abs(startTile.LocalPosition.x - endTile.LocalPosition.x);
            int distanceY = Math.Abs(startTile.LocalPosition.y - endTile.LocalPosition.y);
            return distanceX + distanceY;
        }
        
        private IEnumerator MoveAiOccupantAlongPath(IAIOccupant aiOccupant, IEnumerable<Vector3> pathPoints)
        {
            var pathPointArray = pathPoints as Vector3[] ?? pathPoints.ToArray();
            var destination = pathPointArray.First();
            var destinationsReached = 0;
            
            while (destinationsReached < pathPointArray.Length)
            {
                aiOccupant.Transform.position = Vector3.MoveTowards(
                    aiOccupant.Transform.position,
                    destination,
                    2f * Time.deltaTime);

                if (Vector3.Distance(aiOccupant.Transform.position, destination) < 0.01f)
                {
                    aiOccupant.Transform.position = destination;
                    destinationsReached++;
                    if (destinationsReached < pathPointArray.Length)
                    {
                        destination = pathPointArray[destinationsReached];
                    }
                }

                yield return null;
            }
            
            TryExecuteTileInteractor(aiOccupant.TileAssigned,  aiOccupant);
            aiOccupant.IsAIActionFinished = true;
        }

        private void TryExecuteTileInteractor(LevelTile tile, IOccupant occupant)
        {
            var interactor = tile.TileInteractor;
            interactor?.Interact(occupant);
        }
        
        private void OnDisable()
        {
            mediator.OnOccupantSelected -= OccupantSelected;
            mediator.OnPlayerOccupantMove -= MovePlayerOccupantToTile;
            mediator.OnPerformAIAction -= MoveAIOccupantToTile;
        }
    }
}
