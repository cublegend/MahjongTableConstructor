using System;
using System.Collections.Generic;

namespace ShantenCalculator {
    /// <summary>
    /// How to use the shanten table constructed
    /// </summary>
    public static class ShantenTable
    {
        public static string path = "table.csv";
        public static Dictionary<UInt32, UInt16> table = new();
        public static bool TableLoaded = false;

        public static void LoadTable()
        {
            if (TableLoaded) return;
            string[] lines = System.IO.File.ReadAllLines(path);
            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                UInt32 key = Convert.ToUInt32(parts[0]);
                UInt16 value = Convert.ToUInt16(parts[1]);
                table[key] = value;
            }
            TableLoaded = true;
        }

        /// <summary>
        /// Given a suit of int[9], return an int[4]
        /// where the elements are # of:
        /// 0: pair-partial-set
        /// 1: pair-full-set
        /// 2: non-pair-partial-set
        /// 3: non-pair-full-set
        /// </summary>
        /// <param name="suit"></param>
        /// <returns></returns>
        public static int[] GetSuitSets(int[] suit)
        {
            if (!TableLoaded) LoadTable();
            
            UInt32 key = EncodeKey(suit);
            if (table.ContainsKey(key))
            {
                return DecodeValue(table[key]);
            }
            return new int[4];
        }

        private static UInt32 EncodeKey(int[] suit)
        {
            UInt32 key = 0;
            for (int i = 0; i < 9; i++)
            {
                key |= (UInt32)suit[i] << (i * 3);
            }
            return key;
        }

        private static int[] DecodeValue(UInt16 value)
        {
            int[] sets = new int[4];
            sets[0] = value & 0b111;
            sets[1] = (value >> 3) & 0b111;
            sets[2] = (value >> 6) & 0b111;
            sets[3] = (value >> 9) & 0b111;
            return sets;
        }

    }
}
