﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TimeSeriesAnalysis;
using TimeSeriesAnalysis.Utility;

namespace TimeSeriesAnalysis.SysId
{

    public class DefaultProcessModel : IProcessModel
    {
        private DefaultProcessModelParameters modelParameters;
        private LowPass lowPass;
        private double timeBase_s;


        public DefaultProcessModel(DefaultProcessModelParameters modelParameters, double timeBase_s)
        {
            this.modelParameters = modelParameters;
            this.timeBase_s = timeBase_s;
            InitSim(timeBase_s);
        }

        /// <summary>
        /// Initalizer of model that for the given dataSet also creates the resulting y_sim
        /// </summary>
        /// <param name="modelParameters"></param>
        /// <param name="dataSet"></param>
        public DefaultProcessModel(DefaultProcessModelParameters modelParameters, ProcessDataSet dataSet)
        {
            this.modelParameters = modelParameters;
            InitSim(dataSet.TimeBase_s);
        }




        //  public DefaultProcessModelParameters GetModelParameters()
        public IModelParameters GetModelParameters()
        {
            return modelParameters;
        }

        public void InitSim(double timeBase_s)
        {
            this.lowPass = new LowPass(timeBase_s);
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
                if (modelParameters.U0 != null)
                {
                    y_static += modelParameters.ProcessGains[curInput] *
                        (inputsU[curInput] - modelParameters.U0[curInput]);
                }
                else
                {
                    y_static += modelParameters.ProcessGains[curInput] *
                            inputsU[curInput];
                }

                if (modelParameters.ProcessGainCurvatures != null)
                { 
                    //TODO
                }
            }
            double y = lowPass.Filter(y_static, modelParameters.TimeConstant_s);
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

        override public string ToString()
        {
            int sDigits = 3;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("DefaultProcessModel");
            sb.AppendLine("-------------------------");
            sb.AppendLine("TimeConstant_s        : " +modelParameters.TimeConstant_s);
            sb.AppendLine("TimeDelay_s           : " + modelParameters.TimeDelay_s);
            sb.AppendLine("ProcessGains          : " + Vec.ToString(modelParameters.ProcessGains, sDigits));
            sb.AppendLine("ProcessGainCurvatures : " + Vec.ToString(modelParameters.ProcessGainCurvatures, sDigits));
            sb.AppendLine("Bias                  : " + SignificantDigits.Format(modelParameters.Bias, sDigits));
            sb.AppendLine("u0                    : " + Vec.ToString(modelParameters.U0,sDigits));
            sb.AppendLine("-------------------------");
            sb.AppendLine("fitting objective     : " + modelParameters.GetFittingObjFunVal() );
            sb.AppendLine("fitting R2            : " + modelParameters.GetFittingR2());
            foreach (var warning in modelParameters.GetWarningList())
                sb.AppendLine("fitting warning                 :" + warning.ToString());
            if (modelParameters.GetWarningList().Count == 0)
            {
                sb.AppendLine("fitting                         : no error or warnings");
            }
            return sb.ToString();
        }





    }
}
