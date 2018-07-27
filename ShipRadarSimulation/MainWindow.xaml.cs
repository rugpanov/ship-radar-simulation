using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using ShipRadarSimulation.bll;
using ShipRadarSimulation.Entities;

namespace ShipRadarSimulation
{
    /*
     * Одна морская миля = 10 Кабельтовых
     * 1 узел = 1 морская миля/час = 10 Кб/час = 10/3600 Кб/c = 1/360 Кб/c
     * 
     */
    public partial class MainWindow
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
                var ellipse = new Ellipse
                {
                    Stroke = Brushes.Green,
                    StrokeThickness = i % 2 == 0 ? 1 : 2
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
                Stroke = Brushes.Green,
                StrokeThickness = 1
            };
            MyCanvas.Children.Add(line);
            myLines[0] = line;

            var line2 = new Line
            {
                Stroke = Brushes.Green,
                StrokeThickness = 1
            };
            MyCanvas.Children.Add(line2);
            myLines[3] = line2;
        }

        private void T_Tick(object sender, EventArgs e)
        {
//            TimerDisplay.Text = Convert.ToString(DateTime.Now - start);
        }


        private void CanvasSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Redraw();
        }

        private void Redraw()
        {
            var canvasSize = MyCanvas.RenderSize;
            var minSize = Math.Min(canvasSize.Height, canvasSize.Width);
            var ellipse200Rad = (minSize - 2 * DrawIndent) / 2;
            var scale = ellipse200Rad / 200;

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
            Panel.SetZIndex(EllipseCenter, 240);


            if (myTargetShip != null && myShip != null)
            {
                var movementY = myTargetShip.GetY() - myShip.GetY();
                var movementX = myTargetShip.GetX() - myShip.GetX();

                Canvas.SetTop(EllipseEnemy, DrawIndent + ellipse200Rad - scale * movementY - 2);
                Canvas.SetLeft(EllipseEnemy, DrawIndent + ellipse200Rad + scale * movementX - 2);
                EllipseEnemy.Width = DotSize;
                EllipseEnemy.Height = DotSize;
                Panel.SetZIndex(EllipseEnemy, 239);
            }

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

        private Ship myTargetShip;
        private Ship myShip;
        private DispatcherTimer myDispatcherTimer;

        private void OnClickRequestChangeParameters(object sender, RoutedEventArgs e)
        {
            if (myShip == null) return;

            var speedInKnot = double.Parse(OurSpeedInKnot.Text);
            var theCourse = double.Parse(OurCourseInGrad.Text);
            myShip = new Ship(myShip.GetX(), myShip.GetY(), speedInKnot / 360, theCourse);
        }

        private void OnClickStartSimulationButton(object sender, RoutedEventArgs e)
        {
            myDataContext.ShowStartButton = false;
            myDataContext.ShowPauseSimulation = true;
            myDataContext.ShowStopButton = true;
            InitBattleField();
            myDispatcherTimer?.Stop();
            myDispatcherTimer = new DispatcherTimer();
            myDispatcherTimer.Tick += DispatcherTimerTick;
            myDispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            myDispatcherTimer.Start();
        }

        private void DispatcherTimerTick(object sender, EventArgs e)
        {
            myShip.ProcessOneSecond();
            myTargetShip.ProcessOneSecond();
            Redraw();
        }

        private void InitBattleField()
        {
            var targetBearing = double.Parse(TargetBearingInGrad.Text);
            var targetDistance = double.Parse(TargetDistanceKb.Text);
            var targetX = targetDistance * Math.Sin(Utils.DegreeToRadian(targetBearing));
            var targetY = targetDistance * Math.Cos(Utils.DegreeToRadian(targetBearing));
            var targetSpeedInKnot = double.Parse(TargetSpeedInKnot.Text);
            var targetCourseInGrad = double.Parse(TargetCourseInGrad.Text);
            myTargetShip = new Ship(targetX, targetY, targetSpeedInKnot / 360, targetCourseInGrad);
            myShip = new Ship(0, 0, double.Parse(OurSpeedInKnot.Text) / 360, double.Parse(OurCourseInGrad.Text));
            Redraw();
        }
        
        private void OnClickResetSimulationButton(object sender, RoutedEventArgs e)
        {
            myDataContext.ShowStartButton = true;
            myDataContext.ShowResumeSimulation = false;
            myDataContext.ShowPauseSimulation = false;
            myDataContext.ShowStopButton = false;

            myDispatcherTimer?.Stop();
            myShip = new Ship(0, 0, 0, 0);
            myTargetShip = new Ship(0, 0, 0, 0);
            Redraw();
        }

        private void OnClickPouseSimulationButton(object sender, RoutedEventArgs e)
        {
            myDataContext.ShowResumeSimulation = true;
            myDataContext.ShowPauseSimulation = false;
            myDispatcherTimer?.Stop();
        }

        private void OnClickUpPouseSimulationButton(object sender, RoutedEventArgs e)
        {
            myDataContext.ShowResumeSimulation = false;
            myDataContext.ShowPauseSimulation = true;
            myDispatcherTimer?.Start();
        }
    }
}