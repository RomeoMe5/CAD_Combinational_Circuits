using Genetics.Parent;
using Genetics.Recombination;

using System;

namespace Genetics
{
    public class TruthTableParameters : GeneticParameters
    {
        public int size
        {
            get
            {
                return (int)Math.Pow(2, this.inputs);
            }
        }
        public TruthTableParameters(int inps = 0, int outs = 0)
        {
            inputs = inps;
            outputs = outs;
        }
        public TruthTableParameters(GeneticParameters gp)
        {
            inputs = gp.inputs;
            outputs = gp.outputs;
            populationSize = gp.populationSize;
            numOfCycles = gp.numOfCycles;
            SelectionParameter = gp.SelectionParameter;
            RecombinationParameter = gp.RecombinationParameter;
            MutationParameter = gp.MutationParameter;
            keyEndProcessIndex = gp.keyEndProcessIndex;
        }
    }

    public class OrientedGraphParameters : GeneticParameters
    {
        
        public int maxLevel { get; set; }
        public int maxElements { get; set; }

        public OrientedGraphParameters()
        {

        }
    }

    public class GeneticParameters
    {
        public int inputs { get; set; }
        public int outputs { get; set; }
        public int populationSize { get; set; }
        public int numOfCycles { get; set; }
        public SelectionParameters SelectionParameter { get; set; }
        public RecombinationParameters RecombinationParameter { get; set; }
        public MutationParameters MutationParameter { get; set; }
        public double keyEndProcessIndex { get; set; }

        public GeneticParameters(int inps = 0, int outs = 0)
        {
            inputs = inps;
            outputs = outs;
            populationSize = 0;
            numOfCycles = 0;
            SelectionParameter = new SelectionParameters();
            RecombinationParameter = new RecombinationParameters();
            MutationParameter = new MutationParameters();
            keyEndProcessIndex = 0;
        }
    }
}