namespace ShipRadarSimulation.Entities
{
    public interface IShip
    {
        double GetSpeedInKbS();
        double GetSpeedInMs();
        void SetSpeedInKbs(double newValue);

        double GetCourseInGrad();
        void SetCourseInGrad(double newValue);

        double GetX();
        double GetY();
    }
}