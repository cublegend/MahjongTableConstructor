using System;
using System.Collections.Generic;
/// <summary>
/// A CSV writer class that writes an array int[] to a CSV file.
/// </summary>
namespace ShantenCalculator {
    public static class CSVWriter
    {
        public static void WriteCSV(IEnumerable<(UInt32, UInt16)> values, string path)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path))
            {
                foreach (var value in values)
                {
                    file.WriteLine(value.Item1 + ": " + value.Item2);
                }
            }
        }
    }
}
