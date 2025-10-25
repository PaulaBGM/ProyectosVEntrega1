using System;
using _Scripts.Tiles;
using UnityEngine;

namespace _Scripts.Occupants
{
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

            TileAssigned = levelTile;

            if (TileAssigned is not null)
                TileAssigned.Occupant = this;
        }
    }
}
