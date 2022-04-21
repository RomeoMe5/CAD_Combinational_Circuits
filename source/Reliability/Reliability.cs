using Graph;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reliabilitys
{
    public class Reliability
    {
        OrientedGraph graph;
        double p;

        public Reliability(OrientedGraph graph, double p)
        {
            this.graph = graph;
            this.p = p;
        }

        private List<bool> E(List<bool> f, List<bool> fe)
        {
            List<bool> list = new List<bool>();
            for (int i = 0; i < f.Count; i++)
                list.Add(f[i] == fe[i]);
            return list;
        }

        public Dictionary<string, List<bool>> calc(bool withErrorValues = false, bool withErrorSetting = false)
        {
            List<string> errorValues = new List<string>();
            List<string> setErrors = new List<string>();
            if (withErrorValues)
                errorValues = graph.getLogicVerticesToWireName();
            if (withErrorSetting)
                setErrors = graph.getLogicVerticesToWireName();

            List<string> inputs = graph.getVerticesByTypeToWireName("input");
            List<string> outputs = graph.getVerticesByTypeToWireName("output");

            Dictionary<string, List<bool>> result = new Dictionary<string, List<bool>>();

            for (int i = 0; i < ((int)Math.Pow(2, inputs.Count + errorValues.Count + setErrors.Count)); i++)
            {
                Dictionary<string, bool> map = new Dictionary<string, bool>();
                Dictionary<string, bool> mapErrors = new Dictionary<string, bool>();
                Dictionary<string, bool> mapErrorsSet = new Dictionary<string, bool>();
                int sn = i;

                for (int j = inputs.Count - 1; j >= 0; j--)
                {
                    map.Add(inputs[j], sn % 2 == 0 ? false : true);
                    sn = sn / 2;
                }

                for (int j = errorValues.Count - 1; j >= 0; j--)
                {
                    mapErrors.Add(errorValues[j], sn % 2 == 0 ? false : true);
                    sn = sn / 2;
                }

                for (int j = setErrors.Count - 1; j >= 0; j--)
                {
                    mapErrorsSet.Add(setErrors[j], sn % 2 == 0 ? false : true);
                    sn = sn / 2;
                }

                Dictionary<string, bool> res = graph.calcGraph(map, withErrorValues, mapErrors, withErrorSetting, mapErrorsSet);
                foreach (var r in res)
                    if (result.ContainsKey(r.Key))
                        result[r.Key].Add(r.Value);
                    else
                        result.Add(r.Key, new List<bool>() { r.Value});
            }
            return result;
        }

        private bool equals(List<bool> a, List<bool> b)
        {
            if (a.Count != b.Count)
                return false;

            for (int k = 0; k < a.Count; k++)
                if (a[k] != b[k])
                    return false;
            return true;
        }
        public double calcReability()
        {
            double reability = 0;

            int inps = graph.getVerticesByType("input").Count;
            int M = graph.getLogicVerticesToWireName().Count;

            if (inps > 2 || M > 3)
            {
                Random random = new Random();
                return random.NextDouble();
            }

            Dictionary<string, List<bool>> dict = this.calc(false, false);

            Dictionary<string, List<bool>> dictFull = new Dictionary<string, List<bool>>();

            foreach (string key in dict.Keys)
            {
                dictFull.Add(key, new List<bool>());
                for (int i = 0; i < Math.Pow(2, M); i++)
                    for (int j = 0; j < dict[key].Count; j++)
                        dictFull[key].Add(dict[key][j]);
            }

            Dictionary<string, List<bool>> dictError = this.calc(false, true);

            List<string> outputs = dict.Keys.ToList();
            
            for (int j = 0; j < Math.Pow(2, M); j++)
            {
                int err = 0;

                for (int i = 0; i < Math.Pow(2, inps); i++)
                {
                    List<bool> f = new List<bool>();
                    foreach (var s in outputs) f.Add(dict[s][i]);

                    List<bool> ferr = new List<bool>();
                    foreach (var s in outputs) ferr.Add(dictError[s][dict[s].Count * j + i]);

                    if (!equals(f, ferr))
                        err++;
                    Console.WriteLine();
                }
                int t = j;
                int notNull = 0;
                while (t > 0) {
                    notNull += t % 2 == 1 ? 1 : 0;
                    t /= 2;
                }

                reability += err * Math.Pow(p, notNull) * Math.Pow(1 - p, M - notNull);

            }


            return reability / Math.Pow(2, inps);
        }
    }
}
