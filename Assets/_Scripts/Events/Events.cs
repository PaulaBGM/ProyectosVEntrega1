using System.Collections.Generic;
using System.Numerics;

public interface IEvent { }

public class OnTileClicked : IEvent
{
    public UnityEngine.Vector3 Point { get; set; }
}