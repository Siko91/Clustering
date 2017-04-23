using AssociationFinder.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssociationFinder
{
    public class AssociationClaim : IAssociationClaim
    {
        public Dictionary<string, string> Inputs { get; set; }
        public KeyValuePair<string, string> Output { get; set; }
        public double Support { get; set; }
        public double Confidence { get; set; }

        public override string ToString()
        {
            return string.Format("Claim [ {0} ==> {1} ] [Support:{2:P2}] [Confidence:{3:P2}]",
                string.Join(", ", Inputs.Select(i => string.Format("{0}({1})", i.Key, i.Value))),
                string.Format("{0}({1})", Output.Key, Output.Value),
                Support,
                Confidence);
        }

        public override bool Equals(object obj)
        {
            return IsEqualTo(obj as AssociationClaim);

        }
        
        private bool IsEqualTo(AssociationClaim other)
        {
            if (other != null)
            {
                var otherInputFields = other.Inputs.Keys;
                
                if (other.Output.Key == Output.Key)
                    if (other.Output.Value == Output.Value)
                        if (other.Inputs.Count == Inputs.Count)
                            if (otherInputFields.All(f => Inputs.ContainsKey(f)))
                                if (otherInputFields.All(f => other.Inputs[f] == Inputs[f]))
                                    return true;
            }

            return false;
        }
    }
}
