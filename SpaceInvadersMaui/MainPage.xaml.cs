namespace SpaceInvadersMaui;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using ParallelWorldEngine;
using SpaceInvaders;
using System;

public partial class MainPage : ContentPage
{
    private readonly GameEngine _gameEngine;
    private readonly GraphicsView _graphicsView;
    private DateTime _lastUpdateTime;

    public MainPage()
    {
        InitializeComponent();

        // ゲームエンジンの初期化
        _gameEngine = new GameEngine();
        _gameEngine.SceneManager.ChangeSceneAsync<SpaceInvadersScene>().Wait();

        // グラフィックスビューの設定
        _graphicsView = new GraphicsView
        {
            Drawable = new InvadersDrawable(_gameEngine),
            HeightRequest = 600,
            WidthRequest = 800,
            BackgroundColor = Colors.Black
        };

        // タッチイベントハンドラを設定
        _graphicsView.StartInteraction += OnStartInteraction;
        _graphicsView.EndInteraction += OnEndInteraction;

        // グラフィックスビューをページに追加
        Content = new VerticalStackLayout
        {
            Children = { _graphicsView },
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };

        // ゲームループの開始
        _lastUpdateTime = DateTime.Now;
        Device.StartTimer(TimeSpan.FromMilliseconds(16), OnTimerTick);
    }

    private bool OnTimerTick()
    {
        var now = DateTime.Now;
        var deltaTime = (float)(now - _lastUpdateTime).TotalSeconds;
        _lastUpdateTime = now;

        // ゲームエンジンの更新
        _gameEngine.UpdateAsync(deltaTime).Wait();

        // 入力状態の更新
        InputManager.Update();

        // 再描画
        _graphicsView.Invalidate();

        return true; // タイマーを継続
    }

    private void OnStartInteraction(object sender, TouchEventArgs e)
    {
        var touch = e.Touches.FirstOrDefault();
        if (touch != null)
        {
            // タッチ位置に基づいて左右の入力を模擬
            if (touch.X < _graphicsView.Width / 2)
            {
                InputManager.SetKeyState(Key.Left, true);
            }
            else
            {
                InputManager.SetKeyState(Key.Right, true);
            }
        }
    }

    private void OnEndInteraction(object sender, TouchEventArgs e)
    {
        // タッチ解除で入力をリセット
        InputManager.SetKeyState(Key.Left, false);
        InputManager.SetKeyState(Key.Right, false);
            
        // タップ時に射撃
        InputManager.SetKeyState(Key.Space, true);
            
        // 次のフレームで射撃キーをリセット（ワンショット）
        Device.StartTimer(TimeSpan.FromMilliseconds(100), () => 
        {
            InputManager.SetKeyState(Key.Space, false);
            return false; // ワンショットタイマー
        });
    }
}