using System.Drawing;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using RacingGame;

public class GameController
{
    private GameModel _model;
    private GameView _view;
    private Timer _gameTimer;
    private int record;

    public GameController(GameModel model, GameView view)
    {
        _model = model;
        _view = view;

        _view.KeyDown += OnKeyDown;
        _view.StartGame += OnStartGame;
        _view.RestartGame += OnRestartGame;
        InitializeTimer();
    }

    private void InitializeTimer()
    {
        _gameTimer = new Timer();
        _gameTimer.Interval = 50;
        _gameTimer.Tick += GameTick;
        _gameTimer.Stop();
    }

    private void GameTick(object sender, EventArgs e)
    {
        MoveOpponents();
        MoveCoins();
        MoveObstacles();
        CheckCollisions();
        MoveBackground();
        _view.UpdateView(_model);
    }

    private void MoveOpponents()
    {
        foreach (var car in _model.Opponents)
        {
            if (_model.CoinCount >= 50)
            {
                Point playerPosition = new Point(_model.PlayerCar.PositionX / 50, _model.PlayerCar.PositionY / 50);
                Point carPosition = new Point(car.PositionX / 50, car.PositionY / 50);
                List<Point> path = _model.FindPath(carPosition, playerPosition);

                if (path != null && path.Count > 1)
                {
                    Point nextPosition = path[1];
                    int deltaX = nextPosition.X - carPosition.X;
                    int deltaY = nextPosition.Y - carPosition.Y;
                    car.Move(deltaX, deltaY);
                }
                else
                {
                    car.Move(0, 1);
                }
            }
            else
            {
                car.Move(0, 1);
            }

            if (car.PositionY > 600)
            {
                car.PositionY = 0;
                car.PositionX = _model.rand.Next(GameModel.LeftBoundary, GameModel.RightBoundary - 50 + 1);
            }
        }
    }

    private void MoveCoins()
    {
        _model.MoveCoins();
    }

    private void MoveObstacles()
    {
        if (_model.CoinCount >= 20)
        {
            _model.MoveObstacles();
        }
    }

    private void CheckCollisions()
    {
        Rectangle playerRect = _model.GetPlayerRectangle();
        foreach (var car in _model.Opponents)
        {
            Rectangle carRect = _model.GetOpponentRectangle(car);
            if (playerRect.IntersectsWith(carRect))
            {
                EndGame();
                return;
            }
        }

        foreach (var obstacle in _model.Obstacles)
        {
            Rectangle obstacleRect = _model.GetObstacleRectangle(obstacle);
            if (playerRect.IntersectsWith(obstacleRect))
            {
                EndGame();
                return;
            }
        }
    }

    private void EndGame()
    {
        _gameTimer.Stop();
        if (_model.CoinCount > record)
        {
            record = _model.CoinCount;
        }
        _view.ShowGameOver(_model.CoinCount, record);
    }

    private void MoveBackground()
    {
        _view.MoveBackground();
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (_gameTimer.Enabled)
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

    private void OnStartGame(object sender, EventArgs e)
    {
        _view.ShowGame();
        _gameTimer.Start();
    }

    private void OnRestartGame(object sender, EventArgs e)
    {
        _model = new GameModel();
        _view.UpdateView(_model);
        _view.ShowGame();
        _gameTimer.Start();
    }
}
