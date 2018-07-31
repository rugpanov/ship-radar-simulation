namespace ShipRadarSimulation.Entities
{
    public class Order
    {
        public double NewCourseInGrad { get; }
        public double NewSpeed { get; }

        public Order(double newCourseInGrad, double newSpeed)
        {
            NewCourseInGrad = newCourseInGrad;
            NewSpeed = newSpeed;
        }
    }
}