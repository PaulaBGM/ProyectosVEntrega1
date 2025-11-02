using System;
using _Scripts.Events;
using _Scripts.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Input
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public static PlayerInputHandler Instance {get; private set;}

        public Action<Vector3> OnSelect;
        
        private PlayerInput _playerInput;
        
        private bool _isPlayerTurn = true;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                _playerInput = GetComponent<PlayerInput>();
                
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            EventBus<OnLevelStateChanged>.Subscribe(evt => HandleLevelStateChanged(evt.NewState));
            
            _playerInput.actions["Select"].started += Select;
        }

        private void HandleLevelStateChanged(LevelManager.LevelState state)
        {
            switch (state)
            {
                case LevelManager.LevelState.PlayerTurn:
                    _isPlayerTurn = true;
                    break;
                case LevelManager.LevelState.AITurn:
                    _isPlayerTurn = false;
                    break;
            }
        }

        public void Select(InputAction.CallbackContext context)
        {
            if (!_isPlayerTurn)
                return;

            if (Camera.main != null)
            {
                Vector3 point = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);

                OnSelect?.Invoke(point);
            }
            else
            {
                Debug.LogError("Main Camera not found in the scene.");
            }
        }
        
        private void OnDisable()
        {
            EventBus<OnLevelStateChanged>.Unsubscribe(evt => HandleLevelStateChanged(evt.NewState));
            
            _playerInput.actions["Select"].started -= Select;
        }
    }
}
