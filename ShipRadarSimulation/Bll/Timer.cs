using System;

namespace ShipRadarSimulation.bll
{
    public class Timer
    {
        private DateTime myTimerTimeStart;
        private double myTimerPousedTimeInMs;
        private bool isStarted = false;
        
        public Timer()
        {
            myTimerPousedTimeInMs = 0.0;
        }

        public void Start()
        {
            isStarted = true;
            myTimerTimeStart = DateTime.Now;
        }

        public void Stop()
        {
            isStarted = false;
            var timePassed = DateTime.Now - myTimerTimeStart;
            myTimerPousedTimeInMs += timePassed.TotalMilliseconds;
        }

        public void Reset()
        {
            isStarted = false;
            myTimerPousedTimeInMs = 0.0;
        }
        
        
        public double GetTimePassedInMs()
        {
            if (isStarted)
            {
                return (DateTime.Now - myTimerTimeStart).TotalMilliseconds + myTimerPousedTimeInMs;
            }
            
            return myTimerPousedTimeInMs;
        }
    }
}