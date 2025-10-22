using _Scripts.Events;
using _Scripts.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Input
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private PlayerInput _playerInput;
        
        private bool _isPlayerTurn = true;

        private void Awake()
        {
            _playerInput = new PlayerInput();
        }

        private void OnEnable()
        {
            EventBus<OnLevelStateChanged>.Subscribe(evt => HandleLevelStateChanged(evt.NewState));
            
            _playerInput.InLevel.Select.performed += OnSelect;
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

        public void OnSelect(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed || !_isPlayerTurn)
                return;

            if (Camera.main != null)
            {
                Vector3 point = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);

                EventBus<OnTileClicked>.Publish(new OnTileClicked
                {
                    Point = point
                });
            }
            else
            {
                Debug.LogError("Main Camera not found in the scene.");
            }
        }
        
        private void OnDisable()
        {
            EventBus<OnLevelStateChanged>.Unsubscribe(evt => HandleLevelStateChanged(evt.NewState));
            
            if (_playerInput != null)
            {
                _playerInput.InLevel.Select.performed -= OnSelect;
                _playerInput.Dispose();
            }
        }
    }
}
