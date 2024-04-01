using System;
using System.Collections.Concurrent;

namespace ShantenCalculator {
    public static class ShantenTableConstructor
    {
        private static readonly ConcurrentBag<(UInt32, UInt16)> A = new();
        private static int processTracker = 0;

        public static void WriteShantenCSV(string path)
        {
            GenerateAllConfigs(new byte[9], 0, 14);
            // remove all repetitive entries
            WriteCSV(new HashSet<(UInt32, UInt16)>(A), path);
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
            A.Add(Encode(key, value));

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

        private static void WriteCSV(IEnumerable<(UInt32, UInt16)> values, string path)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path))
            {
                foreach (var value in values)
                {
                    file.WriteLine(value.Item1 + ": " + value.Item2);
                }
            }
        }

        /// <summary>
        /// Encode a key of byte[9] (27bit) to a UInt32
        /// Encode a value of byte[4] (12bit) to a UInt16
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static (UInt32, UInt16) Encode(byte[] key, byte[] value)
        {
            if (key.Length != 9 || value.Length != 4)
                throw new ArgumentException("Invalid key or value length.");

            UInt32 keyInt = 0;
            UInt16 valueInt = 0;

            // Encode the key into a UInt32
            for (int i = 0; i < 9; i++)
            {
                // Ensure the value fits within 3 bits
                keyInt |= (UInt32)(key[i] & 0x07) << (3 * i);
            }

            // Encode the value into a UInt16
            for (int i = 0; i < 4; i++)
            {
                // Ensure the value fits within 3 bits
                valueInt |= (UInt16)((value[i] & 0x07) << (3 * i));
            }

            // Return a tuple consisting of the encoded key and value
            return (keyInt, valueInt);
        }

    }
}
