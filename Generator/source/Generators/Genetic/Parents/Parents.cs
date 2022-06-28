using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Properties;
using Generators;

namespace Genetics.Parent
{
    partial class Parents<Type, ParametersType> where ParametersType : GeneticParameters
                                        where Type : Chromosome<ParametersType>, new()
    {
        private delegate List<int> GenerateDelegate(ParentsParameters parentsParameters, List<ChromosomeType<Type, ParametersType>> population);
        public List<int> ParentsTypes(ParentsParameters parentsParameters, List<ChromosomeType<Type, ParametersType>> population)
        {
            GenerateDelegate handler;
            string s = parentsParameters.ParentsType.ToString();
            string methodName = "Parents" + s + "";
            MethodInfo mi = typeof(Parents<Type, ParametersType>).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            handler = (GenerateDelegate)Delegate.CreateDelegate(type: typeof(GenerateDelegate), firstArgument: this, method: mi);
            return handler(parentsParameters, population);
        }      
        
    }
}
