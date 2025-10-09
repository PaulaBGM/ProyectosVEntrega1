using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelTile
{
    public Vector3Int LocalPosition { get; set; }

    public Vector3 WorldPosition { get; set; }

    public TileBase TileBase { get; set; }

    public Tilemap TilemapMember { get; set; }

    //Others

    public IOccupant Occupant { get; set; } = null;

    public int HeightLayer { get; set; }

    public float WidthSize => TilemapMember.cellSize.x;

    public float HeightSize => TilemapMember.cellSize.y;
}
