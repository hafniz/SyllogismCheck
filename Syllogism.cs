using System;
using System.Collections.Generic;
using System.Linq;

namespace SyllogismCheck
{
    public struct Syllogism : IEquatable<Syllogism>
    {
        public Sequent PremiseL;
        public Sequent PremiseR;
        public Sequent Conclusion;
        public IEnumerable<SyllogismTransformation> TransformationHistory;
        public bool IsLegitimate => PremiseL.IsLegitimate && PremiseR.IsLegitimate && Conclusion.IsLegitimate;
        public static readonly Syllogism Barbara = new("1a11b1b11c1a11c");

        public Syllogism(Sequent premiseL, Sequent premiseR, Sequent conclusion, IEnumerable<SyllogismTransformation> transformationHistory)
        {
            PremiseL = premiseL;
            PremiseR = premiseR;
            Conclusion = conclusion;
            TransformationHistory = transformationHistory;
        }

        public Syllogism(string s)
        {
            PremiseL = new(s[..5]);
            PremiseR = new(s[5..10]);
            Conclusion = new(s[10..]);
            TransformationHistory = new List<SyllogismTransformation>();
        }
        public string Serialize() => $"{PremiseL.Serialize()}{PremiseR.Serialize()}{Conclusion.Serialize()}";

        public Syllogism Transform(SyllogismTransformation transformation) => transformation switch
        {
            SyllogismTransformation.NegatePredicateA => new(PremiseL.Transform(SequentTransformation.NegatePredicateA), PremiseR.Transform(SequentTransformation.NegatePredicateA), Conclusion.Transform(SequentTransformation.NegatePredicateA), TransformationHistory.Append(transformation)),
            SyllogismTransformation.NegatePredicateB => new(PremiseL.Transform(SequentTransformation.NegatePredicateB), PremiseR.Transform(SequentTransformation.NegatePredicateB), Conclusion.Transform(SequentTransformation.NegatePredicateB), TransformationHistory.Append(transformation)),
            SyllogismTransformation.NegatePredicateC => new(PremiseL.Transform(SequentTransformation.NegatePredicateC), PremiseR.Transform(SequentTransformation.NegatePredicateC), Conclusion.Transform(SequentTransformation.NegatePredicateC), TransformationHistory.Append(transformation)),
            SyllogismTransformation.ContraposePremiseL => new(PremiseL.Transform(SequentTransformation.Contrapose), PremiseR, Conclusion, TransformationHistory.Append(transformation)),
            SyllogismTransformation.ContraposePremiseR => new(PremiseL, PremiseR.Transform(SequentTransformation.Contrapose), Conclusion, TransformationHistory.Append(transformation)),
            SyllogismTransformation.ContraposeConclusion => new(PremiseL, PremiseR, Conclusion.Transform(SequentTransformation.Contrapose), TransformationHistory.Append(transformation)),
            SyllogismTransformation.DenyConclusionWithPremiseL => new(Conclusion.Transform(SequentTransformation.NegateRelationship), PremiseR, PremiseL.Transform(SequentTransformation.NegateRelationship), TransformationHistory.Append(transformation)),
            SyllogismTransformation.DenyConclusionWithPremiseR => new(PremiseL, Conclusion.Transform(SequentTransformation.NegateRelationship), PremiseR.Transform(SequentTransformation.NegateRelationship), TransformationHistory.Append(transformation)),
            _ => throw new ArgumentOutOfRangeException(nameof(transformation))
        };

        public IEnumerable<Syllogism> WithAlternativePredicatePermutations()
        {
            List<Dictionary<char, char>> predicateSwitches = new()
            {
                new() { { 'b', 'c' }, { 'c', 'b' } },
                new() { { 'a', 'c' }, { 'c', 'a' } },
                new() { { 'a', 'b' }, { 'b', 'a' } },
                new() { { 'a', 'b' }, { 'b', 'c' }, { 'c', 'a' } },
                new() { { 'a', 'c' }, { 'b', 'a' }, { 'c', 'b' } },
            };
            foreach (Dictionary<char, char> dict in predicateSwitches)
            {
                yield return new(string.Join("", Serialize().Select(c => dict.ContainsKey(c) ? dict[c] : c)));
            }
        }

        public override string ToString() => $"{PremiseL}\t{PremiseR}\t=>\t{Conclusion}\t[{string.Join(", ", TransformationHistory)}]";
        public bool Equals(Syllogism other) => other.Conclusion == Conclusion
            && (other.PremiseL == PremiseL && other.PremiseR == PremiseR
            || other.PremiseL == PremiseR && other.PremiseR == PremiseL);
        public static bool operator ==(Syllogism syllogism1, Syllogism syllogism2) => syllogism1.Equals(syllogism2);
        public static bool operator !=(Syllogism syllogism1, Syllogism syllogism2) => !syllogism1.Equals(syllogism2);
    }
}
