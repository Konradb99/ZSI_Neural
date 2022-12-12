using CarsNeuralNetwork.Handlers;
using CarsNeuralNetwork.Models;
using Microsoft.AspNetCore.Mvc;
using ZSI_Core.Constants;
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
        public async void GetSimpleNetwork()
        {
            Console.WriteLine("\nBegin neural network back-propagation demo");

            int numInput = 4; // number features
            int numHidden = 10;
            int numOutput = 3; // number of classes for Y

            //      ICollection<CarDto> cars = await _dataService.GetTestData();

            double[][] trainData = await _fileService.GetDataSet();
            double[][] testData = TestData.testDataArray;

            Console.WriteLine("Training data:");
            NeuralNetworkHandler.ShowMatrix(trainData, 4, 2, true);
            Console.WriteLine("Test data:");
            NeuralNetworkHandler.ShowMatrix(testData, 4, 2, true);

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
                NeuralNetworkHandler.ShowVector(y, 8, 3, true);
            }

            //double trainAcc = NeuralNetworkHandler.Accuracy(trainData);
            //Console.WriteLine("\nFinal accuracy on training data = " +
            //  trainAcc.ToString("F4"));

            //double testAcc = NeuralNetworkHandler.Accuracy(testData);
            //Console.WriteLine("Final accuracy on test data     = " +
            //  testAcc.ToString("F4"));

            Console.WriteLine("\nEnd back-propagation demo\n");
            Console.ReadLine();

        }
    }
}
