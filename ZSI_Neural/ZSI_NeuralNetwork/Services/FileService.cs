using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSI_Core.Models;

namespace ZSI_NeuralNetwork.Services
{
    public class FileService : IFileService
    {
        public async Task<double[][]> GetDataSet()
        {
            double[][] result = new double[150][];
            int k = 0;
            using (StreamReader r = new StreamReader("DataSet/iris.json"))
            {
                string json = r.ReadToEnd();
                List<Iris> items = JsonConvert.DeserializeObject<List<Iris>>(json);
                foreach (var item in items)
                {
                    switch (item.species)
                    {
                        case "setosa":
                            double[] setosaItem = new double[] 
                            { 
                                item.sepalLength, 
                                item.sepalWidth, 
                                item.petalLength, 
                                item.petalWidth, 
                                0,
                                0,
                                1
                            };
                            result[k] = setosaItem;
                            break;
                        case "versicolor":
                            double[] versicolorItem = new double[]
                            {
                                item.sepalLength,
                                item.sepalWidth,
                                item.petalLength,
                                item.petalWidth,
                                0,
                                1,
                                0
                            };
                            result[k] = versicolorItem;
                            break;
                        case "virginica":
                            double[] virginicaItem = new double[]
                            {
                                item.sepalLength,
                                item.sepalWidth,
                                item.petalLength,
                                item.petalWidth,
                                1,
                                0,
                                0
                            };
                            result[k] = virginicaItem;
                            break;
                    }
                    k = k + 1;
                }
            }
            return result;
        }
    }
}
