﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSeriesAnalysis.Dynamic
{
    /// <summary>
    /// Model of PID-controller
    /// This class should acta as a wrapper for PIDcontroller class.
    /// </summary>
    public class PIDModel : IProcessModel<PIDModelParameters>
    {
        int timeBase_s;
        PIDModelParameters pidParameters;
        PIDcontroller pid;
        string outputID;

        public PIDModel(PIDModelParameters pidParameters, int timeBase_s, string outputID="not_named")
        {
            this.outputID       = outputID;
            this.timeBase_s     = timeBase_s;
            this.pidParameters  = pidParameters;
            pid                 = new PIDcontroller(timeBase_s,pidParameters.Kp, 
                pidParameters.Ti_s, pidParameters.Td_s, pidParameters.NanValue);
            if (pidParameters.Scaling != null)
            {
                pid.SetScaling(pidParameters.Scaling);
            }
            else
            {// write back default scaling
                this.pidParameters.Scaling = pid.GetScaling();
            }
            //pid.SetGainScehduling(pidParameters.GainScheduling);
            pid.SetAntiSurgeParams(pidParameters.AntiSugeParams);
        }


        public ProcessModelType GetProcessModelType()
        {
            return ProcessModelType.PID;
        }

        public string GetOutputID()
        {
            return outputID;
        }

        /// <summary>
        /// [Method not currently implemented]
        /// </summary>
        /// <param name="y0"></param>
        /// <param name="inputIdx"></param>
        /// <returns></returns>
        public double? GetSteadyStateInput(double y0, int inputIdx=0)
        {
            return null;
        }

        /// <summary>
        /// Initalizes the controller internal state(integral term) to be steady at the given process value and output value, 
        /// useful to avoid bumps when staring controller
        /// </summary>
        public void WarmStart(double y_process_abs, double y, double u)
        {
            pid.WarmStart(y_process_abs, y, u);
        }

        /// <summary>
        /// Iterate the PID controller one step
        /// </summary>
        /// <param name="inputs">is a vector of length 2, 3 or 4. 
        /// First value is<c>y_process_abs</c>,  second value is <c>y_set_abs</c>, optional third value is
        /// <c>uTrackSignal</c>, optional fourth value is <c>gainSchedulingVariable</c>
        /// </param>
        /// <param name="badDataID">value of inputs that is to be treated as <c>NaN</c></param>
        /// <returns>the output <c>u</c> of the pid-controller. If not enough inputs, it returns <c>NaN</c></returns>
        public double Iterate(double[] inputs, double badDataID = -9999)
        {
            if (inputs.Length < 2)
            {
                return Double.NaN;
            }
            double y_process_abs = inputs[0];
            double y_set_abs = inputs[1];
            double? uTrackSignal = null;
            double? gainSchedulingVariable = null;
            if (inputs.Length >= 3)
            {
                uTrackSignal = inputs[2];
            }
            if (inputs.Length >= 4)
            {
                gainSchedulingVariable = inputs[3];
            }
            return pid.Iterate(y_process_abs,y_set_abs, uTrackSignal, gainSchedulingVariable);
        }

        /// <summary>
        /// Get the model parameters
        /// </summary>
        /// <returns>the parameters object of the model</returns>
        public PIDModelParameters GetModelParameters()
        {
            return pidParameters;
        }
    }
}
