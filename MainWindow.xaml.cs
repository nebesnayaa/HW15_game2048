using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HW15_game2048
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            StartNewGame();
        }

        private const int areaSize = 4;

        private Brush GetNumberColor(int num)
        {
            switch (num)
            {
                case 2:
                    return new SolidColorBrush(Color.FromRgb(236, 224, 202)); // Колір для цифри 2
                case 4:
                    return new SolidColorBrush(Color.FromRgb(230, 221, 171)); // Колір для цифри 4
                case 8:
                    return new SolidColorBrush(Color.FromRgb(247, 196, 114));
                case 16:
                    return new SolidColorBrush(Color.FromRgb(250, 169, 115));
                case 32:
                    return new SolidColorBrush(Color.FromRgb(255, 158, 158));
                case 64:
                    return new SolidColorBrush(Color.FromRgb(255, 125, 125));
                case 128:
                    return new SolidColorBrush(Color.FromRgb(255, 242, 154));
                case 256:
                    return new SolidColorBrush(Color.FromRgb(255, 239, 134));
                case 512:
                    return new SolidColorBrush(Color.FromRgb(255, 235, 106));
                case 1024:
                    return new SolidColorBrush(Color.FromRgb(220, 147, 198));
                case 2048:
                    return new SolidColorBrush(Color.FromRgb(255, 117, 182));
                default:
                    return Brushes.White;
            }
        }

        private void AddNewNumber(int positionX, int positionY, int Num)
        {
            System.Windows.Controls.Label label = new System.Windows.Controls.Label();
            label.Content = Num.ToString();
            label.Background = GetNumberColor(Num);

            Grid.SetColumn(label, positionX);
            Grid.SetRow(label, positionY);

            Field.Children.Add(label);
        }

        private void RandomLabelSpawn(List<Point> PossibleLocate)
        {
            Random random = new Random();

            const int SmallerNumber = 2;
            const int LargerNumber = 4;
            const int ChanceLargerNumber = 10;

            int NumToSpawn = random.Next(0, 100) <= ChanceLargerNumber ? LargerNumber : SmallerNumber;

            Point LocateToSpawn = PossibleLocate[random.Next(PossibleLocate.Count)];
            AddNewNumber((int)LocateToSpawn.X, (int)LocateToSpawn.Y, NumToSpawn);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Left && 
                e.Key != Key.Right && 
                e.Key != Key.Up && 
                e.Key != Key.Down) 
                return;

            int[,] Nums = new int[areaSize, areaSize];
            int[,] CopyNums = new int[areaSize, areaSize];

            foreach (System.Windows.Controls.Label l in Field.Children)
            {
                Nums[Grid.GetColumn(l), Grid.GetRow(l)] = int.Parse(l.Content.ToString());
                CopyNums[Grid.GetColumn(l), Grid.GetRow(l)] = int.Parse(l.Content.ToString());
            }

            switch (e.Key)
            {
                case Key.Right:
                    {
                        RotateMatrix.Degrees90(Nums);
                        RotateMatrix.Degrees90(Nums);
                    }
                    break;
                case Key.Down:
                    {
                        RotateMatrix.DegreesMinus90(Nums);
                    }
                    break;
                case Key.Up:
                    {
                        RotateMatrix.Degrees90(Nums);
                    }
                    break;
            }

            for (int i = 0; i < areaSize; i++)
                for (int y = 1; y < areaSize; y++)
                    if (Nums[y, i] != 0)
                    {
                        bool IsJoined = false;
                        for (int z = y - 1; z >= 0; z--)
                            if (Nums[z, i] == Nums[y, i])
                            {
                                Nums[z, i] = Nums[z, i] + Nums[y, i];
                                Nums[y, i] = 0;
                                AddScore(Nums[z, i]);
                                IsJoined = true;
                                break;
                            }
                            else if (Nums[z, i] != 0) break;

                        if (!IsJoined)
                            for (int z = 0; z < y; z++)
                                if (Nums[z, i] == 0)
                                {
                                    Nums[z, i] = Nums[y, i];
                                    Nums[y, i] = 0;
                                    break;
                                }
                    }

            switch (e.Key)
            {
                case Key.Right:
                    {
                        RotateMatrix.Degrees90(Nums);
                        RotateMatrix.Degrees90(Nums);
                    }
                    break;
                case Key.Down:
                    {
                        RotateMatrix.Degrees90(Nums);
                    }
                    break;
                case Key.Up:
                    {
                        RotateMatrix.DegreesMinus90(Nums);
                    }
                    break;
            }

            Field.Children.Clear();
            for (int y = 0; y < areaSize; y++)
                for (int x = 0; x < areaSize; x++)
                {
                    if (Nums[x, y] != 0) AddNewNumber(x, y, Nums[x, y]);
                }

            List<Point> CloseLocate = new List<Point>();
            foreach (System.Windows.Controls.Label l in Field.Children)
            {
                CloseLocate.Add(new Point(Grid.GetColumn(l), Grid.GetRow(l)));
            }

            List<Point> FreeLocate = new List<Point>();
            for (int x = 0; x < areaSize; x++)
                for (int y = 0; y < areaSize; y++)
                {
                    Point point = new Point(x, y);
                    if (!CloseLocate.Contains(point)) FreeLocate.Add(point);
                }
            
            if (FreeLocate.Count != 0 && 
                !CopyNums.Cast<int>().SequenceEqual(Nums.Cast<int>())) 
                RandomLabelSpawn(FreeLocate);

            if (IsWin())
            {
                MessageBox.Show("You won");
                StartNewGame();
            }
        }

        private bool IsWin()
        {
            foreach (System.Windows.Controls.Label l in Field.Children)
            {
                if (int.Parse(l.Content.ToString()) == 2048) 
                    return true;
            }
            return false;
        }

        private void StartNewGame()
        {
            scoreTextBlock.Text = "0";
            Field.Children.Clear();

            List<Point> FreeLocate = new List<Point>();
            for (int x = 0; x < areaSize; x++)
                for (int y = 0; y < areaSize; y++)
                {
                    FreeLocate.Add(new Point(x, y));
                }

            if (FreeLocate.Count != 0) RandomLabelSpawn(FreeLocate);
        }

        private void AddScore(int Value)
        {
            scoreTextBlock.Text = (int.Parse(scoreTextBlock.Text) + Value).ToString();
        }

        private void restartButton_Click(object sender, RoutedEventArgs e)
        {
            StartNewGame();
        }
    }

    public static class RotateMatrix
    {
        public static void Degrees90(int[,] Matrix)
        {
            int Rows = Matrix.GetLength(0);
            int Cols = Matrix.GetLength(1);

            int[,] TempMatrix = new int[Cols, Rows];

            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Cols; j++)
                {
                    TempMatrix[j, Rows - 1 - i] = Matrix[i, j];
                }

            for (int i = 0; i < Cols; i++)
                for (int j = 0; j < Rows; j++)
                {
                    Matrix[i, j] = TempMatrix[i, j];
                }
        }

        public static void DegreesMinus90(int[,] Matrix)
        {
            int Rows = Matrix.GetLength(0);
            int Cols = Matrix.GetLength(1);

            int[,] RotatedMatrix = new int[Cols, Rows];

            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Cols; j++)
                {
                    RotatedMatrix[Cols - 1 - j, i] = Matrix[i, j];
                }

            for (int i = 0; i < Cols; i++)
                for (int j = 0; j < Rows; j++)
                {
                    Matrix[i, j] = RotatedMatrix[i, j];
                }
        }
    }
}
