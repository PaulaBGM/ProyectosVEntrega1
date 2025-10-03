using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelTile
{
    public Vector3Int LocalPosition { get; set; }

    public Vector3 WorldPosition { get; set; }

    public TileBase TileBase { get; set; }

    public Tilemap TilemapMember { get; set; }

    //Others

    public int IsOccupied { get; set; }

    public int IsWalkable { get; set; }
}
