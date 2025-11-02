using System;
using System.Linq;
using System.Collections.Generic;
using _Scripts.Core.Mediator;
using _Scripts.TileInteractor;
using _Scripts.Tiles;
using _Scripts.Tiles.TileGrid;
using UnityEngine;

public class TileGridTileInteractorSystem : MediatorClientSystem<TileGridMediator>
{
    [SerializeField]
    private GameObject[] _interactors;
    
    private void OnEnable()
    {
        mediator.OnWorldTilesSet += WorldTilesSet;
    }

    private void WorldTilesSet(Dictionary<Vector3, LevelTile> tiles)
    {
        var tilemap = tiles.First().Value.TilemapMember;

        foreach (var tileInteractor in _interactors)
        {
            if (tileInteractor.TryGetComponent<ITileInteractor>(out var interactor))
            {
                if (tiles.TryGetValue(tilemap.WorldToCell(tileInteractor.transform.position), out var tile))
                {
                    tile.TileInteractor = interactor;
                    interactor.SetTileInteractor(tiles);
                }
            }
            else
            {
                Debug.LogError("Gameobject: " +  tileInteractor.name + " doesn't implement the " +
                               "IOccupant interface or misses the script that does");
            }
        }
    }

    private void OnDisable()
    {
        mediator.OnWorldTilesSet -= WorldTilesSet;
    }
}
