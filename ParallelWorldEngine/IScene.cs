namespace ParallelWorldEngine;

public interface IScene
{
    void Initialize(World world, CommandSystem commandSystem, EventSystem eventSystem);
    void Update(float deltaTime);
}