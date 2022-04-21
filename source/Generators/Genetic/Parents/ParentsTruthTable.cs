using System;
using System.Collections.Generic;
using System.Linq;
using Generators;
using Genetics.Recombination;

using source;

namespace Genetics.Parent
{
    partial class Parents<Type, ParametersType> where ParametersType : GeneticParameters
                                        where Type : Chromosome<ParametersType>, new()
    {        
        private List<int> GetHemming(int t, List<ChromosomeType<TruthTable, TruthTableParameters>> population)
        {
            int count;
            bool[][] p1 = population[t].chromosome.OutTable;
            Dictionary<int, int> dict = new Dictionary<int, int>();
            List<int> lst = new List<int>();

            for (int i = 0; i < population.Count; i++)
            {
                if (i != t)
                {
                    count = 0;
                    bool[][] p2 = population[i].chromosome.OutTable;
                    for (int j = 0; j < p1.Length; j++)
                    {
                        for (int k = 0; k < p1[0].Length; j++)
                        {
                            if (p2[j][k] == p1[j][k])
                            {
                                count++;
                            }
                        }
                    }
                    dict.Add(i, count);
                }
            }

            var sortedDict = AuxiliaryMethods.SortDictByValue(dict);

            foreach (var pair in sortedDict)
                lst.Add(pair.Key);

            return lst;
        }

        private List<int> ParentsPanmixia(ParentsParameters parentsParameters, List<ChromosomeType<TruthTable, TruthTableParameters>> population)
        {
            Random random = new Random();
            int parent1 = 0, parent2 = 0;
            while (population.Count > 2 && parent1 == parent2)
            {
                parent1 = random.Next(0, population.Count);
                parent2 = random.Next(0, population.Count);
            }
            List<int> parents = new List<int>() { parent1, parent2 };
            return parents;
        }        

        private List<int> ParentsInbriding(ParentsParameters parentsParameters, List<ChromosomeType<TruthTable, TruthTableParameters>> population)
        {
            
            Random random = new Random();
            int parent1 = random.Next(0, population.Count);

            int parent2 = GetHemming(parent1, population).Last();

            List<int> parents = new List<int>() { parent1, parent2 };

            return parents;
        }

        private List<int> ParentsOutbriding(ParentsParameters parentsParameters, List<ChromosomeType<TruthTable, TruthTableParameters>> population)
        {
            Random random = new Random();
            int parent1 = random.Next(0, population.Count);

            int parent2 = GetHemming(parent1, population)[0];

            List<int> parents = new List<int>() { parent1, parent2 };

            return parents;
        }

        private List<int> ParentsTournament(ParentsParameters parentsParameters, List<ChromosomeType<TruthTable, TruthTableParameters>> population)
        {
            List<int> lst = AuxiliaryMethods.GetRandomIntList(parentsParameters.TournematnNumber, 0, population.Count, false);
            Dictionary<int, double> lstToAdaptationIndex = new Dictionary<int, double>();

            foreach (int k in lst)
                lstToAdaptationIndex.Add(k, population[k].AdaptationIndex);

            var sortedDict = AuxiliaryMethods.SortDictByValue(lstToAdaptationIndex, false);

            lst.Clear();
            foreach (var pair in sortedDict)
                lst.Add(pair.Key);

            List<int> parents = new List<int> { lst[0], lst[1] };
            return parents;
        }
        private List<int> ParentsRoulette(ParentsParameters parentsParameters, List<ChromosomeType<TruthTable, TruthTableParameters>> population)
        {
            List<int> lst = AuxiliaryMethods.GetRandomIntList(parentsParameters.TournematnNumber, 0, population.Count, false);
            Dictionary<int, double> lstToAdaptationIndex = new Dictionary<int, double>();

            foreach (int k in lst)
                lstToAdaptationIndex.Add(k, population[k].AdaptationIndex);

            var sortedDict = AuxiliaryMethods.SortDictByValue(lstToAdaptationIndex, false);

            lst.Clear();
            foreach (var pair in sortedDict)
                lst.Add(pair.Key);

            List<int> parents = new List<int> { lst[0], lst[1] };
            return parents;
        }
}
}
