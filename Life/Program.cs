using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Text.Json;

namespace cli_life
{
    public class Settings
    {
        public int Width { get; set; } = 50;
        public int Height { get; set; } = 20;
        public int CellSize { get; set; } = 1;
        public double LiveDensity { get; set; } = 0.5;
    }
    public class Cell
    {
        public bool IsAlive;
        public readonly List<Cell> neighbors = new List<Cell>();
        private bool IsAliveNext;
        public void DetermineNextLiveState()
        {
            int liveNeighbors = neighbors.Where(x => x.IsAlive).Count();
            if (IsAlive)
                IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            else
                IsAliveNext = liveNeighbors == 3;
        }
        public void Advance()
        {
            IsAlive = IsAliveNext;
        }
    }
    public class Board
    {
        public readonly Cell[,] Cells;
        public readonly int CellSize;

        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }

        public Board(int width, int height, int cellSize, double liveDensity = .1)
        {
            CellSize = cellSize;

            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
            Randomize(liveDensity);
        }

        readonly Random rand = new Random();
        public void Randomize(double liveDensity)
        {
            foreach (var cell in Cells)
                cell.IsAlive = rand.NextDouble() < liveDensity;
        }

        public void Advance()
        {
            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
            foreach (var cell in Cells)
                cell.Advance();
        }
        private void ConnectNeighbors()
        {
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    int xL = (x > 0) ? x - 1 : Columns - 1;
                    int xR = (x < Columns - 1) ? x + 1 : 0;

                    int yT = (y > 0) ? y - 1 : Rows - 1;
                    int yB = (y < Rows - 1) ? y + 1 : 0;

                    Cells[x, y].neighbors.Add(Cells[xL, yT]);
                    Cells[x, y].neighbors.Add(Cells[x, yT]);
                    Cells[x, y].neighbors.Add(Cells[xR, yT]);
                    Cells[x, y].neighbors.Add(Cells[xL, y]);
                    Cells[x, y].neighbors.Add(Cells[xR, y]);
                    Cells[x, y].neighbors.Add(Cells[xL, yB]);
                    Cells[x, y].neighbors.Add(Cells[x, yB]);
                    Cells[x, y].neighbors.Add(Cells[xR, yB]);
                }
            }
        }
        public void Save(string path)
        {
            string[] lines = new string[Rows];
            for (int y = 0; y < Rows; y++)
            {
                string line = "";
                for (int x = 0; x < Columns; x++)
                    line += Cells[x, y].IsAlive ? "*" : " ";
                lines[y] = line;
            }
            File.WriteAllLines(path, lines);
        }

        public void Load(string path)
        {
            if (!File.Exists(path)) return;
            string[] lines = File.ReadAllLines(path);
            for (int y = 0; y < Rows && y < lines.Length; y++)
            {
                for (int x = 0; x < Columns && x < lines[y].Length; x++)
                {
                    Cells[x, y].IsAlive = (lines[y][x] == '*');
                }
            }
        }

        public int CountClusters()
        {
            var visited = new HashSet<Cell>();
            int clusters = 0;
            foreach (var cell in Cells)
            {
                if (cell.IsAlive && !visited.Contains(cell))
                {
                    clusters++;
                    var stack = new Stack<Cell>();
                    stack.Push(cell);
                    visited.Add(cell);
                    while (stack.Count > 0)
                    {
                        var c = stack.Pop();
                        foreach (var n in c.neighbors)
                        {
                            if (n.IsAlive && !visited.Contains(n))
                            {
                                visited.Add(n);
                                stack.Push(n);
                            }
                        }
                    }
                }
            }
            return clusters;
        }
    }
    class Program
    {
        static Board board;
        static private void Reset()
        {
            board = new Board(
                width: 50,
                height: 20,
                cellSize: 1,
                liveDensity: 0.5);
        }
        static void Render()
        {
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)   
                {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive)
                    {
                        Console.Write('*');
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
                Console.Write('\n');
            }
        }

        public static void RunResearch()
        {
            Directory.CreateDirectory("../Data");
            string data = "";

            for (double density = 0.1; density <= 0.9; density += 0.1)
            {
                int totalGenerations = 0;
                int attempts = 5;

                for (int a = 0; a < attempts; a++)
                {
                    Board b = new Board(50, 20, 1, density);
                    int gens = 0;
                    int sameCount = 0;
                    int lastAlive = 0;

                    while (sameCount < 15 && gens < 1000)
                    {
                        b.Advance();
                        int currentAlive = 0;
                        foreach (var cell in b.Cells) if (cell.IsAlive) currentAlive++;
                        
                        if (currentAlive == lastAlive) sameCount++;
                        else sameCount = 0;

                        lastAlive = currentAlive;
                        gens++;
                    }
                    totalGenerations += (gens - 15);
                }
                data += $"{density:F1} {totalGenerations / (double)attempts}\n";
                Console.WriteLine($"Плотность {density:F1}: {totalGenerations / (double)attempts} пок.");
            }
            File.WriteAllText("../Data/data.txt", data);
            Console.WriteLine("Данные сохранены в Data/data.txt");
        }

        public static void SetupFromJson()
        {
            if (File.Exists("settings.json"))
            {
                string json = File.ReadAllText("settings.json");
                Settings s = JsonSerializer.Deserialize<Settings>(json);
                board = new Board(s.Width, s.Height, s.CellSize, s.LiveDensity);
            }
            if (File.Exists("figure.txt"))
            {
                board.Load("figure.txt");
            }
        }

        public static void PrintStats()
        {
            int alive = 0;
            foreach (var cell in board.Cells) if (cell.IsAlive) alive++;
            Console.WriteLine($"Живых: {alive}, Групп (комбинаций): {board.CountClusters()}");
            Console.WriteLine("Наблюдаемые фигуры: Блок, Улей, Лодка и др.");
            board.Save("state_backup.txt");
        }
        
        static void Main(string[] args)
        {
            Console.WriteLine("Нажмите 2 для исследования или Enter для обычной игры:");
            if (Console.ReadLine() == "2") { RunResearch(); return; }
            Reset();
            SetupFromJson();
            while(true)
            {
                Console.Clear();
                Render();
                PrintStats();
                board.Advance();
                Thread.Sleep(1000);
            }
        }
    }
}
