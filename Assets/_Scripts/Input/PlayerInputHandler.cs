using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerInputHandler : MonoBehaviour
{
    public void OnSelect(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed)
            return;

        Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        EventBus<OnTileClicked>.Publish(new OnTileClicked
        {
            Point = point
        });
    }
}
