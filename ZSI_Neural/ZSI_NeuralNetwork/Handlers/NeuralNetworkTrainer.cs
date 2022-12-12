using CarsNeuralNetwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsNeuralNetwork.Handlers
{
    public class NeuralNetworkTrainer
    {
        private NeuralNetwork nn;

        public NeuralNetworkTrainer(NeuralNetwork nn)
        {
            this.nn = nn;
        }

        public double[] Train(double[][] trainData, int maxEpochs,
        double learnRate, double momentum)
        {
            // train using back-prop
            // back-prop specific arrays
            double[][] hoGrads = NeuralNetworkHandler.MakeMatrix(nn.HiddenNumber, nn.OutputNumber, 0.0); // hidden-to-output weight gradients
            double[] obGrads = new double[nn.OutputNumber];                   // output bias gradients

            double[][] ihGrads = NeuralNetworkHandler.MakeMatrix(nn.InputNumber, nn.HiddenNumber, 0.0);  // input-to-hidden weight gradients
            double[] hbGrads = new double[nn.HiddenNumber];                   // hidden bias gradients

            double[] oSignals = new double[nn.OutputNumber];                  // local gradient output signals - gradients w/o associated input terms
            double[] hSignals = new double[nn.HiddenNumber];                  // local gradient hidden node signals

            // back-prop momentum specific arrays
            double[][] ihPrevWeightsDelta = NeuralNetworkHandler.MakeMatrix(nn.InputNumber, nn.HiddenNumber, 0.0);
            double[] hPrevBiasesDelta = new double[nn.HiddenNumber];
            double[][] hoPrevWeightsDelta = NeuralNetworkHandler.MakeMatrix(nn.HiddenNumber, nn.OutputNumber, 0.0);
            double[] oPrevBiasesDelta = new double[nn.OutputNumber];

            int epoch = 0;
            double[] xValues = new double[nn.InputNumber]; // inputs
            double[] tValues = new double[nn.OutputNumber]; // target values
            double derivative = 0.0;
            double errorSignal = 0.0;

            int[] sequence = new int[trainData.Length];
            for (int i = 0; i < sequence.Length; ++i)
                sequence[i] = i;

            int errInterval = maxEpochs / 10; // interval to check error
            while (epoch < maxEpochs)
            {
                ++epoch;

                if (epoch % errInterval == 0 && epoch < maxEpochs)
                {
                    double trainErr = Error(trainData);
                    Console.WriteLine("epoch = " + epoch + "  error = " +
                      trainErr.ToString("F4"));
                    //Console.ReadLine();
                }

                NeuralNetworkHandler.Shuffle(sequence); // visit each training data in random order
                for (int ii = 0; ii < trainData.Length; ++ii)
                {
                    int idx = sequence[ii];
                    Array.Copy(trainData[idx], xValues, nn.InputNumber);
                    Array.Copy(trainData[idx], nn.InputNumber, tValues, 0, nn.OutputNumber);
                    NeuralNetworkHandler.ComputeOutputs(xValues, nn); // copy xValues in, compute outputs

                    // indices: i = inputs, j = hiddens, k = outputs

                    // 1. compute output node signals (assumes softmax)
                    for (int k = 0; k < nn.OutputNumber; ++k)
                    {
                        errorSignal = tValues[k] - nn.Outputs[k];  // Wikipedia uses (o-t)
                        derivative = (1 - nn.Outputs[k]) * nn.Outputs[k]; // for softmax
                        oSignals[k] = errorSignal * derivative;
                    }

                    // 2. compute hidden-to-output weight gradients using output signals
                    for (int j = 0; j < nn.HiddenNumber; ++j)
                        for (int k = 0; k < nn.OutputNumber; ++k)
                            hoGrads[j][k] = oSignals[k] * nn.hOutputs[j];

                    // 2b. compute output bias gradients using output signals
                    for (int k = 0; k < nn.OutputNumber; ++k)
                        obGrads[k] = oSignals[k] * 1.0; // dummy assoc. input value

                    // 3. compute hidden node signals
                    for (int j = 0; j < nn.HiddenNumber; ++j)
                    {
                        derivative = (1 + nn.hOutputs[j]) * (1 - nn.hOutputs[j]); // for tanh
                        double sum = 0.0; // need sums of output signals times hidden-to-output weights
                        for (int k = 0; k < nn.OutputNumber; ++k)
                        {
                            sum += oSignals[k] * nn.hoWeights[j][k]; // represents error signal
                        }
                        hSignals[j] = derivative * sum;
                    }

                    // 4. compute input-hidden weight gradients
                    for (int i = 0; i < nn.InputNumber; ++i)
                        for (int j = 0; j < nn.HiddenNumber; ++j)
                            ihGrads[i][j] = hSignals[j] * nn.Inputs[i];

                    // 4b. compute hidden node bias gradients
                    for (int j = 0; j < nn.HiddenNumber; ++j)
                        hbGrads[j] = hSignals[j] * 1.0; // dummy 1.0 input

                    // == update weights and biases

                    // update input-to-hidden weights
                    for (int i = 0; i < nn.InputNumber; ++i)
                    {
                        for (int j = 0; j < nn.HiddenNumber; ++j)
                        {
                            double delta = ihGrads[i][j] * learnRate;
                            nn.ihWeights[i][j] += delta; // would be -= if (o-t)
                            nn.ihWeights[i][j] += ihPrevWeightsDelta[i][j] * momentum;
                            ihPrevWeightsDelta[i][j] = delta; // save for next time
                        }
                    }

                    // update hidden biases
                    for (int j = 0; j < nn.HiddenNumber; ++j)
                    {
                        double delta = hbGrads[j] * learnRate;
                        nn.hBiases[j] += delta;
                        nn.hBiases[j] += hPrevBiasesDelta[j] * momentum;
                        hPrevBiasesDelta[j] = delta;
                    }

                    // update hidden-to-output weights
                    for (int j = 0; j < nn.HiddenNumber; ++j)
                    {
                        for (int k = 0; k < nn.OutputNumber; ++k)
                        {
                            double delta = hoGrads[j][k] * learnRate;
                            nn.hoWeights[j][k] += delta;
                            nn.hoWeights[j][k] += hoPrevWeightsDelta[j][k] * momentum;
                            hoPrevWeightsDelta[j][k] = delta;
                        }
                    }

                    // update output node biases
                    for (int k = 0; k < nn.OutputNumber; ++k)
                    {
                        double delta = obGrads[k] * learnRate;
                        nn.oBiases[k] += delta;
                        nn.oBiases[k] += oPrevBiasesDelta[k] * momentum;
                        oPrevBiasesDelta[k] = delta;
                    }

                } // each training item

            } // while
            double[] bestWts = nn.GetWeights();
            return bestWts;
        } // Train

        private double Error(double[][] trainData)
        {
            // average squared error per training item
            double sumSquaredError = 0.0;
            double[] xValues = new double[nn.InputNumber]; // first numInput values in trainData
            double[] tValues = new double[nn.OutputNumber]; // last numOutput values

            // walk thru each training case. looks like (6.9 3.2 5.7 2.3) (0 0 1)
            for (int i = 0; i < trainData.Length; ++i)
            {
                Array.Copy(trainData[i], xValues, nn.InputNumber);
                Array.Copy(trainData[i], nn.InputNumber, tValues, 0, nn.OutputNumber); // get target values
                double[] yValues = NeuralNetworkHandler.ComputeOutputs(xValues, nn); // outputs using current weights
                for (int j = 0; j < nn.OutputNumber; ++j)
                {
                    double err = tValues[j] - yValues[j];
                    sumSquaredError += err * err;
                }
            }
            return sumSquaredError / trainData.Length;
        } // MeanSquaredError

        public double Accuracy(double[][] testData)
        {
            // percentage correct using winner-takes all
            int numCorrect = 0;
            int numWrong = 0;
            double[] xValues = new double[nn.InputNumber]; // inputs
            double[] tValues = new double[nn.OutputNumber]; // targets
            double[] yValues; // computed Y

            for (int i = 0; i < testData.Length; ++i)
            {
                Array.Copy(testData[i], xValues, nn.InputNumber); // get x-values
                Array.Copy(testData[i], nn.InputNumber, tValues, 0, nn.OutputNumber); // get t-values
                yValues = NeuralNetworkHandler.ComputeOutputs(xValues, nn);
                int maxIndex = NeuralNetworkHandler.MaxIndex(yValues); // which cell in yValues has largest value?
                int tMaxIndex = NeuralNetworkHandler.MaxIndex(tValues);

                if (maxIndex == tMaxIndex)
                    ++numCorrect;
                else
                    ++numWrong;
            }
            return (numCorrect * 1.0) / (numCorrect + numWrong);
        }

    }
}