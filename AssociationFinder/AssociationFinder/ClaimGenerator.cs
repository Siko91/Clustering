using AssociationFinder.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssociationFinder
{
    public class ClaimGenerator : IClaimGenerator
    {
        public ClaimGenerator(int maxInputFields = 1)
        {
            MaxInputFields = maxInputFields;
        }

        public int MaxInputFields { get; private set; }

        public IEnumerable<IAssociationClaim> GenerateClaims(
            Dictionary<string, string[]> allFieldsWithAllOfTheirPossibleStates,
            string outcomeField)
        {
            var outcomeOptions = allFieldsWithAllOfTheirPossibleStates[outcomeField]
                .Select(o => new KeyValuePair<string, string>(outcomeField, o))
                .ToArray();

            allFieldsWithAllOfTheirPossibleStates = allFieldsWithAllOfTheirPossibleStates
                .Where(f => f.Key != outcomeField)
                .ToDictionary(f => f.Key, f => f.Value);

            List<Dictionary<string, string>> inputsOptions = new List<Dictionary<string, string>>();
            for (int currentMaxFields = 1; currentMaxFields <= MaxInputFields; currentMaxFields++)
                inputsOptions.AddRange(GenerateInputOptions(allFieldsWithAllOfTheirPossibleStates, currentMaxFields));

            var resultClaims = inputsOptions
                .SelectMany(i => outcomeOptions
                    .Select(o => new AssociationClaim() { Inputs = i, Output = o }))
                .GroupBy(claim => claim
                  //  , new AssociationClaim.Comparer()
                ).Select(group => group.Key); // group to remove repeating claims

            return resultClaims.ToList();
        }

        private IEnumerable<Dictionary<string, string>> GenerateInputOptions(
            Dictionary<string, string[]> allOptions,
            int currentMaxFields)
        {
            if (currentMaxFields < 1)
                throw new ArgumentException("MaxFields must be 1 or bigger");

            var combinations = allOptions.Keys.ToList().GetCombinations(currentMaxFields);

            var combinationsWithValues = combinations
                .SelectMany(combination => combination
                    .Select(f => allOptions[f]
                        .Select(o => new KeyValuePair<string, string>(f, o))
                        .ToArray())
                    .ToArray()
                    .GetJaggedCombinations())
                .Select(result => result.ToDictionary(r => r.Key, r => r.Value));

            return combinationsWithValues;

            //currentResults.AddRange(allFieldsWithAllOfTheirPossibleStates
            //    .SelectMany(field => field.Value
            //        .Select(value => new Dictionary<string, string>() { { field.Key, value } })));

            //for (int i = 0; i < currentMaxFields - 1; i++)
            //{
            //    currentResults = currentResults.SelectMany(r => allFieldsWithAllOfTheirPossibleStates
            //        .Where(f => r.Keys.All(k => k != f.Key))
            //        .SelectMany(f => f.Value
            //            .Select(v => new Dictionary<string, string>(r) { { f.Key, v } })))
            //        .ToList();
            //}
        }
    }
}
