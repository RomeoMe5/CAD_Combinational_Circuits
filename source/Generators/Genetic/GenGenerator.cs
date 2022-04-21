using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Circuits;

using Generators;

using Genetics.Mutation;
using Genetics.Parent;
using Genetics.Recombination;

using Graph;

using Properties;

namespace Genetics
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Type">Тип генотипа</typeparam>
    /// <typeparam name="ParametersType">Тип передаваемых параметров. Должен соответствовать генотипу.</typeparam>
    class GeneticGenerator<Type, ParametersType>    where ParametersType : GeneticParameters
                                                    where Type : Chromosome<ParametersType>, new()

    {
        private int inputs, outputs;
        //глобальные параметры
        private int num_cross;//количество скрещиваний
        private List<ChromosomeType<Type, ParametersType>> population;
        private Settings settings;
        ParametersType parameters;
        private string mainPath;

        /// Конструктор класса
        public GeneticGenerator(ParametersType parameters, Tuple<int, int> inout, string mainPath = "")
        {
            this.settings = Settings.GetInstance();
            this.parameters = parameters;
            this.mainPath = mainPath;
            inputs = inout.Item1;
            outputs = inout.Item2;
        }

        public List<ChromosomeType<Type, ParametersType>> Generate()
        {
            //TODO: Нужна проверка на null            
            Recombinations<Type, ParametersType> R = new Recombinations<Type, ParametersType>();
            Selections<Type, ParametersType> S = new Selections<Type, ParametersType>();
            Mutations<Type, ParametersType> M = new Mutations<Type, ParametersType>();

            CreatePopulation();
            double d = EndProcessFunction();
            for (int i = 0; (i < parameters.numOfCycles) && (EndProcessFunction() < parameters.keyEndProcessIndex); i++)
            {
                List<ChromosomeType<Type, ParametersType>> newPopulation = R.RecombinationType(parameters.RecombinationParameter, population);
                List<ChromosomeType<Type, ParametersType>> mutants = M.MutationType(parameters.MutationParameter, newPopulation);
                population = S.SelectionType(parameters.SelectionParameter, mutants);
                savePopulation(population);
            }
            return population;
        }

        private void savePopulation(List<ChromosomeType<Type, ParametersType>> population)
        {
            foreach (var ttp in population)
            {
                TruthTable tt = new TruthTable();
                tt = (dynamic)ttp.chromosome;
                SimpleGenerators tftt = new SimpleGenerators();
                List<Tuple<string, List<string>>> circs = new List<Tuple<string, List<string>>>();
                circs.Add(Tuple.Create("CNFT", tftt.cnfFromTruthTable(tt, true)));
                circs.Add(Tuple.Create("CNFF", tftt.cnfFromTruthTable(tt, false)));
                foreach (Tuple<string, List<string>> nameexpr in circs)
                {
                    string name = nameexpr.Item1;
                    List<string> expr = nameexpr.Item2;
                    Parser pCNFT = new Parser(expr);
                    pCNFT.ParseAll();
                    OrientedGraph graph = pCNFT.Graph;
                    Circuit c = new Circuit(graph, expr);
                    c.tTable = tt;
                    c.path = this.mainPath + $"\\Genetic\\";
                    c.circuitName = name;
                    c.generate();
                }
            }
        }

        private void CreatePopulation()
        {
            this.population = new List<ChromosomeType<Type, ParametersType>>();
            for (int i = 0; i < parameters.populationSize; i++)
            {
                Type gen = new Type();
                gen.GenerateRandom(this.parameters);
                ChromosomeType<Type, ParametersType> ind = new ChromosomeType<Type, ParametersType>($@"ind{i}", gen);
                population.Add(ind);
            }
            savePopulation(population);
        }

        private double EndProcessFunction()
        {
            double max = 0;
            foreach (var i in population)
            {
                if (max < i.AdaptationIndex)
                    max = i.AdaptationIndex;
            }

            return max;
        }
    }
}
