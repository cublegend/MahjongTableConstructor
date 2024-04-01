using System;
using System.Collections.Generic;

namespace ShantenCalculator
{
    public static class MeldFinder
    {
        /// <summary>
        /// Disassemble a set of tiles of one suit (int[9]) into a list of Melds. 
        /// This list contains the best combo of sets, pairs and partial sets where sets are valued 2 points and the rest 1.
        /// </summary>
        public static List<SmallMeld> Find(int[] tiles, bool withPair) =>
            withPair ? FindBestMeldComboWithOnePair(tiles) : FindBestMeldCombo(tiles, new List<SmallMeld>(), new List<SmallMeld>());
        /// <summary>
        /// From a given set of tiles (int[9]) list all possible combinations into sets, pSets and pairs.
        /// </summary>
        public static List<SmallMeld> FindAllGroups(int[] tiles)
        {
            List<SmallMeld> groups = new List<SmallMeld>();
            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i] >= 3) // 111
                {
                    int[] group = new int[tiles.Length];
                    group[i] = 3;
                    groups.Add(new SmallMeld(group, MeldType.Set));
                }
            }

            //we have to prioritize runs over partial sets because that way we can get better shanten with less groups
            for (int i = 0; i < 7; i++)
            {
                if (tiles[i] >= 1 && tiles[i + 1] >= 1 && tiles[i + 2] >= 1)
                {
                    int[] group = new int[tiles.Length];
                    group[i] = 1; group[i + 1] = 1; group[i + 2] = 1;
                    groups.Add(new SmallMeld(group, MeldType.Set));
                }
            }

            for (int i = 0; i < tiles.Length; i++)
            {
                if (i < 7) //13
                {
                    if (tiles[i] >= 1 && tiles[i + 2] >= 1)
                    {
                        int[] group = new int[tiles.Length];
                        group[i] = 1; group[i + 2] = 1;
                        groups.Add(new SmallMeld(group, MeldType.PSet));
                    }
                }
                if (i < 8) //12
                {
                    if (tiles[i] >= 1 && tiles[i + 1] >= 1)
                    {
                        int[] group = new int[tiles.Length];
                        group[i] = 1; group[i + 1] = 1;
                        groups.Add(new SmallMeld(group, MeldType.PSet));
                    }
                }
                // partial sets
                if (tiles[i] >= 2)
                {
                    int[] group = new int[tiles.Length];
                    group[i] = 2;
                    groups.Add(new SmallMeld(group, MeldType.PSet));
                }
            }
            return groups;
        }

        private static List<SmallMeld> FindAllPairs(int[] tiles)
        {
            List<SmallMeld> groups = new List<SmallMeld>();

            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i] >= 2)
                {
                    int[] group = new int[tiles.Length];
                    group[i] = 2;
                    groups.Add(new SmallMeld(group, MeldType.Pair));
                }
            }

            return groups;
        }

        private static int CompareMelds(ICollection<SmallMeld> melds1, ICollection<SmallMeld> melds2)
        {
            Count(melds1, false, out int pSets1, out int sets1);
            Count(melds2, false, out int pSets2, out int sets2);
            int value1 = ValueMelds(melds1);
            int value2 = ValueMelds(melds2);
            if (value1 > value2)
            {
                return 1;
            }
            else if (value1 < value2)
            {
                return -1;
            }
            else
            {
                // when values are the same, prioritize sets over psets
                if (sets1 > sets2)
                {
                    return 1;
                }
                else if (sets1 < sets2)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Value a list of melds. every PSet is counted as 1, and every set is counted as 0
        /// the smaller the value, the smaller the shanten
        /// </summary>
        public static int ValueMelds(ICollection<SmallMeld> melds)
        {
            int pSets = 0;
            int sets = 0;
            foreach (SmallMeld meld in melds)
            {
                switch (meld.MeldType)
                {
                    case MeldType.Set:
                        sets++;
                        break;
                    case MeldType.PSet:
                        pSets++;
                        break;
                }
            }
            return ((sets > 4) ? 8 : 2 * sets) + ((pSets > (4 - sets)) ? (4 - sets) : pSets);
        }

        /// <summary>
        /// Find the best possible meld combo and increment a reference value accordingly.
        /// </summary>
        public static void Count(ICollection<SmallMeld> melds, bool needPair, out int pSets, out int sets)
        {
            pSets = 0;
            sets = 0;
            int pairs = 0;
            foreach (SmallMeld meld in melds)
            {
                switch (meld.MeldType)
                {
                    case MeldType.Set:
                        sets++;
                        break;
                    case MeldType.PSet:
                        pSets++;
                        break;
                    case MeldType.Pair:
                        pairs++;
                        break;
                }
            }
            if (pairs == 0 && needPair) {
                pSets = 7; // 0b111, means no pairs while required
                sets = 7;
            }
        }

        private static List<SmallMeld> FindBestMeldComboWithOnePair(int[] tiles)
        {
            List<SmallMeld> bestMelds = new List<SmallMeld>();
            List<SmallMeld> pairs = FindAllPairs(tiles);
            if (pairs.Count == 0) return bestMelds;
            SmallMeld bestPair = pairs[0];
            foreach (var pair in pairs)
            {
                //apply hand
                for (int i = 0; i < tiles.Length; i++)
                {
                    tiles[i] -= pair.Tiles[i];
                }
                var melds = FindBestMeldCombo(tiles, new List<SmallMeld>(), new List<SmallMeld>());
                if (CompareMelds(melds, bestMelds) > 0)
                {
                    bestMelds = melds;
                    bestPair = pair;
                }
                //unapply hand
                for (int i = 0; i < tiles.Length; i++)
                {
                    tiles[i] += pair.Tiles[i];
                }
            }
            bestMelds.Add(bestPair);
            return bestMelds;
        }

        /// <summary>
        /// Recursively search best melds from a set of tiles (int[9]).<br/>
        /// We apply a recursive DFS algorithm to find the most value set of (partial) melds.
        /// </summary>
        private static List<SmallMeld> FindBestMeldCombo(int[] tiles, List<SmallMeld> bestMelds, List<SmallMeld> currentMelds)
        {
            List<SmallMeld> groups = FindAllGroups(tiles);
            if (groups.Count > 0) //check if we can recourse further
            {
                foreach (SmallMeld meld in groups)
                {
                    //apply hand
                    for (int i = 0; i < tiles.Length; i++)
                    {
                        tiles[i] -= meld.Tiles[i];
                    }
                    currentMelds.Add(meld);
                    FindBestMeldCombo(tiles, bestMelds, currentMelds);
                    currentMelds.RemoveAt(currentMelds.Count - 1);
                    //unapply hand
                    for (int i = 0; i < tiles.Length; i++)
                    {
                        tiles[i] += meld.Tiles[i];
                    }
                }
            }
            else //end of recursion
            {
                if (CompareMelds(currentMelds, bestMelds) > 0)
                {
                    bestMelds.Clear();
                    bestMelds.AddRange(currentMelds);
                }
            }
            return bestMelds;
        }

        public static string Stringify(List<SmallMeld> melds)
        {
            string str = "";
            int sets = 0;
            int pSets = 0;
            int pairs = 0;
            foreach (SmallMeld meld in melds)
            {
                switch (meld.MeldType)
                {
                    case MeldType.Set:
                        sets++;
                        break;
                    case MeldType.PSet:
                        pSets++;
                        break;
                }
            }
            str += string.Format($"{sets} sets, {pSets} pSets, {pairs} pairs: {ValueMelds(melds)}");
            return str;
        }

        public enum MeldType
        {
            Set,
            PSet,
            Pair
        }

        /// <summary>
        /// SmallMeld represents a meld from a set of 9 tiles. Used individually for man, pin and sou.
        /// </summary>
        public struct SmallMeld
        {
            public int[] Tiles;
            public MeldType MeldType;

            public SmallMeld(int[] tiles, MeldType meld)
            {
                Tiles = tiles;
                MeldType = meld;
            }
        }
    }
}
