using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Events
{
    public static class EventBus<T> where T : IEvent
    {
        private static readonly HashSet<Action<T>> ListenersSet = new();

        public static void Subscribe(Action<T> listener)
        {
            if (listener == null) throw new ArgumentNullException(nameof(listener));
            ListenersSet.Add(listener);
        }
        public static void Unsubscribe(Action<T> listener)
        {
            if (listener == null) throw new ArgumentNullException(nameof(listener));
            ListenersSet.Remove(listener);
        }

        public static void Publish(T eventData)
        {
            var setSnapshot = new Action<T>[ListenersSet.Count];
            ListenersSet.CopyTo(setSnapshot);

            foreach (var listener in setSnapshot)
            {
                try
                {
                    listener?.Invoke(eventData);;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error al invocar listener con args de {typeof(T).Name}: {ex}");
                }
            }
        }

        public static void Clear()
        {
            ListenersSet.Clear();
        }
    }
}
