using System;
using System.Collections.Generic;
using System.Reflection;

namespace Genetics
{
    public partial class Selections<Type, ParametersType>  where ParametersType : GeneticParameters
                                            where Type : Chromosome<ParametersType>, new()
    {

        private delegate List<ChromosomeType<Type, ParametersType>> GenerateDelegate( SelectionParameters selectionParameters, 
                                                List<ChromosomeType<Type, ParametersType>> population);

        public List<ChromosomeType<Type, ParametersType>> SelectionType(  SelectionParameters selectionParameters, 
                                    List<ChromosomeType<Type, ParametersType>> population)
        {
            GenerateDelegate handler;
            string s = selectionParameters.SelectionType.ToString();
            string methodName = "Selection" + s;
            MethodInfo mi = typeof(Selections<Type, ParametersType>).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            handler = (GenerateDelegate)Delegate.CreateDelegate(type: typeof(GenerateDelegate), firstArgument: this, method: mi);
            return handler(selectionParameters, population);
        }
    }
}