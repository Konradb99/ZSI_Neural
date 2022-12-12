using CarsNeuralNetwork.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsNeuralNetwork.Models
{
    public class NeuralNetwork
    {
        public double[][] ihWeights; // input-hidden
        public double[] hBiases;
        public double[] hOutputs;

        public double[][] hoWeights; // hidden-output
        public double[] oBiases;

        private Random rnd;

        public NeuralNetwork(int inputNumber, int numHidden, int outputNumber)
        {
            InputNumber = inputNumber;
            HiddenNumber = numHidden;
            OutputNumber = outputNumber;
            rnd = new Random();

            Inputs = new double[inputNumber];

            this.ihWeights = NeuralNetworkHandler.MakeMatrix(inputNumber, numHidden, 0.0);
            this.hBiases = new double[numHidden];
            this.hOutputs = new double[numHidden];

            this.hoWeights = NeuralNetworkHandler.MakeMatrix(numHidden, outputNumber, 0.0);
            this.oBiases = new double[outputNumber];
            Outputs = new double[outputNumber];

            WeightsNumber = (inputNumber * numHidden) +
                 (numHidden * outputNumber) + numHidden + outputNumber;
            double[] initialWeights = new double[WeightsNumber];
            for (int i = 0; i < initialWeights.Length; ++i)
                initialWeights[i] = (0.001 - 0.0001) * rnd.NextDouble() + 0.0001;
            this.SetWeights(initialWeights);
        }

        public int InputNumber
        {
            get; set;
        }

        public int HiddenNumber
        {
            get; set;
        }

        public int OutputNumber
        {
            get; set;
        }

        public int WeightsNumber
        {
            get; set;
        }

        public double[] Inputs
        {
            get; set;
        }

        public double[] Outputs
        {
            get; set;
        }

        public void SetWeights(double[] weights)
        {
            // copy serialized weights and biases in weights[] array
            // to i-h weights, i-h biases, h-o weights, h-o biases
            int numWeights = (InputNumber * HiddenNumber) +
              (HiddenNumber * OutputNumber) + HiddenNumber + OutputNumber;
            if (weights.Length != numWeights)
                throw new Exception("Bad weights array in SetWeights");

            int k = 0; // points into weights param

            for (int i = 0; i < InputNumber; ++i)
                for (int j = 0; j < HiddenNumber; ++j)
                    ihWeights[i][j] = weights[k++];
            for (int i = 0; i < HiddenNumber; ++i)
                hBiases[i] = weights[k++];
            for (int i = 0; i < HiddenNumber; ++i)
                for (int j = 0; j < OutputNumber; ++j)
                    hoWeights[i][j] = weights[k++];
            for (int i = 0; i < OutputNumber; ++i)
                oBiases[i] = weights[k++];
        }

        public double[] GetWeights()
        {
            int numWeights = (InputNumber * HiddenNumber) +
              (HiddenNumber * OutputNumber) + HiddenNumber + OutputNumber;
            double[] result = new double[numWeights];
            int k = 0;
            for (int i = 0; i < ihWeights.Length; ++i)
                for (int j = 0; j < ihWeights[0].Length; ++j)
                    result[k++] = ihWeights[i][j];
            for (int i = 0; i < hBiases.Length; ++i)
                result[k++] = hBiases[i];
            for (int i = 0; i < hoWeights.Length; ++i)
                for (int j = 0; j < hoWeights[0].Length; ++j)
                    result[k++] = hoWeights[i][j];
            for (int i = 0; i < oBiases.Length; ++i)
                result[k++] = oBiases[i];
            return result;
        }
    }
}