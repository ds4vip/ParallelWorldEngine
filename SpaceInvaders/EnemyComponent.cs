using ParallelWorldEngine;

namespace SpaceInvaders;

public class EnemyComponent : Component
{
    public ReactiveProperty<float> Speed { get; private set; }
    public ReactiveProperty<int> Points { get; private set; }

    public EnemyComponent()
    {
        Speed = new ReactiveProperty<float>(30f, this, nameof(Speed), null);
        Points = new ReactiveProperty<int>(10, this, nameof(Points), null);
    }

    public override void Initialize()
    {
        base.Initialize();
        Speed = new ReactiveProperty<float>(Speed.Value, this, nameof(Speed), CommandSystem);
        Points = new ReactiveProperty<int>(Points.Value, this, nameof(Points), CommandSystem);
    }
}