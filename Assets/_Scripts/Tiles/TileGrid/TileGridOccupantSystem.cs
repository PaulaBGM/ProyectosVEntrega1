using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileGridOccupantSystem : MediatorClientSystem<TileGridMediator>
{
    [SerializeField]
    private GameObject[] _occupants;

    private void OnEnable()
    {
        mediator.OnWorldTilesSet += SetOccupantsInTiles;
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

    private void OnDisable()
    {
        mediator.OnWorldTilesSet -= SetOccupantsInTiles;
    }
}
