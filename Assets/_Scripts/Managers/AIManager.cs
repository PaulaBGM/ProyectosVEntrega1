using System.Collections;
using System.Linq;
using System.Collections.Generic;
using _Scripts.Events;
using _Scripts.Occupants;
using _Scripts.Tiles;
using UnityEngine;

namespace _Scripts.Managers
{
    public class AIManager : MonoBehaviour
    {
        private IAIOccupant[] _aiOccupants;
        
        private void OnEnable()
        {
            EventBus<OnWorldTilesSet>.Subscribe(evt => GetAIOccupants(evt.Tiles));
            EventBus<OnLevelStateChanged>.Subscribe(
                evt => HandleLevelStateChanged(evt.NewState));
        }

        private void GetAIOccupants(IEnumerable<LevelTile> tiles)
        {
            _aiOccupants = tiles.Where(tile => tile.Occupant is IAIOccupant)
                .Select(tile => tile.Occupant as IAIOccupant)
                .ToArray();
        }

        private void HandleLevelStateChanged(LevelManager.LevelState newState)
        {
            if (newState == LevelManager.LevelState.AITurn)
            {
                StartCoroutine(PerformAIActions());
            }
        }
        private IEnumerator PerformAIActions()
        {
            foreach (var aiOccupant in _aiOccupants)
            {
                EventBus<OnPerformAIAction>.Publish(new OnPerformAIAction
                {
                    AiOccupant = aiOccupant
                });
                
                aiOccupant.IsAIActionFinished = false;
                yield return new WaitUntil(() => aiOccupant.IsAIActionFinished);
            }

            EventBus<OnAITurnCompleted>.Publish(new OnAITurnCompleted());
        }

        private void OnDisable()
        {
            EventBus<OnWorldTilesSet>.Unsubscribe(evt => GetAIOccupants(evt.Tiles));
            EventBus<OnLevelStateChanged>.Unsubscribe(
                evt => HandleLevelStateChanged(evt.NewState));
        }
    }
}
