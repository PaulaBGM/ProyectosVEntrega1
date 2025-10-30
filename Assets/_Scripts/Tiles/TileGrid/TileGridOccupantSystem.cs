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

    public void SetOccupantsInTiles(Dictionary<TileKey, LevelTile> tiles)
    {
        var tilemap = tiles.First().Value.TilemapMember;

        foreach (var occupant in _occupants)
        {
            if (occupant.TryGetComponent<IOccupant>(out var occup))
            {
                var cell = tilemap.WorldToCell(occupant.transform.position);

                // Asigna el tile del layer más alto que exista en esa celda
                var candidate = tiles
                    .Where(kv => kv.Key.Cell == cell)
                    .OrderByDescending(kv => kv.Key.Layer)
                    .Select(kv => kv.Value)
                    .FirstOrDefault();

                if (candidate != null)
                {
                    occup.AssignTile(candidate);
                }

                Debug.Log(tilemap.WorldToCell(occupant.transform.position));
            }
            else
            {
                Debug.LogError("Gameobject: " + occupant.name + " doesn't implement the " +
                    "IOccupant interface or misses the script that does");
            }
        }
    }

    private void OnDisable()
    {
        mediator.OnWorldTilesSet -= SetOccupantsInTiles;
    }
}
