using Generators;

using Properties;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetics
{
    public class ChromosomeType<Type, ParametersType>   where ParametersType : GeneticParameters
                                                        where Type : Chromosome<ParametersType>
    {
        double adaptationIndex;
        public Type chromosome { set; get; }
        private Settings settings;
        private string name;

        public ChromosomeType(string name, Type obj)
        {
            this.settings = Settings.GetInstance();
            this.chromosome = obj;
            this.name = name;
            adaptationFunction();
        }

        /// <summary>
        /// Адаптационная функция, подсчитывающая коэффициент для каждого типа генов.
        /// </summary>
        public void adaptationFunction()
        {
            if (chromosome is TruthTable tt)
            {
                int num_unit = 0;
                foreach (bool[] i in tt.OutTable)
                {
                    foreach (bool j in i)
                    {
                        if (j)
                        {
                            num_unit++;
                        }
                    }
                }
                adaptationIndex = Math.Abs(0.5 - (num_unit / (tt.Size * tt.Output)));
                return;
            }
        }

        public double AdaptationIndex
        {
            get
            {
                return adaptationIndex;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

    }
}
