namespace ShipRadarSimulation.Entities
{
    public class Order
    {
        public double NewCourseInGrad { get; }
        public double NewSpeed { get; }
        public double NewDepth { get; }

        public Order(double newCourseInGrad, double newSpeed, double newDepth)
        {
            NewCourseInGrad = newCourseInGrad;
            NewSpeed = newSpeed;
            NewDepth = newDepth;
        }
    }
}