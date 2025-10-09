using UnityEngine;

public class PlayerInputSystem : MonoBehaviour
{
    [SerializeField]
    private PlayerInputActions _playerInputActions;

    private void Awake()
    {
        _playerInputActions.Enable();
    }

    private void Update()
    {
        
    }
}
