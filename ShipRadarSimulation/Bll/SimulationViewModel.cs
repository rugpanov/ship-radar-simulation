using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
                NotifyPropertyChanged();
            }
        }

        private bool myShowStopButton;

        public bool ShowStopButton
        {
            get => myShowStopButton;
            set
            {
                myShowStopButton = value;
                NotifyPropertyChanged();
            }
        }

        private bool myShowPauseSimulation;

        public bool ShowPauseSimulation
        {
            get => myShowPauseSimulation;
            set
            {
                myShowPauseSimulation = value;
                NotifyPropertyChanged();
            }
        }

        private bool myShowResumeSimulation;

        public bool ShowResumeSimulation
        {
            get => myShowResumeSimulation;
            set
            {
                myShowResumeSimulation = value;
                NotifyPropertyChanged();
            }
        }

        private double myTargetDistance;

        public double TargetDistance
        {
            get => Math.Round(myTargetDistance, 4);
            set
            {
                myTargetDistance = value;
                NotifyPropertyChanged();
            }
        }

        private double myTargetBearing;

        public double TargetBearing
        {
            get => Math.Round(myTargetBearing, 4);
            set
            {
                myTargetBearing = value;
                NotifyPropertyChanged();
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

        public string TimerTimePresentable
        {
            get
            {
                var timeInMinutes = myTimerTimeInMs / 1000.0 / 60.0;
                return $"{timeInMinutes:0.00}";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}