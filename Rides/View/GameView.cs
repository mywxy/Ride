using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System;
using System.Collections.Generic;

public class GameView : Form
{
    public PictureBox PlayerCarView { get; private set; }
    public PictureBox[] OpponentViews { get; private set; }
    private Label coinCounterLabel;
    public List<PictureBox> CoinViews { get; private set; }
    private Image backgroundImage;
    private Image backgroundBuffer;
    private int backgroundY;

    public GameView()
    {
        this.ClientSize = new Size(800, 600);
        this.Text = "Advanced Racing Game";
        this.DoubleBuffered = true;

        string backgroundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "background_image.jpg");
        if (File.Exists(backgroundPath))
        {
            backgroundImage = Image.FromFile(backgroundPath);
            backgroundBuffer = new Bitmap(800, 1200);
        }
        else
        {
            MessageBox.Show($"Файл фонового изображения не найден: {backgroundPath}", "Ошибка загрузки", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        PlayerCarView = CreateCarPictureBox("player_car.png");
        OpponentViews = new PictureBox[2];

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
    }

    public void MoveBackground(int offset)
    {
        if (backgroundImage != null)
        {
            using (Graphics g = Graphics.FromImage(backgroundBuffer))
            {
                g.DrawImage(backgroundImage, new Rectangle(0, offset, 800, 600));
                g.DrawImage(backgroundImage, new Rectangle(0, offset - 600, 800, 600));
            }
            this.BackgroundImage = backgroundBuffer;
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        if (backgroundImage != null)
        {
            e.Graphics.DrawImage(backgroundBuffer, 0, 0);
        }
        base.OnPaint(e);
    }
}
