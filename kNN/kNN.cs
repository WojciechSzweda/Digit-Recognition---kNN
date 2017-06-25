using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace knnLib
{
    public class kNN
    {
        public DataSetStruct[] DataSet { get; set; }
        public int K { get; set; }

        public struct DataSetStruct
        {
            public int[] Data { get; set; }
            public double Distance { get; set; }
        }

        public kNN(int[][] _dataSet, int k)
        {
            DataSet = new DataSetStruct[_dataSet.GetLength(0)];
            for (int i = 0; i < _dataSet.GetLength(0); i++)
            {
                DataSet[i] = new DataSetStruct { Data = _dataSet[i] };
            }
            K = k;
        }

        private void CalcDistances(int[] sample)
        {
            for (int i = 0; i < DataSet.Length; i++)
            {
                int Sum = 0;
                var data = DataSet[i].Data;
                for (int j = 0; j < sample.Length - 1; j++)
                {
                    var diff = data[j] - sample[j];
                    Sum += diff * diff;
                }
                DataSet[i].Distance = Math.Sqrt(Sum);
            }
        }

        private DataSetStruct[] SortDataSet()
        {
            return DataSet.OrderBy(x => x.Distance).ToArray();
        }

        public double Evaluate(int[] sample)
        {
            CalcDistances(sample);
            DataSet = SortDataSet();

            var knn = GetNearest(K);

            return knn.GroupBy(x => x.Data.Last()).OrderByDescending(x => x.Count()).FirstOrDefault().Key;
        }

        private DataSetStruct[] GetNearest(int k)
        {
            return DataSet.Take(k).ToArray();
        }
    }
}
