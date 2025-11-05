using System.Collections.Generic;
using _Scripts.Tiles;
using UnityEngine;

namespace _Scripts.Occupants
{
    public interface IOccupant
    {
        LevelTile TileAssigned { get; }
        int MaxMovementTiles { get; }
        Transform Transform { get; }
        void AssignTile(LevelTile levelTile);
    }

    public interface IPlayerOccupant : IOccupant
    {
        Animator Animator { get; }
        void TriggerMove();
        void TriggerUnstick();
        
        public bool IsUnstick {get; }
    }

    public interface IAIOccupant : IOccupant
    {
        bool IsAIActionFinished { get; set; }
        bool IsCaught { get; set; }
        
        void Catch();
    }
}