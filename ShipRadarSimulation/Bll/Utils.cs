using System;

namespace ShipRadarSimulation.bll
{
    public static class Utils
    {
        public static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}