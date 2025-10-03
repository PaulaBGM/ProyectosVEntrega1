using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestTile : MonoBehaviour
{
    private LevelTile _levelTile;

    private Dictionary<Vector3, LevelTile> _tiles;

    private void Awake()
    {
        EventBus<OnGameTilesAssigned>.Subscribe(AssignTiles);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var worldPoint = new Vector3Int(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y), 0);

            if (_tiles.TryGetValue(worldPoint, out _levelTile))
            {
                Debug.Log("Tile is Ocuppied: " + _levelTile.IsOccupied);
                _levelTile.TilemapMember.SetTileFlags(_levelTile.LocalPosition, TileFlags.None);
                _levelTile.TilemapMember.SetColor(_levelTile.LocalPosition, Color.green);
            }
        }
    }

    private void AssignTiles(OnGameTilesAssigned @event)
    {
        _tiles = @event.GameTiles.Tiles;
    }
}