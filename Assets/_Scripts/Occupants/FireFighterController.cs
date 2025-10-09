using UnityEngine;
using UnityEngine.Tilemaps;

public class FireFighterController : MonoBehaviour, IOccupant
{
    [SerializeField]
    private int _maxMovementTiles;

    public LevelTile TileAssigned { get; set; }

    public void AssignTile(LevelTile levelTile)
    {
        if (TileAssigned is not null)
            TileAssigned.Occupant = null;

        TileAssigned = null;
        TileAssigned = levelTile;
        TileAssigned.Occupant = this;
    }

    public void MoveTo(LevelTile newLevelTile)
    {
        if (!CanMoveToTile(newLevelTile)) return;

        AssignTile(newLevelTile);

        Debug.Log("Se mueve");
    }

    private void Update()
    {
        Vector3 targetPos = TileAssigned.TilemapMember.CellToWorld(TileAssigned.LocalPosition)
                     + (Vector3)TileAssigned.TilemapMember.cellSize * 0.5f;

        if (transform.position != targetPos)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                5f * Time.deltaTime
            );
        }
    }


    private bool CanMoveToTile(LevelTile newLevelTile)
    {
        int dx = Mathf.Abs(newLevelTile.LocalPosition.x - TileAssigned.LocalPosition.x);
        int dy = Mathf.Abs(newLevelTile.LocalPosition.y - TileAssigned.LocalPosition.y);
        return (dx + dy) <= _maxMovementTiles;
    }
}
