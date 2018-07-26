using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ShipRadarSimulation.bll
{
    public class SimulationViewModel : INotifyPropertyChanged
    {
        private bool mySimulationNotStarted = true;
        
        public bool SimulationStarted
        {
            get => !mySimulationNotStarted;
            set
            {
                mySimulationNotStarted = !value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("SimulationNotStarted");
            }
        }
        
        public bool SimulationNotStarted
        {
            get => mySimulationNotStarted;
            set
            {
                mySimulationNotStarted = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("SimulationStarted");
            }
        }
        
        private int p = 100;
        
       // public DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(string), typeof(SimulationDataContext), new UIPropertyMetadata(string.Empty));
        public int Position
        {
            get => p;
            set
            {
                p = value;
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