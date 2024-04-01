using System;
using System.Collections.Generic;

namespace ShantenCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            //ShantenHelper.WriteShantenCSV("Assets/Resources/ShantenTable.csv");
            // 345m222355p12379s 0
            // int[] test = new int[] { 0, 0, 1, 1, 1, 0, 0, 0, 0,
            //                         0, 3, 1, 0, 2, 0, 0, 0, 0,
            //                         1, 1, 1, 0, 0, 0, 1, 0, 1 };
            // var melds = MeldFinder.Find(test, false);
            // Console.WriteLine(MeldFinder.ValueMelds(melds));
            // EXAMPLE USAGE
            // Later replace the calculation with table lookup
            // Test.ShantenTests();
            ShantenTableConstructor.WriteShantenCSV("table.csv");
        }

        // FIXME: this should be moved to shanten helper
        public static int CalculateShanten(int[] hand)
        {
            int[] testMan = new int[9];
            int[] testPin = new int[9]; 
            int[] testSol = new int[9];
            Array.Copy(hand, 0, testMan, 0, 9);
            Array.Copy(hand, 9, testPin, 0, 9);
            Array.Copy(hand, 18, testSol, 0, 9);
            int bestValue = 0;
            // with pairs
            var man = MeldFinder.Find(testMan, true);
            var sol = MeldFinder.Find(testSol, true);
            var pin = MeldFinder.Find(testPin, true);
            // without pairs
            var man2 = MeldFinder.Find(testMan, false);
            var sol2 = MeldFinder.Find(testSol, false);
            var pin2 = MeldFinder.Find(testPin, false);
            // try all combinations of one with pair combo and two without pairs
            // first combine counts into int[4]
            int[] manSet = new int[4];
            int[] solSet = new int[4];
            int[] pinSet = new int[4];
            MeldFinder.Count(man, true, out var tempP, out var tempSet);
            manSet[0] = tempP;
            manSet[1] = tempSet;
            MeldFinder.Count(man2, false, out tempP, out tempSet);
            manSet[2] = tempP;
            manSet[3] = tempSet;
            MeldFinder.Count(sol, true, out tempP, out tempSet);
            solSet[0] = tempP;
            solSet[1] = tempSet;
            MeldFinder.Count(sol2, false, out tempP, out tempSet);
            solSet[2] = tempP;
            solSet[3] = tempSet;
            MeldFinder.Count(pin, true, out tempP, out tempSet);
            pinSet[0] = tempP;
            pinSet[1] = tempSet;
            MeldFinder.Count(pin2, false, out tempP, out tempSet);
            pinSet[2] = tempP;
            pinSet[3] = tempSet;

            for (int i = 0; i < 4; i++)
            {
                int value = 0;
                int set, pSet;
                switch (i)
                {
                    case 0:
                        if (manSet[0]+manSet[1] > 7) continue;
                        set = manSet[1]+pinSet[3]+solSet[3];
                        pSet = manSet[0]+pinSet[2]+solSet[2];
                        value = ((set > 4) ? 8 : 2 * set) + ((pSet > (4 - set)) ? (4 - set) : pSet);
                        value += 1; // pair
                        break;
                    case 1:
                        if (pinSet[0]+pinSet[1] > 7) continue;
                        set = manSet[3]+pinSet[1]+solSet[3];
                        pSet = manSet[2]+pinSet[0]+solSet[2];
                        value = ((set > 4) ? 8 : 2 * set) + ((pSet > (4 - set)) ? (4 - set) : pSet);
                        value += 1; // pair
                        break;
                    case 2:
                        if (solSet[0] + solSet[1] > 7) continue;
                        set = manSet[3]+pinSet[3]+solSet[1];
                        pSet = manSet[2]+pinSet[2]+solSet[0];
                        value = ((set > 4) ? 8 : 2 * set) + ((pSet > (4 - set)) ? (4 - set) : pSet);
                        value += 1; // pair
                        break;
                    case 3: // no pairs at all
                        set = manSet[3]+pinSet[3]+solSet[3];
                        pSet = manSet[2]+pinSet[2]+solSet[2];
                        value = ((set > 4) ? 8 : 2 * set) + ((pSet > (4 - set)) ? (4 - set) : pSet);
                        break;
                }
                if (value > bestValue)
                {
                    bestValue = value;
                }
            }
            // Console.WriteLine("Best value: " + (8 - bestValue).ToString());
            return 8 - bestValue;
        }

        private static void PrintMelds(List<MeldFinder.SmallMeld> melds)
        {
            foreach (var meld in melds)
            {
                Console.WriteLine(meld.MeldType + " " + string.Join(" ", meld.Tiles));
            }
        }
    }
}