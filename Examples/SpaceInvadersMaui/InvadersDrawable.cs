namespace SpaceInvadersMaui;

using Microsoft.Maui.Graphics;
using ParallelWorldEngine;
using SpaceInvaders;
using System;
using System.Numerics;

public class InvadersDrawable : IDrawable
{
    private readonly GameEngine _gameEngine;

    public InvadersDrawable(GameEngine gameEngine)
    {
        _gameEngine = gameEngine;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.StrokeColor = Colors.Green;
        canvas.StrokeSize = 1;

        var snapshot = _gameEngine.CurrentSnapshot;
        if (snapshot == null) return;

        foreach (var (_, entitySnapshot) in snapshot.EntitySnapshots)
        {
            if (entitySnapshot.TryGetComponentData<TransformData>(out var transformData) &&
                entitySnapshot.TryGetComponentData<WireframeRendererData>(out var rendererData))
            {
                // エンティティの座標や回転を適用
                var position = transformData.Position;
                var rotation = transformData.Rotation;
                var scale = transformData.Scale;

                canvas.SaveState();
                canvas.Translate((float)position.X, (float)position.Y);
                canvas.Rotate((float)(rotation * 180 / Math.PI));
                canvas.Scale((float)scale.X, (float)scale.Y);

                // ワイヤーフレームの描画
                var points = rendererData.Points;
                if (points.Length >= 4)
                {
                    var pathF = new PathF();
                    pathF.MoveTo(points[0], points[1]);

                    for (int i = 2; i < points.Length; i += 2)
                    {
                        pathF.LineTo(points[i], points[i + 1]);
                    }

                    if (rendererData.IsClosed)
                    {
                        pathF.Close();
                    }

                    canvas.StrokeSize = rendererData.LineThickness;
                    canvas.DrawPath(pathF);
                }

                canvas.RestoreState();
            }
        }

        // スコアやゲームオーバーメッセージの表示
        var currentScene = _gameEngine.SceneManager.GetCurrentScene() as SpaceInvadersScene;
        var score = currentScene?.GetScore() ?? 0;
        var isGameOver = currentScene?.IsGameOver() ?? false;
        var playerWon = currentScene?.HasPlayerWon() ?? false;

        canvas.FontColor = Colors.White;
        canvas.FontSize = 20;
        canvas.DrawString($"Score: {score}", 10, 30, HorizontalAlignment.Left);

        if (isGameOver)
        {
            canvas.FontSize = 40;
            canvas.DrawString(
                playerWon ? "You Win!" : "Game Over!",
                dirtyRect.Width / 2,
                dirtyRect.Height / 2,
                HorizontalAlignment.Center);
        }
    }
}