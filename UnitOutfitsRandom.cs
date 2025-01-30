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
            new int[] { 1, 5, 102, 201, 202, 203, 204, 205, 206, 207, 208 },      // Protagonist  - 01
            new int[] { 1, 5, 102, 201, 202, 203, 204, 205, 206, 207, 208 },      // Strohl       - 02
            new int[] { 1, 5, 201, 202, 203, 204, 205, 206, 207, 208 },           // Hulkenberg   - 03
            new int[] { 1, 1, 201, 202, 203, 204, 205, 206, 207, 208 },              // Heismay      - 04
            new int[] { 1, 5, 201, 202, 203, 204, 205, 206, 207, 208 },           // Junah        - 05  - 103, 104, 
            new int[] { 1, 5, 201, 202, 203, 204, 205, 206, 207, 208 },           // Eupha        - 06
            new int[] { 1, 5, 201, 202, 203, 204, 205, 206, 207, 208 },           // Fidelio      - 07
            new int[] { 1, 5, 201, 202, 203, 204, 205, 206, 207, 208 },           // Basilio      - 08
            new int[] { 1, 5, 201, 202, 203, 204, 205, 206, 207, 208 },           // Prince       - 09
            new int[] { 1, 5, 201, 202, 203, 204, 205, 206, 207, 208 },           // Grius        - 10
            new int[] { 1 },           // Gallica      - 11
            new int[] { 1, 5, 201, 202, 203, 204, 205, 206, 207, 208 },           // ?????        - 12
            new int[] { 1, 5, 201, 202, 203, 204, 205, 206, 207, 208 },           // Neuras       - 13
            new int[] { 1, 5, 201, 202, 203, 204, 205, 206, 207, 208 },           // Rella        - 14
            new int[] { 1, 5, 201, 202, 203, 204, 205, 206, 207, 208 },           // Louis        - 15
            new int[] { 1, 5, 201, 202, 203, 204, 205, 206, 207, 208 },           // RESERVE      - 16
            new int[] { 1, 5, 201, 202, 203, 204, 205, 206, 207, 208 },           // RESERVE      - 17
            new int[] { 1, 5, 201, 202, 203, 204, 205, 206, 207, 208 },           // RESERVE      - 18
            new int[] { 1, 5, 201, 202, 203, 204, 205, 206, 207, 208 },           // RESERVE      - 19
            new int[] { 1, 5, 102, 201, 202, 203, 204, 205, 206, 207, 208 },      // Protagonist  - 20
            new int[] { 1, 5, 102, 201, 202, 203, 204, 205, 206, 207, 208 }       // DUMMY
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
