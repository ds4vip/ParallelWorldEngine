namespace SpaceInvaders;

using ParallelWorldEngine;
using System;
using System.Numerics;
using System.Threading.Tasks;

// スペースインベーダーのメインシーン
public class SpaceInvadersScene : IScene
{
    private World _world;
    private CommandSystem _commandSystem;
    private EventSystem _eventSystem;
    private CollisionSystem _collisionSystem;

    private Entity _player;
    private readonly List<Entity> _enemies = new();
    private readonly List<Entity> _bullets = new();
    private readonly List<Entity> _entitiesToRemove = new();

    // ゲーム設定
    private const int EnemyRows = 5;
    private const int EnemyColumns = 11;
    private float _enemyMoveDirection = 1f;
    private float _enemyMoveTimer = 0f;
    private float _enemyMoveInterval = 0.5f;
    private float _enemyShotTimer = 0f;
    private float _enemyShotInterval = 1f;
    private int _score = 0;
    private bool _gameOver = false;
    private bool _playerWon = false;

    // ウィンドウのサイズ
    private float _windowWidth = 800f;
    private float _windowHeight = 600f;

    public void Initialize(World world, CommandSystem commandSystem, EventSystem eventSystem)
    {
        _world = world;
        _commandSystem = commandSystem;
        _eventSystem = eventSystem;
        _collisionSystem = new CollisionSystem(world, eventSystem);

        // イベントのサブスクライブ
        _eventSystem.Subscribe<CollisionEvent>(HandleCollision);
        _eventSystem.Subscribe<ScoreChangeEvent>(HandleScoreChange);
        _eventSystem.Subscribe<GameOverEvent>(HandleGameOver);

        // プレイヤーの作成
        CreatePlayer();

        // 敵の作成
        CreateEnemies();
    }

    private void CreatePlayer()
    {
        _player = _world.CreateEntity();

        var transform = _player.AddComponent<TransformComponent>();
        transform.Position.Value = new Vector2(_windowWidth / 2, _windowHeight - 50);

        var collider = _player.AddComponent<ColliderComponent>();
        collider.Width.Value = 30f;
        collider.Height.Value = 15f;
        collider.Tag = "Player";

        var player = _player.AddComponent<PlayerComponent>();

        var renderer = _player.AddComponent<WireframeRendererComponent>();
        renderer.Points.Value = new float[]
        {
            -15, 7.5f, // 左上
            0, -7.5f, // 中央下
            15, 7.5f // 右上
        };
        renderer.IsClosed.Value = true;
    }

    private void CreateEnemies()
    {
        const float startX = 100;
        const float startY = 50;
        const float spacingX = 50;
        const float spacingY = 40;

        for (int row = 0; row < EnemyRows; row++)
        {
            for (int col = 0; col < EnemyColumns; col++)
            {
                var enemy = _world.CreateEntity();

                var transform = enemy.AddComponent<TransformComponent>();
                transform.Position.Value = new Vector2(startX + col * spacingX, startY + row * spacingY);

                var collider = enemy.AddComponent<ColliderComponent>();
                collider.Width.Value = 30f;
                collider.Height.Value = 20f;
                collider.Tag = "Enemy";

                var enemyComp = enemy.AddComponent<EnemyComponent>();
                enemyComp.Points.Value = (EnemyRows - row) * 10; // 上の行ほど高得点

                var renderer = enemy.AddComponent<WireframeRendererComponent>();

                // 敵のタイプによって異なる形状
                if (row == 0)
                {
                    // 上段: UFO形状
                    renderer.Points.Value = new float[]
                    {
                        -15, 5, // 左
                        -10, -5, // 左下
                        10, -5, // 右下
                        15, 5 // 右
                    };
                }
                else if (row < 3)
                {
                    // 中段: カニのような形状
                    renderer.Points.Value = new float[]
                    {
                        -15, -5, // 左下
                        -15, 5, // 左上
                        -5, 10, // 左上突起
                        5, 10, // 右上突起
                        15, 5, // 右上
                        15, -5, // 右下
                        5, -10, // 右下突起
                        -5, -10 // 左下突起
                    };
                }
                else
                {
                    // 下段: タコのような形状
                    renderer.Points.Value = new float[]
                    {
                        -15, 0, // 左
                        -10, 10, // 左上
                        10, 10, // 右上
                        15, 0, // 右
                        10, -10, // 右下
                        -10, -10 // 左下
                    };
                }

                renderer.IsClosed.Value = true;

                _enemies.Add(enemy);
            }
        }
    }

    private void CreatePlayerBullet()
    {
        var playerTransform = _player.GetComponent<TransformComponent>();
        var playerComponent = _player.GetComponent<PlayerComponent>();

        if (playerTransform != null && playerComponent != null && playerComponent.CanShoot())
        {
            var bullet = _world.CreateEntity();

            var transform = bullet.AddComponent<TransformComponent>();
            transform.Position.Value =
                new Vector2(playerTransform.Position.Value.X, playerTransform.Position.Value.Y - 15);

            var collider = bullet.AddComponent<ColliderComponent>();
            collider.Width.Value = 3f;
            collider.Height.Value = 10f;
            collider.Tag = "PlayerBullet";

            var bulletComp = bullet.AddComponent<BulletComponent>();
            bulletComp.IsPlayerBullet.Value = true;

            var renderer = bullet.AddComponent<WireframeRendererComponent>();
            renderer.Points.Value = new float[]
            {
                0, -5, // 上
                0, 5 // 下
            };
            renderer.IsClosed.Value = false;
            renderer.LineThickness.Value = 2f;

            _bullets.Add(bullet);

            playerComponent.Shoot();
        }
    }

    private void CreateEnemyBullet(Entity enemy)
    {
        var enemyTransform = enemy.GetComponent<TransformComponent>();

        if (enemyTransform != null)
        {
            var bullet = _world.CreateEntity();

            var transform = bullet.AddComponent<TransformComponent>();
            transform.Position.Value =
                new Vector2(enemyTransform.Position.Value.X, enemyTransform.Position.Value.Y + 15);

            var collider = bullet.AddComponent<ColliderComponent>();
            collider.Width.Value = 3f;
            collider.Height.Value = 10f;
            collider.Tag = "EnemyBullet";

            var bulletComp = bullet.AddComponent<BulletComponent>();
            bulletComp.IsPlayerBullet.Value = false;

            var renderer = bullet.AddComponent<WireframeRendererComponent>();
            renderer.Points.Value = new float[]
            {
                0, -5, // 上
                -2, 0, // 中左
                2, 0, // 中右
                0, 5 // 下
            };
            renderer.IsClosed.Value = false;
            renderer.LineThickness.Value = 2f;

            _bullets.Add(bullet);
        }
    }

    public void Update(float deltaTime)
    {
        if (_gameOver) return;

        // プレイヤーの入力処理
        UpdatePlayerInput(deltaTime);

        // 敵の移動
        UpdateEnemyMovement(deltaTime);

        // 敵の射撃
        UpdateEnemyShooting(deltaTime);

        // 弾の範囲チェック
        UpdateBullets();

        // 衝突検出
        _collisionSystem.Update();

        // エンティティの削除
        foreach (var entity in _entitiesToRemove)
        {
            if (_bullets.Contains(entity))
            {
                _bullets.Remove(entity);
            }

            if (_enemies.Contains(entity))
            {
                _enemies.Remove(entity);
            }

            _world.RemoveEntity(entity);
        }

        _entitiesToRemove.Clear();

        // 勝利条件チェック
        if (_enemies.Count == 0)
        {
            _playerWon = true;
            _eventSystem.Publish(new GameOverEvent(true));
            _gameOver = true;
        }
    }

    private void UpdatePlayerInput(float deltaTime)
    {
        var transform = _player.GetComponent<TransformComponent>();
        var player = _player.GetComponent<PlayerComponent>();

        if (transform == null || player == null) return;

        // 左右移動
        var position = transform.Position.Value;

        if (InputManager.IsKeyDown(Key.Left) && position.X > 20)
        {
            position.X -= player.Speed.Value * deltaTime;
        }

        if (InputManager.IsKeyDown(Key.Right) && position.X < _windowWidth - 20)
        {
            position.X += player.Speed.Value * deltaTime;
        }

        transform.Position.Value = position;

        // 射撃
        if (InputManager.IsKeyPressed(Key.Space))
        {
            CreatePlayerBullet();
        }
    }

    private void UpdateEnemyMovement(float deltaTime)
    {
        _enemyMoveTimer += deltaTime;

        if (_enemyMoveTimer >= _enemyMoveInterval)
        {
            _enemyMoveTimer = 0;

            bool changeDirection = false;

            // 敵の位置を計算して、画面端に達したかチェック
            foreach (var enemy in _enemies)
            {
                var transform = enemy.GetComponent<TransformComponent>();
                if (transform == null) continue;

                var position = transform.Position.Value;

                if ((_enemyMoveDirection > 0 && position.X > _windowWidth - 30) ||
                    (_enemyMoveDirection < 0 && position.X < 30))
                {
                    changeDirection = true;
                    break;
                }
            }

            if (changeDirection)
            {
                _enemyMoveDirection *= -1;

                // 敵を下に移動
                foreach (var enemy in _enemies)
                {
                    var transform = enemy.GetComponent<TransformComponent>();
                    if (transform == null) continue;

                    var position = transform.Position.Value;
                    position.Y += 20;
                    transform.Position.Value = position;

                    // 敵が下に到達したらゲームオーバー
                    if (position.Y > _windowHeight - 100)
                    {
                        _eventSystem.Publish(new GameOverEvent(false));
                        _gameOver = true;
                    }
                }

                // 難易度調整: 敵の数が減るほど移動が速くなる
                _enemyMoveInterval = Math.Max(0.1f, _enemyMoveInterval * 0.9f);
            }
            else
            {
                // 敵を横に移動
                foreach (var enemy in _enemies)
                {
                    var transform = enemy.GetComponent<TransformComponent>();
                    if (transform == null) continue;

                    var position = transform.Position.Value;
                    position.X += _enemyMoveDirection * 10;
                    transform.Position.Value = position;
                }
            }
        }
    }

    private void UpdateEnemyShooting(float deltaTime)
    {
        if (_enemies.Count == 0) return;

        _enemyShotTimer += deltaTime;

        if (_enemyShotTimer >= _enemyShotInterval)
        {
            _enemyShotTimer = 0;

            // ランダムな敵が射撃
            if (_enemies.Count > 0)
            {
                var randomEnemy = _enemies[new Random().Next(_enemies.Count)];
                CreateEnemyBullet(randomEnemy);
            }

            // 難易度調整: 敵の数が減るほど射撃頻度が上がる
            _enemyShotInterval = Math.Max(0.5f, 1.0f * _enemies.Count / (EnemyRows * EnemyColumns));
        }
    }

    private void UpdateBullets()
    {
        foreach (var bullet in _bullets.ToList())
        {
            var transform = bullet.GetComponent<TransformComponent>();
            if (transform == null) continue;

            var position = transform.Position.Value;

            // 画面外にでたら削除
            if (position.Y < 0 || position.Y > _windowHeight)
            {
                _entitiesToRemove.Add(bullet);
            }
        }
    }

    private void HandleCollision(CollisionEvent evt)
    {
        var entityA = evt.EntityA;
        var entityB = evt.EntityB;

        var colliderA = entityA.GetComponent<ColliderComponent>();
        var colliderB = entityB.GetComponent<ColliderComponent>();

        if (colliderA == null || colliderB == null) return;

        // プレイヤーの弾と敵
        if ((colliderA.Tag == "PlayerBullet" && colliderB.Tag == "Enemy") ||
            (colliderA.Tag == "Enemy" && colliderB.Tag == "PlayerBullet"))
        {
            var enemy = colliderA.Tag == "Enemy" ? entityA : entityB;
            var bullet = colliderA.Tag == "PlayerBullet" ? entityA : entityB;

            var enemyComponent = enemy.GetComponent<EnemyComponent>();
            if (enemyComponent != null)
            {
                _eventSystem.Publish(new ScoreChangeEvent(enemyComponent.Points.Value));
            }

            _entitiesToRemove.Add(enemy);
            _entitiesToRemove.Add(bullet);
        }

        // 敵の弾とプレイヤー
        if ((colliderA.Tag == "EnemyBullet" && colliderB.Tag == "Player") ||
            (colliderA.Tag == "Player" && colliderB.Tag == "EnemyBullet"))
        {
            var bullet = colliderA.Tag == "EnemyBullet" ? entityA : entityB;

            _entitiesToRemove.Add(bullet);
            _eventSystem.Publish(new GameOverEvent(false));
            _gameOver = true;
        }
    }

    private void HandleScoreChange(ScoreChangeEvent evt)
    {
        _score += evt.Points;
    }

    private void HandleGameOver(GameOverEvent evt)
    {
        _gameOver = true;
        _playerWon = evt.PlayerWon;
    }

    public int GetScore() => _score;
    public bool IsGameOver() => _gameOver;
    public bool HasPlayerWon() => _playerWon;
}