﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace TimeSeriesAnalysis
{
    public static class VecExtensionMethods
    {
        public static string ToString(this double[] array,int nSignificantDigits,string dividerStr =";")
        {
            StringBuilder sb = new StringBuilder();
            if (array.Length > 0)
            {
                sb.Append("[");
                sb.Append(SignificantDigits.Format(array[0], nSignificantDigits).ToString("", CultureInfo.InvariantCulture));
                for (int i = 1; i < array.Length; i++)
                {
                    sb.Append(dividerStr);
                    sb.Append(SignificantDigits.Format(array[i], nSignificantDigits).ToString("",CultureInfo.InvariantCulture) );
                }
                sb.Append("]");
            }
            return sb.ToString();
        }
    }
}
