using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;

namespace ShipRadarSimulation.Entities
{
    public class ParamSnapshot
    {
        private readonly Dictionary<string, double> mySnapshot = new Dictionary<string, double>();
        
        public ParamSnapshot(MainWindow mw)
        {
            var myTextBoxs = new[]{mw.TargetBearingInGrad, mw.TargetDistanceKb, mw.TargetSpeedInKnot, 
                mw.TargetCourseInGrad, mw.OurSpeedInKnot, mw.OurCourseInGrad};
            foreach (var textBox in myTextBoxs)
            {
                mySnapshot.Add(textBox.Name, textBox.Text != "" ? double.Parse(textBox.Text, CultureInfo.InvariantCulture) : 0.0);
            }
        }

        public double GetValue(TextBox textBox)
        {
            return mySnapshot[textBox.Name];
        }
    }
}