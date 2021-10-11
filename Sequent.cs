using System;

namespace SyllogismCheck
{
    public struct Sequent : IEquatable<Sequent>
    {
        public bool PositiveL;
        public Predicate PredicateL;
        public bool PositiveRelationship;
        public bool PositiveR;
        public Predicate PredicateR;
        public bool IsLegitimate => PositiveL;

        public Sequent(bool positiveL, Predicate predicateL, bool positiveRelationship, bool positiveR, Predicate predicateR)
        {
            PositiveL = positiveL;
            PredicateL = predicateL;
            PositiveRelationship = positiveRelationship;
            PositiveR = positiveR;
            PredicateR = predicateR;
        }

        public Sequent(string s)
        {
            PositiveL = s[0] == '1';
            PredicateL = Enum.Parse<Predicate>(s[1..2]);
            PositiveRelationship = s[2] == '1';
            PositiveR = s[3] == '1';
            PredicateR = Enum.Parse<Predicate>(s[4..5]);
        }
        public string Serialize() => $"{(PositiveL ? '1' : '0')}{PredicateL}{(PositiveRelationship ? '1' : '0')}{(PositiveR ? '1' : '0')}{PredicateR}";

        public Sequent Transform(SequentTransformation transformation) => transformation switch
        {
            SequentTransformation.NegatePredicateA when PredicateL == Predicate.a => new(!PositiveL, PredicateL, PositiveRelationship, PositiveR, PredicateR),
            SequentTransformation.NegatePredicateB when PredicateL == Predicate.b => new(!PositiveL, PredicateL, PositiveRelationship, PositiveR, PredicateR),
            SequentTransformation.NegatePredicateC when PredicateL == Predicate.c => new(!PositiveL, PredicateL, PositiveRelationship, PositiveR, PredicateR),
            SequentTransformation.NegatePredicateA when PredicateR == Predicate.a => new(PositiveL, PredicateL, PositiveRelationship, !PositiveR, PredicateR),
            SequentTransformation.NegatePredicateB when PredicateR == Predicate.b => new(PositiveL, PredicateL, PositiveRelationship, !PositiveR, PredicateR),
            SequentTransformation.NegatePredicateC when PredicateR == Predicate.c => new(PositiveL, PredicateL, PositiveRelationship, !PositiveR, PredicateR),
            SequentTransformation.Contrapose => new(!PositiveR, PredicateR, PositiveRelationship, !PositiveL, PredicateL),
            SequentTransformation.NegateRelationship => new(PositiveL, PredicateL, !PositiveRelationship, PositiveR, PredicateR),
            _ => new(PositiveL, PredicateL, PositiveRelationship, PositiveR, PredicateR)
        };

        public override string ToString() => (PositiveL ? "" : "!") + PredicateL + (PositiveRelationship ? "|=" : "|/=") + (PositiveR ? "" : "!") + PredicateR;
        public bool Equals(Sequent other) => other.Serialize() == Serialize();
        public static bool operator ==(Sequent sequent1, Sequent sequent2) => sequent1.Equals(sequent2);
        public static bool operator !=(Sequent sequent1, Sequent sequent2) => !sequent1.Equals(sequent2);
    }
}
