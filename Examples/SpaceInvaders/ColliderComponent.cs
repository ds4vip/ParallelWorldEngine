using ParallelWorldEngine;

namespace SpaceInvaders;

public class ColliderData : IComponentData
{
    public float Width { get; set; }
    public float Height { get; set; }
    public string Tag { get; set; }
}

public class ColliderComponent : Component, ISnapshotProvider
{
    public ReactiveProperty<float> Width { get; private set; }
    public ReactiveProperty<float> Height { get; private set; }
    public string Tag { get; set; }

    public ColliderComponent()
    {
        Width = new ReactiveProperty<float>(10f, this, nameof(Width), null);
        Height = new ReactiveProperty<float>(10f, this, nameof(Height), null);
    }

    public override void Initialize()
    {
        base.Initialize();
        Width = new ReactiveProperty<float>(Width.Value, this, nameof(Width), CommandSystem);
        Height = new ReactiveProperty<float>(Height.Value, this, nameof(Height), CommandSystem);
    }

    public bool Intersects(ColliderComponent other)
    {
        var thisTransform = Owner.GetComponent<TransformComponent>();
        var otherTransform = other.Owner.GetComponent<TransformComponent>();

        if (thisTransform == null || otherTransform == null) return false;

        var thisPos = thisTransform.Position.Value;
        var otherPos = otherTransform.Position.Value;

        var thisHalfWidth = Width.Value / 2;
        var thisHalfHeight = Height.Value / 2;
        var otherHalfWidth = other.Width.Value / 2;
        var otherHalfHeight = other.Height.Value / 2;

        return !(thisPos.X + thisHalfWidth < otherPos.X - otherHalfWidth ||
                 thisPos.X - thisHalfWidth > otherPos.X + otherHalfWidth ||
                 thisPos.Y + thisHalfHeight < otherPos.Y - otherHalfHeight ||
                 thisPos.Y - thisHalfHeight > otherPos.Y + otherHalfHeight);
    }

    public IComponentData CaptureSnapshot()
    {
        return new ColliderData
        {
            Width = Width.Value,
            Height = Height.Value,
            Tag = Tag
        };
    }
}