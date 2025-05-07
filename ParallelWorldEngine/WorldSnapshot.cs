namespace ParallelWorldEngine;

/// <summary>
/// ワールドスナップショット
/// </summary>
public class WorldSnapshot
{
    private readonly Dictionary<Guid, EntitySnapshot> _entitySnapshots = new();

    public WorldSnapshot(World world)
    {
        foreach (var entity in world.Entities)
        {
            _entitySnapshots[entity.Id] = new EntitySnapshot(entity);
        }
    }

    public IReadOnlyDictionary<Guid, EntitySnapshot> EntitySnapshots => _entitySnapshots;
}