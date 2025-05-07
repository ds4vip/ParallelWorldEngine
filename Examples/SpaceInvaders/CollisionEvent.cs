namespace SpaceInvaders;

using ParallelWorldEngine;

// 衝突イベント
public struct CollisionEvent
{
    public Entity EntityA { get; }
    public Entity EntityB { get; }

    public CollisionEvent(Entity entityA, Entity entityB)
    {
        EntityA = entityA;
        EntityB = entityB;
    }
}

// スコアイベント
public struct ScoreChangeEvent
{
    public int Points { get; }

    public ScoreChangeEvent(int points)
    {
        Points = points;
    }
}

// ゲームオーバーイベント
public struct GameOverEvent
{
    public bool PlayerWon { get; }

    public GameOverEvent(bool playerWon)
    {
        PlayerWon = playerWon;
    }
}