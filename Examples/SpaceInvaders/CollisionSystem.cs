namespace SpaceInvaders;

using ParallelWorldEngine;

// 衝突システム
public class CollisionSystem
{
    private readonly World _world;
    private readonly EventSystem _eventSystem;

    public CollisionSystem(World world, EventSystem eventSystem)
    {
        _world = world;
        _eventSystem = eventSystem;
    }

    public void Update()
    {
        var entities = _world.Entities;
            
        for (int i = 0; i < entities.Count; i++)
        {
            var entityA = entities[i];
            var colliderA = entityA.GetComponent<ColliderComponent>();
                
            if (colliderA == null) continue;
                
            for (int j = i + 1; j < entities.Count; j++)
            {
                var entityB = entities[j];
                var colliderB = entityB.GetComponent<ColliderComponent>();
                    
                if (colliderB == null) continue;
                    
                if (colliderA.Intersects(colliderB))
                {
                    _eventSystem.Publish(new CollisionEvent(entityA, entityB));
                }
            }
        }
    }
}