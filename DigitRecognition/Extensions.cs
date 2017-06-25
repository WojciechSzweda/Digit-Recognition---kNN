using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognition
{
    public static class Extensions
    {
        public static int[] ToIntArray(this string[] dataString)
        {
            var data = new int[dataString.Length];
            for (int i = 0; i < dataString.Length; i++)
            {
                data[i] = Convert.ToInt32(dataString[i]);
            }
            return data;
        }
    }
}
