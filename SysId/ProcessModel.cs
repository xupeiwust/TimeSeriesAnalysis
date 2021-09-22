﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TimeSeriesAnalysis;
using TimeSeriesAnalysis.Utility;

namespace TimeSeriesAnalysis.SysId
{
    public class ProcessModelParamters
    {
        public double TimeConstant_s { get; set; } = 0;
        public int TimeDelay_s { get; set; } = 0;
        public double[] ProcessGain { get; set; } = null;
        public double[] ProcessGain_CurvatureTerm { get; set; } = null;//TODO: nonlinear curvature term
        public  double[] u0 { get; set; } = null;
        public  double Bias { get; set; } = 0;
    }

    public class ProcessModel
    {
        private ProcessModelParamters modelParameters;
        private LowPass lp;

        public ProcessModel(ProcessModelParamters modelParamters)
        {
            this.modelParameters = modelParamters;
            this.lp = null;
        }

        public void InitSim(double dT_s)
        {
            this.lp = new LowPass(dT_s);
        }

        /// <summary>
        /// Iterates the process model state one time step, based on the inputs given
        /// </summary>
        /// <param name="inputsU">vector of inputs</param>
        /// <returns>the updated process model output</returns>
        public double Iterate(double[] inputsU)
        {
            double y_static = modelParameters.Bias;
            for (int curInput = 0; curInput < inputsU.Length; curInput++)
            {
                if (modelParameters.u0 != null)
                {
                    y_static += modelParameters.ProcessGain[curInput] *
                        (inputsU[curInput] - modelParameters.u0[curInput]);
                }
                else
                {
                    y_static += modelParameters.ProcessGain[curInput] *
                            inputsU[curInput];
                }

                if (modelParameters.ProcessGain_CurvatureTerm != null)
                { 
                    //TODO
                }
            }
            double y = lp.Filter(y_static, modelParameters.TimeConstant_s);
            // TODO: add time-delay


            return y;
        }

        /// <summary>
        /// Is the model static or dynamic?
        /// </summary>
        /// <returns>Returns true if the model is static(no time constant or time delay terms),otherwise false.</returns>
        public bool IsModelStatic()
        {
           return modelParameters.TimeConstant_s == 0 && modelParameters.TimeDelay_s == 0;
        }

        /// <summary>
        /// Simulates the process model over a period of time, based on a matrix of input vectors
        /// </summary>
        /// <param name="inputsU">a 2D matrix, where each column represents the intputs at each progressive time step to be simulated</param>
        /// <param name="dT_s"> the time step in seconds of the simulation. This can be omitted if the model is static.
        /// <returns>null in inputsU is null or if dT_s is not specified and the model is not static</returns>
        public double[] Simulate(double[,] inputsU, double? dT_s= null)
        {
            if (dT_s.HasValue)
            {
                InitSim(dT_s.Value);
            }
            if (inputsU == null)
                return null;
            bool isModelStatic = modelParameters.TimeConstant_s == 0 && modelParameters.TimeDelay_s == 0;
            if (lp == null && !IsModelStatic())
            {
                return null;
            }
            int N = inputsU.GetNRows();
            double[] output = new double[N];
            for (int rowIdx = 0; rowIdx < N; rowIdx++)
            {
                output[rowIdx] = Iterate(inputsU.GetRow(rowIdx));
            }
            return output;
        }



    }
}
