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
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}