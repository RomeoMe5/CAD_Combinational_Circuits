public enum SelectionTypes
{
    Base
}

namespace Genetics
{
    public class SelectionParameters
    {
        public SelectionTypes SelectionType { get; set; }
        public int numOfSurvivors { get; set; }

        public SelectionParameters()
        {
            SelectionType = SelectionTypes.Base;
            numOfSurvivors = 0;
        }
    }
}