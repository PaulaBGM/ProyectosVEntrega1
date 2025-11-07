using System;
using _Scripts.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Managers
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private int playerActionsPerTurn = 3;
        [SerializeField] private int maxTurns = 5;

        [Header("Level Flow")]
        [SerializeField] private string currentLevelId;
        [SerializeField] private string nextLevelId;
        [SerializeField] private string levelSelectSceneName = "LevelSelectScene";

        private int _actionsLeft;
        private int _turnsLeft;

        private bool _isGameWon = false;
        private bool _hasEnded = false;    

        public enum LevelState
        {
            PlayerTurn,
            AITurn,
            EndGame
        }

        private LevelState _currentState = LevelState.PlayerTurn;
        public LevelState CurrentState => _currentState;

        private void OnEnable()
        {
            EventBus<OnPlayerAction>.Subscribe(HandlePlayerAction);
            EventBus<OnAITurnCompleted>.Subscribe(HandleAITurnCompleted);
            EventBus<OnGameFinished>.Subscribe(HandleOnGameFinished);
        }

        private void Start()
        {
            _actionsLeft = playerActionsPerTurn;
            _turnsLeft = maxTurns;
        }

        private void HandlePlayerAction(OnPlayerAction _)
        {
            _actionsLeft--;

            if (_actionsLeft <= 0)
            {
                ChangeState(LevelState.AITurn);
            }
        }

        private void HandleAITurnCompleted(OnAITurnCompleted _)
        {
            ChangeState(LevelState.PlayerTurn);
        }

        private void HandleOnGameFinished(OnGameFinished eventData)
        {
            if (_hasEnded) return;  
            _isGameWon = eventData.IsWin;
            ChangeState(LevelState.EndGame);
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
                    _turnsLeft--;

                    if (_turnsLeft <= 0)
                    {
                        if (!_hasEnded)
                        {
                            EventBus<OnGameFinished>.Publish(new OnGameFinished { IsWin = false });
                            ChangeState(LevelState.EndGame);
                        }
                    }
                    break;

                case LevelState.EndGame:
                    _hasEnded = true;   

                    if (_isGameWon)
                    {
                        if (!string.IsNullOrEmpty(nextLevelId))
                        {
                            LevelProgress.Unlock(nextLevelId);
                            PlayerPrefs.SetString("PendingLevelUnlock", nextLevelId);
                        }

                        if (!string.IsNullOrEmpty(levelSelectSceneName))
                        {
                            SceneManager.LoadScene(levelSelectSceneName);
                        }
                    }
                    else
                    {
                        Debug.Log("EndGame - Derrota");
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void OnDisable()
        {
            EventBus<OnPlayerAction>.Unsubscribe(HandlePlayerAction);
            EventBus<OnAITurnCompleted>.Unsubscribe(HandleAITurnCompleted);
            EventBus<OnGameFinished>.Unsubscribe(HandleOnGameFinished);
        }
    }
}
