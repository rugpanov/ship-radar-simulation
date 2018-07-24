using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using ShipRadarSimulation.bll;

namespace ShipRadarSimulation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer t;
        DateTime start;
        private const int DrawIndent = 50;
        private const int DotSize = 4;
        private Ellipse[] myEllipses;
        private Line[] myLines;
        private readonly SimulationViewModel myDataContext;

        public MainWindow()
        {
            InitializeComponent();
            SizeToContent = SizeToContent.WidthAndHeight;
            myDataContext = new SimulationViewModel();
            DataContext = myDataContext;

            t = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 50), DispatcherPriority.Background,
                T_Tick, Dispatcher.CurrentDispatcher) {IsEnabled = true};
            start = DateTime.Now;
            MyCanvas.SizeChanged += CanvasSizeChanged;

            InitLines();
            InitEllipses(8);
        }

        private void InitEllipses(int num)
        {
            myEllipses = new Ellipse[num];
            for (var i = 0; i < num; i++)
            {
                var ellipse = i != num - 1
                    ? new Ellipse
                    {
                        Stroke = Brushes.Gray,
                        StrokeThickness = 2
                    }
                    : new Ellipse
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 3
                    };

                MyCanvas.Children.Add(ellipse);
                myEllipses[i] = ellipse;
            }
        }

        private void InitLines()
        {
            myLines = new Line[4];
            var line = new Line
            {
                Stroke = Brushes.Gray,
                StrokeThickness = 1
            };
            MyCanvas.Children.Add(line);
            myLines[0] = line;

            var line2 = new Line
            {
                Stroke = Brushes.Gray,
                StrokeThickness = 1
            };
            MyCanvas.Children.Add(line2);
            myLines[3] = line2;
        }

        private void T_Tick(object sender, EventArgs e)
        {
            //TimerDisplay.Text = Convert.ToString(DateTime.Now - start);
        }


        private void CanvasSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var canvasSize = e.NewSize;
            var minSize = Math.Min(canvasSize.Height, canvasSize.Width);
            var ellipse200Rad = (minSize - 2 * DrawIndent) / 2;

            for (var i = 0; i < 8; i++)
            {
                var ellipse = myEllipses[i];
                Canvas.SetTop(ellipse, DrawIndent + ellipse200Rad * (8 - (i + 1)) / 8);
                Canvas.SetLeft(ellipse, DrawIndent + ellipse200Rad * (8 - (i + 1)) / 8);
                ellipse.Width = ellipse200Rad * (i + 1) / 4;
                ellipse.Height = ellipse200Rad * (i + 1) / 4;
            }

            Canvas.SetTop(EllipseCenter, minSize / 2 - 2);
            Canvas.SetLeft(EllipseCenter, minSize / 2 - 2);
            EllipseCenter.Width = DotSize;
            EllipseCenter.Height = DotSize;

            Canvas.SetTop(EllipseCenter, minSize / 2 - 2);
            Canvas.SetLeft(EllipseCenter, minSize / 2 - 2);
            EllipseCenter.Width = DotSize;
            EllipseCenter.Height = DotSize;

            var line = myLines[0];
            line.X1 = DrawIndent;
            line.X2 = DrawIndent + 2 * ellipse200Rad;
            line.Y1 = DrawIndent + ellipse200Rad;
            line.Y2 = DrawIndent + ellipse200Rad;

            var line2 = myLines[3];
            line2.X1 = DrawIndent + ellipse200Rad;
            line2.X2 = DrawIndent + ellipse200Rad;
            line2.Y1 = DrawIndent;
            line2.Y2 = DrawIndent + ellipse200Rad * 2;
        }

        private void OnClickStartSimulationButton(object sender, RoutedEventArgs e)
        {
            myDataContext.Position += 200;
        }
    }
}