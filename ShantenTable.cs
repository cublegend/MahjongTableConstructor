using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace ShantenCalculator {
    public static class ShantenTable
    {
        public static Dictionary<byte[], byte[]> dictionary = new Dictionary<byte[], byte[]>();
        private static string path = "Assets/Resources/ShantenTable.csv";

        public static void LoadTable()
        {
            // if (File.Exists(path))
            // {
            //     string[] lines = File.ReadAllLines(path);
            //     foreach (string line in lines)
            //     {
            //         int value = int.Parse(line);
            //         KeyValuePair<byte[], byte[]> entry = Decode(value);
            //         dictionary.Add(entry.Key, entry.Value);
            //     }
            // }
            // else
            // {
            //     throw new FileNotFoundException("ShantenTable.csv not found");
            // }
        }

        /// <summary>
        /// Encode a key of byte[9] (27bit) to a UInt32
        /// Encode a value of byte[4] (12bit) to a UInt16
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static (UInt32, UInt16) Encode(byte[] key, byte[] value)
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