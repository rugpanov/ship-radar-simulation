using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ShipRadarSimulation.bll
{
    public class SimulationViewModel : INotifyPropertyChanged
    {
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