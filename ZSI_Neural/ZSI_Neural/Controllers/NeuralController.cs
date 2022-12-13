using CarsNeuralNetwork.Handlers;
using CarsNeuralNetwork.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using ZSI_Core.Constants;
using ZSI_Core.Models;
using ZSI_NeuralNetwork.Services;

namespace ZSI_Neural.Controllers
{
    [ApiController]
    [Route("/[controller]/[action]")]
    public class NeuralController : Controller
    {
        private readonly IFileService _fileService;

        public NeuralController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<ResponseString> PredictFlower([FromQuery]IrisPredict irisToPredict)
        {
            Console.WriteLine("\nBegin neural network back-propagation demo");

            int numInput = 4; // number features
            int numHidden = 10;
            int numOutput = 3; // number of classes for Y

            //      ICollection<CarDto> cars = await _dataService.GetTestData();

            double[][] trainData = await _fileService.GetDataSet();

            double[] testDataSingle = new double[] { irisToPredict.sepalLength, irisToPredict.sepalWidth, irisToPredict.petalLength, irisToPredict.petalWidth };

            double[][] testData = new double[1][] { testDataSingle };

            Console.WriteLine("Training data:");
            NeuralNetworkHandler.ShowMatrix(trainData, 4, 2, true);
            Console.WriteLine("Test data:");
            NeuralNetworkHandler.ShowMatrix(testData, 1, 2, true);

            Console.WriteLine("Creating a " + numInput + "-" + numHidden +
              "-" + numOutput + " neural network");
            NeuralNetwork nn = new NeuralNetwork(numInput, numHidden, numOutput);
            NeuralNetworkTrainer trainer = new NeuralNetworkTrainer(nn);

            int maxEpochs = 1000;
            double learnRate = 0.05;
            double momentum = 0.01;
            Console.WriteLine("\nSetting maxEpochs = " + maxEpochs);
            Console.WriteLine("Setting learnRate = " + learnRate.ToString("F2"));
            Console.WriteLine("Setting momentum  = " + momentum.ToString("F2"));

            Console.WriteLine("\nStarting training");
            double[] weights = trainer.Train(trainData, maxEpochs, learnRate, momentum);
            Console.WriteLine("Done");
            Console.WriteLine("\nFinal neural network model weights and biases:\n");
            NeuralNetworkHandler.ShowVector(weights, 2, 10, true);

            for (int i = 0; i < testData.Length; i++)
            {
                double[] y = NeuralNetworkHandler.ComputeOutputs(testData[i], nn);

                int maxIndex = Array.IndexOf(y, y.Max());

                NeuralNetworkHandler.ShowVector(y, 3, 3, true);
                switch (maxIndex)
                {
                    case 0:
                        return new ResponseString { response = ReturnConstants.Setosa };
                        break;
                    case 1:
                        return new ResponseString { response = ReturnConstants.Versicolor };
                        break;
                    case 2:
                        return new ResponseString { response = ReturnConstants.Virginica };
                        break;
                }
            }
            throw new Exception("Error predicting flower type");
        }
    }
}
