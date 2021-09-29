﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TimeSeriesAnalysis.Utility;

namespace TimeSeriesAnalysis.Dynamic
{
    /// <summary>
    /// The data for a porition of a process, containg only one output and one or multiple inputs that influence it
    /// </summary>
    public class ProcessDataSet
    {
        public  string ProcessName { get;}
        public DateTime[] Times { get; }
        public double[] Y_meas { get; set; }
        public double[] Y_sim { get; set; }//TODO: add support for multiple y_sim

        public double[,] U { get;}

        public int NumDataPoints { get; }

        public double TimeBase_s { get; set; }

        public DateTime t0;



        /// <summary>
        /// Constructor for data set without inputs - for "autonomous" processes such as sinusoids, rand walks or other disturbancs.
        /// </summary>
        /// <param name="timeBase_s">the time base in seconds</param>
        /// <param name="numDataPoints">the desired nubmer of datapoints of the dataset</param>
        /// <param name="name">optional internal name of dataset</param>
        public ProcessDataSet(double timeBase_s, int numDataPoints, string name = null)
        {
            this.NumDataPoints = numDataPoints;
            this.Y_meas = null;
            this.U = null;
            this.TimeBase_s = timeBase_s;
            this.ProcessName = name;
        }

        /// <summary>
        /// Constructor for dta set with inputs <c>U</c>, i.e. where a relationship that at least partially explains <c>y_meas</c> is konwn
        /// </summary>
        /// <param name="timeBase_s">the time base in seconds</param>
        /// <param name="U">The number of rows of the 2D-array U determines the duration dataset</param>
        /// <param name="y_meas">the measured output of the system, can be null </param>
        /// <param name="name">optional internal name of dataset</param>
        public ProcessDataSet(double timeBase_s, double[,] U, double[] y_meas= null, string name=null)
        {
            this.Y_meas = y_meas;
            NumDataPoints = U.GetNRows();
            this.U = U;
            this.TimeBase_s = timeBase_s;
            this.ProcessName = name;
        }


        /// <summary>
        /// Get the time spanned by the dataset
        /// </summary>
        /// <returns>The time spanned by the dataset</returns>
        public TimeSpan GetTimeSpan()
        {
            if (Times == null)
            {
                return new TimeSpan(0, 0, (int)Math.Ceiling((double)NumDataPoints * TimeBase_s));
            }
            else
            {
                return Times.Last() - Times.First();
            }
        }
        /// <summary>
        /// Get the average value of each input in the dataset. This is useful when defining model local around a working point.
        /// </summary>
        /// <returns>an array of averages, each corrsponding to one column of U. 
        /// Returns null if it was not possible to calculate averages</returns>
        public double[] GetAverageU()
        {
            if (U == null)
            {
                return null;
            }
            List<double> averages = new List<double>();

            for (int i = 0; i < U.GetNColumns(); i++)
            {
                double? avg = Vec.Mean(U.GetColumn(i));
                if (!avg.HasValue)
                    return null;
                averages.Add(avg.Value);
            }
            return averages.ToArray();
        }


    }
}
