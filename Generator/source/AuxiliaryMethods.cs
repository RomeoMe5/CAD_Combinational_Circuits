using Circuits;

using Graph;

using System;
using System.Collections.Generic;
using System.IO;
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
        public static IEnumerable<String> LineReader(String fileName)
        {
            String line;
            using (var file = System.IO.File.OpenText(fileName))
            {
                // read each line, ensuring not null (EOF)
                while ((line = file.ReadLine()) != null)
                {
                    // return trimmed line
                    yield return line.Trim();
                }
            }
        }
        public static string RemoveSpaces(string s)
        {
            char[] ch = { ' ', '\t', '\n', '\r'};
            while (s.IndexOfAny(ch) > -1)
                s = s.Remove(s.IndexOfAny(ch), 1);
            return s;
        }
        public static int SkipSpaces(string s, int start = 0)
        {
            char[] ch = {' ', '\t', '\n', '\r'};
            while (s.IndexOfAny(ch, start) == start)
                start++;
            return start;
        }
        public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }
        public static Circuit ParseVerilog(string filepath)
        {
            Circuit circuit = new Circuit();
            circuit.path = Path.GetDirectoryName(filepath);
            circuit.graph = new OrientedGraph();

            string s = File.ReadAllText(filepath);

            int start = s.IndexOf("module ");
            char[] ch = {' ', '('};
            int n2 = s.IndexOfAny(ch, start + 7);
            if (n2 == -1)
                return null;
            circuit.circuitName = RemoveSpaces(s.Substring(start + 7, n2 - start - 7));
            start = SkipSpaces(s, n2 + 1);
            List<string> inputs = new List<string>();
            string inps = "";
            string outs = "";
            if (s.IndexOf("input", start) == start)
            {
                int k = s.IndexOf("input ", start) + 6;
                int t = s.IndexOf("output ", k + 1) + 7;
                inps = RemoveSpaces(s.Substring(k, t - k - 7));
                inps = inps.Substring(0, inps.Length - 1);
                outs = RemoveSpaces(s.Substring(t, s.IndexOf(");", t + 1) - t));
                start = s.IndexOf(");", t + 1);
            }
            else
            {
                start = s.IndexOf(");", start);
                int k = s.IndexOf("input ", start) + 6;
                int k_end = s.IndexOf(";", k);
                int t = s.IndexOf("output ", start) + 7;
                int t_end = s.IndexOf(";", t);
                inps = RemoveSpaces(s.Substring(k, k_end - k));
                outs = RemoveSpaces(s.Substring(t, t_end - t));
                start = Math.Max(k_end, t_end);
            }
            foreach (string input in inps.Split(','))
            {
                circuit.graph.addVertex(input, "input");
            }

            foreach (string output in outs.Split(','))
            {
                circuit.graph.addVertex(output, "output");
            }

            { 
                int k = s.IndexOf("wire ", start) + 5;
                int k_end = s.IndexOf(";", k);
                string wires = RemoveSpaces(s.Substring(k, k_end - k));
                start = k_end + 1;
            }

            {
                s = s.Remove(0, start);
                s = s.Remove(s.IndexOf("endmodule"));
                s = s.Remove(0, SkipSpaces(s));
                int n = 0;
                while (s != "")
                {                    
                    if (!s.StartsWith("assign"))
                    {
                        int end = s.IndexOf(";");
                        string type = s.Substring(0, s.IndexOfAny(ch));

                        string[] wires = RemoveSpaces(s.Substring(s.IndexOf('(') + 1, end - 2 - s.IndexOf('('))).Split(',');
                        int tt = circuit.graph.getIndexOfWireName(wires[0]);
                        if (tt != -1 && circuit.graph.Vertices[tt].operation == "output")
                        {
                            string w = $"ewr_{n++}";
                            circuit.graph.addVertex(w, "none", w);
                            circuit.graph.addEdge(w, wires[0]);
                            wires[0] = w;
                        }
                        foreach (string wire in wires)
                            if (circuit.graph.getIndexOfWireName(wire) == -1)
                                circuit.graph.addVertex(wire, "none", wire);
                        
                        circuit.graph.Vertices[circuit.graph.getIndexOfWireName(wires[0])].operation = type;

                        if (wires.Length > 3)
                        {
                            string prev = wires[1];
                            for (int i = 2; i < wires.Length - 1; i++)
                            {
                                string wire = $"ewr_{n++}";
                                circuit.graph.addVertex(wire, type, wire);
                                circuit.graph.addDoubleEdge(wires[i], prev, wire);
                                prev = wire;
                            }
                            circuit.graph.addDoubleEdge(wires[wires.Length - 1], prev, wires[0]);
                        }
                        else
                            for (int i = 1; i < wires.Length; i++)
                                circuit.graph.addEdge(wires[i], wires[0]);

                        //Console.WriteLine(wires);
                        s = s.Remove(0, end + 1);
                    }
                    else
                    {
                        int end = s.IndexOf(";");
                        string left = RemoveSpaces(s.Substring(6, s.IndexOf('=') - 6));
                        string right = RemoveSpaces(s.Substring(s.IndexOf('=') + 1, s.IndexOf(';') - s.IndexOf('=') - 1));
                        bool f = circuit.graph.addEdge(right, left, false);
                        //Console.WriteLine(left + "=" + right);
                        s = s.Remove(0, end + 1);
                    }
                    s = s.Remove(0, SkipSpaces(s));
                }
            }
            //circuit.graph.printAdjacencyMatrix();
            return circuit;
        }

        private static object FileInfo(string filepath)
        {
            throw new NotImplementedException();
        }
    }
}
