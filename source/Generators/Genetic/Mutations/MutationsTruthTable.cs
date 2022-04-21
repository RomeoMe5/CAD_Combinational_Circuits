using Generators;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Genetics.Mutation
{
    public partial class Mutations<Type, ParametersType> where ParametersType : GeneticParameters
                                          where Type : Chromosome<ParametersType>, new()
    {
        public static bool[][] MutationTable(double probability, bool[][] table)
        {
            Random random = new Random();
            for (int i = 0; i < table.Length; i++)
            {
                for (int j = 0; j < table[i].Length; j++)
                {
                    if (random.NextDouble() < probability)
                    {
                        table[i][j] = !table[i][j];
                    }
                }
            }
            return table;
        }
        private List<ChromosomeType<TruthTable, TruthTableParameters>> MutationBinary(MutationParameters mutationParameters,
                                                                    List<ChromosomeType<TruthTable, TruthTableParameters>> population)
        {
            List<ChromosomeType<TruthTable, TruthTableParameters>> mutants = new List<ChromosomeType<TruthTable, TruthTableParameters>> ();
            int num_mut, k, n;
            int size = population[0].chromosome.Size;
            int output = population[0].chromosome.Output;
            Random random = new Random();

            for (int i = 0; i < population.Count; i++)
            {
                num_mut = random.Next(1, size * output);
                bool[][] mutant = population[i].chromosome.OutTable;
                List<int> m = new List<int> ();
                for (int j = 0; j < num_mut; j++)
                {
                    do {
                        n = random.Next(output);
                        k = random.Next(size);
                    } while (m.Contains(k * output + n));
                    m.Add(k * output + n);

                    mutant[k][n] = !mutant[k][n];
                }
                TruthTable tt = new TruthTable(population[i].chromosome, mutant);
                ChromosomeType<TruthTable, TruthTableParameters> tmp = new ChromosomeType<TruthTable, TruthTableParameters>(population[i].Name, tt);
                mutants.Add(tmp);
            }
            return mutants;
        }

        private List<ChromosomeType<TruthTable, TruthTableParameters>> MutationDensity(MutationParameters mutationParameters,
                                                                    List<ChromosomeType<TruthTable, TruthTableParameters>> population)
        {
            Random random = new Random();

            for (int i = 0; i < population.Count; i++)
            {
                if (random.NextDouble() < mutationParameters.probabilityGen)
                {
                    bool[][] mutant = MutationTable(mutationParameters.probabilityTruthTable, population[i].chromosome.OutTable);
                    population[i].chromosome = new TruthTable(population[i].chromosome, mutant);
                }
            }
            return population;
        }

        private List<ChromosomeType<TruthTable, TruthTableParameters>> MutationAccessionDel(MutationParameters mutationParameters,
                                                                    List<ChromosomeType<TruthTable, TruthTableParameters>> population)
        {
            int size = population[0].chromosome.Size;
            Random random = new Random();

            TruthTable tt = new TruthTable(population[0].chromosome.Input, population[0].chromosome.Output, null);
            bool[][] bin = tt.OutTable;

            for (int i = 0; i < population.Count; i++)
            {
                if (random.NextDouble() < mutationParameters.probabilityGen)
                {
                    bool[][] bin2 = population[i].chromosome.array;
                    bin2[size - 1] = bin[random.Next(0, size)];
                    population[i].chromosome = new TruthTable(  population[0].chromosome,
                                                                bin2);
                }
            }
            return population;
        }

        private List<ChromosomeType<TruthTable, TruthTableParameters>> MutationInsertDel(MutationParameters mutationParameters,
                                                                    List<ChromosomeType<TruthTable, TruthTableParameters>> population)
        {
            int size = population[0].chromosome.Size;
            Random random = new Random();

            TruthTable tt = new TruthTable(population[0].chromosome);
            bool[][] bin = tt.OutTable;

            for (int i = 0; i < population.Count; i++)
            {
                if (random.NextDouble() < mutationParameters.probabilityGen)
                {
                    bool[][] bin2 = population[i].chromosome.array;
                    bin2[random.Next(0, size)] = bin[random.Next(0, size)];
                    population[i].chromosome = new TruthTable(  population[0].chromosome,
                                                                bin2);
                }
            }
            return population;
        }

        private List<ChromosomeType<TruthTable, TruthTableParameters>> MutationExchange(MutationParameters mutationParameters,
                                                                    List<ChromosomeType<TruthTable, TruthTableParameters>> population)
        {
            int type = mutationParameters.exchangeType;
            int size = population[0].chromosome.Size;
            int output = population[0].chromosome.Output;
            Random random = new Random();
            int k = random.Next(0, size);
            int n = random.Next(0, output);

            for (int z = 0; z < population.Count; z++)
            {
                if (random.NextDouble() < mutationParameters.probabilityGen)
                {
                    if ((type == 0) || (type == 1) || (type == 2))
                    {
                        if ((type == 0) || (type == 1))
                        {
                            bool[][] bin = population[z].chromosome.array;
                            bool temp;
                            temp = bin[k - 1][n];
                            bin[k - 1][n] = bin[k + 1][n];
                            bin[k + 1][n] = temp;
                            population[z].chromosome = new TruthTable(  population[z].chromosome,
                                                                        bin);
                        }

                        if ((type == 0) || (type == 2))
                        {
                            bool[][] bin = population[z].chromosome.array;
                            bool temp;
                            temp = bin[k][n-1];
                            bin[k][n - 1] = bin[k][n + 1];
                            bin[k][n + 1] = temp;
                            population[z].chromosome = new TruthTable(population[z].chromosome.Input,
                                                                        population[z].chromosome.Output,
                                                                        bin);
                        }
                    }

                    if (type == 3)
                    {
                        bool[][] bin = population[z].chromosome.array;
                        bool[] temp;
                        temp = bin[k - 1];
                        bin[k - 1] = bin[k + 1];
                        bin[k + 1] = temp;
                        population[z].chromosome = new TruthTable(population[z].chromosome.Input,
                                                                    population[z].chromosome.Output,
                                                                    bin);
                    }
                }
            }
            return population;
        }

        private List<ChromosomeType<TruthTable, TruthTableParameters>> MutationDelete(MutationParameters mutationParameters,
                                                                    List<ChromosomeType<TruthTable, TruthTableParameters>> population)
        {
            int size = population[0].chromosome.Size;
            int output = population[0].chromosome.Output;
            Random random = new Random();

            for (int i = 0; i < population.Count; i++)
            {
                if (random.NextDouble() < mutationParameters.probabilityGen)
                {
                    for (int j = 0; j < size; j++)
                    {
                        for (int k = 0; k < output; k++)
                        {
                            if (random.NextDouble() < mutationParameters.probabilityTruthTable)
                            {
                                bool[][] bin = population[i].chromosome.array;
                                bin[j][k] = false;
                                population[i].chromosome = new TruthTable(population[i].chromosome, bin);
                            }
                        }
                    }
                }
            }
            return population;

        }


    }
}