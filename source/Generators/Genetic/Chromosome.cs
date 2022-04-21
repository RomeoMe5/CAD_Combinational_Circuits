
namespace Genetics
{
    enum genotypeParametersTypes
    {
        TruthTable,
        //OrientedGraph
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Каждому типу генов соответствует свой набор параметров.</typeparam>
    public interface Chromosome<T>
    {
        void GenerateRandom(T gp);
    }
}
