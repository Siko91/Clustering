using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Expando;

namespace AssociationFinder.Test
{
    public class DataGenerator
    {
        static Random rnd = new Random();

        public static IEnumerable<Dictionary<string, string>> GetRandomData(
            Dictionary<string, string[]> fields, uint count = 1000)
        {
            for (int i = 0; i < count; i++)
            {
                yield return fields.ToDictionary(f => f.Key, f => f.Value[rnd.Next(f.Value.Length)]);
            }
        }
    }
}