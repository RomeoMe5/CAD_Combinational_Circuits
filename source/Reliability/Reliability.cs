using Graph;

using Newtonsoft.Json.Linq;

using Properties;

using source;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reliabilitys
{
    public class Reliability
    {
        OrientedGraph graph;
        double p;
        private Settings settings;

        public Reliability(OrientedGraph graph, double p = 0.5)
        {
            this.graph = graph;
            this.p = p;
            this.settings = Settings.GetInstance();
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
                        result.Add(r.Key, new List<bool>() { r.Value });
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
        public double calcReabilityBase()
        {
            double reability = 0;

            int inps = graph.getVerticesByType("input").Count;
            int M = graph.getLogicVerticesToWireName().Count;


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
                while (t > 0)
                {
                    notNull += t % 2 == 1 ? 1 : 0;
                    t /= 2;
                }

                reability += err * Math.Pow(p, notNull) * Math.Pow(1 - p, M - notNull);

            }


            return reability / Math.Pow(2, inps);
        }

        public Dictionary<string, double> runNadezhda(string path, string circuitName)
        {
            Dictionary<string, double> dict = new Dictionary<string, double>
            {
                {"reliability_metric", 0},
                {"area", 0},
                {"size", 0},
                {"longest_path", 0},
                {"gates", 0},
                {"sensitivity_factor", 0},
                {"sensitivity_factor_percent", 0},
                {"sensitive_area", 0},
                {"sensitive_area_percent", 0},
            };
            string curPath = Directory.GetCurrentDirectory();
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.WorkingDirectory = curPath;
            cmd.Start();

            cmd.StandardInput.WriteLine("cd " + Settings.pathNadezhda);
            cmd.StandardInput.Write(Settings.nadezhda["python"] + " ");
            cmd.StandardInput.Write(Settings.nadezhda["resynthesis"] + " ");
            cmd.StandardInput.Write(path + "\\" + circuitName + ".v ");
            cmd.StandardInput.Write(Settings.nadezhda["liberty"] + " ");
            cmd.StandardInput.Write(path + "\\res ");
            cmd.StandardInput.Write("-y");
            cmd.StandardInput.WriteLine();
            cmd.StandardInput.WriteLine("copy " + path + "\\res\\" + circuitName + "r.v " + path + "\\" + circuitName + "_Nangate.v");
            cmd.StandardInput.WriteLine("copy " + path + "\\res\\" + circuitName + "_report.json " + path + "\\report.json");
            cmd.StandardInput.WriteLine("rd /s /q " + path + "\\res");

            cmd.StandardInput.Write(Settings.nadezhda["python"] + " ");
            cmd.StandardInput.Write(Settings.nadezhda["reliability"] + " ");
            cmd.StandardInput.Write(path + "\\" + circuitName + "_Nangate.v ");
            cmd.StandardInput.Write(Settings.nadezhda["liberty"] + " ");
            cmd.StandardInput.Write(path + "\\report.txt");
            cmd.StandardInput.WriteLine();
            //cmd.StandardInput.WriteLine("rd /s /q " + path + "\\" + circuitName + "_Nangate.v");

            //cmd.StandardInput.Write(Settings.nadezhda["python"] + " ");
            //cmd.StandardInput.Write(Settings.nadezhda["reliability"] + " ");
            //cmd.StandardInput.Write(path + "\\" + circuitName + "r.v ");
            //cmd.StandardInput.Write(Settings.nadezhda["liberty"] + " ");
            //cmd.StandardInput.Write(path + "\\reportr.txt");
            //cmd.StandardInput.WriteLine();

            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();

            if (File.Exists(path + "\\report.json")) {
                string s = File.ReadAllText(path + "\\report.json");
                JObject obj = JObject.Parse(s);
                foreach (var token in obj)
                {
                    //change the display Content of the parent
                    if (token.Key.ToString() == "before")
                    {
                        foreach (var itm in (JObject)token.Value)
                        {
                            if (itm.Key.ToString() == "reliability_metric")
                                dict[itm.Key.ToString()] = (double)itm.Value;
                            if (itm.Key.ToString() == "area")
                                dict[itm.Key.ToString()] = (double)itm.Value;
                            if (itm.Key.ToString() == "size")
                                dict[itm.Key.ToString()] = (double)itm.Value;
                            if (itm.Key.ToString() == "longest_path")
                                dict[itm.Key.ToString()] = (double)itm.Value;
                        }
                    }
                }
                File.Delete(path + "\\report.json");                
            }

            if (File.Exists(path + "\\report.txt"))
            {
                string s = File.ReadAllText(path + "\\report.txt");
                int start = 0;
                start = s.IndexOf(": ");
                int end = s.IndexOf('\n', start);
                string sub = s.Substring(start + 2, end - start - 2);
                sub = sub.Replace(".", ",");
                dict["gates"] = Convert.ToDouble(AuxiliaryMethods.RemoveSpaces(sub));
                start = end;

                start = s.IndexOf(": ", start);
                end = s.IndexOf(' ', start + 2);
                sub = s.Substring(start + 2, end - start - 2);
                sub = sub.Replace(".", ",");
                dict["sensitivity_factor"] = Convert.ToDouble(AuxiliaryMethods.RemoveSpaces(sub));
                start = end;

                start = s.IndexOf(" ", start);
                end = s.IndexOf('\n', start + 1);
                sub = s.Substring(start + 2, end - start - 5);
                sub = sub.Replace(".", ",");
                dict["sensitivity_factor_percent"] = Convert.ToDouble(AuxiliaryMethods.RemoveSpaces(sub));
                start = end;

                start = s.IndexOf(": ", start);
                end = s.IndexOf(' ', start + 2);
                sub = s.Substring(start + 2, end - start - 2);
                sub = sub.Replace(".", ",");
                dict["sensitive_area"] = Convert.ToDouble(AuxiliaryMethods.RemoveSpaces(sub));
                start = end;

                start = s.IndexOf(" ", start);
                end = s.IndexOf('\n', start + 1);
                sub = s.Substring(start + 2, end - start - 5);
                sub = sub.Replace(".", ",");
                dict["sensitive_area_percent"] = Convert.ToDouble(AuxiliaryMethods.RemoveSpaces(sub));

                File.Delete(path + "\\report.txt");
            }            

            return dict;
        }
    }
}
