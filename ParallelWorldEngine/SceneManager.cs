namespace ParallelWorldEngine;

public class SceneManager
{
    private IScene? _currentScene;
    private readonly World _world;
    private readonly CommandSystem _commandSystem;
    private readonly EventSystem _eventSystem;

    public SceneManager(World world, CommandSystem commandSystem, EventSystem eventSystem)
    {
        _world = world;
        _commandSystem = commandSystem;
        _eventSystem = eventSystem;
        _currentScene = null;
    }

    public async Task ChangeSceneAsync<T>() where T : IScene, new()
    {
        // 既存シーンのクリーンアップ
        await _commandSystem.ExecuteQueuedCommandsAsync();
            
        // 世界をクリア
        foreach (var entity in _world.Entities.ToList())
        {
            _world.RemoveEntity(entity);
        }

        // 新しいシーンを初期化
        _currentScene = new T();
        _currentScene.Initialize(_world, _commandSystem, _eventSystem);
    }

    public IScene? GetCurrentScene()
    {
        return _currentScene;
    }
}