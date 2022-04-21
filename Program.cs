using Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Graph;
using Properties;
using Circuits;
using DataBaseGenerators;
using ConsoleTables;
using Reliabilitys;

[assembly: CLSCompliant(true)]
namespace CombinationalCircuitDatabaseGenerator
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

            /*DataBaseGeneratorParameters dbgp = new DataBaseGeneratorParameters();
            dbgp.generationTypes = GenerationTypes.CNFFromTruthTable;
            dbgp.minInputs = 1;
            dbgp.maxInputs = 10;
            dbgp.minOutputs = 1;
            dbgp.maxOutputs = 10;
            dbgp.eachIteration = 100;*/

            //dbgp.generationParameters.geneticParameters.populationSize = 10;
            //dbgp.generationParameters.geneticParameters.numOfCycles = 100;
            //dbgp.generationParameters.geneticParameters.keyEndProcessIndex = 0.7;

            //dbgp.generationParameters.geneticParameters.RecombinationParameter.ParentsParameter.ParentsType = ParentsTypes.Panmixia;
            //dbgp.generationParameters.geneticParameters.RecombinationParameter.RecombinationType = RecombinationTypes.CrossingEachExitInTurnMany;
            //dbgp.generationParameters.geneticParameters.RecombinationParameter.refPoints = 2;

            //dbgp.generationParameters.geneticParameters.MutationParameter.MutationType = MutationTypes.Binary;
            //dbgp.generationParameters.geneticParameters.MutationParameter.probabilityGen = 0.1;
            //dbgp.generationParameters.geneticParameters.MutationParameter.probabilityTruthTable = 0.1;

            //dbgp.generationParameters.geneticParameters.SelectionParameter.SelectionType = SelectionTypes.Base;
            //dbgp.generationParameters.geneticParameters.SelectionParameter.numOfSurvivors = 10;

            //DataBaseGenerator generator = new DataBaseGenerator();
            //generator.GenerateType(dbgp);

            //Parser parser = new Parser("f0 = (x1 and x2) nand x3 or 1'b1");
            //parser.Parse("f0 = (x1 and x2) nand x3 or 1'b1");
            //OrientedGraph graph = parser.Graph;
            //Circuit circuit = new Circuit(graph);
            //circuit.path = "test\\";
            //circuit.circuitName = "test";
            //graph.printAdjacencyMatrix(true);
        }
    }
}
