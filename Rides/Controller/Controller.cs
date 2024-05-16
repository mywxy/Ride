using System.Drawing;
using System.Windows.Forms;
using System;

public class GameController
{
    private GameModel _model;
    private GameView _view;
    private Timer _gameTimer;
    private int backgroundY;

    public GameController(GameModel model, GameView view)
    {
        _model = model;
        _view = view;

        _view.KeyDown += OnKeyDown;
        InitializeTimer();
        backgroundY = 0;
    }

    private void InitializeTimer()
    {
        _gameTimer = new Timer();
        _gameTimer.Interval = 50;
        _gameTimer.Tick += GameTick;
        _gameTimer.Start();
    }

    private void GameTick(object sender, EventArgs e)
    {
        MoveOpponents();
        MoveCoins();
        CheckCollisions();
        MoveBackground();
        _view.UpdateView(_model);
    }

    private void MoveOpponents()
    {
        bool resetNeeded = false;
        foreach (var car in _model.Opponents)
        {
            car.Move(0, 1);
            if (car.PositionY > 600)
            {
                resetNeeded = true;
            }
        }
        if (resetNeeded)
        {
            _model.ResetOpponents();
        }
    }

    private void MoveCoins()
    {
        _model.MoveCoins();
    }

    private void CheckCollisions()
    {
        Rectangle playerRect = new Rectangle(_model.PlayerCar.PositionX, _model.PlayerCar.PositionY, 50, 100);
        foreach (var car in _model.Opponents)
        {
            Rectangle carRect = new Rectangle(car.PositionX, car.PositionY, 50, 100);
            if (playerRect.IntersectsWith(carRect))
            {
                _gameTimer.Stop();
                MessageBox.Show("Столкновение! Игра окончена.", "Конец игры", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();
            }
        }
    }

    private void MoveBackground()
    {
        backgroundY += 5;
        if (backgroundY >= 600)
        {
            backgroundY = 0;
        }
        _view.MoveBackground(backgroundY);
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Left:
                _model.UpdatePlayerPosition(-1, 0);
                break;
            case Keys.Right:
                _model.UpdatePlayerPosition(1, 0);
                break;
            case Keys.Up:
                _model.UpdatePlayerPosition(0, -1);
                break;
            case Keys.Down:
                _model.UpdatePlayerPosition(0, 1);
                break;
        }

        _view.UpdateView(_model);
    }
}
