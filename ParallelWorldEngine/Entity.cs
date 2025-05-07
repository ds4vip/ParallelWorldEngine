namespace ParallelWorldEngine;

public class Entity
{
    public World World { get; }
    public Guid Id { get; } = Guid.NewGuid();
    private readonly List<Component> _components = new();

    public IReadOnlyList<Component> Components => _components;

    public Entity(World world)
    {
        World = world;
    }

    public T AddComponent<T>() where T : Component, new()
    {
        var component = new T { Owner = this };
        _components.Add(component);
        component.Initialize();
        return component;
    }

    public T? GetComponent<T>() where T : Component
    {
        return _components.OfType<T>().FirstOrDefault();
    }

    public void Update(float deltaTime)
    {
        foreach (var component in _components)
        {
            component.Update(deltaTime);
        }
    }
}