using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
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
     */
    public partial class MainWindow
    {
        private const int DrawIndent = 50;
        private const int DotSize = 4;
        private const int radialLinesCount = 12;
        private Ellipse[] myEllipses;
        private TextBlock[] degreeLable;
        private Line[] myLines;
        private TextBox[] ourFields;
        private TextBox[] allFields;
        private readonly SimulationViewModel myDataContext;

        private Ship myTargetShip;
        private Ship myShip;
        private readonly Timer myShownTimer;
        private int myOldTimeInSec;
        private const double DefaultTargetDistance = 100;
        private const double DefaultTargetBearing = 0;

        public MainWindow()
        {
            InitializeComponent();
            SizeToContent = SizeToContent.WidthAndHeight;
            myDataContext = new SimulationViewModel();
            DataContext = myDataContext;
            myDataContext.TargetDistance = DefaultTargetDistance;
            myDataContext.TargetBearing = DefaultTargetBearing;
            MyCanvas.SizeChanged += CanvasSizeChanged;

            myTargetShip = new Ship(0, 0, 0, 0);
            myShip = new Ship(0, 0, 0, 0);
            InitLines();
            InitEllipses(8);
            InitDegreeLables();
            InitTexboxes();

            // ReSharper disable once ObjectCreationAsStatement
            new DispatcherTimer(
                    new TimeSpan(0, 0, 0, 0, 100),
                    DispatcherPriority.Background,
                    TimerTick,
                    Dispatcher.CurrentDispatcher)
                {IsEnabled = true};

            myShownTimer = new Timer();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            myDataContext.TimerTimeInMs = myShownTimer.GetTimePassedInMs();
            var  timeInSec = (int) myDataContext.TimerTimeInMs / 1000;
            if (myOldTimeInSec == timeInSec) return;
            
            myOldTimeInSec = timeInSec;
            myShip.ProcessOneSecond();
            myTargetShip.ProcessOneSecond();
            myDataContext.TargetDistance = myShip.MeasureDistance(myTargetShip);
            myDataContext.TargetBearing = myShip.MeasureBearing(myTargetShip);
            myDataContext.MyCourseInGrad = myShip.GetCourseInGrad();
            myDataContext.MySpeedInKnot = myShip.GetSpeedInKbS() * 360;
            Redraw();
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
        }

        private void InitDegreeLables()
        {
            var degree = 90;
            degreeLable = new TextBlock[radialLinesCount * 2];
            for (var i = 0; i < degreeLable.Length; i++, degree += 15)
            {
                degreeLable[i] = new TextBlock
                {
                    Text = degree + "°",
                    Foreground = new SolidColorBrush(Colors.Green)
                };
                MyCanvas.Children.Add(degreeLable[i]);
                if (degree == 345)
                    degree = -15;
            }
        }

        private void DrawRadialLines15DegreeStep(double ellipse200Rad)
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
                        Canvas.SetLeft(degreeLable[i + myLines.Length],
                            DrawIndent + labelRad - labelRad * Math.Cos(degree) - 25);
                        Canvas.SetTop(degreeLable[i + myLines.Length],
                            DrawIndent + labelRad - labelRad * Math.Sin(degree) - 7);
                    }
                    else
                    {
                        Canvas.SetLeft(degreeLable[i], DrawIndent + labelRad + labelRad * Math.Cos(degree));
                        Canvas.SetTop(degreeLable[i], DrawIndent + labelRad + labelRad * Math.Sin(degree));
                        Canvas.SetLeft(degreeLable[i + myLines.Length],
                            DrawIndent + labelRad - labelRad * Math.Cos(degree) - 25);
                        Canvas.SetTop(degreeLable[i + myLines.Length],
                            DrawIndent + labelRad - labelRad * Math.Sin(degree) - 14);
                    }
                }
                else
                {
                    if (i == 11)
                    {
                        Canvas.SetLeft(degreeLable[i], DrawIndent + labelRad + labelRad * Math.Cos(degree) - 25);
                        Canvas.SetTop(degreeLable[i], DrawIndent + labelRad + labelRad * Math.Sin(degree) - 7);
                        Canvas.SetLeft(degreeLable[i + myLines.Length],
                            DrawIndent + labelRad - labelRad * Math.Cos(degree) + 3);
                        Canvas.SetTop(degreeLable[i + myLines.Length],
                            DrawIndent + labelRad - labelRad * Math.Sin(degree) - 10);
                    }
                    else if (i == 6)
                    {
                        Canvas.SetLeft(degreeLable[i], DrawIndent + labelRad + labelRad * Math.Cos(degree) - 12);
                        Canvas.SetTop(degreeLable[i], DrawIndent + labelRad + labelRad * Math.Sin(degree));
                        Canvas.SetLeft(degreeLable[i + myLines.Length],
                            DrawIndent + labelRad - labelRad * Math.Cos(degree) - 5);
                        Canvas.SetTop(degreeLable[i + myLines.Length],
                            DrawIndent + labelRad - labelRad * Math.Sin(degree) - 17);
                    }
                    else
                    {
                        Canvas.SetLeft(degreeLable[i], DrawIndent + labelRad + labelRad * Math.Cos(degree) - 20);
                        Canvas.SetTop(degreeLable[i], DrawIndent + labelRad + labelRad * Math.Sin(degree) + 1);
                        Canvas.SetLeft(degreeLable[i + myLines.Length],
                            DrawIndent + labelRad - labelRad * Math.Cos(degree));
                        Canvas.SetTop(degreeLable[i + myLines.Length],
                            DrawIndent + labelRad - labelRad * Math.Sin(degree) - 17);
                    }
                }

                degree = Utils.RadianToDegree(degree);
            }
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


            DrawRadialLines15DegreeStep(ellipse200Rad);
        }

        private void InitTexboxes()
        {
            ourFields = new TextBox[2];
            allFields = new TextBox[6];
            ourFields[0] = OurCourseInGrad;
            ourFields[1] = OurSpeedInKnot; 
            allFields[0] = TargetDistanceKb;
            allFields[1] = TargetBearingInGrad;            
            allFields[2] = TargetSpeedInKnot;
            allFields[3] = TargetCourseInGrad;            
            allFields[4] = OurSpeedInKnot;
            allFields[5] = OurCourseInGrad;          
        }
        private double[] fieldsValidation(TextBox[] fields, double[] movementParams, out bool isValid)
        {           
            isValid = true;
            Regex regExp = new Regex(@"^[0-9]{1,3}((\.|,)[0-9]{1,4})?$");
            for (var i = 0; i < fields.Length; i++)
            {
                if (!regExp.IsMatch(fields[i].Text))
                {                   
                    fields[i].BorderBrush = new SolidColorBrush(Colors.Red);
                    isValid = false;
                }
                else
                {
                    double.TryParse(fields[i].Text.Replace(',', '.'), out movementParams[i]);
                    fields[i].BorderBrush = SystemColors.ControlDarkBrush;                                  
                }
            }
            return movementParams;
        }

        private void OnClickRequestChangeParameters(object sender, RoutedEventArgs e)
        {
            if (myShip == null) return;      
            double[] movementParams = new double[ourFields.Length];;
            movementParams = fieldsValidation(ourFields, movementParams, out var isValid);
            if (isValid)
            {                
                myShip.AddOrder(new Order(movementParams[0],  movementParams[1] / 360));
            }
            
        }

        private void OnClickStartSimulationButton(object sender, RoutedEventArgs e)
        {
            var exitStatus = InitBattleField();
            if (!exitStatus) return;
            myDataContext.ShowStartButton = false;
            myDataContext.ShowPauseSimulation = true;
            myDataContext.ShowStopButton = true;

            myShownTimer.Start();
        }

        private bool InitBattleField()
        {
            double[] movementParams = new double[allFields.Length];;
            movementParams = fieldsValidation(allFields, movementParams, out var isValid);
            if (isValid)
            {
                var targetX = movementParams[0] * Math.Sin(Utils.DegreeToRadian(movementParams[1]));
                var targetY = movementParams[0] * Math.Cos(Utils.DegreeToRadian(movementParams[1]));
                myTargetShip = new Ship(targetX, targetY, movementParams[2] / 360, movementParams[3]);
                myShip = new Ship(0, 0, movementParams[4] / 360, movementParams[5]);
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

            myShip = new Ship(0, 0, 0, 0);
            myTargetShip = new Ship(0, 0, 0, 0);
            myDataContext.TargetDistance = DefaultTargetDistance;
            myDataContext.TargetBearing = DefaultTargetBearing;
            Redraw();
            
            myShownTimer.Reset();
            myDataContext.TimerTimeInMs = 0;
        }

        private void OnClickPouseSimulationButton(object sender, RoutedEventArgs e)
        {
            myDataContext.ShowResumeSimulation = true;
            myDataContext.ShowPauseSimulation = false;
            
            myShownTimer.Stop();
        }

        private void OnClickUpPouseSimulationButton(object sender, RoutedEventArgs e)
        {
            myDataContext.ShowResumeSimulation = false;
            myDataContext.ShowPauseSimulation = true;
            
            myShownTimer.Start();
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