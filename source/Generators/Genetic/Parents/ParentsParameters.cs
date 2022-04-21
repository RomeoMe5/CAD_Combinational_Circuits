public enum ParentsTypes
{
    Panmixia,
    Inbriding,
    Otbriding,
    Tournament,
    Roulette
}

namespace Genetics.Parent
{
    public class ParentsParameters
    {
        public ParentsTypes ParentsType { get; set; }
        public int TournematnNumber;
        public ParentsParameters()
        {
            ParentsType = ParentsTypes.Panmixia;
            TournematnNumber = 2;
        }
    }
}
