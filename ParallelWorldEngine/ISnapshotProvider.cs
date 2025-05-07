namespace ParallelWorldEngine;

/// <summary>
/// スナップショットプロバイダーインターフェース
/// </summary>
public interface ISnapshotProvider
{
    IComponentData CaptureSnapshot();
}
