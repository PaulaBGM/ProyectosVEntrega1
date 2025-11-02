using System;
using UnityEngine;

[Serializable]
public readonly struct TileKey : IEquatable<TileKey>
{
    public readonly Vector3Int Cell; // posición de celda (localPosition)
    public readonly int Layer;       // altura/capa

    public TileKey(Vector3Int cell, int layer)
    {
        Cell = cell;
        Layer = layer;
    }

    public bool Equals(TileKey other) => Cell.Equals(other.Cell) && Layer == other.Layer;
    public override bool Equals(object obj) => obj is TileKey other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Cell, Layer);
    public override string ToString() => $"({Cell.x},{Cell.y})@{Layer}";
}
