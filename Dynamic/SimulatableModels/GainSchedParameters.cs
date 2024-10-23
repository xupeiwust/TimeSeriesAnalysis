﻿using Accord.Math;
using System.Collections.Generic;
using System.Linq;
using TimeSeriesAnalysis.Dynamic;

namespace TimeSeriesAnalysis.Dynamic
{
    /// <summary>
    /// Parameters data class of the <seealso cref="GainSchedModel"/>
    /// </summary>
    public class GainSchedParameters : ModelParametersBaseClass
    {
        /// <summary>
        /// The minimum allowed output value(if set to NaN, no minimum is applied)
        /// </summary>
        public double Y_min = double.NaN;

        /// <summary>
        /// the maximum allowed output value(if set to NaN, no maximum is applied)
        /// </summary>
        public double Y_max = double.NaN;

        /// <summary>
        /// The defined constraints on fitting, and defines an operating point if applicable.
        /// </summary>
        public FittingSpecs FittingSpecs = new FittingSpecs();

        /// <summary>
        /// A time constant in seconds, the time a 1. order linear system requires to do 63% of a step response.
        /// Set to zero to turn off time constant in model.
        /// </summary>
        public double[] TimeConstant_s { get; set; } =null;

        /// <summary>
        /// The index of the scheduling-parameter among the model inputs(by default the first input is the schedulng variable)
        /// </summary>
        public int GainSchedParameterIndex = 0;

        /// <summary>
        /// Thresholds for when to use timeconstants. 
        /// </summary>
        public double[] TimeConstantThresholds { get; set; } = null;

        /// <summary>
        /// The uncertinty of the time constant estimate
        /// </summary>
        public double[] TimeConstantUnc_s { get; set; } = null;

        /// <summary>
        /// The time delay in seconds.This number needs to be a multiple of the sampling rate.
        /// Set to zero to turn off time delay in model.
        /// There is no scheduling on the time delay.
        /// </summary>
        public double TimeDelay_s { get; set; } = 0;
        /// <summary>
        /// An list of arrays of gains that determine how much in the steady state each input change affects the output(multiplied with (u-u0))
        /// The size of the list should be one higher than the size of LinearGainThresholds.
        /// </summary>
        public List<double[]> LinearGains { get; set; } = null;

        /// <summary>
        /// Threshold for when to use different LinearGains, the size should be one less than LinearGains
        /// </summary>
        public double[] LinearGainThresholds { get; set; } = null;

        /// <summary>
        /// An array of 95%  uncertatinty in the linear gains  (u-u0))
        /// </summary>
        public List<double[]> LinearGainUnc { get; set; } = null;

        /// <summary>
        /// The working point of the model, the value of each U around which the model is localized.
        /// If value is <c>null</c>c> then no U0 is used in the model
        /// </summary>
   //     public double[] U0 { get; set; } = null; // TODO: consider removing for gain-scheduled model...


        /// <summary>
        /// Note that for gain-scheduled models this is a private paramter that is calcualted 
        /// </summary>
        private double Bias = 0;

        /// <summary>
        /// The "operating point" specifies the value y that the model should have for the gain scheduled input u. 
        /// </summary>
        public double OperatingPoint_U=0, OperatingPoint_Y=0;

        private List<GainSchedIdentWarnings> errorsAndWarningMessages;

        /// <summary>
        /// Default constructor
        /// </summary>
        public GainSchedParameters()
        {
            Fitting = null;
            errorsAndWarningMessages = new List<GainSchedIdentWarnings>();
        }

        /// <summary>
        /// Returns the bias calculated from OperatingPoint_U, OperatingPoint_Y;
        /// </summary>
        /// <returns></returns>
        public double GetBias()
        {
            if (OperatingPoint_U == 0)
                return OperatingPoint_Y;
            else
            {
                return 0;//todo
            }
        }

        /// <summary>
        /// Get the number of inputs U to the model.
        /// </summary>
        /// <returns></returns>
        public int GetNumInputs()
        {
            if (LinearGains != null)
                return LinearGains.First().Length;
            else
                return 0;
        }


        /// <summary>
        /// Adds a identifiation warning to the object
        /// </summary>
        /// <param name="warning"></param>
        public void AddWarning(GainSchedIdentWarnings warning)
        {
            if (!errorsAndWarningMessages.Contains(warning))
                errorsAndWarningMessages.Add(warning);
        }

        /// <summary>
        /// Get the list of all warnings given during identification of the model
        /// </summary>
        /// <returns></returns>
        public List<GainSchedIdentWarnings> GetWarningList()
        {
            return errorsAndWarningMessages;
        }

    }
}
