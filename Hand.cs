using System;
using System.Collections.Generic;
namespace ShantenCalculator
{
    public static class Hand
    {
        public const int NUM_TILES = 27;

        public static int[] Parse(string line)
        {
            List<int> indices = new List<int>();
            int[] tiles = new int[NUM_TILES];
            for (int i = 0; i < line.Length; i++)
            {
                int offset = 0;
                bool isNumber = false;
                switch (line[i])
                {
                    case 'm': break;
                    case 'p': offset = 9; break;
                    case 's': offset = 18; break;
                    default: isNumber = true; break;
                }

                if (isNumber)
                {
                    indices.Add(int.Parse("" + line[i]) - 1);
                }
                else
                {
                    indices.ForEach((i) => tiles[i + offset]++);
                    indices.Clear();
                }
            }
            return tiles;
        }

        public static void Print(int[] tiles)
        {
            Console.WriteLine(Encode(tiles));
        }

        public static string Encode(int[] tiles)
        {
            string code = "";
            string[] suffix = new string[] { "m", "p", "s" };
            for (int i = 0; i < 4; i++)
            {
                bool hadTile = false;
                for (int j = 0; j < 9 && (i * 9 + j) < 3 * 9 + 7; j++)
                {
                    int num = tiles[i * 9 + j];
                    if (num != 0)
                    {
                        hadTile = true;
                        code += new string((char)(j + 49), num);
                    }
                }
                if (hadTile)
                {
                    code += suffix[i];
                }
            }
            return code;
        }

    }
}
