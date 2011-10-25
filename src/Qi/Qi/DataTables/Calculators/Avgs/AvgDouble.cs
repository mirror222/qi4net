using System;

namespace Qi.DataTables.Calculators.Avgs
{
    internal class AvgDouble : Avg<double>
    {
        public AvgDouble()
            : base(Convert.ToDouble, (a, b) => a + b)
        {
        }

        internal static AvgDouble Create()
        {
            return new AvgDouble();
        }

        protected override double getAvag(double total, int count)
        {
            return total/count;
        }
    }
}