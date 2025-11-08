using _Scripts.Events;

namespace _Scripts.Events
{
    public struct OnTurnsChanged : IEvent
    {
        public int TurnsLeft { get; set; }
    }
}
