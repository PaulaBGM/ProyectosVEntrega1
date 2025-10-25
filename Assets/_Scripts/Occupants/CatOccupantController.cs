namespace _Scripts.Occupants
{
    public class CatOccupantController : OccupantController, IAIOccupant
    {
        public bool IsAIActionFinished { get; set; } = true;
        public bool IsCaught { get; set; } = false;
        
        public void Catch()
        {
            IsCaught = true;
            AssignTile(null);
            gameObject.SetActive(false);
        }
    }
}
