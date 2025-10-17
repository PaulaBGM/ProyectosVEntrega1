using System;
using _Scripts.Events;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Managers
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField]
        private int playerActionsPerTurn;

        [SerializeField]
        private int maxTurns;
        
        private int _actionsLeft;

        public enum LevelState
        {
            PlayerTurn,
            AITurn
        }

        private LevelState _currentState = LevelState.PlayerTurn;
        public LevelState CurrentState => _currentState;

        private void OnEnable()
        {
            EventBus<OnPlayerAction>.Subscribe(HandlePlayerAction);
        }

        private void Start()
        {
            _actionsLeft = playerActionsPerTurn;
        }

        private void HandlePlayerAction(OnPlayerAction _)
        {
            _actionsLeft--;

            if (_actionsLeft <= 0)
            {
                ChangeState(LevelState.AITurn);
            }
        }

        public void ChangeState(LevelState newState)
        {
            if (_currentState == newState)
                return;

            _currentState = newState;

            EventBus<OnLevelStateChanged>.Publish(new OnLevelStateChanged
            {
                NewState = _currentState
            });

            HandleStateEnter(_currentState);
        }

        private void HandleStateEnter(LevelState state)
        {
            switch (state)
            {
                case LevelState.PlayerTurn:
                    _actionsLeft = playerActionsPerTurn;
                    break;
                case LevelState.AITurn:
                    // Handle AI turn logic here
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
        
        private void OnDisable()
        {
            EventBus<OnPlayerAction>.Unsubscribe(HandlePlayerAction);
        }
    }
}
