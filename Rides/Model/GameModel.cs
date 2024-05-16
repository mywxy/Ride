using System;
using System.Collections.Generic;
using System.Drawing;

public class GameModel
{
    public Car PlayerCar { get; set; }
    public List<Car> Opponents { get; private set; }
    public List<Rectangle> Coins { get; private set; }
    public int CoinCount { get; private set; }
    private Random rand = new Random();

    public GameModel()
    {
        PlayerCar = new Car(375, 500); // начальная позиция игрока
        Opponents = new List<Car>
        {
            new Car(100, 0), // начальная позиция противников
            new Car(600, 0)
        };
        Coins = new List<Rectangle>();
        CoinCount = 0;
        GenerateCoins();
    }

    public void UpdatePlayerPosition(int deltaX, int deltaY)
    {
        PlayerCar.Move(deltaX, deltaY);
        CheckPlayerBounds();
        CheckCoinCollection();
    }

    private void CheckPlayerBounds()
    {
        if (PlayerCar.PositionX < 0) PlayerCar.PositionX = 0;
        if (PlayerCar.PositionX > 750) PlayerCar.PositionX = 750; // 800 (ширина формы) - 50 (ширина машины)
        if (PlayerCar.PositionY < 0) PlayerCar.PositionY = 0;
        if (PlayerCar.PositionY > 500) PlayerCar.PositionY = 500; // 600 (высота формы) - 100 (высота машины)
    }

    public void ResetOpponents()
    {
        foreach (var car in Opponents)
        {
            do
            {
                car.PositionX = rand.Next(0, 751); // случайная позиция по X
                car.PositionY = rand.Next(-400, -100); // случайная позиция по Y (выше экрана)
            }
            while (IsOverlapping(car));
        }
    }

    private bool IsOverlapping(Car newCar)
    {
        foreach (var car in Opponents)
        {
            if (car != newCar)
            {
                Rectangle newCarRect = new Rectangle(newCar.PositionX, newCar.PositionY, 50, 100);
                Rectangle carRect = new Rectangle(car.PositionX, car.PositionY, 50, 100);
                if (newCarRect.IntersectsWith(carRect))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void GenerateCoins()
    {
        Coins.Clear();
        for (int i = 0; i < 5; i++)
        {
            AddCoin();
        }
    }

    private void AddCoin()
    {
        int x = rand.Next(0, 751);
        int y = rand.Next(-600, -50);
        Coins.Add(new Rectangle(x, y, 30, 30)); // размер монеты 30x30
    }

    public void MoveCoins()
    {
        for (int i = 0; i < Coins.Count; i++)
        {
            Rectangle coin = Coins[i];
            coin.Y += 5; // скорость монет
            if (coin.Y > 600) // если монета вышла за нижний край экрана
            {
                Coins.RemoveAt(i);
                i--;
                AddCoin();
            }
            else
            {
                Coins[i] = coin;
            }
        }
    }

    private void CheckCoinCollection()
    {
        Rectangle playerRect = new Rectangle(PlayerCar.PositionX, PlayerCar.PositionY, 50, 100);
        for (int i = 0; i < Coins.Count; i++)
        {
            if (playerRect.IntersectsWith(Coins[i]))
            {
                Coins.RemoveAt(i);
                i--;
                CoinCount++;
                AddCoin();
            }
        }
    }
}

public class Car
{
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public int Speed { get; set; }

    public Car(int x, int y)
    {
        PositionX = x;
        PositionY = y;
        Speed = 5;
    }

    public void Move(int deltaX, int deltaY)
    {
        PositionX += deltaX * Speed;
        PositionY += deltaY * Speed;
    }
}
