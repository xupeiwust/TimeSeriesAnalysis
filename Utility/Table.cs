﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Globalization;


namespace TimeSeriesAnalysis.Utility
{
    public class Table
    {
        List<string> names;
        List<double[]> values;
        int nValues = 0;
        List<string> columnNames;

        const int nSignificantDigits = 5;

        public Table(List<string> columnNames)
        {
            this.nValues = columnNames.Count;
            this.columnNames = columnNames;
            names = new List<string>();
            values = new List<double[]>();
        }

        public void AddRow(double[] rowValues, string rowName="")
        {
            names.Add(rowName);
            values.Add(rowValues);
        }

        //for this to be parsed by plotly, use comma as csv-separator
        public void ToCSV(string fileName, string CSVseparator = ",")
        {
            StringBuilder sb = new StringBuilder();
            // make header
            sb.Append("Name" + CSVseparator);
            sb.Append(string.Join(CSVseparator, columnNames));
            sb.Append("\r\n");

            for (int curRow = 0; curRow < values.Count; curRow++)
            {
                var dataAtTime = values.ElementAt(curRow);
                sb.Append(names.ElementAt(curRow));
                for (int curColIdx = 0; curColIdx < dataAtTime.Length; curColIdx++)
                {
                    // sb.Append(CSVseparator + dataAtTime[curColIdx]);
                    sb.Append(CSVseparator + SignificantDigits.Format(dataAtTime[curColIdx], nSignificantDigits).ToString(CultureInfo.InvariantCulture));
                    //       sb.Append(CSVseparator + SignificantDigits.Format(dataAtTime[curColIdx], nSignificantDigits).ToString());
                }
                sb.Append("\r\n");
            }

            using (StringToFileWriter writer = new StringToFileWriter(fileName))
            {
                try
                {
                    writer.Write(sb.ToString());
                    writer.Close();
                }
                catch (Exception)
                {
                    Console.WriteLine("Exception writing file:" + fileName);
                }
            }
            return;
        }
    }
}
