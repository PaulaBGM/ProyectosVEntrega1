namespace _Scripts.Occupants
{
    public class CatOccupantController : OccupantController, IAIOccupant
    {
        public bool IsAIActionFinished { get; set; } = true;
    }
}
