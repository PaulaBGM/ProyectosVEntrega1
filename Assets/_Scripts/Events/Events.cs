public interface IEvent { }

public class OnPlayerOccupantSelected : IEvent 
{
    public LevelTile tileSelected { get; set; }
    public int MaxMovementTiles { get; set; }
}

public class OnPlayerOccupantMove : IEvent
{
    public LevelTile tileToMove { get; set; }
    public IOccupant Occupant { get; set; }
}