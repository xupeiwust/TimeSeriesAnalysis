﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSeriesAnalysis.Dynamic
{
    /// <summary>
    /// Handles naming of individual signals in a process simulation
    /// </summary>
    class SignalNamer
    {
        private const char separator = '-';// should not be "_"

        /// <summary>
        /// Get a unique signal name for a given signal, based on the model and signal type.
        /// </summary>
        /// <param name="modelID"></param>
        /// <param name="signalType"></param>
        /// <param name="idx">models can have multiple inputs, in which case an index is needed to uniquely identify it.</param>
        /// <returns>a unique string identifier that is used to identify a signal</returns>
        public static string GetSignalName(string modelID, SignalType signalType, int idx = 0)
        {
            if (idx == 0)
                return modelID + separator + signalType.ToString();
            else
                return modelID + separator + signalType.ToString() + separator + idx.ToString();
        }


    }
}
