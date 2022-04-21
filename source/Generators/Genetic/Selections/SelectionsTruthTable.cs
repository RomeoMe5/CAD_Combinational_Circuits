using Generators;

using System;
using System.Collections.Generic;

namespace Genetics
{
    public partial class Selections<Type, ParametersType> where ParametersType : GeneticParameters
                                            where Type : Chromosome<ParametersType>, new()
    {
        private List<ChromosomeType<TruthTable, TruthTableParameters>> SelectionBase(SelectionParameters selectionParameters,
                                    List<ChromosomeType<TruthTable, TruthTableParameters>> population)
        {
            Random random = new Random();
            List<ChromosomeType<TruthTable, TruthTableParameters>> survivors = new List<ChromosomeType<TruthTable, TruthTableParameters>>();
            while (survivors.Count < selectionParameters.numOfSurvivors)
            {
                int r1 = 0, r2 = 0;
                while (population.Count > 2 && r2 == r1)
                {
                    r1 = random.Next(population.Count);
                    r2 = random.Next(population.Count);
                }

                if ((population[r1].AdaptationIndex < population[r2].AdaptationIndex) & (!survivors.Contains(population[r1])))
                {
                    survivors.Add(population[r1]);
                }
                else if ((population[r1].AdaptationIndex > population[r2].AdaptationIndex) & (!survivors.Contains(population[r2])))
                {
                    survivors.Add(population[r2]);
                }
                else if (population[r1].AdaptationIndex == population[r2].AdaptationIndex)
                {
                    if (!survivors.Contains(population[r1]))
                    {
                        survivors.Add(population[r1]);
                    }
                    else if (!survivors.Contains(population[r2]))
                    {
                        survivors.Add(population[r2]);
                    }
                    else if (population.Count < selectionParameters.numOfSurvivors)
                        survivors.Add(population[r1]);
                }
            }
            return survivors;
        }

    }
}