using System.Collections.Generic;
using _Scripts.Managers;
using _Scripts.Occupants;
using _Scripts.Tiles;

namespace _Scripts.Events
{
    public interface IEvent { }

    #region Level Events

    public struct OnWorldTilesSet : IEvent
    {
        public IEnumerable<LevelTile> Tiles { get; set; }
    }
    
    public class OnTileClicked : IEvent
    {
        public UnityEngine.Vector3 Point { get; set; }
    }
    
    public struct OnPlayerAction : IEvent { }
    
    public class OnPerformAIAction : IEvent
    {
        public IAIOccupant AiOccupant { get; set; }
    }
    
    public struct OnLevelStateChanged : IEvent
    {
        public LevelManager.LevelState NewState { get; set; }
    }
    
    public struct OnAITurnCompleted : IEvent { }
    
    #endregion
}