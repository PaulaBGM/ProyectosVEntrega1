public interface IEvent { }

#region Level
public class OnGameTilesAssigned : IEvent
{
    public GameTiles GameTiles { get; set; }
}
#endregion