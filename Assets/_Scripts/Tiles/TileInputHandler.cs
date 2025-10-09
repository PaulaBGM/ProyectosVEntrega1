using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileInputHandler : MonoBehaviour
{
    public Dictionary<Vector3, LevelTile> _tiles;


    private IOccupant _occupantSelected = null;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector3Int cellPos = _tiles.First().Value.TilemapMember.WorldToCell(point);
            Vector3 worldPoint = _tiles.First().Value.TilemapMember.CellToWorld(cellPos);

            if (_tiles.TryGetValue(worldPoint, out var _levelTile))
            {
                Debug.Log("Tile is Ocuppied: " + _levelTile.Occupant == null +
                    "\nWorld pos: " + _levelTile.WorldPosition +
                    "\nLocal pos: " + _levelTile.LocalPosition);

                StartCoroutine(SelectionTest(_levelTile));

                if (_occupantSelected is not null)
                {
                    _occupantSelected.MoveTo(_levelTile);
                    _occupantSelected = null;
                }
                else
                {
                    _occupantSelected = _levelTile.Occupant;
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