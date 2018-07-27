using System;

namespace ShipRadarSimulation.bll
{
    public static class Utils
    {
        public static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }
        
        public static double RadianToDegree(double rad)
        {
            return rad * 180.0 / Math.PI;
        }
    }
}