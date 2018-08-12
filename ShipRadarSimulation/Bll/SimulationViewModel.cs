using System;
using System.ComponentModel;

namespace ShipRadarSimulation.bll
{
    public class SimulationViewModel : INotifyPropertyChanged
    {
        private bool myShowStartButton = true;

        public bool ShowStartButton
        {
            get => myShowStartButton;
            set
            {
                myShowStartButton = value;
                NotifyPropertyChanged("ShowStartButton");
            }
        }

        private bool myShowStopButton;

        public bool ShowStopButton
        {
            // ReSharper disable once UnusedMember.Global
            get => myShowStopButton;
            set
            {
                myShowStopButton = value;
                NotifyPropertyChanged("ShowStopButton");
            }
        }

        private double myAngularVelocityInGradSec = Constants.DefaultAngularVelocityInGradSec;

        public double MyAngularVelocityInGradSec
        {
            get => myAngularVelocityInGradSec;
            set
            {
                myAngularVelocityInGradSec = value;
                NotifyPropertyChanged("MyAngularVelocityInGradSec");
            }
        }
        
        private double myAccelerationInKnotSecInKnotSec = Constants.DefaultAccelerationInKnotSec;

        public double MyAccelerationInKnotSec
        {
            get => myAccelerationInKnotSecInKnotSec;
            set
            {
                myAccelerationInKnotSecInKnotSec = value;
                NotifyPropertyChanged("MyAccelerationInKnotSec");
            }
        }
        
        private bool myShowPauseSimulation;

        public bool ShowPauseSimulation
        {
            get => myShowPauseSimulation;
            set
            {
                myShowPauseSimulation = value;
                NotifyPropertyChanged("ShowPauseSimulation");
            }
        }

        private bool myShowResumeSimulation;

        public bool ShowResumeSimulation
        {
            get => myShowResumeSimulation;
            set
            {
                myShowResumeSimulation = value;
                NotifyPropertyChanged("ShowResumeSimulation");
            }
        }

        private double myCourceInGrad;
        
        public double MyCourseInGrad
        {
            // ReSharper disable once UnusedMember.Global
            get => Math.Round(myCourceInGrad, 4);
            set
            {
                myCourceInGrad = value;
                NotifyPropertyChanged("MyCourseInGrad");
            }
        }

        private double mySpeedInKnot;
        
        public double MySpeedInKnot
        {
            // ReSharper disable once UnusedMember.Global
            get => Math.Round(mySpeedInKnot, 4);
            set
            {
                mySpeedInKnot = value;
                NotifyPropertyChanged("MySpeedInKnot");
            }
        }
        
        private double myTargetDistance;

        public double TargetDistance
        {
            // ReSharper disable once UnusedMember.Global
            get => Math.Round(myTargetDistance, 4);
            set
            {
                myTargetDistance = value;
                NotifyPropertyChanged("TargetDistance");
            }
        }

        private double myTargetBearing;

        public double TargetBearing
        {
            // ReSharper disable once UnusedMember.Global
            get => Math.Round(myTargetBearing, 4);
            set
            {
                myTargetBearing = value;
                NotifyPropertyChanged("TargetBearing");
            }
        }

        private double myTimerTimeInMs;

        public double TimerTimeInMs
        {
            set
            {
                myTimerTimeInMs = value;
                NotifyPropertyChanged("TimerTimePresentable");
            }
            get => myTimerTimeInMs;
        }

        // ReSharper disable once UnusedMember.Global
        public string TimerTimePresentable
        {
            get
            {
                var timeInMinutes = myTimerTimeInMs / 1000.0 / 60.0;
                return $"{timeInMinutes:0.00}";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName = "")
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}