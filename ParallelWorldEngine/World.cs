namespace ParallelWorldEngine;

public class World
{
    private readonly List<Entity> _entities = new();
    public CommandSystem CommandSystem { get; }

    public IReadOnlyList<Entity> Entities => _entities;

    public World(CommandSystem commandSystem)
    {
        CommandSystem = commandSystem;
    }

    public Entity CreateEntity()
    {
        var entity = new Entity(this);
        _entities.Add(entity);
        return entity;
    }

    public void RemoveEntity(Entity entity)
    {
        _entities.Remove(entity);
    }

    public void Update(float deltaTime)
    {
        // エンティティのリストをコピーして更新中に変更が起きても問題ないようにする
        foreach (var entity in _entities.ToList())
        {
            entity.Update(deltaTime);
        }
    }

    public WorldSnapshot CreateSnapshot()
    {
        return new WorldSnapshot(this);
    }
}