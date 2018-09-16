using System;
using ShipRadarSimulation.bll;

namespace ShipRadarSimulation.Entities
{
    public class Ship
    {
        private double myX;
        private double myY;
        private double mySpeedInKbS;
        private double myCourseInGrad;
        private double myDepth;
        private readonly double myAccelerationInKbS;
        private readonly double myAngularVelocityInGradSec;
        private readonly double myDepthChange;
        private Order myOrder;

        public Ship(double x,
            double y,
            double speedInKbS,
            double courseInGrad,
            double depth,
            double accelerationInKbS,
            double angularVelocityInGradSec,
            double depthChange)
        {
            myX = x;
            myY = y;
            mySpeedInKbS = speedInKbS;
            myCourseInGrad = courseInGrad;
            myDepth = depth;
            myAccelerationInKbS = accelerationInKbS;
            myAngularVelocityInGradSec = angularVelocityInGradSec;
            myDepthChange = depthChange;
        }

        public double GetSpeedInKbS()
        {
            return mySpeedInKbS;
        }

        public double GetCourseInGrad()
        {
            return myCourseInGrad;
        }

        public double GetX()
        {
            return myX;
        }

        public double GetY()
        {
            return myY;
        }

        public double GetDepth()
        {
            return myDepth;
        }

        public void ProcessOneSecond()
        {
            ProcessOrderOneSecond();

            var deltaX = 1 * mySpeedInKbS * Math.Sin(Utils.DegreeToRadian(myCourseInGrad));
            var deltaY = 1 * mySpeedInKbS * Math.Cos(Utils.DegreeToRadian(myCourseInGrad));

            myX += deltaX;
            myY += deltaY;
        }

        private void ProcessOrderOneSecond()
        {
            if (myOrder == null) return;

            var deltaCourse = myCourseInGrad - myOrder.NewCourseInGrad;
            var deltaSpeed = mySpeedInKbS - myOrder.NewSpeed;
            var deltaDepth = myDepth - myOrder.NewDepth;

            if (Math.Abs(deltaCourse) < Constants.Epselon &&
                Math.Abs(deltaSpeed) < Constants.Epselon &&
                Math.Abs(deltaDepth) < Constants.Epselon)
            {
                myOrder = null;
                return;
            }

            if (deltaCourse > myAngularVelocityInGradSec && deltaCourse < 180 ||
                deltaCourse < myAngularVelocityInGradSec && deltaCourse < -180)
            {
                myCourseInGrad -= myAngularVelocityInGradSec;
            }
            else if (deltaCourse < -myAngularVelocityInGradSec && deltaCourse >= -180 ||
                     deltaCourse > myAngularVelocityInGradSec && deltaCourse >= 180)
            {
                myCourseInGrad += myAngularVelocityInGradSec;
            }
            else
            {
                myCourseInGrad = myOrder.NewCourseInGrad;
            }

            if (deltaSpeed > myAccelerationInKbS)
            {
                mySpeedInKbS -= myAccelerationInKbS;
            }
            else if (deltaSpeed < -myAccelerationInKbS)
            {
                mySpeedInKbS += myAccelerationInKbS;
            }
            else
            {
                mySpeedInKbS = myOrder.NewSpeed;
            }

            if (deltaDepth > myDepthChange)
            {
                myDepth -= myDepthChange;
            }
            else if (deltaDepth < -myDepthChange)
            {
                myDepth += myDepthChange;
            }
            else
            {
                myDepth = myOrder.NewDepth;
            }
            
            
            if (myCourseInGrad >= 360)
            {
                myCourseInGrad -= 360;
            }

            if (myCourseInGrad < 0)
            {
                myCourseInGrad += 360;
            }
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

        public void AddOrder(Order order)
        {
            myOrder = order;
        }
    }
}