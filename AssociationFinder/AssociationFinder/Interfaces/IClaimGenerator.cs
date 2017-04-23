using System.Collections.Generic;

namespace AssociationFinder.Interfaces
{
    public interface IClaimGenerator
    {
        IEnumerable<IAssociationClaim> GenerateClaims(
            Dictionary<string, string[]> allFieldsWithAllOfTheirPossibleStates, 
            string outcomeParameter);
    }
}