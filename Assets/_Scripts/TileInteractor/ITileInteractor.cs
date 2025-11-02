using System.Collections.Generic;
using _Scripts.Occupants;
using _Scripts.Tiles;
using UnityEngine;

namespace _Scripts.TileInteractor
{
    public interface ITileInteractor
    {
        void Interact(IOccupant occupant);
        void SetTileInteractor(Dictionary<Vector3, LevelTile> tiles);
    }
}
