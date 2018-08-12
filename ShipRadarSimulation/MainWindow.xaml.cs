using System;
using System.Globalization;
using System.IO;
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
        private ParamSnapshot MyPreviousSnapshot;
        private const int DrawIndent = 50;
        private const int DotSize = 4;
        private const int RadialLinesCount = 12;
        private Ellipse[] myEllipses;
        private TextBlock[] myDegreeLables;
        private Line[] myLines;
        private Line myCourceLine;
        private readonly SimulationViewModel myDataContext;

        private Ship myTargetShip;
        private Ship myShip;
        private readonly Timer myShownTimer;
        private int myOldTimeInSec;
        private Line myTargetCourceLine;
        private Line myTargetCourceArrow1Line;
        private Line myTargetCourceArrow2Line;
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

            InitLines();
            InitEllipses(8);
            InitDegreeLables();
            InitTargetArrowLines();
            // ReSharper disable once ObjectCreationAsStatement
            new DispatcherTimer(
                    new TimeSpan(0, 0, 0, 0, 100),
                    DispatcherPriority.Background,
                    TimerTick,
                    Dispatcher.CurrentDispatcher)
                {IsEnabled = true};

            myShownTimer = new Timer();
        }

        private void InitLines()
        {
            myLines = new Line[RadialLinesCount];
            for (var i = 0; i < RadialLinesCount; i++)
            {
                var line = new Line
                {
                    Stroke = Brushes.Green,
                    StrokeThickness = 1
                };
                MyCanvas.Children.Add(line);
                myLines[i] = line;
            }

            myCourceLine = new Line
            {
                Stroke = Brushes.Tomato,
                StrokeThickness = 1
            };
            MyCanvas.Children.Add(myCourceLine);
            Panel.SetZIndex(myCourceLine, 100);
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

        private void InitDegreeLables()
        {
            var degree = 90;
            myDegreeLables = new TextBlock[RadialLinesCount * 2];
            for (var i = 0; i < myDegreeLables.Length; i++, degree += 15)
            {
                myDegreeLables[i] = new TextBlock
                {
                    Text = degree + "°",
                    Foreground = new SolidColorBrush(Colors.Green)
                };
                MyCanvas.Children.Add(myDegreeLables[i]);
                if (degree == 345)
                    degree = -15;
            }
        }

        private void InitTargetArrowLines()
        {
            myTargetCourceLine = new Line
            {
                Stroke = Brushes.Red,
                StrokeThickness = 1,
                Opacity = 0.75
            };
            MyCanvas.Children.Add(myTargetCourceLine);
            Panel.SetZIndex(myTargetCourceLine, 101);

            myTargetCourceArrow1Line = new Line
            {
                Stroke = Brushes.Red,
                StrokeThickness = 1,
                Opacity = 0.75
            };
            MyCanvas.Children.Add(myTargetCourceArrow1Line);
            Panel.SetZIndex(myTargetCourceArrow1Line, 101);

            myTargetCourceArrow2Line = new Line
            {
                Stroke = Brushes.Red,
                StrokeThickness = 1,
                Opacity = 0.75
            };
            MyCanvas.Children.Add(myTargetCourceArrow2Line);
            Panel.SetZIndex(myTargetCourceArrow2Line, 101);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            myDataContext.TimerTimeInMs = myShownTimer.GetTimePassedInMs();
            var timeInSec = (int) myDataContext.TimerTimeInMs / 1000;
            if (myOldTimeInSec == timeInSec || myShip == null) return;

            myOldTimeInSec = timeInSec;
            myShip.ProcessOneSecond();
            myTargetShip.ProcessOneSecond();
            myDataContext.TargetDistance = myShip.MeasureDistance(myTargetShip);
            myDataContext.TargetBearing = myShip.MeasureBearing(myTargetShip);
            myDataContext.MyCourseInGrad = myShip.GetCourseInGrad();
            myDataContext.MySpeedInKnot = myShip.GetSpeedInKbS() * 360;
            Redraw();
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
            var centerXy = minSize / 2;


            for (var i = 0; i < 8; i++)
            {
                var ellipse = myEllipses[i];
                Canvas.SetTop(ellipse, DrawIndent + ellipse200Rad * (8 - (i + 1)) / 8);
                Canvas.SetLeft(ellipse, DrawIndent + ellipse200Rad * (8 - (i + 1)) / 8);
                ellipse.Width = ellipse200Rad * (i + 1) / 4;
                ellipse.Height = ellipse200Rad * (i + 1) / 4;
            }

            Canvas.SetTop(EllipseCenter, centerXy - 2);
            Canvas.SetLeft(EllipseCenter, centerXy - 2);
            EllipseCenter.Width = DotSize;
            EllipseCenter.Height = DotSize;
            Panel.SetZIndex(EllipseCenter, 240);


            if (myTargetShip != null)
            {
                var movementY = myTargetShip.GetY() - myShip.GetY();
                var movementX = myTargetShip.GetX() - myShip.GetX();
                var targetRenderedX = DrawIndent + ellipse200Rad + scale * movementX;
                var targetRenderedY = DrawIndent + ellipse200Rad - scale * movementY;

                Canvas.SetTop(EllipseEnemy, targetRenderedY - 2);
                Canvas.SetLeft(EllipseEnemy, targetRenderedX - 2);
                EllipseEnemy.Width = DotSize;
                EllipseEnemy.Height = DotSize;
                Panel.SetZIndex(EllipseEnemy, 239);

                DrawTargetArrowLine(scale, targetRenderedX, targetRenderedY);
            }

            if (myShip != null)
            {
                var courseInRadian = Utils.DegreeToRadian(myShip.GetCourseInGrad());
                myCourceLine.X1 = centerXy;
                myCourceLine.Y1 = centerXy;
                myCourceLine.X2 = DrawIndent + ellipse200Rad + ellipse200Rad * Math.Sin(courseInRadian);
                myCourceLine.Y2 = DrawIndent + ellipse200Rad - ellipse200Rad * Math.Cos(courseInRadian);
            }

            DrawRadialLines15DegreeStep(ellipse200Rad);
        }

        private void DrawTargetArrowLine(double scale, double targetRenderedX, double targetRenderedY)
        {
            var targetCourseInRadian = Utils.DegreeToRadian(myTargetShip.GetCourseInGrad());
            var lineSize = scale * Utils.FromKbsToKnot(myTargetShip.GetSpeedInKbS());
            var endOfArrowLineX = targetRenderedX + lineSize * Math.Sin(targetCourseInRadian);
            var endOfArrowLineY = targetRenderedY - lineSize * Math.Cos(targetCourseInRadian);
            myTargetCourceLine.X1 = targetRenderedX;
            myTargetCourceLine.Y1 = targetRenderedY;
            myTargetCourceLine.X2 = endOfArrowLineX;
            myTargetCourceLine.Y2 = endOfArrowLineY;

            myTargetCourceArrow1Line.X1 = endOfArrowLineX;
            myTargetCourceArrow1Line.Y1 = endOfArrowLineY;
            myTargetCourceArrow1Line.X2 = endOfArrowLineX + 10 * Math.Cos(targetCourseInRadian + 45);
            myTargetCourceArrow1Line.Y2 = endOfArrowLineY + 10 * Math.Sin(targetCourseInRadian + 45);

            myTargetCourceArrow2Line.X1 = endOfArrowLineX;
            myTargetCourceArrow2Line.Y1 = endOfArrowLineY;
            myTargetCourceArrow2Line.X2 = endOfArrowLineX - 10 * Math.Cos(targetCourseInRadian - 45);
            myTargetCourceArrow2Line.Y2 = endOfArrowLineY - 10 * Math.Sin(targetCourseInRadian - 45);
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
                        Canvas.SetLeft(myDegreeLables[i], DrawIndent + labelRad + labelRad * Math.Cos(degree) + 3);
                        Canvas.SetTop(myDegreeLables[i], DrawIndent + labelRad + labelRad * Math.Sin(degree) - 8);
                        Canvas.SetLeft(myDegreeLables[i + myLines.Length],
                            DrawIndent + labelRad - labelRad * Math.Cos(degree) - 25);
                        Canvas.SetTop(myDegreeLables[i + myLines.Length],
                            DrawIndent + labelRad - labelRad * Math.Sin(degree) - 7);
                    }
                    else
                    {
                        Canvas.SetLeft(myDegreeLables[i], DrawIndent + labelRad + labelRad * Math.Cos(degree));
                        Canvas.SetTop(myDegreeLables[i], DrawIndent + labelRad + labelRad * Math.Sin(degree));
                        Canvas.SetLeft(myDegreeLables[i + myLines.Length],
                            DrawIndent + labelRad - labelRad * Math.Cos(degree) - 25);
                        Canvas.SetTop(myDegreeLables[i + myLines.Length],
                            DrawIndent + labelRad - labelRad * Math.Sin(degree) - 14);
                    }
                }
                else
                {
                    if (i == 11)
                    {
                        Canvas.SetLeft(myDegreeLables[i], DrawIndent + labelRad + labelRad * Math.Cos(degree) - 25);
                        Canvas.SetTop(myDegreeLables[i], DrawIndent + labelRad + labelRad * Math.Sin(degree) - 7);
                        Canvas.SetLeft(myDegreeLables[i + myLines.Length],
                            DrawIndent + labelRad - labelRad * Math.Cos(degree) + 3);
                        Canvas.SetTop(myDegreeLables[i + myLines.Length],
                            DrawIndent + labelRad - labelRad * Math.Sin(degree) - 10);
                    }
                    else if (i == 6)
                    {
                        Canvas.SetLeft(myDegreeLables[i], DrawIndent + labelRad + labelRad * Math.Cos(degree) - 12);
                        Canvas.SetTop(myDegreeLables[i], DrawIndent + labelRad + labelRad * Math.Sin(degree));
                        Canvas.SetLeft(myDegreeLables[i + myLines.Length],
                            DrawIndent + labelRad - labelRad * Math.Cos(degree) - 5);
                        Canvas.SetTop(myDegreeLables[i + myLines.Length],
                            DrawIndent + labelRad - labelRad * Math.Sin(degree) - 17);
                    }
                    else
                    {
                        Canvas.SetLeft(myDegreeLables[i], DrawIndent + labelRad + labelRad * Math.Cos(degree) - 20);
                        Canvas.SetTop(myDegreeLables[i], DrawIndent + labelRad + labelRad * Math.Sin(degree) + 1);
                        Canvas.SetLeft(myDegreeLables[i + myLines.Length],
                            DrawIndent + labelRad - labelRad * Math.Cos(degree));
                        Canvas.SetTop(myDegreeLables[i + myLines.Length],
                            DrawIndent + labelRad - labelRad * Math.Sin(degree) - 17);
                    }
                }

                degree = Utils.RadianToDegree(degree);
            }
        }

        private void OnClickRequestChangeParameters(object sender, RoutedEventArgs e)
        {
            if (myShip == null) return;

            try
            {
                var speedInKnot = OurSpeedInKnot.Text != "" ? double.Parse(OurSpeedInKnot.Text, CultureInfo.InvariantCulture) : 0;
                var theCourse = OurCourseInGrad.Text != "" ? double.Parse(OurCourseInGrad.Text, CultureInfo.InvariantCulture) : 0;
                myShip.AddOrder(new Order(theCourse, speedInKnot / 360.0));
            }
            catch (Exception exception)
            {
                LogException(exception);
            }
        }

        private void OnClickStartSimulationButton(object sender, RoutedEventArgs e)
        {
            InitBattleField();

            myDataContext.ShowStartButton = false;
            myDataContext.ShowPauseSimulation = true;
            myDataContext.ShowStopButton = true;

            myShownTimer.Start();
        }

        private void InitBattleField()
        {
            MyPreviousSnapshot = new ParamSnapshot(this);
            var targetBearing = TargetBearingInGrad.Text != "" ? double.Parse(TargetBearingInGrad.Text, CultureInfo.InvariantCulture) : 0;
            var targetDistance = TargetDistanceKb.Text != "" ? double.Parse(TargetDistanceKb.Text, CultureInfo.InvariantCulture) : 0;
            var targetSpeedInKnot = TargetSpeedInKnot.Text != "" ? double.Parse(TargetSpeedInKnot.Text, CultureInfo.InvariantCulture) : 0;
            var targetCourseInGrad = TargetCourseInGrad.Text != "" ? double.Parse(TargetCourseInGrad.Text, CultureInfo.InvariantCulture) : 0;
            var speedInKnot = OurSpeedInKnot.Text != "" ? double.Parse(OurSpeedInKnot.Text, CultureInfo.InvariantCulture) : 0;
            var theCourse = OurCourseInGrad.Text != "" ? double.Parse(OurCourseInGrad.Text, CultureInfo.InvariantCulture) : 0;

            var targetX = targetDistance * Math.Sin(Utils.DegreeToRadian(targetBearing));
            var targetY = targetDistance * Math.Cos(Utils.DegreeToRadian(targetBearing));
            myTargetShip = new Ship(targetX, targetY, targetSpeedInKnot / 360, targetCourseInGrad);
            myShip = new Ship(0, 0, speedInKnot / 360, theCourse,
                myDataContext.MyAccelerationInKnotSec / 360, myDataContext.MyAngularVelocityInGradSec);
            Redraw();
        }

        private void OnClickResetSimulationButton(object sender, RoutedEventArgs e)
        {
            myDataContext.ShowStartButton = true;
            myDataContext.ShowResumeSimulation = false;
            myDataContext.ShowPauseSimulation = false;
            myDataContext.ShowStopButton = false;

            myShownTimer.Reset();
            myShip = null;
            myTargetShip = null;
            myDataContext.TimerTimeInMs = 0;
            PropogateParameters(MyPreviousSnapshot);
            Redraw();
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

        private void OnClickAbout(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow();
            aboutWindow.Show();
        }

        private void DoubleValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var textBox = (TextBox) sender;
            var regExp = new Regex(@"^[0-9]+(\.([0-9]+)?)?$");
            e.Handled = !regExp.IsMatch(textBox.Text + e.Text);
        }

        private void PropogateParameters(ParamSnapshot paramSnapshot)
        {
            myDataContext.TargetDistance = paramSnapshot.GetValue(TargetDistanceKb);
            myDataContext.TargetBearing = paramSnapshot.GetValue(TargetBearingInGrad);
            myDataContext.MyCourseInGrad = paramSnapshot.GetValue(OurCourseInGrad);
            myDataContext.MySpeedInKnot = paramSnapshot.GetValue(OurSpeedInKnot);
            OurSpeedInKnot.Text = paramSnapshot.GetValue(OurSpeedInKnot).ToString(CultureInfo.InvariantCulture);
            OurCourseInGrad.Text = paramSnapshot.GetValue(OurCourseInGrad).ToString(CultureInfo.InvariantCulture);
        }
        
        private void OnClickExit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private static void LogException(Exception ex)
        {
            using (var writer = new StreamWriter("./Exceptions.txt", true))
            {
                writer.WriteLine("Message :" + ex.Message + "\n" +
                                 "StackTrace :" + ex.StackTrace + "\n" +
                                 "Date :" + DateTime.Now);
                writer.WriteLine("\n-----------------------------------------------------------------------------\n");
            }
        }
    }
}