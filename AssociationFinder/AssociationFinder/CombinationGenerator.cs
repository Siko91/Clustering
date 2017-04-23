using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssociationFinder
{
    public static class CombinationGenerator
    {
        public static IEnumerable<List<T>> GetCombinations<T>(this List<T> elements, int k)
        {
            int[] indexes = Enumerable.Range(0, k).ToArray();
            for (int indexToMove = k - 1; indexToMove >= 0; indexToMove--)
            {
                for (int moveToMake = 0; moveToMake < elements.Count - k; moveToMake++)
                {
                    indexes[indexToMove]++;
                    yield return indexes.Select(i => elements[i]).ToList();
                }
            }
        }

        public static IEnumerable<T[]> GetJaggedCombinations<T>(this T[][] jaggedArray)
        {
            if (!jaggedArray.Any() || jaggedArray.Any(arr => !arr.Any()))
                throw new ArgumentException("Emtpy arrays are not allowed in GetJaggedCombinations");

            var len = jaggedArray.Length;
            var indexes = new int[len];

            yield return indexes.Zip(jaggedArray, (i, arr) => arr[i]).ToArray();

            while (indexes[0] < jaggedArray[0].Length - 1 ||
                indexes[len - 1] < jaggedArray[len - 1].Length - 1)
            {
                if (indexes[0] >= jaggedArray[0].Length - 1)
                {
                    indexes[0] = 0;
                    for (int i = 1; i < len; i++)
                        if (indexes[i] < jaggedArray[i].Length - 1)
                        {
                            indexes[i]++;
                            break;
                        }
                }
                else
                {
                    indexes[0]++;
                }

                yield return indexes.Zip(jaggedArray, (i, arr) => arr[i]).ToArray();
            }
        }
    }
}
