using UnityEngine;
using UnityEngine.Tilemaps;

public static class LevelTileMetadata
{
    // Sustituye esta heurística por tu sistema real (ScriptableObject, diccionario, RuleTile, etc.)
    public static bool IsLadder(TileBase tileBase)
    {
        if (tileBase == null) return false;
        var name = tileBase.name.ToLowerInvariant();
        return name.Contains("ladder") || name.Contains("escalera");
    }

    // Ajusta si hay tiles no pisables (muros, huecos, etc.)
    public static bool IsWalkable(TileBase tileBase) => tileBase != null;
}
