namespace SpaceInvaders;

using ParallelWorldEngine;
using System.Numerics;

public class PlayerComponent : Component
{
    public ReactiveProperty<float> Speed { get; private set; }
    public ReactiveProperty<float> ShootCooldown { get; private set; }
    private float _shootTimer = 0f;

    public PlayerComponent()
    {
        Speed = new ReactiveProperty<float>(150f, this, nameof(Speed), null);
        ShootCooldown = new ReactiveProperty<float>(0.5f, this, nameof(ShootCooldown), null);
    }

    public override void Initialize()
    {
        base.Initialize();
        Speed = new ReactiveProperty<float>(Speed.Value, this, nameof(Speed), CommandSystem);
        ShootCooldown = new ReactiveProperty<float>(ShootCooldown.Value, this, nameof(ShootCooldown), CommandSystem);
    }

    public override void Update(float deltaTime)
    {
        // 射撃クールダウンの更新
        if (_shootTimer > 0)
        {
            _shootTimer -= deltaTime;
        }
    }

    public bool CanShoot()
    {
        return _shootTimer <= 0;
    }

    public void Shoot()
    {
        _shootTimer = ShootCooldown.Value;
    }
}

// 弾コンポーネント