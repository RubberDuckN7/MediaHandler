using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalTools
{
    /*
     * TODO: 
     *  - Add more tools here to be in a centralized place.
     */

    public class Conversion
    {
        public static int StringToInt32(string pValue, int pDefaultValue)
        {
            try
            {
                int data = Int32.Parse(pValue);
                return data;
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
                return pDefaultValue;
            }
        }

        public static int BytesToMegabytes(long pBytes)
        {
            pBytes = pBytes / 1024; // To Kb 
            pBytes = pBytes / 1024; // To Mb
            return (int)pBytes;
        }
    }
}
