using UnityEngine;

/// <summary>
/// Interface that defines which scripts may be used as mediators
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IMediator { }

/// <summary>
/// Class designed for clients of a mediator class that reduces coupling between scripts
/// </summary>
/// <typeparam name="T"></typeparam>
public class MediatorClientSystem<T> : MonoBehaviour where T : IMediator
{
    protected T mediator;

    protected void Awake()
    {
        mediator = transform.root.GetComponent<T>();
    }
}
