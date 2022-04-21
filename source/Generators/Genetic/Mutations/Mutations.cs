using System;
using System.Collections.Generic;
using System.Reflection;

namespace Genetics.Mutation
{
    public partial class Mutations<Type, ParametersType> where ParametersType : GeneticParameters
                                          where Type : Chromosome<ParametersType>, new()
    {

        private delegate List<ChromosomeType<Type, ParametersType>> GenerateDelegate(MutationParameters mutationParameters, List<ChromosomeType<Type, ParametersType>> population);
        public List<ChromosomeType<Type, ParametersType>> MutationType(MutationParameters mutationParameters, List<ChromosomeType<Type, ParametersType>> population)
        {
            GenerateDelegate handler;
            string s = mutationParameters.MutationType.ToString();
            string methodName = "Mutation" + s;
            MethodInfo mi = typeof(Mutations<Type, ParametersType>).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            handler = (GenerateDelegate)Delegate.CreateDelegate(type: typeof(GenerateDelegate), firstArgument: this, method: mi);
            return handler(mutationParameters, population);
        }
    }
}