using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Generators;
using Properties;
using Graph;
using Circuits;
using Genetics;
using System.IO;

namespace DataBaseGenerators
{
    class DataBaseGenerator
    {
        private delegate void GenerateDelegate(GenerationParameters gp);
        private string mainPath;
        private Settings settings;
        private DataBaseGeneratorParameters parameters;
        /// <summary>
        /// Счетчик для создания папок (названия схем)
        /// </summary>
        private int n;

        public DataBaseGenerator()
        {
            this.settings = Settings.GetInstance();
            this.parameters = null;
            this.mainPath = Settings.datasetPath;
            this.n = 0;
        }
        public DataBaseGenerator(DataBaseGeneratorParameters p) : this()
        {
            this.parameters = p;
        }

        public void GenerateType(DataBaseGeneratorParameters gp, bool parallel = true)
        {
            GenerateDelegate handler;
            string s = gp.generationTypes.ToString();
            string methodName = "GenerateDataBase" + s;
            MethodInfo mi = typeof(DataBaseGenerator).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            handler = (GenerateDelegate)Delegate.CreateDelegate(type: typeof(GenerateDelegate), firstArgument: this, method: mi);
            
            GenerationParameters genParams = gp.generationParameters;

            DirectoryInfo dir = new DirectoryInfo(Settings.datasetPath + "\\" + s);
            n = 0;
            if (dir.Exists)
            {
                foreach (var item in dir.GetDirectories())
                {
                    string s0 = item.Name;
                    s0 = s0.Replace(Settings.generationMethodsToPrefix[s], "");
                    int jk = s0.IndexOf("_");
                    if (jk == -1)
                        jk = s0.Length;
                    s0 = s0.Substring(0, jk);
                    if (n <= Int32.Parse(s0))
                        n = Int32.Parse(s0) + 1;
                }
            }

            for (int i = gp.minInputs; i <= gp.maxInputs; i++)
                for (int j = gp.minOutputs; j <= gp.maxOutputs; j++)
                {
                    genParams.inputs = i;
                    genParams.outputs = j;                   

                    if (parallel)
                    {
                        ParallelOptions po = new ParallelOptions();
                        if (Settings.numThreads > 0)
                            po.MaxDegreeOfParallelism = Settings.numThreads;
                        List<GenerationParameters> lgp = new List<GenerationParameters>();
                        for (int jt = 0; jt < gp.eachIteration; jt++)
                            lgp.Add(genParams);
                        ParallelLoopResult result = Parallel.For(0, gp.eachIteration, po, tt =>
                        {
                            lgp[tt].iteration = tt;
                            lgp[tt].name = Settings.generationMethodsToPrefix[s] + (n + tt).ToString();
                            handler(lgp[tt]);
                        });
                        while (!result.IsCompleted);                        
                    }
                    else
                    {
                        for (int tt = 0; tt < gp.eachIteration; tt++)
                        {
                            genParams.iteration = tt;
                            genParams.name = Settings.generationMethodsToPrefix[s] + (n + tt).ToString();
                            handler(genParams);
                        }
                    }
                    this.n += gp.eachIteration;
                }            
        }

        private void GenerateDataBaseFromRandomTruthTable(GenerationParameters param)
        {
            TruthTable tt = new TruthTable(param.inputs, param.outputs, null);
            tt.GenerateRandom(new TruthTableParameters(param.inputs, param.outputs));
            SimpleGenerators tftt = new SimpleGenerators();
            List<Tuple<string, List<string>>> circs = new List<Tuple<string, List<string>>>();
            if (param.cnfFromTruthTableParameters.CNFT)
                circs.Add(Tuple.Create("CNFT", tftt.cnfFromTruthTable(tt, true)));
            if (param.cnfFromTruthTableParameters.CNFF)
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
                c.path = this.mainPath + $"\\FromRandomTruthTable\\";
                c.circuitName = param.name + "_" + name;
                c.generate();
            }
        }

        private void GenerateDataBaseRandLevel(GenerationParameters param)
        {
            SimpleGenerators generator = new SimpleGenerators();
            List<Tuple<string, OrientedGraph>> circs = new List<Tuple<string, OrientedGraph>>();
            circs.Add(Tuple.Create("RandLevel", generator.generatorRandLevel(param.generatorRandLevelParameters.maxLevel,
                                                                        param.generatorRandLevelParameters.maxElements,
                                                                        param.inputs, param.outputs)));
            foreach (Tuple<string, OrientedGraph> nameexpr in circs)
            {
                string name = nameexpr.Item1;
                OrientedGraph graph = nameexpr.Item2;
                Circuit c = new Circuit(graph);
                c.path = this.mainPath + $"\\RandLevel\\";
                c.circuitName = param.name;
                c.generate();
            }
        }
        private void GenerateDataBaseNumOperation(GenerationParameters param)
        {
            SimpleGenerators generator = new SimpleGenerators();
            List<Tuple<string, OrientedGraph>> circs = new List<Tuple<string, OrientedGraph>>();
            circs.Add(Tuple.Create("RandLevel", generator.generatorNumOperation(param.inputs, param.outputs,
                                                                                param.generatorNumOperationParameters.logicOper,
                                                                                param.generatorNumOperationParameters.leaveEmptyOut)));
            foreach (Tuple<string, OrientedGraph> nameexpr in circs)
            {
                string name = nameexpr.Item1;
                OrientedGraph graph = nameexpr.Item2;
                Circuit c = new Circuit(graph);
                c.path = this.mainPath + $"\\NumOperation\\";
                c.circuitName = param.name;
                c.generate();
            }
        }
        private void GenerateDataBaseGenetic(GenerationParameters param)
        {
            param.geneticParameters.inputs = param.inputs;
            param.geneticParameters.outputs = param.outputs;
            GeneticGenerator<TruthTable, TruthTableParameters> gg = 
                new GeneticGenerator<TruthTable, TruthTableParameters>(
                    new TruthTableParameters(   param.geneticParameters),
                                                new Tuple<int, int>(param.inputs, param.outputs),
                                                this.mainPath+"\\");
            gg.Generate();
        }
    }
}
