using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventBus<T> where T : IEvent
{
    private static readonly HashSet<Action<T>> listenersSet = new();

    public static void Subscribe(Action<T> listener)
    {
        if (listener == null) throw new ArgumentNullException(nameof(listener));
        listenersSet.Add(listener);
    }
    public static void Unsubscribe(Action<T> listener)
    {
        if (listener == null) throw new ArgumentNullException(nameof(listener));
        listenersSet.Remove(listener);
    }

    public static void Publish(T eventData)
    {
        var setSnapshot = new Action<T>[listenersSet.Count];
        listenersSet.CopyTo(setSnapshot);

        foreach (var listener in setSnapshot)
        {
            try
            {
                listener?.Invoke(eventData);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error al invocar listener con args de {typeof(T).Name}: {ex}");
            }
        }
    }

    public static void Clear()
    {
        listenersSet.Clear();
    }
}
