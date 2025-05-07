using ParallelWorldEngine;

namespace SpaceInvaders;

public class BulletComponent : Component
{
    public ReactiveProperty<float> Speed { get; private set; }
    public ReactiveProperty<bool> IsPlayerBullet { get; private set; }

    public BulletComponent()
    {
        Speed = new ReactiveProperty<float>(200f, this, nameof(Speed), null);
        IsPlayerBullet = new ReactiveProperty<bool>(true, this, nameof(IsPlayerBullet), null);
    }

    public override void Initialize()
    {
        base.Initialize();
        Speed = new ReactiveProperty<float>(Speed.Value, this, nameof(Speed), CommandSystem);
        IsPlayerBullet = new ReactiveProperty<bool>(IsPlayerBullet.Value, this, nameof(IsPlayerBullet), CommandSystem);
    }

    public override void Update(float deltaTime)
    {
        var transform = Owner.GetComponent<TransformComponent>();
        if (transform != null)
        {
            var direction = IsPlayerBullet.Value ? -1f : 1f;
            var position = transform.Position.Value;
            position.Y += direction * Speed.Value * deltaTime;
            transform.Position.Value = position;
        }
    }
}