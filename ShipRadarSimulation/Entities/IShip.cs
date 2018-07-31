namespace ShipRadarSimulation.Entities
{
    public interface IShip
    {
        double GetSpeedInKbS();
        void SetSpeedInKbs(double newValue);

        double GetCourseInGrad();
        void SetCourseInGrad(double newValue);

        double GetX();
        double GetY();
    }
}