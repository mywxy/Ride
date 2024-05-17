using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using RacingGame;

public class GameView : Form
{
    public PictureBox PlayerCarView { get; private set; }
    public PictureBox[] OpponentViews { get; private set; }
    private Label coinCounterLabel;
    private Label scoreLabel;
    private Label recordLabel;
    private Label goalLabel;
    private Label welcomeLabel;
    public List<PictureBox> CoinViews { get; private set; }
    public List<PictureBox> ObstacleViews { get; private set; }
    private Image backgroundImage;
    private Image startBackgroundImage;
    private Image gameOverBackgroundImage;
    private int backgroundY;
    private Button startButton;
    private Button restartButton;
    public event EventHandler StartGame;
    public event EventHandler RestartGame;

    public GameView()
    {
        this.ClientSize = new Size(800, 600);
        this.Text = "Advanced Racing Game";
        this.DoubleBuffered = true;

        string backgroundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "background_image.jpg");
        if (File.Exists(backgroundPath))
        {
            backgroundImage = Image.FromFile(backgroundPath);
        }
        else
        {
            MessageBox.Show($"Файл фонового изображения не найден: {backgroundPath}", "Ошибка загрузки", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        string startBackgroundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "start_background_image.jpg");
        if (File.Exists(startBackgroundPath))
        {
            startBackgroundImage = Image.FromFile(startBackgroundPath);
        }
        else
        {
            MessageBox.Show($"Файл фонового изображения для стартового экрана не найден: {startBackgroundPath}", "Ошибка загрузки", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        string gameOverBackgroundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "gameover_background_image.jpg");
        if (File.Exists(gameOverBackgroundPath))
        {
            gameOverBackgroundImage = Image.FromFile(gameOverBackgroundPath);
        }
        else
        {
            MessageBox.Show($"Файл фонового изображения для экрана проигрыша не найден: {gameOverBackgroundPath}", "Ошибка загрузки", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        PlayerCarView = CreateCarPictureBox("player_car.png");
        OpponentViews = new PictureBox[4];

        for (int i = 0; i < OpponentViews.Length; i++)
        {
            OpponentViews[i] = CreateCarPictureBox("opponent_car.png");
            this.Controls.Add(OpponentViews[i]);
        }

        this.Controls.Add(PlayerCarView);

        coinCounterLabel = new Label
        {
            Location = new Point(10, 10),
            AutoSize = true,
            Font = new Font("Arial", 16, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.Transparent
        };
        this.Controls.Add(coinCounterLabel);

        scoreLabel = new Label
        {
            Location = new Point(350, 200),
            AutoSize = true,
            Font = new Font("Arial", 24, FontStyle.Bold),
            ForeColor = Color.Red,
            BackColor = Color.Transparent,
            Visible = false
        };
        this.Controls.Add(scoreLabel);

        recordLabel = new Label
        {
            Location = new Point(350, 300),
            AutoSize = true,
            Font = new Font("Arial", 24, FontStyle.Bold),
            ForeColor = Color.Red,
            BackColor = Color.Transparent,
            Visible = false
        };
        this.Controls.Add(recordLabel);

        goalLabel = new Label
        {
            Location = new Point(10, 40),
            AutoSize = true,
            Font = new Font("Arial", 16, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.Transparent
        };
        this.Controls.Add(goalLabel);

        welcomeLabel = new Label
        {
            Text = "Давно тебя не было в уличных гонках!",
            Location = new Point(250, 320),
            AutoSize = true,
            Font = new Font("Arial", 16, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.Transparent
        };
        this.Controls.Add(welcomeLabel);

        CoinViews = new List<PictureBox>();
        ObstacleViews = new List<PictureBox>();

        startButton = new Button
        {
            Text = "Start",
            Location = new Point(350, 250),
            Size = new Size(100, 50),
            Font = new Font("Arial", 14, FontStyle.Bold)
        };
        startButton.Click += (sender, e) => StartGame?.Invoke(this, EventArgs.Empty);
        this.Controls.Add(startButton);

        restartButton = new Button
        {
            Text = "Restart",
            Location = new Point(350, 400),
            Size = new Size(100, 50),
            Font = new Font("Arial", 14, FontStyle.Bold),
            Visible = false
        };
        restartButton.Click += (sender, e) => RestartGame?.Invoke(this, EventArgs.Empty);
        this.Controls.Add(restartButton);
    }

    private PictureBox CreateCarPictureBox(string imageFile)
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", imageFile);
        if (!File.Exists(filePath))
        {
            MessageBox.Show($"Файл изображения не найден: {filePath}", "Ошибка загрузки", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return new PictureBox();
        }

        return new PictureBox
        {
            Size = new Size(50, 100),
            Image = Image.FromFile(filePath),
            SizeMode = PictureBoxSizeMode.StretchImage,
            BackColor = Color.Transparent
        };
    }

    private PictureBox CreateCoinPictureBox()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "coin.png");
        if (!File.Exists(filePath))
        {
            MessageBox.Show($"Файл изображения не найден: {filePath}", "Ошибка загрузки", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return new PictureBox();
        }

        return new PictureBox
        {
            Size = new Size(30, 30),
            Image = Image.FromFile(filePath),
            SizeMode = PictureBoxSizeMode.StretchImage,
            BackColor = Color.Transparent
        };
    }

    private PictureBox CreateObstaclePictureBox()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "obstacle.png");
        if (!File.Exists(filePath))
        {
            MessageBox.Show($"Файл изображения препятствия не найден: {filePath}", "Ошибка загрузки", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return new PictureBox
            {
                Size = new Size(50, 50),
                BackColor = Color.Gray
            };
        }

        return new PictureBox
        {
            Size = new Size(50, 50),
            Image = Image.FromFile(filePath),
            SizeMode = PictureBoxSizeMode.StretchImage,
            BackColor = Color.Transparent
        };
    }

    public void UpdateView(GameModel model)
    {
        PlayerCarView.Location = new Point(model.PlayerCar.PositionX, model.PlayerCar.PositionY);

        for (int i = 0; i < OpponentViews.Length; i++)
        {
            OpponentViews[i].Location = new Point(model.Opponents[i].PositionX, model.Opponents[i].PositionY);
        }

        coinCounterLabel.Text = $"Монеты: {model.CoinCount}";
        goalLabel.Text = $"Цель: {model.CurrentGoal}";

        foreach (var coinView in CoinViews)
        {
            this.Controls.Remove(coinView);
        }
        CoinViews.Clear();

        foreach (var coin in model.Coins)
        {
            var coinView = CreateCoinPictureBox();
            coinView.Location = new Point(coin.X, coin.Y);
            CoinViews.Add(coinView);
            this.Controls.Add(coinView);
            coinView.BringToFront();
        }

        foreach (var obstacleView in ObstacleViews)
        {
            this.Controls.Remove(obstacleView);
        }
        ObstacleViews.Clear();

        foreach (var obstacle in model.Obstacles)
        {
            var obstacleView = CreateObstaclePictureBox();
            obstacleView.Location = new Point(obstacle.X, obstacle.Y);
            ObstacleViews.Add(obstacleView);
            this.Controls.Add(obstacleView);
            obstacleView.BringToFront();
        }
    }

    public void MoveBackground()
    {
        if (backgroundImage != null)
        {
            backgroundY += 7;
            if (backgroundY >= 600)
            {
                backgroundY = 0;
            }
            Invalidate();
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (startButton.Visible && startBackgroundImage != null)
        {
            e.Graphics.DrawImage(startBackgroundImage, new Rectangle(0, 0, 800, 600));
        }
        else if (restartButton.Visible && gameOverBackgroundImage != null)
        {
            e.Graphics.DrawImage(gameOverBackgroundImage, new Rectangle(0, 0, 800, 600));
        }
        else if (backgroundImage != null)
        {
            e.Graphics.DrawImage(backgroundImage, new Rectangle(0, backgroundY, 800, 600));
            e.Graphics.DrawImage(backgroundImage, new Rectangle(0, backgroundY - 600, 800, 600));
        }
    }

    public void ShowStartMenu()
    {
        startButton.Visible = true;
        restartButton.Visible = false;
        welcomeLabel.Visible = true;
        coinCounterLabel.Visible = false;
        goalLabel.Visible = false;
        scoreLabel.Visible = false;
        recordLabel.Visible = false;
        PlayerCarView.Visible = false;
        foreach (var opponentView in OpponentViews)
        {
            opponentView.Visible = false;
        }
        foreach (var coinView in CoinViews)
        {
            coinView.Visible = false;
        }
        foreach (var obstacleView in ObstacleViews)
        {
            obstacleView.Visible = false;
        }
        Invalidate();
    }

    public void ShowGame()
    {
        startButton.Visible = false;
        restartButton.Visible = false;
        welcomeLabel.Visible = false;
        scoreLabel.Visible = false;
        recordLabel.Visible = false;
        coinCounterLabel.Visible = true;
        goalLabel.Visible = true;
        PlayerCarView.Visible = true;
        foreach (var opponentView in OpponentViews)
        {
            opponentView.Visible = true;
        }
        foreach (var coinView in CoinViews)
        {
            coinView.Visible = true;
        }
        foreach (var obstacleView in ObstacleViews)
        {
            obstacleView.Visible = true;
        }
        this.Focus();
    }

    public void ShowGameOver(int score, int record)
    {
        scoreLabel.Text = $"Ваш счет: {score}";
        recordLabel.Text = $"Рекорд: {record}";
        scoreLabel.Visible = true;
        recordLabel.Visible = true;
        restartButton.Visible = true;
        restartButton.BringToFront();

        coinCounterLabel.Visible = false;
        goalLabel.Visible = false;
        PlayerCarView.Visible = false;
        foreach (var opponentView in OpponentViews)
        {
            opponentView.Visible = false;
        }
        foreach (var coinView in CoinViews)
        {
            coinView.Visible = false;
        }
        foreach (var obstacleView in ObstacleViews)
        {
            obstacleView.Visible = false;
        }

        foreach (var coinView in CoinViews)
        {
            this.Controls.Remove(coinView); 
        }
        CoinViews.Clear(); 

        foreach (var obstacleView in ObstacleViews)
        {
            this.Controls.Remove(obstacleView); 
        }
        ObstacleViews.Clear(); 

        Invalidate(); 
    }
}
