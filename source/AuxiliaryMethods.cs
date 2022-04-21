using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace source
{
    static class AuxiliaryMethods
    {
        public static List<int> GetRandomIntList(int n, int minNumber, int maxNumber, bool repite = false)
        {
            Random random = new Random();
            List<int> lst = new List<int>();
            bool flag;
            do
            {
                int i;
                flag = false;
                int k = n - lst.Count;
                for (i = 0; i < k; i++)
                    lst.Add(random.Next(minNumber, maxNumber));

                lst.Sort(); //сортируем

                if (!repite)
                {
                    i = 0;
                    while (i < lst.Count - 1)
                    {
                        if (lst[i] == lst[i + 1])
                        {
                            flag = true;
                            lst.Remove(lst[i]);
                        }
                        else
                            i++;
                    }
                }
            } while (flag);

            return lst;
        }
    
        public static Dictionary<TypeK, TypeV> SortDictByValue<TypeK, TypeV>(Dictionary<TypeK, TypeV> dict, bool up = true)
        {
            if (up)
                return (Dictionary<TypeK, TypeV>)(from entry in dict orderby entry.Value ascending select entry);

            return (Dictionary<TypeK, TypeV>)(from entry in dict orderby entry.Value descending select entry);
        }

        public static T ToEnum<T>(this string value, bool ignoreCase = true)
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }
    }
}
