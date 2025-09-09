using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrfpc.patch.outfitseverywhere
{
    internal class UnitOutfitsRandom
    {
        internal static readonly List<int[]> random_outfits = new List<int[]>
        {
            new int[] { 1 }, // None
            new int[] { 1, 102, 201, 202, 203, 204, 205, 206, 207, 208, 500 },      // Protagonist  - 01
            new int[] { 1, 102, 201, 202, 203, 204, 205, 206, 207, 208, 500 },      // Strohl       - 02
            new int[] { 1, 201, 202, 203, 204, 205, 206, 207, 208, 500 },           // Hulkenberg   - 03
            new int[] { 1, 1, 201, 202, 203, 204, 205, 206, 207, 208 },              // Heismay      - 04
            new int[] { 1, 201, 202, 203, 204, 205, 206, 207, 208, 500 },           // Junah        - 05  - 103, 104, 
            new int[] { 1, 201, 202, 203, 204, 205, 206, 207, 208, 500 },           // Eupha        - 06
            new int[] { 1, 201, 202, 203, 204, 205, 206, 207, 208, 500 },           // Fidelio      - 07
            new int[] { 1, 201, 202, 203, 204, 205, 206, 207, 208, 500 },           // Basilio      - 08
            new int[] { 1, 201, 202, 203, 204, 205, 206, 207, 208, 500 },           // Prince       - 09
            new int[] { 1, 201, 202, 203, 204, 205, 206, 207, 208, 500 },           // Grius        - 10
            new int[] { 1 },           // Gallica      - 11
            new int[] { 1, 201, 202, 203, 204, 205, 206, 207, 208, 500 },           // ?????        - 12
            new int[] { 1, 201, 202, 203, 204, 205, 206, 207, 208, 500 },           // Neuras       - 13
            new int[] { 1, 201, 202, 203, 204, 205, 206, 207, 208, 500 },           // Rella        - 14
            new int[] { 1, 201, 202, 203, 204, 205, 206, 207, 208, 500 },           // Louis        - 15
            new int[] { 1, 201, 202, 203, 204, 205, 206, 207, 208, 500 },           // RESERVE      - 16
            new int[] { 1, 201, 202, 203, 204, 205, 206, 207, 208, 500 },           // RESERVE      - 17
            new int[] { 1, 201, 202, 203, 204, 205, 206, 207, 208, 500 },           // RESERVE      - 18
            new int[] { 1, 201, 202, 203, 204, 205, 206, 207, 208, 500 },           // RESERVE      - 19
            new int[] { 1, 102, 201, 202, 203, 204, 205, 206, 207, 208, 500 },      // Protagonist  - 20
            new int[] { 1, 102, 201, 202, 203, 204, 205, 206, 207, 208, 500 }       // DUMMY
        };


        internal static int GetRandomCostume(int arrayIndex)
        {
            if (arrayIndex < 0 || arrayIndex >= random_outfits.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Invalid array index.");
            }

            int[] selectedArray = random_outfits[arrayIndex];
            Random random = new Random();
            int randomIndex = random.Next(0, selectedArray.Length);
            return selectedArray[randomIndex];
        }
    }
}
