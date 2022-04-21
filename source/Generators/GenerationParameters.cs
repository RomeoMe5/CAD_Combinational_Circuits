using Genetics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseGenerators
{
    public class CNFFromTruthTableParameters
    {
        public bool generateLimitation;
        public bool CNFF;
        public bool CNFT;
        public CNFFromTruthTableParameters()
        {
            generateLimitation = false;
            CNFF = false;
            CNFT = false;
        }
    }

    public class GeneratorRandLevelParameters
    {
        public int maxLevel { get; set; }
        public int maxElements { get; set; }
        public GeneratorRandLevelParameters()
        {
            maxElements = 0;
            maxLevel = 0;
        }
    }
    public class GeneratorNumOperationParameters
    {
        public Dictionary<string, int> logicOper { get; set; }
        public bool leaveEmptyOut { get; set; }
        public GeneratorNumOperationParameters()
        {

            logicOper = new Dictionary<string, int>();
            leaveEmptyOut = true;
        }
    }
    public class GenerationParameters
    {
        public int inputs { get; set; }
        public int outputs { get; set; }
        public int iteration { get; set; }

        public CNFFromTruthTableParameters cnfFromTruthTableParameters;        
        public GeneratorRandLevelParameters generatorRandLevelParameters;
        public GeneratorNumOperationParameters generatorNumOperationParameters;

        public GeneticParameters geneticParameters;

        public GenerationParameters()
        {
            inputs = 0;
            outputs = 0;
            iteration = 0;

            cnfFromTruthTableParameters = new CNFFromTruthTableParameters();
            generatorRandLevelParameters = new GeneratorRandLevelParameters();
            generatorNumOperationParameters = new GeneratorNumOperationParameters();
            geneticParameters = new GeneticParameters();
        }
    }
}
