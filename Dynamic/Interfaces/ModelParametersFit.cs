﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSeriesAnalysis.Dynamic
{
    abstract public class ModelParametersFit
    {

        public bool WasAbleToIdentify { get; set; }
        public double FittingRsq { get; set; }
        public double FittingObjFunVal { get; set; }
        public double NFittingBadDataPoints { get; set; }
        public double NFittingTotalDataPoints { get; set; }

        public double GetFittingR2()
        {
            return FittingRsq;
        }

        public double GetFittingObjFunVal()
        {
            return FittingObjFunVal;
        }

        public bool AbleToIdentify()
        {
            return WasAbleToIdentify;
        }



    }
}
