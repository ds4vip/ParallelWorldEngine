namespace ParallelWorldEngine;

/// <summary>
/// エンティティスナップショット
/// </summary>
public class EntitySnapshot
{
    private readonly Dictionary<Type, IComponentData> _componentData = new();
    public Guid EntityId { get; }

    public EntitySnapshot(Entity entity)
    {
        EntityId = entity.Id;
        foreach (var component in entity.Components)
        {
            if (component is ISnapshotProvider provider)
            {
                var data = provider.CaptureSnapshot();
                if (data != null)
                {
                    _componentData[data.GetType()] = data;
                }
            }
        }
    }

    public bool TryGetComponentData<T>(out T data) where T : class, IComponentData
    {
        if (_componentData.TryGetValue(typeof(T), out var componentData) && componentData is T typedData)
        {
            data = typedData;
            return true;
        }

        data = default;
        return false;
    }
}
