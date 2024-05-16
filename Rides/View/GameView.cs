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
    public List<PictureBox> CoinViews { get; private set; }
    public List<PictureBox> ObstacleViews { get; private set; }
    private Image backgroundImage;
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

        PlayerCarView = CreateCarPictureBox("player_car.png");
        OpponentViews = new PictureBox[4]; // увеличиваем количество противников

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
            Location = new Point(350, 250),
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
            // Логирование для отладки
            MessageBox.Show($"Файл изображения препятствия не найден: {filePath}", "Ошибка загрузки", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return new PictureBox
            {
                Size = new Size(50, 50),
                BackColor = Color.Gray // Поддержка временной альтернативы
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
            backgroundY += 7; // устанавливаем скорость фона
            if (backgroundY >= 600)
            {
                backgroundY = 0;
            }
            Invalidate(); // перерисовка формы
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (backgroundImage != null)
        {
            e.Graphics.DrawImage(backgroundImage, new Rectangle(0, backgroundY, 800, 600));
            e.Graphics.DrawImage(backgroundImage, new Rectangle(0, backgroundY - 600, 800, 600));
        }
    }

    public void ShowStartMenu()
    {
        startButton.Visible = true;
        restartButton.Visible = false;
        coinCounterLabel.Visible = false;
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
    }

    public void ShowGame()
    {
        startButton.Visible = false;
        restartButton.Visible = false;
        coinCounterLabel.Visible = true;
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
        this.Focus(); // Убедимся, что форма имеет фокус для получения событий клавиатуры
    }

    public void ShowGameOver()
    {
        restartButton.Visible = true;
        restartButton.BringToFront(); // Переносим кнопку на передний план, чтобы она была видимой
    }
}
