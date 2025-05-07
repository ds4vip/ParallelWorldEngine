namespace ParallelWorldEngine;

public class GameEngine
{
    private readonly object _updateLock = new();
        
    private readonly CommandSystem _commandSystem = new();
    private readonly EventSystem _eventSystem = new();
    private readonly World _world;
    private readonly SceneManager _sceneManager;
        
    private WorldSnapshot _currentSnapshot;
    private float _accumulator = 0f;
    private const float FixedTimeStep = 1f / 30f;  // 30 Hz の固定更新レート

    public GameEngine()
    {
        _world = new World(_commandSystem);
        _sceneManager = new SceneManager(_world, _commandSystem, _eventSystem);
    }

    public SceneManager SceneManager => _sceneManager;
    public EventSystem EventSystem => _eventSystem;
    public WorldSnapshot CurrentSnapshot => _currentSnapshot;

    public async Task UpdateAsync(float deltaTime)
    {
        lock (_updateLock)
        {
            // コマンドの即時実行（前フレームの残りを処理）
            _commandSystem.ExecuteQueuedCommandsAsync().Wait();

            // 固定時間ステップでの更新
            _accumulator += deltaTime;
            while (_accumulator >= FixedTimeStep)
            {
                // シーン更新（ゲームロジック）
                var scene = _sceneManager.GetCurrentScene();
                scene?.Update(FixedTimeStep);

                // ワールド状態更新
                _world.Update(FixedTimeStep);

                // イベント処理
                _eventSystem.ProcessEvents();

                _accumulator -= FixedTimeStep;
            }

            // コマンドの実行（現在フレームの変更を反映）
            _commandSystem.ExecuteQueuedCommandsAsync().Wait();

            // スナップショット更新（レンダリング用）
            _currentSnapshot = _world.CreateSnapshot();
        }
    }
}