using CarsNeuralNetwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsNeuralNetwork.Handlers
{
    public class NeuralNetworkHandler
    {
        public static double[][] MakeMatrix(int rows,
        int cols, double v)
        {
            double[][] result = new double[rows][];
            for (int r = 0; r < result.Length; ++r)
                result[r] = new double[cols];
            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    result[i][j] = v;
            return result;
        }

        public static double[] ComputeOutputs(double[] xValues, NeuralNetwork nn)
        {
            double[] hSums = new double[nn.HiddenNumber]; // hidden nodes sums scratch array
            double[] oSums = new double[nn.OutputNumber]; // output nodes sums

            for (int i = 0; i < xValues.Length; ++i) // copy x-values to inputs
                nn.Inputs[i] = xValues[i];
            // note: no need to copy x-values unless you implement a ToString.
            // more efficient is to simply use the xValues[] directly.

            for (int j = 0; j < nn.HiddenNumber; ++j)  // compute i-h sum of weights * inputs
                for (int i = 0; i < nn.InputNumber; ++i)
                    hSums[j] += nn.Inputs[i] * nn.ihWeights[i][j]; // note +=

            for (int i = 0; i < nn.HiddenNumber; ++i)  // add biases to hidden sums
                hSums[i] += nn.hBiases[i];

            for (int i = 0; i < nn.HiddenNumber; ++i)   // apply activation
                nn.hOutputs[i] = ActivationFunctions.HyperTanFunction(hSums[i])[0]; // hard-coded

            for (int j = 0; j < nn.OutputNumber; ++j)   // compute h-o sum of weights * hOutputs
                for (int i = 0; i < nn.HiddenNumber; ++i)
                    oSums[j] += nn.hOutputs[i] * nn.hoWeights[i][j];

            for (int i = 0; i < nn.OutputNumber; ++i)  // add biases to output sums
                oSums[i] += nn.oBiases[i];

            double[] softOut = Softmax(oSums); // all outputs at once for efficiency
            Array.Copy(softOut, nn.Outputs, softOut.Length);

            double[] retResult = new double[nn.OutputNumber]; // could define a GetOutputs
            Array.Copy(nn.Outputs, retResult, retResult.Length);
            return retResult;
        }

        public static double[] Softmax(double[] oSums)
        {
            // does all output nodes at once so scale
            // doesn't have to be re-computed each time

            double sum = 0.0;
            for (int i = 0; i < oSums.Length; ++i)
                sum += Math.Exp(oSums[i]);

            double[] result = new double[oSums.Length];
            for (int i = 0; i < oSums.Length; ++i)
                result[i] = Math.Exp(oSums[i]) / sum;

            return result; // now scaled so that xi sum to 1.0
        }

        public static int MaxIndex(double[] vector) // helper for Accuracy()
        {
            // index of largest value
            int bigIndex = 0;
            double biggestVal = vector[0];
            for (int i = 0; i < vector.Length; ++i)
            {
                if (vector[i] > biggestVal)
                {
                    biggestVal = vector[i];
                    bigIndex = i;
                }
            }
            return bigIndex;
        }

        public static void Shuffle(int[] sequence) // instance method
        {
            Random rnd = new Random();
            for (int i = 0; i < sequence.Length; ++i)
            {
                int r = rnd.Next(i, sequence.Length);
                int tmp = sequence[r];
                sequence[r] = sequence[i];
                sequence[i] = tmp;
            }
        } // Shuffle

        public static void ShowVector(double[] vector, int decimals,
     int lineLen, bool newLine)
        {
            for (int i = 0; i < vector.Length; ++i)
            {
                if (i > 0 && i % lineLen == 0) Console.WriteLine("");
                if (vector[i] >= 0) Console.Write(" ");
                Console.Write(vector[i].ToString("F" + decimals) + " ");
            }
            if (newLine == true)
                Console.WriteLine("");
        }

        public static void ShowMatrix(double[][] matrix, int numRows,
     int decimals, bool indices)
        {
            int len = matrix.Length.ToString().Length;
            for (int i = 0; i < numRows; ++i)
            {
                if (indices == true)
                    Console.Write("[" + i.ToString().PadLeft(len) + "]  ");
                for (int j = 0; j < matrix[i].Length; ++j)
                {
                    double v = matrix[i][j];
                    if (v >= 0.0)
                        Console.Write(" "); // '+'
                    Console.Write(v.ToString("F" + decimals) + "  ");
                }
                Console.WriteLine("");
            }
        }
    }
}