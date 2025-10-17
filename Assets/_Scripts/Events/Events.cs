using _Scripts.Managers;

namespace _Scripts.Events
{
    public interface IEvent { }

    #region Level Events

    public class OnTileClicked : IEvent
    {
        public UnityEngine.Vector3 Point { get; set; }
    }

    public struct OnPlayerAction : IEvent { }

    public struct OnLevelStateChanged : IEvent
    {
        public LevelManager.LevelState NewState { get; set; }
    }
    
    #endregion
}