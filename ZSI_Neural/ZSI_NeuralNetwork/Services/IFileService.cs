using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSI_NeuralNetwork.Services
{
    public interface IFileService
    {
        public Task<double[][]> GetDataSet();
    }
}
