using Genetics.Parent;

public enum RecombinationTypes
{
    CrossingEachExitInTurnMany,//относительно нескольких опорных точек
    CrossingUniform,//однородный кроссинговер
    CrossingTriadic,//триадный кроссинговер
    CrossingReducedReplacemnt,//кроссинговер с уменьшением замены
    CrossingShuffling//перетасовочный кроссинговер
}

namespace Genetics.Recombination
{  
    public class RecombinationParameters
    {
        public RecombinationTypes RecombinationType { get; set; }
        public ParentsParameters ParentsParameter { get; set; }
        public int refPoints { get; set; }
        public double maskProbability { get; set; }
        public int recombinationNumber { get; set; }

        public RecombinationParameters()
        {
            RecombinationType = RecombinationTypes.CrossingEachExitInTurnMany;
            ParentsParameter = new ParentsParameters();
            refPoints = 1;
            maskProbability = 0.5;
            recombinationNumber = 1;
        }
    }

}
