using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsNeuralNetwork
{
    public static class ActivationFunctions //wraz z ich pochodnymi
    {
        public static double[] LinearFunction(double input)
        {
            return new double[] { input, 1 };
        }

        public static double TresholdFunction(double input, double treshold)
        {
            double result = 0;
            if (input > treshold)
            {
                result = 1;
            }
            else if (input < treshold)
            {
                result = -1;
            }

            return result;
        }

        public static double[] HyperTanFunction(double x)
        {
            if (x < -20.0) return new double[2] { -1.0, 0 }; // approximation is correct to 30 decimals
            else if (x > 20.0) return new double[2] { 1.0, 0 };
            else return new double[2] {
                Math.Tanh(x),
                (1 + x) * (1 - x)
            };
        }

    }
}