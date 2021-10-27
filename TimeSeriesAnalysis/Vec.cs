﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics.Models.Regression.Linear;
using Accord.Statistics.Models.Regression.Fitting;
using System.Globalization;
using System.IO;

using TimeSeriesAnalysis.Utility;

namespace TimeSeriesAnalysis
{
    /// <summary>
    /// Utility functions and operations for treating arrays as mathetmatical vectors
    /// This class considers doubles, methods that require comparisons cannot be easily ported to generic (Vec)
    /// </summary>
    public class Vec
    {
        private  double valuteToReturnElementIsNaN;// so fi an element is either NaN or "-9999", what value shoudl a calculation return?
        private double nanValue;// an input value that is to be considrered "NaN" 

        /// <summary>
        /// Constructor
        /// 
        /// </summary>
        /// <param name="nanValue">inputs values matching this value are treated as "NaN" 
        /// and are excluded from all calculations</param>
        /// <param name="valuteToReturnElementIsNaN">value to return in elementwise calculations to indiate Nan output</param>
        public Vec(double nanValue = -9999, double valuteToReturnElementIsNaN = Double.NaN)
        {
            this.nanValue = nanValue;
            this.valuteToReturnElementIsNaN = valuteToReturnElementIsNaN;
        }

        //  Methods should be sorted alphabetically


        ///<summary>
        /// returns an array where each value is the absolute value of array1
        ///</summary>
        public double[] Abs(double[] array1)
        {
            if (array1 == null)
                return null;
            double[] retVal = new double[array1.Length];

            for (int i = 0; i < array1.Length; i++)
            {
                if (IsNaN(array1[i]))
                    retVal[i] = valuteToReturnElementIsNaN;
                else
                    retVal[i] = Math.Abs(array1[i]);
            }
            return retVal;
        }

        ///<summary>
        /// returns an array which is the elementwise addition of array1 and array2 
        ///</summary>
        public double[] Add(double[] array1, double[] array2)
        {

            if (array1 == null || array2 == null)
                return null;
            double[] retVal = new double[array1.Length];
            for (int i = 0; i < array1.Length; i++)
            {
                if (IsNaN(array1[i]))
                    retVal[i] = valuteToReturnElementIsNaN;
                else
                {
                    retVal[i] = array1[i] + array2[i];
                }
            }
            return retVal;
        }

        ///<summary>
        /// elementwise addition of val2 to array1
        ///</summary>

        public int[] Add(int[] array1, int val2)
        {
            if (array1 == null)
                return null;
            int[] retVal = new int[array1.Length];
            for (int i = 0; i < array1.Length; i++)
            {
                if (IsNaN(array1[i]))
                    retVal[i] = (int)valuteToReturnElementIsNaN;
                else
                    retVal[i] = array1[i] + val2;
            }
            return retVal;
        }

        ///<summary>
        /// elementwise addition of val2 to array1
        ///</summary>
        public double[] Add(double[] array1, double val2)
        {
            if (array1 == null)
                return null;
            double[] retVal = new double[array1.Length];
            for (int i = 0; i < array1.Length; i++)
            {
                if (IsNaN(array1[i]))
                    retVal[i] = valuteToReturnElementIsNaN;
                else
                    retVal[i] = array1[i] + val2;
            }
            return retVal;
        }

        /// <summary>
        ///  When filtering out bad data before identification, before fitting 
        ///  data to difference equations that depend both y[k] and y[k-1]
        ///  it will some times be neccessary, to append the trailing indices
        ///  for instance on 
        /// 
        /// </summary>
        static public List<int> AppendTrailingIndices(List<int> indiceArray)
        {
            List<int> appendedIndiceArray = new List<int>(indiceArray);
            List<int> indicesToAdd = new List<int>();
            for (int i = 0; i < indiceArray.Count; i++)
            {
                int curVal = indiceArray.ElementAt(i);
                if (!indiceArray.Contains(curVal + 1))
                    indicesToAdd.Add(curVal + 1);
            }
            appendedIndiceArray.AddRange(indicesToAdd);

            appendedIndiceArray.Sort();

            return appendedIndiceArray;
        }

        ///<summary>
        /// Returns true f array contains a "-9999" or NaN indicating missing data
        ///</summary>
        public bool ContainsBadData(double[] x)
        {
            bool doesContainBadData = false;
            for (int i = 0; i < x.Length; i++)
            {
                if (IsNaN(x[i]))
                {
                    doesContainBadData = true;
                }
            }
            return doesContainBadData;
        }

        ///<summary>
        ///  returns the co-variance of two arrays(interpreted as "vectors")
        ///</summary>
        public double Cov(double[] array1, double[] array2, bool doNormalize = false)
        {
            double retVal = 0;
            double avg1 = Mean(array1).Value;
            double avg2 = Mean(array2).Value;
            int N = 0;
            for (int i = 0; i < array1.Length; i++)
            {
                if (IsNaN(array1[i]) || IsNaN(array2[i]))
                    continue;
                N++;
                retVal += (array1[i] - avg1) * (array2[i] - avg2);
            }
            if (doNormalize)
            {
                retVal /= N;
            }
            return retVal;
        }

        /// <summary>
        ///  de-serializes a single vector/array (written by serialize)
        /// </summary>
        static public double[] Deserialize(string fileName)
        {
            List<double> values = new List<double>();
            string[] lines = File.ReadAllLines(fileName);

            foreach (string line in lines)
            {
                bool isOk = Double.TryParse(line, NumberStyles.Any,
                    CultureInfo.InvariantCulture, out double result);
                if (isOk)
                {
                    values.Add(result);
                }
                else
                {
                    values.Add(Double.NaN);
                }
            }
            return values.ToArray();
        }


        ///<summary>
        /// returns an array of the difference between every neighbhoring item in array
        ///</summary>
        public  double[] Diff(double[] array)
        {
            double[] ucur = Vec<double>.SubArray(array, 1);
            double[] uprev = Vec<double>.SubArray(array, 0, array.Length - 2);
            double[] uDiff = Subtract(ucur, uprev);
            return Vec<double>.Concat(new double[] { 0 }, uDiff);
        }

        /// <summary>
        /// Divides an vector by a scalar value
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="scalar"></param>
        /// <returns>an vector of values representing the array didived by a scalar. 
        /// In case of NaN inputs or divide-by-zero NaN elements are returned.  </returns>
        public double[] Div(double[] vector, double scalar)
        {
            double[] outArray = new double[vector.Length];
            for (int i = 0; i < vector.Length; i++)
            {
                if (IsNaN(vector[i]) || scalar == 0)
                {
                    outArray[i] = valuteToReturnElementIsNaN;
                }
                else
                {
                    outArray[i] = vector[i] / scalar;
                }
            }
            return outArray;
        }

        /// <summary>
        /// Divides two vectors of equal length
        /// </summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <returns>an vector of values representing the array didived by a scalar. 
        /// In case of NaN inputs or divide-by-zero NaN elements are returned</returns>
        public double[] Div(double[] vector1, double[] vector2)
        {
            int N = Math.Min(vector1.Length, vector2.Length);
            double[] outArray = new double[N];
            for (int i = 0; i < N; i++)
            {
                if (IsNaN(vector1[i]) || IsNaN(vector2[i])|| vector2[i]==0)
                {
                    outArray[i] = valuteToReturnElementIsNaN;
                }
                else
                {
                    outArray[i] = vector1[i] / vector2[i];
                }
            }
            return outArray;
        }




        ///<summary>
        /// return the indices of elements in the array that have certain relation to value given type (bigger,smaller,equal etc.)
        /// Also capable of finding NaN values
        ///</summary>
        public List<int> FindValues(double[] vec, double value, VectorFindValueType type)
        {
            List<int> indices = new List<int>();

            if (type == TimeSeriesAnalysis.VectorFindValueType.BiggerThan)
            {
                for (int i = 0; i < vec.Length; i++)
                {
                    if (vec[i] > value && !IsNaN(vec[i]))
                        indices.Add(i);
                }
            }
            else if (type == TimeSeriesAnalysis.VectorFindValueType.SmallerThan)
            {
                for (int i = 0; i < vec.Length; i++)
                {
                    if (vec[i] < value && !IsNaN(vec[i]))
                        indices.Add(i);
                }
            }
            else if (type == TimeSeriesAnalysis.VectorFindValueType.BiggerOrEqual)
            {
                for (int i = 0; i < vec.Length; i++)
                {
                    if (vec[i] >= value && !IsNaN(vec[i]))
                        indices.Add(i);
                }
            }
            else if (type == TimeSeriesAnalysis.VectorFindValueType.SmallerOrEqual)
            {
                for (int i = 0; i < vec.Length; i++)
                {
                    if (vec[i] <= value && !IsNaN(vec[i]))
                        indices.Add(i);
                }
            }
            else if (type == TimeSeriesAnalysis.VectorFindValueType.Equal)
            {
                for (int i = 0; i < vec.Length; i++)
                {
                    if (vec[i] == value)
                        indices.Add(i);
                }
            }
            else if (type == TimeSeriesAnalysis.VectorFindValueType.NaN)
            {
                for (int i = 0; i < vec.Length; i++)
                {
                    if (IsNaN(vec[i])|| vec[i] == value)
                        indices.Add(i);
                }
            }
            else if (type == TimeSeriesAnalysis.VectorFindValueType.NotNaN)
            {
                for (int i = 0; i < vec.Length; i++)
                {
                    if (!IsNaN(vec[i]))
                        indices.Add(i);
                }
            }
            return indices;
        }

        ///<summary>
        /// returns the intersection of array1 and array2, a list of elements that are in both vectors
        ///</summary>
        public static List<int> Intersect(List<int> vec1, List<int> vec2)
        {
            return vec1.Intersect(vec2).ToList();
        }


        ///<summary>
        /// given a list of sorted indeces and a desired vector size N, returns the indices that are not in "sortedIndices"
        /// i.e. of the "other vectors
        ///</summary>
        public static List<int> InverseIndices(int N, List<int> sortedIndices)
        {
            List<int> ret = new List<int>();

            int curInd = 0;
            bool lastSortedIndFound = false;
            int nSortedIndices = sortedIndices.Count();
            for (int i = 0; i < N; i++)
            {
                if (curInd < nSortedIndices)
                {
                    if (i < sortedIndices[curInd])
                    {
                        ret.Add(i);
                    }
                    else if (i == sortedIndices[curInd])
                    {
                        if (curInd + 1 < sortedIndices.Count)
                            curInd++;
                        else
                            lastSortedIndFound = true;

                    }
                    else if (lastSortedIndFound)
                    {
                        ret.Add(i);
                    }
                }
            }
            return ret;
        }


        ///<summary>
        /// Returns true if all elements in array are the specific value
        ///</summary>
        public static bool IsAllValue(double[] array, double value = 0)
        {
            int count = 0;
            while (array[count] == value && count < array.Length - 1)
            {
                count++;
            }
            if (count >= array.Length - 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        ///<summary>
        /// Returns true if all elements in array are "-9999" or Double.NaN
        ///</summary>
        public bool IsAllNaN(double[] array)
        {
            int count = 0;
            while (IsNaN(array[count]) && count < array.Length - 1)
            {
                count++;
            }
            if (count >= array.Length - 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        ///<summary>
        /// All checks for NaN will test both for Double.IsNan and if value== a specific "nan" value (-9999)
        ///</summary>
        private bool IsNaN(double value)
        {
            if (double.IsNaN(value) || value == nanValue)
                return true;
            else
                return false;
        }

        ///<summary>
        ///  Returns maximum value of two array as new array 
        ///</summary>
        public double[] Max(double[] array1, double[] array2)
        {
            double[] retVal = new double[array1.Length];

            for (int i = 0; i < retVal.Length; i++)
            {
                if (array1[i] > array2[i])
                    retVal[i] = array1[i];
                else
                    retVal[i] = array2[i];
            }
            return retVal;
        }

        ///<summary>
        ///  Returns maximum value of array between indices startInd and endInd
        ///</summary>
        public double Max(double[] array, int startInd, int endInd)
        {
            double maxVal = double.MinValue;
            for (int i = startInd; i < endInd; i++)
            {
                double thisNum = array[i];
                if (IsNaN(thisNum))
                    continue;
                if (thisNum > maxVal)
                {
                    maxVal = thisNum;
                }
            }
            return maxVal;
        }

        ///<summary>
        ///  Returns minimum value of two array as new array 
        ///</summary>
        static public double[] Min(double[] array1, double[] array2)
        {
            double[] retVal = new double[array1.Length];

            for (int i = 0; i < retVal.Length; i++)
            {
                if (array1[i] > array2[i])
                    retVal[i] = array2[i];
                else
                    retVal[i] = array1[i];
            }
            return retVal;
        }


        ///<summary>
        ///  Returns maximum value of array and index of maximum value 
        ///</summary>
        public double Max(double[] array, out int ind)
        {
            ind = 0;
            double maxVal = double.MinValue;
            for (int i = 0; i < array.Length; i++)
            {
                double thisNum = array[i];
                if (IsNaN(thisNum))
                    continue;
                if (thisNum > maxVal)
                {
                    maxVal = thisNum;
                    ind = i;
                }
            }
            return maxVal;
        }

        ///<summary>
        ///  Returns element-wise minimum of array element and value
        ///</summary>
        public double[] Min(double[] array, double value)
        {
            double[] retArray = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                double thisNum = array[i];
                if (IsNaN(thisNum))
                    continue;
                if (thisNum < value)
                {
                    retArray[i] = thisNum;
                }
                else
                {
                    retArray[i] = value;
                }
            }
            return retArray;
        }

        ///<summary>
        ///  Returns element-wise maximum of array element and value
        ///</summary>
        public double[] Max(double[] array, double value)
        {
            double[] retArray = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                double thisNum = array[i];
                if (IsNaN(thisNum))
                    continue;
                if (thisNum > value)
                {
                    retArray[i] = thisNum;
                }
                else
                {
                    retArray[i] = value;
                }
            }
            return retArray;
        }



        ///<summary>
        ///  Returns minimum value of array and index of maximum value 
        ///</summary>
        public double Min(double[] array, out int ind)
        {
            ind = 0;
            double minVal = double.MaxValue;
            for (int i = 0; i < array.Length; i++)
            {
                double thisNum = array[i];
                if (IsNaN(thisNum))
                    continue;
                if (thisNum < minVal)
                {
                    minVal = thisNum;
                    ind = i;
                }
            }
            return minVal;
        }

        public double Min(double[] array)
        {
            return Min(array, out _);
        }


        ///<summary>
        ///  Returns minimum value of array 
        ///</summary>
        /*public static double Min(double[] array)
        {
            return (new Vec()).Min(array, out _);
        }*/

        ///<summary>
        ///  Returns maximum value of array 
        ///</summary>
        public double Max(double[] array)
        {
            return Max(array, out _);
        }

        ///<summary>
        ///  creates a monotonically increasing integer (11.12.13...) array starting at startValue and ending at endValue
        ///</summary>
        public static int[] MakeIndexArray(int startValue, int endValue)
        {
            List<int> retList = new List<int>();
            for (int i = startValue; i < endValue; i++)
            {
                retList.Add(i);
            }
            return retList.ToArray();
        }


        ///<summary>
        /// elementwise multipliation of val2 to array1
        ///</summary>
        public double[] Mult(double[] array1, double val2)
        {
            if (array1 == null)
                return null;
            double[] retVal = new double[array1.Length];

            for (int i = 0; i < array1.Length; i++)
            {
                if (IsNaN(array1[i]))
                    retVal[i] = valuteToReturnElementIsNaN;
                else
                    retVal[i] = array1[i] * val2;
            }
            return retVal;
        }

        ///<summary>
        /// elementwise  multiplication of array1 and array2, assuming they are same size
        ///</summary>

        public double[] Multiply(double[] array1, double[] array2)
        {
            if (array1 == null)
                return null;
            if (array2 == null)
                return null;

            if (array1.Length != array2.Length)
                return null;

            double[] retVal = new double[array1.Length];

            for (int i = 0; i < array1.Length; i++)
            {
                if (IsNaN(array1[i]) || IsNaN(array2[i]))
                    retVal[i] = valuteToReturnElementIsNaN;
                else
                    retVal[i] = array1[i] * array2[i];
            }
            return retVal;
        }

        ///<summary>
        /// returns the mean value of array1
        ///</summary>
        public double? Mean(double[] array1)
        {
            if (array1 == null)
                return null;
            double retVal = 0;
            double N = 0;
            for (int i = 0; i < array1.Length; i++)
            {
                if (!IsNaN(array1[i]))
                {
                    N += 1;
                    retVal = retVal * (N - 1) / N + array1[i] * 1 / N;
                }
            }
            return retVal;
        }

        ///<summary>
        ///  Returns range of an array, the difference between minimum and maximum
        ///</summary>
        public double Range(double[] array)
        {
            double range = Max(array) - Min(array);

            return range;
        }



        /// <summary>
        /// Create a vector of random numbers
        /// </summary>
        /// <param name="N">the number of samples of the returned array</param>
        /// <param name="minValue">lower end of random number range</param>
        /// <param name="maxValue">higher end of random number range</param>
        /// <param name="seed">optionally, give in a seed number, this makes random sequence repeatable</param>
        /// <returns>an array of size N of random numbers between minValue and maxValue </returns>
        public static double[] Rand(int N, double minValue = 0, double maxValue = 1,int? seed=null)
        {
            Random rand;//= null;
            if (seed.HasValue)
            {
                rand = new Random(seed.Value);
            }
            else
            {
                rand = new Random();
            }

            double[] ret = new double[N];
            for (int i = 0; i < N; i++)
            {
                ret[i] = rand.NextDouble() * (maxValue - minValue) + minValue; ;//NextDouble by itself return a valube btw 0 and 1
            }
            return ret;
        }


        /*{



        }

        /// <summary>
        /// Linear regression: Fit a linear model to Y based on inputs X
        /// </summary>
        /// <param name="Y">one-dimensional vector/array of output paramters y that is to be modelled</param>
        /// <param name="X">two-dimensonal array of input vectors X that are the inputs </param>
        /// <returns> returns the parameters(one for each column in X and a bias term) which best regress the two-dimensional array X into the vector Y. Returns null if regression fails.</returns>
        public static double[] Regress(double[] Y, double[][] X)
        {
            return Regress(Y, X, null, out _, out _, out _, out _);
        }
        ///<summary>
        /// regression where the rows corresponding to indices yIndToIgnore are ignored (bad data identified in preprocessing)
        ///</summary>
        public static double[] Regress(double[] Y, double[][] X, int[] yIndToIgnore, out double[] param95prcConfInterval, out double[] Y_modelled,
            out double Rsq)
        {
            return Regress(Y, X, yIndToIgnore, out param95prcConfInterval, out _, out Y_modelled, out Rsq);
        }

        ///<summary>
        /// regression where the rows corresponding to indices yIndToIgnore are ignored (bad data identified in preprocessing)
        /// uncertainties in parameters, covariance matrix, modelled output y and R-squared number is also given.
        ///</summary>

        public static double[] Regress(double[] Y, double[][] X, int[] yIndToIgnore,
            out double[] param95prcConfInterval, out double[][] varCovarMatrix,
            out double[] Y_modelled, out double Rsq)
            */
        /// <summary>
        /// Linear regression
        /// </summary>
        /// <param name="Y">vector of responve variable values (to be modelled)</param>
        /// <param name="X">2D matrix of of mainpulated values/independent values/regressors used to explain Y</param>
        /// <param name="yIndToIgnore">(optional) a list of the indices of values in Y to ignore in regression. By default it is <c>null</c></param>
        /// <returns>an object of the <c>RegressionResult</c> class with the paramters, as well as 
        /// some statistics on the fit and uncertainty thereof.</returns>

        public RegressionResults Regress(double[] Y, double[,] X, int[] yIndToIgnore = null)
        {
            return Regress(Y,X.Convert2DtoJagged(),yIndToIgnore);
        }

        /// <summary>
        /// Linear regression
        /// </summary>
        /// <param name="Y">vector of responve variable values (to be modelled)</param>
        /// <param name="X">jagged 2D matrix of of mainpulated values/independent values/regressors used to explain Y</param>
        /// <param name="yIndToIgnore">(optional) a list of the indices of values in Y to ignore in regression. By default it is <c>null</c></param>
        /// <returns>an object of the <c>RegressionResult</c> class with the paramters, as well as 
        /// some statistics on the fit and uncertainty thereof.</returns>
        public RegressionResults Regress(double[] Y, double[][] X, int[] yIndToIgnore=null)
        {
            bool doInterpolateYforBadIndices = true;
            MultipleLinearRegression regression;
            double[][] X_T;
            if (X.GetNColumns() > X.GetNRows())
            {
                //  Accord.Math.Matrix.
                X_T = Accord.Math.Matrix.Transpose(X);
            }
            else
            {
                X_T = X;
            }
            // weight-to-zero all indices which are to be ignored!
            double[] weights = null;
            if (yIndToIgnore != null)
            {
                weights = Vec<double>.Fill(1, Y.Length);
                for (int i = 0; i < yIndToIgnore.Length; i++)
                {
                    int curInd = yIndToIgnore[i];
                    if (curInd >= 0 && curInd < weights.Length)
                    {
                        weights[curInd] = 0;
                    }
                    // set Y and X_T to zero for values that are bad
                    // the weight do not always appear to work, sometimes the accord
                    // solver just returns "null" and hard to know why, and this is a
                    // workaround
                    if (curInd < Y.Length)
                    {
                        Y[curInd] = 0;
                        for (int curX = 0; curX < X_T[curInd].Count(); curX++)
                        {
                            X_T[curInd][curX] = 0;
                        }
                    }
                }
            }

            bool doDebug = false;
            if (doDebug)
            {
                Plot.FromList(new List<double[]> {Y, Array2D<double>.GetColumn(X_T,0),
                    Array2D<double>.GetColumn(X_T,1) },
                    new List<string> { "y1=Y","y3=u1", "y3=u2" },1,null,default,"regresstest");
            }

            OrdinaryLeastSquares accordFittingAlgo = new OrdinaryLeastSquares()
            {
                IsRobust = false // to use SVD or not.
            };
            RegressionResults results = new RegressionResults();
            //TODO: try to catch rank deficient or singular X instead of generating exception.
            try
            {
                // note: weights have no effect prior to accord 3.7.0 
                regression = accordFittingAlgo.Learn(X_T, Y, weights);
                if (yIndToIgnore == null)
                {
                    results.NfittingBadDataPoints = 0;
                }
                else
                {
                    results.NfittingBadDataPoints = yIndToIgnore.Length;
                }
                results.NfittingTotalDataPoints = Y.Length;
                // modelled Y
                results.Y_modelled = regression.Transform(X_T);
                if (yIndToIgnore != null)
                {
                    if (doInterpolateYforBadIndices)
                    {
                        // write interpolated values to y_modelled. 
                        // these should 
                        double lastIgnoredInd = -1;
                        double lastGoodValue = -1;
                        for (int i = 0; i < yIndToIgnore.Length; i++)
                        {
                            int curInd = yIndToIgnore[i];
                            if (curInd == lastIgnoredInd + 1)
                            {
                                results.Y_modelled[yIndToIgnore[i]] = lastGoodValue;
                            }
                            else
                            {
                                lastIgnoredInd = curInd;
                                lastGoodValue = results.Y_modelled[yIndToIgnore[i] - 1];
                                results.Y_modelled[yIndToIgnore[i]] = lastGoodValue;
                            }
                        }
                    }
                }

                if (yIndToIgnore != null)
                {
                    results.Rsq = RSquared(results.Y_modelled, Y, yIndToIgnore.ToList()) * 100;
                }
                else
                {
                    results.Rsq = RSquared(results.Y_modelled, Y) * 100;
                }
                List<int> yIndToIgnoreList=null;
                if (yIndToIgnore != null)
                {
                    yIndToIgnoreList = yIndToIgnore.ToList();
                }
                results.objectiveFunctionValue = (new Vec()).SumOfSquareErr(results.Y_modelled, Y, 0, false, yIndToIgnoreList);

                results.Bias = regression.Intercept;
                results.Gains = regression.Weights;
                results.param = Vec<double>.Concat(regression.Weights, regression.Intercept);

                /*
                // uncertainty estimation
                if (false)// unceratinty does not take into account weights now?
                {
                    //start: estimating uncertainty
                    try
                    {
                        double[][] informationMatrix = accordFittingAlgo.GetInformationMatrix();// this should already include weights
                        double mse = 0;
                        if (areAllWeightsOne)
                            mse = regression.GetStandardError(X_T, Y);
                        else
                            mse = regression.GetStandardError(X_T, Mult(weights, Y));
                        double[] SE = regression.GetStandardErrors(mse, informationMatrix);

                        for (int i = 0; i < theta_Length; i++)
                        {
                            varCovarMatrix[i] = new double[theta_Length];
                            for (int j = 0; j < theta_Length; j++)
                            {
                                varCovarMatrix[i][j] = mse * Math.Sqrt(Math.Abs(informationMatrix[i][j]));
                            }
                        }
                        param95prcConfInterval = Mult(SE, 1.96);
                    }

                    catch (Exception e)
                    {
                        param95prcConfInterval = null;
                    }
                }*/
                results.ableToIdentify = true;
                return results;
            }
            catch 
            {
                results.ableToIdentify = false;
                return results;
            }
        }


        /// <summary>
        /// Replace certain values in an array with a new value. 
        /// </summary>
        /// <param name="array">the array to be replaces</param>
        /// <param name="indList">list of all the indices of all data points in array to be replaced</param>
        /// <param name="valueToReplaceWith">the new value to use in place of old values.</param>
        /// <returns>A copy of the original array with the values repalced as specified</returns>
        public static double[] ReplaceIndWithValue(double[] array, List<int> indList,
            double valueToReplaceWith)
        {
            int[] vecInd = indList.ToArray();
            double[] outArray = new double[array.Length] ;
            array.CopyTo(outArray,0);
            for (int curIndInd = 0; curIndInd < vecInd.Length; curIndInd++)
            {
                int curVecInd = vecInd[curIndInd];
                if (curVecInd > 0)
                {
                    outArray[curVecInd] = valueToReplaceWith;
                }
            }
            return array;
        }

        ///<summary>
        /// elementwise  subtraction of array1 and array2, assuming they are same size
        ///</summary>

        public double[] Subtract(double[] array1, double[] array2)
        {
            if (array1 == null || array2 == null)
                return null;

            double[] retVal = new double[array1.Length];

            for (int i = 0; i < array1.Length; i++)
            {
                if (IsNaN(array1[i]) || IsNaN(array2[i]))
                    retVal[i] = valuteToReturnElementIsNaN;
                else
                    retVal[i] = array1[i] - array2[i];
            }
            return retVal;
        }
        ///<summary>
        /// elementwise subtraction of val2 from array1
        ///</summary>
        public double[] Subtract(double[] array1, double val2)
        {
            if (array1 == null)
                return null;
            double[] retVal = new double[array1.Length];
            for (int i = 0; i < array1.Length; i++)
            {
                if (IsNaN(array1[i]))
                    continue;
                retVal[i] = array1[i] - val2;
            }
            return retVal;
        }

        ///<summary>
        /// subtracts val2 from array2 elements
        ///</summary>
        public int[] Subtract(int[] array1, int val2)
        {
            if (array1 == null)
                return null;
            int[] retVal = new int[array1.Length];

            for (int i = 0; i < array1.Length; i++)
            {
                if (IsNaN(array1[i]))
                    continue;
                retVal[i] = array1[i] - val2;
            }
            return retVal;
        }


        ///<summary>
        /// returns the sum of array1
        ///</summary>
        public double? Sum(double[] array1)
        {
            if (array1 == null)
                return null;
            double retVal = 0;
            for (int i = 0; i < array1.Length; i++)
            {
                if (IsNaN(array1[i]))
                    continue;
                retVal += array1[i];
            }
            return retVal;
        }

        ///<summary>
        ///  The sum of absolute errors <c>(|a1-a2|)</c> between <c>array1</c> and <c>array2</c>
        ///</summary>
        public double SumOfAbsErr(double[] array1, double[] array2, int indexOffset = -1)
        {
            int nGoodValues = 0;
            if (indexOffset == -1)
            {
                indexOffset = array2.Count() - array1.Count();
            }
            double ret = 0;
            for (int i = indexOffset; i < array2.Count(); i++)
            {
                if ( IsNaN(array2[i]) || IsNaN(array1[i]) )
                    continue;
                nGoodValues++;
                ret += Math.Abs(array2[i] - array1[i - indexOffset]);
            }
            if (nGoodValues > 0)
                return ret / nGoodValues;
            else
                return 0;
        }


        /// <summary>
        ///  The sum of square errors <c>(a1-a2)^2</c> between <c>array1</c> and <c>array2</c>.
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <param name="ymodOffset"></param>
        /// <param name="divByN">if true, the result is normalized by the number of good values </param>
        /// <param name="indToIgnore">optionally a list of indices of <c>array1</c> to ignore</param>
        /// <returns></returns>
        public double SumOfSquareErr(double[] array1, double[] array2, int ymodOffset = -1, 
            bool divByN = true, List<int> indToIgnore=null)
        {
            if (array1.Count() < array2.Count())
                return Double.NaN;

            if (ymodOffset == -1)
            {
                ymodOffset = array2.Count() - array1.Count();
            }
            double ret = 0;
            int nGoodValues = 0;
            for (int i = ymodOffset; i < array2.Count(); i++)
            {
                if (IsNaN(array2[i]) || IsNaN(array1[i - ymodOffset]) )
                {
                    continue;
                }
                if (indToIgnore != null)
                {
                    if (indToIgnore.Contains(i))
                        continue;
                }
                nGoodValues++;
                ret += Math.Pow(array2[i] - array1[i - ymodOffset], 2);
            }
            if (divByN && nGoodValues > 0)
                return ret / nGoodValues;
            else
                return ret;
        }

        ///<summary>
        /// sum of square error of the vector compared to a constant. by defautl the return value is normalized by dividing by,
        /// this normalization can be turned off
        ///</summary>
        public static double SumOfSquareErr(double[] vec, double constant, bool doNormalization = true)
        {
            double ret = 0;
            for (int i = 0; i < vec.Count(); i++)
            {
                ret += Math.Pow(vec[i] - constant, 2);
            }
            if (doNormalization)
                return ret / vec.Length;
            else
                return ret;
        }

        ///<summary>
        /// sum of square error of the vector compared to itself
        ///</summary>
        public double SelfSumOfSquareErr(double[] vec)
        {
            return SumOfSquareErr(Vec<double>.SubArray(vec, 1), Vec<double>.SubArray(vec, 0, vec.Length - 2), 0);
        }

        ///<summary>
        /// sum of absolute error of the vector compared to itself
        ///</summary>
        public double SelfSumOfAbsErr(double[] vec)
        {
            return SumOfAbsErr(Vec<double>.SubArray(vec, 1), Vec<double>.SubArray(vec, 0, vec.Length - 2), 0);
        }

        /// <summary>
        /// R-squared 
        /// R-squared (R2) is a statistical measure that represents the proportion of the variance for a dependent 
        /// variable that's explained by an independent variable or variables in a regression model. 
        /// Whereas correlation explains the strength of the relationship between an independent and 
        /// dependent variable, R-squared explains to what extent the variance of one variable explains the
        /// variance of the second variable. So, if the R2 of a model is <c>0.50</c>, then approximately 
        /// half of the observed variation can be explained by the model's inputs.
        /// </summary>
        /// <param name="vector1">first vector</param>
        /// <param name="vector2">second vector</param>
        /// <param name="indToIgnoreExt">optionally: indices to be ignored(for instance bad values)</param>
        /// <returns>R2 squared, a value between <c>-1</c> and <c>1</c>. If an error occured, 
        /// <c>Double.PositiveInfinity</c> is returned </returns>
        public double RSquared(double[] vector1, double[] vector2, List<int> indToIgnoreExt=null)
        {
            if (vector1 == null || vector2 == null)
                return Double.PositiveInfinity;

            double[] x_mod_int = new double[vector1.Length];
            double[] x_meas_int = new double[vector2.Length];//
            vector1.CopyTo(x_mod_int, 0);
            vector2.CopyTo(x_meas_int, 0);

            // protect r-squared from -9999 values.
            List<int> minus9999ind = FindValues(vector2, nanValue, TimeSeriesAnalysis.VectorFindValueType.Equal);
            List<int> nanind = FindValues(vector1, Double.NaN, TimeSeriesAnalysis.VectorFindValueType.NaN);
            List<int> indToIgnoreInt = minus9999ind.Union(nanind).ToList();

            List<int> indToIgnore;
            if (indToIgnoreExt != null)
                indToIgnore = indToIgnoreInt.Union(indToIgnoreExt).ToList();
            else
                indToIgnore = indToIgnoreInt;

            foreach (int ind in indToIgnore)
            {
                x_mod_int[ind] = 0;
                x_meas_int[ind] = 0;
            }

            double SSres = SumOfSquareErr(x_mod_int, x_meas_int, -1, false);//explainedVariation
            double meanOfMeas = Mean(x_meas_int).Value;
            double SStot = SumOfSquareErr(x_mod_int, meanOfMeas, false); //totalVariation
            double Rsq = 1 - SSres / SStot;
            return Rsq;
        }




        /// <summary>
        ///  serializes a single vector/array to a file for persistent storage to a human-readable text format
        /// Vector data can then be retreived by companion method <c>Deserialize</c>
        /// </summary>
        /// <param name="vector">vector to be written to afile</param>
        /// <param name="fileName">the file name (or path) of the file to which the vector is to serialized to</param>
        /// <returns></returns>
        static public bool Serialize(double[] vector, string fileName)
        {
            using (StringToFileWriter writer = new StringToFileWriter(fileName))
            {
                foreach (double val in vector)
                {
                    writer.Write(val.ToString("##.###########", CultureInfo.InvariantCulture) +"\r\n");
                }
                writer.Close();
            }
            return true;
        }




        /// <summary>
        /// Create a compact string of vector with a certain number of significant digits and a chosen divider
        /// </summary>
        /// <param name="array"></param>
        /// <param name="nSignificantDigits"></param>
        /// <param name="dividerStr"></param>
        /// <returns></returns>
        public static string ToString(double[] array, int nSignificantDigits, string dividerStr = ";")
        {
            StringBuilder sb = new StringBuilder();
            if (array == null)
            {
                return "null";
            }
            if (array.Length > 0)
            {
                sb.Append("[");
                sb.Append(SignificantDigits.Format(array[0], nSignificantDigits).ToString("", CultureInfo.InvariantCulture));
                for (int i = 1; i < array.Length; i++)
                {
                    sb.Append(dividerStr);
                    sb.Append(SignificantDigits.Format(array[i], nSignificantDigits).ToString("", CultureInfo.InvariantCulture));
                }
                sb.Append("]");
            }
            return sb.ToString();
        }

        ///<summary>
        ///  returns the variance of the array (always apositive number)
        ///</summary>
        public double Var(double[] array1, bool doNormalize = false)
        {
            double retVal = 0;
            double avg = Mean(array1).Value;
            int N = 0;
            for (int i = 0; i < array1.Length; i++)
            {
                if (IsNaN(array1[i]))
                    continue;
                N++;
                retVal += Math.Pow(array1[i] - avg, 2);
            }
            if (doNormalize)
            {
                retVal = retVal / (N);
            }
            return retVal;
        }


        ///<summary>
        /// returns the union of array1 and array2, a list of elements that are in either vector
        ///</summary>
        public static List<int> Union(List<int> vec1, List<int> vec2)
        {
            List<int> c = vec1.Union(vec2).ToList();
            c.Sort();
            return c;
        }

    }
}
