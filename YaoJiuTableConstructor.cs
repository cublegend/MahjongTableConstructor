using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace ShantenCalculator {
    /// All possible configurations for yaojiu can include:
    /// 12 choose 4:
    /// 123, 111, 789, 999 x3 suits
    /// and 6 choose 1:
    /// 11, 99 x3 suits
    /// with a total of 495*6 = 2970 configurations
    /// however, since we need to consider open hands,
    /// we need to consider all the above configurations
    /// with 1, 2, 3, or 4 yaojiu tiles open
    public static class YaoJiuTableConstructor
    {
        // we use a hashset since combinatorial procedure would result in repeated entries
        private static HashSet<UInt32> table = new();
        private static byte[][] yaojiuConfigs = {
            [1,1,1,0,0,0,0,0,0],
            [3,0,0,0,0,0,0,0,0],
            [0,0,0,0,0,0,1,1,1],
            [0,0,0,0,0,0,0,0,3]
        };
        private static byte[][] yaojiuPairs = {
            [2,0,0,0,0,0,0,0,0],
            [0,0,0,0,0,0,0,0,2]
        };

        public static void CreateYaojiuTable(string path)
        {
            GenerateAllConfigs([2,0,0,0,0,0,0,0,0], 12);
            GenerateAllConfigs([0,0,0,0,0,0,0,0,2], 12);
            WriteCSV(table, path);
        }

        // a configuration is a byte array of 9 elements
        // this only considers one suit, since all suits are equivalent
        // the function recursively generates all possible configurations
        private static void GenerateAllConfigs(byte[] currentConfig, int remainingTiles)
        {
            table.Add(Encode(currentConfig));
            
            if (remainingTiles == 0)
            {
                return;
            }

            for (int i = 0; i < yaojiuConfigs.Length; i++)
            {
                // check if 1 or 9 exceeds 4 tiles which is impossible
                if (currentConfig[0] + yaojiuConfigs[i][0] > 4) continue;
                else if (currentConfig[8] + yaojiuConfigs[i][8] > 4) continue;

                byte[] newConfig = new byte[9];
                Array.Copy(currentConfig, newConfig, 9);
                for (int j = 0; j < 9; j++)
                {
                    newConfig[j] += yaojiuConfigs[i][j];
                }
                GenerateAllConfigs(newConfig, remainingTiles - 3);
            }
        }

        private static UInt32 Encode(byte[] key)
        {
            if (key.Length != 9)
                throw new ArgumentException("Invalid key length.");

            UInt32 keyInt = 0;

            // Encode the key into a UInt32
            for (int i = 0; i < 9; i++)
            {
                // Ensure the value fits within 3 bits
                keyInt |= (UInt32)(key[i] & 0x07) << (3 * i);
            }
            // Return a tuple consisting of the encoded key and value
            return keyInt;
        }

        private static void WriteCSV(IEnumerable<UInt32> values, string path)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path))
            {
                foreach (var value in values)
                {
                    file.WriteLine(value);
                }
            }
        }
        
    }
}
