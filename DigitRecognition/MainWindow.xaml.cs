using knnLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DigitRecognition
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadCSV();
            brushWidth.Content = $"Brush weight: {Drawer.DefaultDrawingAttributes.Width.ToString()}";

        }

        int[][] Data;

        void LoadCSV()
        {
            try
            {
                var csv = File.ReadAllLines("optdigits.dat");
                Data = new int[csv.Length][];
                for (int i = 0; i < csv.Length; i++)
                {
                    Data[i] = csv[i].Split(',').ToIntArray();
                }
            }
            catch (Exception e)
            {
                tbError.Text = e.ToString();
            }
            finally
            {
                tbError.Text = "data loaded";
            }

        }


        int SquareSize = 4;
        int BitmapSize = 32;

        public int[][] Bitmap { get; set; }
        void CreateDataArray()
        {
            Bitmap = new int[BitmapSize][];
            for (int i = 0; i < BitmapSize; i++)
            {
                Bitmap[i] = new int[BitmapSize];
            }
        }

        List<Point> GetPointsFromCanvas()
        {
            List<Point> pointsInside = new List<Point>();
            foreach (var stroke in Drawer.Strokes)
            {

                Geometry sketchGeo = stroke.GetGeometry();
                Rect strokeBounds = sketchGeo.Bounds;

                for (int x = (int)strokeBounds.TopLeft.X; x < (int)strokeBounds.TopRight.X + 1; x++)
                {
                    for (int y = (int)strokeBounds.TopLeft.Y; y < (int)strokeBounds.BottomLeft.Y + 1; y++)
                    {
                        Point p = new Point(x, y);

                        if (sketchGeo.FillContains(p))
                            pointsInside.Add(p);
                    }
                }
            }
            return pointsInside;
        }

        List<Point> GetCanvasStrokes()
        {
            var strokes = Drawer.Strokes;
            var points = new List<Point>();
            for (int i = 0; i < strokes.Count; i++)
            {
                points = points.Concat(strokes[i].StylusPoints.Select(x => new Point(x.X, x.Y))).ToList();
            }
            return points;
        }

        void FillBitmap(List<Point> points)
        {
            for (int i = 0; i < BitmapSize; i++)
            {
                for (int j = 0; j < BitmapSize; j++)
                {
                    Bitmap[i][j] = 0;
                }
            }

            foreach (var point in points)
            {
                if (point.X >= 0 && point.Y >= 0 && point.X < BitmapSize && point.Y < BitmapSize)
                {
                    Bitmap[(int)point.X][(int)point.Y] = 1;
                }
            }
        }

        List<int> SetDigitData()
        {
            var Data = new List<int>();
            for (int x = 0; x < BitmapSize; x += SquareSize)
            {
                for (int y = 0; y < BitmapSize; y += SquareSize)
                {
                    int PixelSum = 0;
                    for (int i = 0; i < SquareSize; i++)
                    {
                        for (int j = 0; j < SquareSize; j++)
                        {
                            if (Bitmap[y + i][x + j] == 1)
                            {
                                PixelSum++;
                            }
                        }
                    }
                    Data.Add(PixelSum);
                }
            }
            return Data;
        }


        int[] CanvasToArray()
        {
            CreateDataArray();
            var points = GetPointsFromCanvas();
            FillBitmap(points);
            var data = SetDigitData();
            return data.ToArray();

        }

        private void btnClearCanvas_Click(object sender, RoutedEventArgs e)
        {
            Drawer.Strokes.Clear();
            tbResult.Text = string.Empty;
        }

        private void Drawer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Right)
            {
                Drawer.Strokes.Clear();
                tbResult.Text = string.Empty;
            }
            else if(e.ChangedButton == MouseButton.Left)
            {
                var sample = CanvasToArray();
                var knn = new kNN(Data, 10);
                tbResult.Text = Math.Round(knn.Evaluate(sample)).ToString();
            }
        }

        private void btnWidthUp_Click(object sender, RoutedEventArgs e)
        {
            Drawer.DefaultDrawingAttributes.Width += 1;
            Drawer.DefaultDrawingAttributes.Height += 1;
            brushWidth.Content = $"Brush weight: {Drawer.DefaultDrawingAttributes.Width.ToString()}";
        }

        private void btnWidthDown_Click(object sender, RoutedEventArgs e)
        {
            if (Drawer.DefaultDrawingAttributes.Width <= 1) return;
            Drawer.DefaultDrawingAttributes.Width -= 1;
            Drawer.DefaultDrawingAttributes.Height -= 1;
            brushWidth.Content = $"Brush weight: {Drawer.DefaultDrawingAttributes.Width.ToString()}";

        }
    }
}
