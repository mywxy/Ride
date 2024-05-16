using System;
using System.Collections.Generic;
using System.Drawing;

namespace RacingGame
{
    public class GameModel
    {
        public Car PlayerCar { get; set; }
        public List<Car> Opponents { get; private set; }
        public List<Rectangle> Coins { get; private set; }
        public List<Obstacle> Obstacles { get; private set; }
        public int CoinCount { get; private set; }
        public Random rand { get; private set; } = new Random();
        public const int LeftBoundary = 180;
        public const int RightBoundary = 650; // 800 - 180 (с каждой стороны) - 50 (ширина машины)
        private const int GridSize = 50;
        private int[,] grid;

        public GameModel()
        {
            PlayerCar = new Car(375, 500); // начальная позиция игрока
            Opponents = new List<Car>
            {
                new Car(100, 0), // начальная позиция противников
                new Car(300, 0),
                new Car(500, 0),
                new Car(700, 0)
            };
            Coins = new List<Rectangle>();
            Obstacles = new List<Obstacle>();
            CoinCount = 0;
            InitializeGrid();
            GenerateCoins();
            ResetOpponents();
        }

        public void UpdatePlayerPosition(int deltaX, int deltaY)
        {
            PlayerCar.Move(deltaX, deltaY);
            CheckPlayerBounds();
            CheckCoinCollection();
            UpdateGrid();
        }

        private void InitializeGrid()
        {
            int columns = (RightBoundary - LeftBoundary) / GridSize + 1;
            int rows = 600 / GridSize;
            grid = new int[columns, rows];
            UpdateGrid();
        }

        private void UpdateGrid()
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    grid[x, y] = 0; // 0 - свободная клетка
                }
            }

            foreach (var obstacle in Obstacles)
            {
                int gridX = (obstacle.X - LeftBoundary) / GridSize;
                int gridY = obstacle.Y / GridSize;
                if (gridX >= 0 && gridX < grid.GetLength(0) && gridY >= 0 && gridY < grid.GetLength(1))
                {
                    grid[gridX, gridY] = 1; // 1 - препятствие
                }
            }
        }

        private void CheckPlayerBounds()
        {
            if (PlayerCar.PositionX < LeftBoundary) PlayerCar.PositionX = LeftBoundary;
            if (PlayerCar.PositionX > RightBoundary - 50) PlayerCar.PositionX = RightBoundary - 50;
            if (PlayerCar.PositionY < 0) PlayerCar.PositionY = 0;
            if (PlayerCar.PositionY > 500) PlayerCar.PositionY = 500; // 600 (высота формы) - 100 (высота машины)
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
            int x = rand.Next(LeftBoundary, RightBoundary - 30 + 1); // Учитываем ширину монеты
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

                    if (CoinCount == 20)
                    {
                        GenerateObstacles();
                    }
                }
            }
        }

        private void GenerateObstacles()
        {
            Obstacles.Clear();
            for (int i = 0; i < 3; i++)
            {
                AddObstacle();
            }
        }

        private void AddObstacle()
        {
            int x, y;
            Rectangle newObstacleRect;
            do
            {
                x = rand.Next(LeftBoundary, RightBoundary - 50 + 1); // Учитываем ширину препятствия
                y = rand.Next(-800, -200);
                newObstacleRect = new Rectangle(x, y, 50, 50);
            } while (IsObstacleOverlapping(newObstacleRect));

            Obstacles.Add(new Obstacle(x, y, 50, 50));
        }

        private bool IsObstacleOverlapping(Rectangle newObstacleRect)
        {
            foreach (var obstacle in Obstacles)
            {
                if (newObstacleRect.IntersectsWith(new Rectangle(obstacle.X, obstacle.Y, obstacle.Width, obstacle.Height)))
                {
                    return true;
                }
            }
            return false;
        }

        public void MoveObstacles()
        {
            for (int i = 0; i < Obstacles.Count; i++)
            {
                var obstacle = Obstacles[i];
                obstacle.Y += 5; // скорость препятствий
                if (obstacle.Y > 600)
                {
                    Obstacles.RemoveAt(i);
                    i--;
                    AddObstacle();
                }
                else
                {
                    Obstacles[i] = obstacle;
                }
            }
        }

        public void ResetOpponents()
        {
            foreach (var car in Opponents)
            {
                do
                {
                    car.PositionX = rand.Next(LeftBoundary, RightBoundary - 50 + 1); // случайная позиция по X в пределах границ
                    car.PositionY = rand.Next(-400, -100); // случайная позиция по Y (выше экрана)
                }
                while (IsOverlapping(car));
            }
        }

        private bool IsOverlapping(Car newCar)
        {
            Rectangle newCarRect = new Rectangle(newCar.PositionX, newCar.PositionY, 50, 100);
            foreach (var car in Opponents)
            {
                if (car != newCar)
                {
                    Rectangle carRect = new Rectangle(car.PositionX, car.PositionY, 50, 100);
                    if (newCarRect.IntersectsWith(carRect))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public List<Point> FindPath(Point start, Point goal)
        {
            var openList = new SortedSet<Node>(new NodeComparer());
            var closedList = new HashSet<Point>();
            var gCosts = new Dictionary<Point, int>();
            var fCosts = new Dictionary<Point, int>();
            var cameFrom = new Dictionary<Point, Point>();

            openList.Add(new Node(start, 0, HeuristicCost(start, goal)));
            gCosts[start] = 0;
            fCosts[start] = HeuristicCost(start, goal);

            while (openList.Count > 0)
            {
                var currentNode = openList.Min;
                openList.Remove(currentNode);
                var current = currentNode.Position;

                if (current == goal)
                {
                    return ReconstructPath(cameFrom, current);
                }

                closedList.Add(current);

                foreach (var neighbor in GetNeighbors(current))
                {
                    if (closedList.Contains(neighbor))
                    {
                        continue;
                    }

                    int tentativeGCost = gCosts[current] + 1;

                    if (!gCosts.ContainsKey(neighbor) || tentativeGCost < gCosts[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gCosts[neighbor] = tentativeGCost;
                        fCosts[neighbor] = tentativeGCost + HeuristicCost(neighbor, goal);

                        openList.Add(new Node(neighbor, gCosts[neighbor], fCosts[neighbor]));
                    }
                }
            }

            return null; // путь не найден
        }

        private List<Point> GetNeighbors(Point point)
        {
            var neighbors = new List<Point>
            {
                new Point(point.X + 1, point.Y),
                new Point(point.X - 1, point.Y),
                new Point(point.X, point.Y + 1),
                new Point(point.X, point.Y - 1)
            };

            neighbors.RemoveAll(p => p.X < 0 || p.X >= grid.GetLength(0) || p.Y < 0 || p.Y >= grid.GetLength(1) || grid[p.X, p.Y] == 1);

            return neighbors;
        }

        private int HeuristicCost(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        private List<Point> ReconstructPath(Dictionary<Point, Point> cameFrom, Point current)
        {
            var totalPath = new List<Point> { current };
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.Add(current);
            }
            totalPath.Reverse();
            return totalPath;
        }

        public Rectangle GetPlayerRectangle()
        {
            return new Rectangle(PlayerCar.PositionX, PlayerCar.PositionY, 50, 100);
        }

        public Rectangle GetOpponentRectangle(Car car)
        {
            return new Rectangle(car.PositionX, car.PositionY, 50, 100);
        }

        public Rectangle GetObstacleRectangle(Obstacle obstacle)
        {
            return new Rectangle(obstacle.X, obstacle.Y, obstacle.Width, obstacle.Height);
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
            Speed = 5; // Вернем прежнюю скорость
        }

        public void Move(int deltaX, int deltaY)
        {
            PositionX += deltaX * Speed;
            PositionY += deltaY * Speed;
        }
    }

    public class Obstacle
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Obstacle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }

    public class Node
    {
        public Point Position { get; }
        public int GCost { get; }
        public int FCost { get; }

        public Node(Point position, int gCost, int fCost)
        {
            Position = position;
            GCost = gCost;
            FCost = fCost;
        }
    }

    public class NodeComparer : IComparer<Node>
    {
        public int Compare(Node x, Node y)
        {
            int compare = x.FCost.CompareTo(y.FCost);
            if (compare == 0)
            {
                compare = x.GCost.CompareTo(y.GCost);
            }
            return compare;
        }
    }
}
