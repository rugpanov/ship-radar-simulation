using System;
using System.Windows.Threading;
using ShipRadarSimulation.Entities;

namespace ShipRadarSimulation.bll
{
    public class SimulationController
    {
        private readonly Ship myShip;
        private readonly Ship myTarget;
        public SimulationController(Ship theShip, Ship targetShip)
        {
            myShip = theShip;
            myTarget = targetShip;
        }

        public void Start()
        {
            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimerTick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            dispatcherTimer.Start();
        }
            
        private void DispatcherTimerTick(object sender, EventArgs e)
        {
            myShip.processOneSecond();
            myTarget.processOneSecond();
        }
    }

}