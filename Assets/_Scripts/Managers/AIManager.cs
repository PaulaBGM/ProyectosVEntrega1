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
            EventBus<OnWorldTilesSet>.Subscribe(GetAIOccupants);
            EventBus<OnLevelStateChanged>.Subscribe(HandleLevelStateChanged);
        }

        private void GetAIOccupants(OnWorldTilesSet evt)
        {
            var tiles = evt.Tiles;
            
            _aiOccupants = tiles.Where(tile => tile.Occupant is IAIOccupant)
                .Select(tile => tile.Occupant as IAIOccupant)
                .ToArray();
        }

        private void HandleLevelStateChanged(OnLevelStateChanged evt)
        {
            var newState = evt.NewState;
            
            if (newState == LevelManager.LevelState.AITurn)
            {
                StartCoroutine(PerformAIActions());
            }
        }
        private IEnumerator PerformAIActions()
        {
            foreach (var aiOccupant in _aiOccupants)
            {
                if (aiOccupant.IsCaught)
                    continue;
                
                aiOccupant.IsAIActionFinished = false;
                MusicManager.Instance.PlayCat();
                
                EventBus<OnPerformAIAction>.Publish(new OnPerformAIAction
                {
                    AiOccupant = aiOccupant
                });
                
                yield return new WaitUntil(() => aiOccupant.IsAIActionFinished);
            }

            EventBus<OnAITurnCompleted>.Publish(new OnAITurnCompleted());
        }

        private void OnDisable()
        {
            EventBus<OnWorldTilesSet>.Unsubscribe(GetAIOccupants);
            EventBus<OnLevelStateChanged>.Unsubscribe(HandleLevelStateChanged);
        }
    }
}
