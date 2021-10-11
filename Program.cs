using System;
using System.Collections.Generic;
using System.Linq;

namespace SyllogismCheck
{
    class Program
    {
        static void Main()
        {
            List<Syllogism> knownSyllogisms = new();
            Queue<Syllogism> awaitingCheck = new();
            awaitingCheck.Enqueue(Syllogism.Barbara);

            while (awaitingCheck.Count > 0)
            {
                Syllogism current = awaitingCheck.Dequeue();
                if (!knownSyllogisms.Contains(current) && !knownSyllogisms.Select(s => s.WithAlternativePredicatePermutations()).SelectMany(s => s).Contains(current))
                {
                    knownSyllogisms.Add(current);
                    Console.WriteLine(current);
                }
                foreach (SyllogismTransformation transformation in Enum.GetValues<SyllogismTransformation>())
                {
                    Syllogism newSyllogism = current.Transform(transformation);
                    if (newSyllogism.IsLegitimate && !knownSyllogisms.Contains(newSyllogism) && !knownSyllogisms.Select(s => s.WithAlternativePredicatePermutations()).SelectMany(s => s).Contains(newSyllogism))
                    {
                        awaitingCheck.Enqueue(newSyllogism);
                    }
                }
            }
            Console.WriteLine($"\n{knownSyllogisms.Count} syllogisms found. ");
        }

        static void ShowAllPredicatePermutations(string s)
        {
            Syllogism syllogism = new(s);
            Console.WriteLine(syllogism);
            foreach (Syllogism alt in syllogism.WithAlternativePredicatePermutations())
            {
                Console.WriteLine(alt);
            }
        }
    }
}
