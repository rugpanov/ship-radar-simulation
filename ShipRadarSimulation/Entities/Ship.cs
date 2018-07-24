using System;

namespace ShipRadarSimulation.Entities
{
    public class Ship : IShip
    {
        private double myX;
        private double myY;
        private double mySpeedInKbS;
        private double myCourseInGrad;

        public Ship(double x, double y, double speedInKbS, double courseInGrad)
        {
            myX = x;
            myY = y;
            mySpeedInKbS = speedInKbS;
            myCourseInGrad = courseInGrad;
        }

        public double GetSpeedInKbS()
        {
            return mySpeedInKbS;
        }

        public double GetSpeedInMs()
        {
            throw new NotImplementedException();
        }

        public void SetSpeedInKbs(double newValue)
        {
            mySpeedInKbS = newValue;
        }

        public double GetCourseInGrad()
        {
            return myCourseInGrad;
        }

        public void SetCourseInGrad(double newValue)
        {
            myCourseInGrad = newValue;
        }

        public double GetX()
        {
            return myX;
        }

        public double GetY()
        {
            return myY;
        }

        public void processOneSecond()
        {
            var deltaX = 1 * mySpeedInKbS * Math.Cos(myCourseInGrad);
            var deltaY = 1 * mySpeedInKbS * Math.Sin(myCourseInGrad);

            myX += deltaX;
            myY += deltaY;
        }
    }
}