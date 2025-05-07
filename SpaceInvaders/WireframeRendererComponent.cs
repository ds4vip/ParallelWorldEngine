namespace SpaceInvaders
{
    using ParallelWorldEngine;

    // ワイヤーフレームレンダリングコンポーネント
    public class WireframeRendererComponent : Component, ISnapshotProvider
    {
        public ReactiveProperty<float[]> Points { get; private set; }
        public ReactiveProperty<bool> IsClosed { get; private set; }
        public ReactiveProperty<float> LineThickness { get; private set; }

        public WireframeRendererComponent()
        {
            Points = new ReactiveProperty<float[]>(Array.Empty<float>(), this, nameof(Points), null);
            IsClosed = new ReactiveProperty<bool>(false, this, nameof(IsClosed), null);
            LineThickness = new ReactiveProperty<float>(1f, this, nameof(LineThickness), null);
        }

        public override void Initialize()
        {
            base.Initialize();
            Points = new ReactiveProperty<float[]>(Points.Value, this, nameof(Points), CommandSystem);
            IsClosed = new ReactiveProperty<bool>(IsClosed.Value, this, nameof(IsClosed), CommandSystem);
            LineThickness = new ReactiveProperty<float>(LineThickness.Value, this, nameof(LineThickness), CommandSystem);
        }

        public IComponentData CaptureSnapshot()
        {
            return new WireframeRendererData
            {
                Points = Points.Value,
                IsClosed = IsClosed.Value,
                LineThickness = LineThickness.Value
            };
        }
    }

    // ワイヤーフレームレンダリングデータ
    public class WireframeRendererData : IComponentData
    {
        public float[] Points { get; set; }
        public bool IsClosed { get; set; }
        public float LineThickness { get; set; }
    }
}