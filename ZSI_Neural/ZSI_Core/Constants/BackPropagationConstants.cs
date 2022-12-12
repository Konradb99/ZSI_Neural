using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsNeuralCore.Constants
{
    public static class BackPropagationConstants
    {
        public static int MaxEpochs = 1000;
        public static double ExitError = 0.10;
        public static double LearnRate = 0.1;
        public static double Momentum = 0.06;
    }
}