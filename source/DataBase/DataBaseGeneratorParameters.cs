public enum GenerationTypes
{
    CNFFromTruthTable = 0,
    RandLevel,
    NumOperation,
    Genetic
}

namespace DataBaseGenerators
{
    public class DataBaseGeneratorParameters
    {
        public int minInputs { get; set; }
        public int maxInputs { get; set; }
        public int minOutputs { get; set; }
        public int maxOutputs { get; set; }
        public int eachIteration { get; set; }
        public GenerationTypes generationTypes { get; set; }
        public GenerationParameters generationParameters { get; set; }
        public DataBaseGeneratorParameters()
        {
            minInputs = 0;
            maxInputs = 0;
            minOutputs = 0;
            maxOutputs = 0;
            eachIteration = 0;
            generationTypes = GenerationTypes.CNFFromTruthTable;
            generationParameters = new GenerationParameters();
        }
    }
}
