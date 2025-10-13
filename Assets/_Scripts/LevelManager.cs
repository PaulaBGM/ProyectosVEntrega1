using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private TileGridDataSystem _gameTiles;

    [SerializeField]
    private PlayerInputHandler _playerInputHandler;

    private void Start()
    {
        _gameTiles.SetWorldTiles();
        _playerInputHandler._tiles = _gameTiles.Tiles;
    }
}
