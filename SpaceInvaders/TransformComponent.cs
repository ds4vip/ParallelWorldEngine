namespace SpaceInvaders;

using ParallelWorldEngine;
using System;
using System.Numerics;

public class TransformComponent : Component, ISnapshotProvider
{
    public ReactiveProperty<Vector2> Position { get; private set; }
    public ReactiveProperty<float> Rotation { get; private set; }
    public ReactiveProperty<Vector2> Scale { get; private set; }

    public TransformComponent()
    {
        Position = new ReactiveProperty<Vector2>(Vector2.Zero, this, nameof(Position), null);
        Rotation = new ReactiveProperty<float>(0f, this, nameof(Rotation), null);
        Scale = new ReactiveProperty<Vector2>(new Vector2(1f, 1f), this, nameof(Scale), null);
    }

    public override void Initialize()
    {
        base.Initialize();
        // CommandSystemが利用可能になった後に初期化
        Position = new ReactiveProperty<Vector2>(Position.Value, this, nameof(Position), CommandSystem);
        Rotation = new ReactiveProperty<float>(Rotation.Value, this, nameof(Rotation), CommandSystem);
        Scale = new ReactiveProperty<Vector2>(Scale.Value, this, nameof(Scale), CommandSystem);
    }

    public IComponentData CaptureSnapshot()
    {
        return new TransformData
        {
            Position = Position.Value,
            Rotation = Rotation.Value,
            Scale = Scale.Value
        };
    }
}

public class TransformData : IComponentData
{
    public Vector2 Position { get; set; }
    public float Rotation { get; set; }
    public Vector2 Scale { get; set; }
}