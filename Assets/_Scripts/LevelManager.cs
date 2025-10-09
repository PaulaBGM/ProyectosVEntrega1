using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private GameTiles _gameTiles;

    [SerializeField]
    private GameObject[] _occupantsInLevel;

    [SerializeField]
    private TileInputHandler _tileInputHandler;

    private void Start()
    {
        _gameTiles.GetWorldTiles();
        _gameTiles.SetOccupantsInTiles(_occupantsInLevel);
        _tileInputHandler._tiles = _gameTiles.Tiles;
    }
}
