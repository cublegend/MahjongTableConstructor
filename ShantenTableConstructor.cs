using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace ShantenCalculator {
    public static class ShantenTableConstructor
    {
        private static ConcurrentBag<(UInt32, UInt16)> A = new ConcurrentBag<(UInt32, UInt16)>();
        private static int processTracker = 0;

        public static void WriteShantenCSV(string path)
        {
            GenerateAllConfigs(new byte[9], 0, 14);
            CSVWriter.WriteCSV(A, path);
        }

        private static void MarkConfig(byte[] key)
        {
            int[] iKey = new int[9];
            Array.Copy(key, iKey, 9);
            var pMelds = MeldFinder.Find(iKey, true);
            var npMelds = MeldFinder.Find(iKey, false);
            MeldFinder.Count(pMelds, true, out var pSets, out var cSets);
            MeldFinder.Count(npMelds, false, out var pSets2, out var cSets2);
            byte[] value = new byte[4] { (byte)pSets, (byte)cSets, (byte)pSets2, (byte)cSets2 };
            A.Add(ShantenTable.Encode(key, value));

            if (Interlocked.Increment(ref processTracker) % 5000 == 0)
            {
                Console.WriteLine($"Progress: {processTracker}");
            }
        }

        private static void GenerateAllConfigs(byte[] currentConfig, int currentIndex, int remainingTiles)
        {
            if (currentIndex == 9) // Base case: All tile types have been considered
            {
                MarkConfig(currentConfig);
                return;
            }

            // Recursive case with parallel execution
            Parallel.For(0, Math.Min(5, remainingTiles + 1), (count) =>
            {
                byte[] newConfig = new byte[9];
                Array.Copy(currentConfig, newConfig, 9);
                newConfig[currentIndex] = (byte)count;
                GenerateAllConfigs(newConfig, currentIndex + 1, remainingTiles - count);
            });
        }
    }
}
