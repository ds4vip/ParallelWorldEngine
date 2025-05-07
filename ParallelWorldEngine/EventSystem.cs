namespace ParallelWorldEngine;

/// <summary>
/// イベントシステム
/// </summary>
public class EventSystem
{
    private readonly Dictionary<Type, List<Delegate>> _eventHandlers = new();
    private readonly List<(Type EventType, object EventData)> _pendingEvents = new();
    private readonly object _eventLock = new();

    public void Subscribe<T>(Action<T> handler) where T : struct
    {
        lock (_eventLock)
        {
            if (!_eventHandlers.TryGetValue(typeof(T), out var handlers))
            {
                handlers = new List<Delegate>();
                _eventHandlers[typeof(T)] = handlers;
            }

            handlers.Add(handler);
        }
    }

    public void Unsubscribe<T>(Action<T> handler) where T : struct
    {
        lock (_eventLock)
        {
            if (_eventHandlers.TryGetValue(typeof(T), out var handlers))
            {
                handlers.Remove(handler);
            }
        }
    }

    public void Publish<T>(T eventData) where T : struct
    {
        lock (_eventLock)
        {
            _pendingEvents.Add((typeof(T), eventData));
        }
    }

    public void ProcessEvents()
    {
        List<(Type EventType, object EventData)> events;
            
        lock (_eventLock)
        {
            events = new List<(Type, object)>(_pendingEvents);
            _pendingEvents.Clear();
        }

        foreach (var (eventType, eventData) in events)
        {
            if (_eventHandlers.TryGetValue(eventType, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    // 型安全なイベント発行
                    dynamic typedHandler = handler;
                    dynamic typedEvent = eventData;
                    typedHandler(typedEvent);
                }
            }
        }
    }
}