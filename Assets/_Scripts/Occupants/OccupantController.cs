using UnityEngine;

public abstract class OccupantController : MonoBehaviour, IOccupant
{
    [SerializeField]
    protected int _maxMovementTiles;

    public int MaxMovementTiles => _maxMovementTiles;

    public LevelTile TileAssigned { get; private set; }

    public Transform Transform => gameObject.transform;

    public void AssignTile(LevelTile levelTile)
    {
        if (TileAssigned is not null)
            TileAssigned.Occupant = null;

        TileAssigned = null;
        TileAssigned = levelTile;
        TileAssigned.Occupant = this;
    }
}
