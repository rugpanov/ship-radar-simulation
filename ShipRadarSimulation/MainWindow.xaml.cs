using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Input;
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
        private const int radialLinesCount = 12;
        private Ellipse[] myEllipses;
        private TextBlock[] degreeLable;
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
                T_Tick, Dispatcher.CurrentDispatcher)
            { IsEnabled = true };
            start = DateTime.Now;
            MyCanvas.SizeChanged += CanvasSizeChanged;

            myTargetShip = new Ship(0, 0, 0, 0);
            myShip = new Ship(0, 0, 0, 0);
            InitLines();
            InitEllipses(8);
            InitDegreeLables();
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
            myLines = new Line[radialLinesCount];
            for (var i = 0; i < radialLinesCount; i++)
            {
                var line = new Line
                {
                    Stroke = Brushes.Green,
                    StrokeThickness = 1
                };
                MyCanvas.Children.Add(line);
                myLines[i] = line;
            }
            return;
        }

        //create textblock mas with degree labels
        private void InitDegreeLables()
        {
            var degree = 90;
            degreeLable = new TextBlock[radialLinesCount * 2];
            for (var i = 0; i < degreeLable.Length; i++, degree += 15)
            {
                degreeLable[i] = new TextBlock();
                degreeLable[i].Text = degree.ToString() + "°";
                degreeLable[i].Foreground = new SolidColorBrush(Colors.Green);
                MyCanvas.Children.Add(degreeLable[i]);
                if (degree == 345)
                    degree = -15;
            }
            return;
        }

        //draw radial lines with 15 degree step
        public void DrawRaialLines(double ellipse200Rad)
        {
            var degree = 0.0;
            var labelRad = ellipse200Rad + 1;
            for (var i = 0; i < myLines.Length; i++, degree += 15)
            {
                degree = Utils.DegreeToRadian(degree);
                myLines[i].X1 = DrawIndent + ellipse200Rad - ellipse200Rad * Math.Cos(degree);
                myLines[i].X2 = DrawIndent + ellipse200Rad + ellipse200Rad * Math.Cos(degree);
                myLines[i].Y1 = DrawIndent + ellipse200Rad - ellipse200Rad * Math.Sin(degree);
                myLines[i].Y2 = DrawIndent + ellipse200Rad + ellipse200Rad * Math.Sin(degree);

                if (Math.Cos(degree) >= 0)
                {
                    if (i == 0)
                    {
                        Canvas.SetLeft(degreeLable[i], DrawIndent + labelRad + labelRad * Math.Cos(degree) + 3);
                        Canvas.SetTop(degreeLable[i], DrawIndent + labelRad + labelRad * Math.Sin(degree) - 8);
                        Canvas.SetLeft(degreeLable[i + myLines.Length], DrawIndent + labelRad - labelRad * Math.Cos(degree) - 25);
                        Canvas.SetTop(degreeLable[i + myLines.Length], DrawIndent + labelRad - labelRad * Math.Sin(degree) - 7);
                    }
                    else
                    {
                        Canvas.SetLeft(degreeLable[i], DrawIndent + labelRad + labelRad * Math.Cos(degree));
                        Canvas.SetTop(degreeLable[i], DrawIndent + labelRad + labelRad * Math.Sin(degree));
                        Canvas.SetLeft(degreeLable[i + myLines.Length], DrawIndent + labelRad - labelRad * Math.Cos(degree) - 25);
                        Canvas.SetTop(degreeLable[i + myLines.Length], DrawIndent + labelRad - labelRad * Math.Sin(degree) - 14);
                    }
                }
                else
                {
                    if (i == 11)
                    {
                        Canvas.SetLeft(degreeLable[i], DrawIndent + labelRad + labelRad * Math.Cos(degree) - 25);
                        Canvas.SetTop(degreeLable[i], DrawIndent + labelRad + labelRad * Math.Sin(degree) - 7);
                        Canvas.SetLeft(degreeLable[i + myLines.Length], DrawIndent + labelRad - labelRad * Math.Cos(degree) + 3);
                        Canvas.SetTop(degreeLable[i + myLines.Length], DrawIndent + labelRad - labelRad * Math.Sin(degree) - 10);
                    }
                    else if (i == 6)
                    {
                        Canvas.SetLeft(degreeLable[i], DrawIndent + labelRad + labelRad * Math.Cos(degree) - 12);
                        Canvas.SetTop(degreeLable[i], DrawIndent + labelRad + labelRad * Math.Sin(degree));
                        Canvas.SetLeft(degreeLable[i + myLines.Length], DrawIndent + labelRad - labelRad * Math.Cos(degree) - 5);
                        Canvas.SetTop(degreeLable[i + myLines.Length], DrawIndent + labelRad - labelRad * Math.Sin(degree) - 17);
                    }
                    else
                    {
                        Canvas.SetLeft(degreeLable[i], DrawIndent + labelRad + labelRad * Math.Cos(degree) - 20);
                        Canvas.SetTop(degreeLable[i], DrawIndent + labelRad + labelRad * Math.Sin(degree) + 1);
                        Canvas.SetLeft(degreeLable[i + myLines.Length], DrawIndent + labelRad - labelRad * Math.Cos(degree));
                        Canvas.SetTop(degreeLable[i + myLines.Length], DrawIndent + labelRad - labelRad * Math.Sin(degree) - 17);
                    }
                }
                degree = Utils.RadianToDegree(degree);
            }
            return;
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


            DrawRaialLines(ellipse200Rad);

        }


        private void OnClickRequestChangeParameters(object sender, RoutedEventArgs e)
        {

            Color defaultColour = (Color)ColorConverter.ConvertFromString("#FFA0A0A0");
            if (myShip == null) return;
            SolidColorBrush brush;
            /*var speedInKnot = double.Parse(OurSpeedInKnot.Text);
            var theCourse = double.Parse(OurCourseInGrad.Text);
            myShip = new Ship(myShip.GetX(), myShip.GetY(), speedInKnot / 360, theCourse);*/
            var convertOurSpeed = double.TryParse(OurSpeedInKnot.Text, out var speedInKnot);
            var convertOurCourse = double.TryParse(OurCourseInGrad.Text, out var theCourse);
            if (!convertOurSpeed || !convertOurCourse)
            {
                brush = new SolidColorBrush(defaultColour + (Colors.Red - defaultColour) * Convert.ToUInt32(!convertOurSpeed));
                OurSpeedInKnot.BorderBrush = brush;
                brush = new SolidColorBrush(defaultColour + (Colors.Red - defaultColour) * Convert.ToUInt32(!convertOurCourse));
                OurCourseInGrad.BorderBrush = brush;
                return;
            }    
             OurSpeedInKnot.BorderBrush = SystemColors.ControlDarkBrush;
             OurCourseInGrad.BorderBrush = SystemColors.ControlDarkBrush;
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
            Color defaultColour = (Color)ColorConverter.ConvertFromString("#FFA0A0A0");
            SolidColorBrush brush;
            var convertTargetBearing = double.TryParse(TargetBearingInGrad.Text, out var targetBearing);
            var convertTargetDistance = double.TryParse(TargetDistanceKb.Text, out var targetDistance);
            var convertTargetSpeedInKnot = double.TryParse(TargetSpeedInKnot.Text, out var targetSpeedInKnot);
            var convertTargetCourseInGrad = double.TryParse(TargetCourseInGrad.Text, out var targetCourseInGrad);
            var convertOurSpeed = double.TryParse(OurSpeedInKnot.Text, out var speedInKnot);
            var convertOurCourse = double.TryParse(OurCourseInGrad.Text, out var theCourse);
            if (!convertTargetBearing || !convertTargetDistance || !convertTargetSpeedInKnot || !convertTargetCourseInGrad || !convertOurSpeed || !convertOurCourse)
            {
                brush = new SolidColorBrush(defaultColour + (Colors.Red - defaultColour) * Convert.ToUInt32(!convertTargetBearing));
                TargetBearingInGrad.BorderBrush = brush;
                brush = new SolidColorBrush(defaultColour + (Colors.Red - defaultColour) * Convert.ToUInt32(!convertTargetDistance));
                TargetDistanceKb.BorderBrush = brush;
                brush = new SolidColorBrush(defaultColour + (Colors.Red - defaultColour) * Convert.ToUInt32(!convertTargetSpeedInKnot));
                TargetSpeedInKnot.BorderBrush = brush;
                brush = new SolidColorBrush(defaultColour + (Colors.Red - defaultColour) * Convert.ToUInt32(!convertTargetCourseInGrad));
                TargetCourseInGrad.BorderBrush = brush;
                brush = new SolidColorBrush(defaultColour + (Colors.Red - defaultColour) * Convert.ToUInt32(!convertOurSpeed));
                OurSpeedInKnot.BorderBrush = brush;
                brush = new SolidColorBrush(defaultColour + (Colors.Red - defaultColour) * Convert.ToUInt32(!convertOurCourse));
                OurCourseInGrad.BorderBrush = brush;           
                return false;
            }
            TargetBearingInGrad.BorderBrush = SystemColors.ControlDarkBrush;
            TargetDistanceKb.BorderBrush = SystemColors.ControlDarkBrush;
            TargetSpeedInKnot.BorderBrush = SystemColors.ControlDarkBrush;
            TargetCourseInGrad.BorderBrush = SystemColors.ControlDarkBrush;
            OurSpeedInKnot.BorderBrush = SystemColors.ControlDarkBrush;
            OurCourseInGrad.BorderBrush = SystemColors.ControlDarkBrush;
            var targetX = targetDistance * Math.Sin(Utils.DegreeToRadian(targetBearing));
            var targetY = targetDistance * Math.Cos(Utils.DegreeToRadian(targetBearing));
            myTargetShip = new Ship(targetX, targetY, targetSpeedInKnot / 360, targetCourseInGrad);                 
            myShip = new Ship(0, 0, double.Parse(OurSpeedInKnot.Text) / 360, double.Parse(OurCourseInGrad.Text));
            Redraw();
            return true;
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
            if (e.Key == Key.Space)
            {
                if (myDataContext.ShowStartButton)
                {
                    StartSimulation.Focus();
                    StartSimulation.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, StartSimulation));
                }
                else if (myDataContext.ShowPauseSimulation)
                {
                    PauseSimulation.Focus();
                    PauseSimulation.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, PauseSimulation));
                }
                else
                {
                    ResumeSimulation.Focus();
                    ResumeSimulation.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, ResumeSimulation));
                }
            }
        }
        private void TargetDistanceKb_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TargetBearingInGrad_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void OurSpeedInKnot_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }

}
    
