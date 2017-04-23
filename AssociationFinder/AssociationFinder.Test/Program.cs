using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssociationFinder.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Run();
        }

        static void Run()
        {
            var fields = new Dictionary<string, string[]>()
            {
                { "Age", new[] { "<18" ,"<40" , ">40" } },
                { "Gender", new[] { "M", "F" } },
                { "Bought_Candy", new[] { "True", "False" } },
                { "Bought_Bread", new[] { "True", "False" } },
                { "Bought_Butter", new[] { "True", "False" } },
                { "Bought_Soda", new[] { "True", "False" } },
                { "Bought_Fruits", new[] { "True", "False" } },
                { "Bought_CocaCola", new[] { "True", "False" } }
            };

            var claims = new ClaimGenerator(4).GenerateClaims(fields, "Bought_CocaCola");

            var randomData = DataGenerator.GetRandomData(fields).ToList();

            foreach (var claim in claims)
                ClaimChecker.Check(randomData, claim);

            var confidentClaims = claims.OrderByDescending(c => c.Confidence).Take(10);
            Console.WriteLine("Confident Claims:");
            foreach (var claim in confidentClaims)
                Console.WriteLine(claim);

            Console.WriteLine();

            var supportedClaims = claims.OrderByDescending(c => c.Support).Take(10);
            Console.WriteLine("Supported Claims:");
            foreach (var claim in supportedClaims)
                Console.WriteLine(claim);
        }

        private static void TestJaggedCombinationGenerator()
        {
            var combinations = new[] {
                new[] { 0, 1, 2 },
                new[] { 0, 1 },
                new[] { 0, 1 }
            }.GetJaggedCombinations();

            foreach (var c in combinations)
                Console.WriteLine(string.Join(", ", c));
        }
    }
}
