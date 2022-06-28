public enum MutationTypes
{
    Binary,
    Density,
    AccessionDel,
    InsertDel,
    Exchange,
    Delete
}

namespace Genetics
{
    public class MutationParameters
    {
        public MutationTypes MutationType { get; set; }
        public double probabilityGen { get; set; }
        public int exchangeType { get; set; }
        public double probabilityTruthTable { get; set; }
        public MutationParameters()
        {
            MutationType = MutationTypes.Binary;
            probabilityGen = 0.1;
            probabilityTruthTable = 0.1;
            exchangeType = 0;
        }
    }
}