using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileGridOccupantSystem : MediatorClientSystem<TileGridMediator>
{
    [SerializeField]
    private GameObject[] _occupants;

    private IOccupant _occupantSelected;

    private void OnEnable()
    {
        mediator.OnWorldTilesSet += SetOccupantsInTiles;
        mediator.OnTileExecuteAction += ExecuteActionInTile;
    }

    public void SetOccupantsInTiles(Dictionary<Vector3, LevelTile> tiles)
    {
        var tilemap = tiles.First().Value.TilemapMember;

        foreach (var occupant in _occupants)
        {
            if (occupant.TryGetComponent<IOccupant>(out var occup))
            {
                if (tiles.TryGetValue(tilemap.WorldToCell(occupant.transform.position), out var tile))
                {
                    occup.AssignTile(tile);
                }

                Debug.Log(tilemap.WorldToCell(occupant.transform.position));
            }
            else
            {
                Debug.LogError("Gameobject: " +  occupant.name + " doesn't implement the " +
                    "IOccupant interface or misses the script that does");
            }
        }
    }

    public void ExecuteActionInTile(LevelTile tileSelected)
    {
        if (_occupantSelected is not null)
        {
            mediator.PlayerOccupantMove(tileSelected, _occupantSelected);

            _occupantSelected = null;
        }
        else
        {
            _occupantSelected = tileSelected.Occupant;

            if (_occupantSelected != null)
            {
                mediator.PlayerOccupantSelected(tileSelected, _occupantSelected.MaxMovementTiles);
            }
        }
    }

    private void OnDisable()
    {
        mediator.OnWorldTilesSet -= SetOccupantsInTiles;
        mediator.OnTileExecuteAction -= ExecuteActionInTile;
    }
}
