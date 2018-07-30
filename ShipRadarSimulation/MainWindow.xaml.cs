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

        private Ship myTargetShip;
        private Ship myShip;
        private DispatcherTimer myDispatcherTimer;
        private const double DefaultTargetDistance = 50;
        private const double DefaultTargetBearing = 60;

        public MainWindow()
        {
            InitializeComponent();
            SizeToContent = SizeToContent.WidthAndHeight;
            myDataContext = new SimulationViewModel();
            DataContext = myDataContext;
            myDataContext.TargetDistance = DefaultTargetDistance;
            myDataContext.TargetBearing = DefaultTargetBearing;

            t = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 50), DispatcherPriority.Background,
                T_Tick, Dispatcher.CurrentDispatcher) {IsEnabled = true};
            start = DateTime.Now;
            MyCanvas.SizeChanged += CanvasSizeChanged;

            myTargetShip = new Ship(0, 0, 0, 0);
            myShip = new Ship(0, 0, 0, 0);
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


            var movementY = myTargetShip.GetY() - myShip.GetY();
            var movementX = myTargetShip.GetX() - myShip.GetX();

            Canvas.SetTop(EllipseEnemy, DrawIndent + ellipse200Rad - scale * movementY - 2);
            Canvas.SetLeft(EllipseEnemy, DrawIndent + ellipse200Rad + scale * movementX - 2);
            EllipseEnemy.Width = DotSize;
            EllipseEnemy.Height = DotSize;
            Panel.SetZIndex(EllipseEnemy, 239);

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

        private void OnClickRequestChangeParameters(object sender, RoutedEventArgs e)
        {
            if (myShip == null) return;
            var isValid = true;
            
            if (!double.TryParse(OurSpeedInKnot.Text, out var speedInKnot))
            {
                OurSpeedInKnot.BorderBrush = new SolidColorBrush(Colors.Red);
                isValid = false;
            }
            else
            {
                OurSpeedInKnot.BorderBrush = SystemColors.ControlDarkBrush;
            }
            if (!double.TryParse(OurCourseInGrad.Text, out var theCourse))
            {
                OurCourseInGrad.BorderBrush = new SolidColorBrush(Colors.Red);
                isValid = false;
            }
            else
            {
                OurCourseInGrad.BorderBrush = SystemColors.ControlDarkBrush;

            var speedInKnot = double.Parse(OurSpeedInKnot.Text);
            var theCourse = double.Parse(OurCourseInGrad.Text);
            myShip = new Ship(myShip.GetX(), myShip.GetY(), speedInKnot / 360, theCourse);
        }

        private void OnClickStartSimulationButton(object sender, RoutedEventArgs e)
        {
            var exitStatus = InitBattleField();
            if (!exitStatus) return;
            myDataContext.ShowStartButton = false;
            myDataContext.ShowPauseSimulation = true;
            myDataContext.ShowStopButton = true;
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
            myDataContext.TargetDistance = myShip.MeasureDistance(myTargetShip);
            myDataContext.TargetBearing = myShip.MeasureBearing(myTargetShip);
            Redraw();
        }

        private bool InitBattleField()
        {
            var isValid = true;
            if (!double.TryParse(TargetBearingInGrad.Text, out var targetBearing))
            {
                TargetBearingInGrad.BorderBrush = new SolidColorBrush(Colors.Red);
                isValid = false;
            }
            else
            {
                TargetBearingInGrad.BorderBrush = SystemColors.ControlDarkBrush;
            }
            
            if (!double.TryParse(TargetDistanceKb.Text, out var targetDistance))
            {
                TargetDistanceKb.BorderBrush = new SolidColorBrush(Colors.Red);
                isValid = false;
            }
            else
            {
                TargetDistanceKb.BorderBrush = SystemColors.ControlDarkBrush;
            }
            
            if (!double.TryParse(TargetSpeedInKnot.Text, out var targetSpeedInKnot))
            {
                TargetSpeedInKnot.BorderBrush = new SolidColorBrush(Colors.Red);
                isValid = false;
            }
            else
            {
                TargetSpeedInKnot.BorderBrush = SystemColors.ControlDarkBrush;
            }

            if (!double.TryParse(TargetCourseInGrad.Text, out var targetCourseInGrad))
            {
                TargetCourseInGrad.BorderBrush = new SolidColorBrush(Colors.Red);
                isValid = false;
            }
            else
            {
                TargetCourseInGrad.BorderBrush = SystemColors.ControlDarkBrush;
            }
            
            if (!double.TryParse(OurSpeedInKnot.Text, out var speedInKnot))
            {
                OurSpeedInKnot.BorderBrush = new SolidColorBrush(Colors.Red);
                isValid = false;
            }
            else
            {
                OurSpeedInKnot.BorderBrush = SystemColors.ControlDarkBrush;
            }
            
            if (!double.TryParse(OurCourseInGrad.Text, out var theCourse))
            {
                OurCourseInGrad.BorderBrush = new SolidColorBrush(Colors.Red);
                isValid = false;
            }
            else
            {
                OurCourseInGrad.BorderBrush = SystemColors.ControlDarkBrush;
            }

            if (isValid)
            {
                var targetX = targetDistance * Math.Sin(Utils.DegreeToRadian(targetBearing));
                var targetY = targetDistance * Math.Cos(Utils.DegreeToRadian(targetBearing));
                myTargetShip = new Ship(targetX, targetY, targetSpeedInKnot / 360, targetCourseInGrad);
                myShip = new Ship(0, 0, speedInKnot / 360, theCourse);
                Redraw();
            }
            
            return isValid;
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
            myDataContext.TargetDistance = DefaultTargetDistance;
            myDataContext.TargetBearing = DefaultTargetBearing;
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

        private void OnClickExit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SpaceIsPressed(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Space) return;
            if (myDataContext.ShowStartButton)
            {
                OnClickStartSimulationButton(null, null);
            }
            else if (myDataContext.ShowPauseSimulation)
            {
                OnClickPouseSimulationButton(null, null);
            }
            else
            {
                OnClickUpPouseSimulationButton(null, null);
            }
        }
    }
}