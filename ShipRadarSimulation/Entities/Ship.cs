using System;
using ShipRadarSimulation.bll;

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

        public void ProcessOneSecond()
        {
            var deltaX = 1 * mySpeedInKbS * Math.Sin(Utils.DegreeToRadian(myCourseInGrad));
            var deltaY = 1 * mySpeedInKbS * Math.Cos(Utils.DegreeToRadian(myCourseInGrad));

            myX += deltaX;
            myY += deltaY;
        }
        
        public double MeasureDistance(Ship another)
        {
            return Math.Sqrt(Math.Pow(GetX() - another.GetX(), 2) + Math.Pow(GetY() - another.GetY(), 2));
        }

        public double MeasureBearing(Ship another)
        {
            var movementY = another.GetY() - GetY();
            var movementX = another.GetX() - GetX();
            var degree = Utils.RadianToDegree(Math.Atan2(movementX, movementY));
            return degree >= 0 ? degree : 360 + degree;
        }
    }
}