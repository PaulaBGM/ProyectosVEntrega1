using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerInputHandler : MonoBehaviour
{
    public Dictionary<Vector3, LevelTile> _tiles;

    private IOccupant _occupantSelected = null;

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed)
            return;

        Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3Int cellPos = _tiles.First().Value.TilemapMember.WorldToCell(point);
        Vector3 worldPoint = _tiles.First().Value.TilemapMember.CellToWorld(cellPos);

        if (_tiles.TryGetValue(worldPoint, out var _levelTile))
        {

            StartCoroutine(SelectionTest(_levelTile));

            if (_occupantSelected is not null)
            {
                EventBus<OnPlayerOccupantMove>.Publish(new OnPlayerOccupantMove
                {
                    tileToMove = _levelTile,
                    Occupant = _occupantSelected
                });

                _occupantSelected = null;
            }
            else
            {
                _occupantSelected = _levelTile.Occupant;

                if (_occupantSelected != null)
                {
                    EventBus<OnPlayerOccupantSelected>.Publish(new OnPlayerOccupantSelected
                    {
                        tileSelected = _levelTile,
                        MaxMovementTiles = _occupantSelected.MaxMovementTiles
                    });
                }
            }
        }
    }

    //ALL TEST, DELETE BELOW

    private IEnumerator SelectionTest(LevelTile tile)
    {
        tile.TilemapMember.SetTileFlags(tile.LocalPosition, TileFlags.LockTransform);
        tile.TilemapMember.SetColor(tile.LocalPosition, Color.green);
        yield return new WaitForSeconds(3);
        tile.TilemapMember.SetColor(tile.LocalPosition, Color.white);
    }
}
