using System;
using System.Collections.Generic;
using System.Reflection;
using Generators;
using Genetics.Parent;

using source;

namespace Genetics.Recombination
{
    public partial class Recombinations<Type, ParametersType>   where ParametersType : GeneticParameters
                                                    where Type : Chromosome<ParametersType>, new()
    {

        private delegate List<ChromosomeType<Type, ParametersType>> GenerateDelegate( RecombinationParameters recombinationParameters,
                                                                                List<ChromosomeType<Type, ParametersType>> population);
        public List<ChromosomeType<Type, ParametersType>> RecombinationType(   RecombinationParameters recombinationParameters,
                                                                    List<ChromosomeType<Type, ParametersType>> population)
        {
            GenerateDelegate handler;
            string s = recombinationParameters.RecombinationType.ToString();
            string methodName = "Recombination" + s;
            MethodInfo mi = typeof(Recombinations<Type, ParametersType>).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            handler = (GenerateDelegate)Delegate.CreateDelegate(type: typeof(GenerateDelegate), firstArgument: this, method: mi);
            
            return handler(recombinationParameters, population);
        }
    }

}
