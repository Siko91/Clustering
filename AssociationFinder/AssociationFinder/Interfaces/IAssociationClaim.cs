using System;
using System.Collections.Generic;

namespace AssociationFinder.Interfaces
{
    public interface IAssociationClaim
    {
        Dictionary<string, string> Inputs { get; set; }
        KeyValuePair<string, string> Output { get; set; }
        double Support { get; set; }
        double Confidence { get; set; }
    }
}