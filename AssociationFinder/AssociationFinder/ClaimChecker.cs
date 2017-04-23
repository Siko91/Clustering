using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssociationFinder.Interfaces;

namespace AssociationFinder
{
    public class ClaimChecker
    {
        public static void Check(
            List<Dictionary<string, string>> data, 
            IAssociationClaim claim)
        {
            var supportingCases = data
                .Where(d => claim.Inputs.All(i=> d[i.Key] == i.Value))
                .ToList();

            claim.Support = (double)supportingCases.Count 
                / data.Count;

            claim.Confidence = (double)supportingCases.Count(d => d[claim.Output.Key] == claim.Output.Value) 
                / supportingCases.Count;
        }
    }
}
