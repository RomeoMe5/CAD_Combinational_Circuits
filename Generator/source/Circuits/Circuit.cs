using System;
using System.Collections.Generic;
using System.IO;
using Graph;
using Properties;
using Generators;
using System.Text;
using System.Security.Cryptography;
using Reliabilitys;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Linq;

namespace Circuits
{
    
    class Circuit
    {
        public OrientedGraph graph { get; set; }
        public List<string> logExpressions { get; private set; }
        public TruthTable tTable { get; set; }
        public string path { get; set; }
        public string circuitName { get; set; }
        public CircuitParameters circuitParameters { get; private set; }
        private Settings settings;

        public Circuit()
        {
            this.graph = null;
            this.settings = Settings.GetInstance();
        }
        public Circuit(OrientedGraph graph, List<string> logExpressions = null)
        {
            this.graph = graph;
            this.graph.updateLevels();
            if (logExpressions == null)
                this.logExpressions = new List<string>();
            else
                this.logExpressions = logExpressions;
            this.settings = Settings.GetInstance();
        }
        public void UpdateCircuitParameters()
        {            
            if (graph == null)
                return;

            graph.updateLevels();

            if (this.circuitParameters == null)
                this.circuitParameters = new CircuitParameters();

            this.circuitParameters.name = circuitName;

            List<string> inputs = graph.getVerticesByType("input");
            List<string> outputs = graph.getVerticesByType("output");
            
            this.circuitParameters.numInputs = 0;
            for (int i = 0; i < inputs.Count; i++)
                if (!inputs[i].Contains("'b"))
                    this.circuitParameters.numInputs++;

            this.circuitParameters.numOutputs = outputs.Count;

            this.circuitParameters.maxLevel = graph.MaxLevel;

            this.circuitParameters.numEdges = 0;
            for (int i = 0; i < graph.AdjacencyMatrix.Count; i++)
                for (int j = 0; j < graph.AdjacencyMatrix[i].Count; j++)
                    if (graph.AdjacencyMatrix[i][j])
                        this.circuitParameters.numEdges++;

            Reliability R = new Reliability(graph, 0.5);
            Dictionary<string, double> dict = R.runNadezhda(path, circuitName);
            this.circuitParameters.reliability = dict["reliability_metric"];
            this.circuitParameters.size = dict["size"];
            this.circuitParameters.area = dict["area"];
            this.circuitParameters.longest_path = Convert.ToInt32(dict["longest_path"]);

            this.circuitParameters.gates = Convert.ToInt32(dict["gates"]);
            this.circuitParameters.sensitivity_factor = dict["sensitivity_factor"];
            this.circuitParameters.reliability_percent = dict["sensitivity_factor_percent"];
            this.circuitParameters.sensitive_area = dict["sensitive_area"];
            this.circuitParameters.sensitive_area_percent = dict["sensitive_area_percent"];            

            //this.circuitParameters.reconvergation = 0; // TODO: 
            //this.circuitParameters.averageDistanceBetweenElements = 0; // TODO:             

            this.circuitParameters.numElementsOfEachType = new Dictionary<string, int>();
            List<GraphVertex> gv = graph.Vertices;
            foreach (var lo in settings.logicOperations)
                this.circuitParameters.numElementsOfEachType.Add(lo.Key, 0);
            foreach (var v in gv)
                foreach (var lo in settings.logicOperations)
                    if (v.Operation == lo.Key)
                        this.circuitParameters.numElementsOfEachType[v.Operation]++;

            this.circuitParameters.numEdgesOfEachType = new Dictionary<Tuple<string, string>, int>();
            foreach (var lo1 in settings.logicOperations)
                foreach (var lo2 in settings.logicOperations)
                    if (lo1.Key != "output" && lo2.Key != "input")
                        this.circuitParameters.numEdgesOfEachType.Add(Tuple.Create<string, string>(lo1.Key, lo2.Key), 0);
            for (int i = 0; i < gv.Count; i++)
                for (int j = 0; j < gv.Count; j++)
                    if (this.graph.AdjacencyMatrix[i][j])
                        this.circuitParameters.numEdgesOfEachType[Tuple.Create<string, string>(gv[i].Operation, gv[j].Operation)]++;


            this.computeHash();

        }
        static string ByteArrayToString(byte[] arrInput)
        {
            int i;
            StringBuilder sOutput = new StringBuilder(arrInput.Length);
            for (i = 0; i < arrInput.Length - 1; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString();
        }
        public void computeHash()
        {
            string s = "";

            s += string.Format("{0,10}\n", circuitParameters.numInputs);
            s += string.Format("{0,10}\n", circuitParameters.numOutputs);
            s += string.Format("{0,10}\n", circuitParameters.maxLevel);
            s += string.Format("{0,10}\n", circuitParameters.numEdges);
            //s += string.Format("{0,10}\n", circuitParameters.reconvergation);
            s += string.Format("{0,10}\n", circuitParameters.reliability);
            s += string.Format("{0,10}\n", circuitParameters.size);
            s += string.Format("{0,10}\n", circuitParameters.area);
            s += string.Format("{0,10}\n", circuitParameters.gates);
            s += string.Format("{0,10}\n", circuitParameters.sensitive_area);
            s += string.Format("{0,10}\n", circuitParameters.sensitive_area_percent);
            s += string.Format("{0,10}\n", circuitParameters.sensitivity_factor);
            s += string.Format("{0,10}\n", circuitParameters.reliability_percent);
            //s += string.Format("{0,10}\n", circuitParameters.averageDistanceBetweenElements);

            List<string> keys = circuitParameters.numElementsOfEachType.Keys.ToList<string>();
            keys.Sort();

            foreach (string key in keys)
            {
                s += String.Format("\t\t\"{0}\": {1}\n", key, circuitParameters.numElementsOfEachType[key]);
            }

            List<Tuple<string, string>> keys2 = circuitParameters.numEdgesOfEachType.Keys.ToList();
            keys2.Sort();
            foreach (var key in keys2)
            {
                s += String.Format("\t\t\"{0}\": {1}\n", key.Item1 + "-" + key.Item2, circuitParameters.numEdgesOfEachType[key]);
            }
            using (SHA256 sha256Hash = SHA256.Create())
            {
                string hash = "";
                byte[] data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(s));
                var sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                    hash = sBuilder.ToString();
                }
                circuitParameters.hashCode = hash;
            }                        
        }
        public bool GraphToVerilog(string path, bool pathExists = false)
        {
            if (graph == null)
                return false;

            if (!pathExists)
            {
                if (!Directory.Exists(Directory.GetCurrentDirectory() + path))
                    Directory.CreateDirectory(path);
            }
            string fileName = this.path + "\\" + circuitName + ".v";
            List<string> inputs = graph.getVerticesByType("input");
            List<string> outputs = graph.getVerticesByType("output");
            List<string> consts = graph.getVerticesByType("const");
            string s = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            bool F = File.Exists(s);
            if (File.Exists(fileName))
                File.Delete(fileName);
            FileStream file = new FileStream(fileName, FileMode.CreateNew);
            StreamWriter writer = new StreamWriter(file);

            if (logExpressions != null)
                for (int i = 0; i < logExpressions.Count; i++)
                    writer.WriteLine($"//{logExpressions[i]}");


            StringBuilder sb = new StringBuilder();
            //StringWriter sw = new StringWriter(sb);
            //TextWriter oldOut = Console.Out;
            //Console.SetOut(sw);
            //this.tTable.printTable();
            //Console.SetOut(oldOut);
            //sw.Close();            
            //writer.WriteLine("//" + sb.Replace("\n", "\n//").ToString());
            //sb.Clear();

            writer.WriteLine($"module {circuitName}(");

            sb.Append("\tinput");
            for (int i = 0; i < inputs.Count; i++)
                if (!inputs[i].Contains("1'b"))
                    sb.Append($" {inputs[i]},");
            if (sb.ToString().Length > 6)
                writer.WriteLine(sb.ToString());

            writer.Write("\toutput");
            for (int i = 0; i < outputs.Count; i++)
                if (i == 0)
                    writer.Write($" {outputs[i]}");
                else
                    writer.Write($", {outputs[i]}");
            writer.WriteLine();

            writer.WriteLine($");");

            if (graph.Vertices.Count - inputs.Count - outputs.Count - consts.Count > 0)
            {
                writer.Write($"\n\twire");
                bool f = true;
                for (int i = 0; i < graph.Vertices.Count; i++)
                {
                    GraphVertex vert = graph.Vertices[i];
                    if (vert.Operation != "input" && vert.Operation != "output" && vert.Operation != "const")
                    {
                        if (f)
                        {
                            writer.Write($" {vert.WireName}");
                            f = false;
                        }
                        else
                            writer.Write($", {vert.WireName}");
                    }
                }
                writer.WriteLine($";\n");
            }

            for (int j = 0; j < graph.Vertices.Count; j++)
            {
                if (graph.Vertices[j].Operation != "input")
                {
                    List<int> inps = new List<int>();
                    for (int i = 0; i < graph.Vertices.Count; i++)
                        if (graph.AdjacencyMatrix[i][j])
                            inps.Add(i);
                    if (graph.Vertices[j].Operation != "output")
                    {
                        if (graph.Vertices[j].Operation != "const")
                        {
                            writer.Write($"\t{graph.Vertices[j].Operation} {""}({graph.Vertices[j].WireName}");
                            foreach (int k in inps)
                                writer.Write($", {graph.Vertices[k].WireName}");
                            writer.WriteLine(");");
                        }
                    }
                    else
                        if (inps.Count > 0)
                            writer.WriteLine($"\tassign {graph.Vertices[j].WireName} = {graph.Vertices[inps[0]].WireName};");
                }
            }

            writer.WriteLine("endmodule");

            writer.Close();
            file.Close();
            return true;
        }
        public bool saveParameters(bool pathExists = false)
        {
            if (graph == null)
                return false;
            if (!pathExists)
                if (!Directory.Exists(Directory.GetCurrentDirectory() + path))
                    Directory.CreateDirectory(path);
            string fileName = this.path + "\\" + circuitName + ".json";            

            if (File.Exists(fileName))
                File.Delete(fileName);
            FileStream file = new FileStream(fileName, FileMode.CreateNew);
            StreamWriter writer = new StreamWriter(file);
            
            writer.WriteLine("{");

            writer.WriteLine("\t\"{0}\": \"{1}\",", "name", this.circuitParameters.name);
            writer.WriteLine("\t\"{0}\": {1},", "numInputs", circuitParameters.numInputs);
            writer.WriteLine("\t\"{0}\": {1},", "numOutputs", circuitParameters.numOutputs);
            writer.WriteLine("\t\"{0}\": {1},", "maxLevel", circuitParameters.maxLevel);
            writer.WriteLine("\t\"{0}\": {1},", "numEdges", circuitParameters.numEdges);
            //writer.WriteLine("\t\"{0}\": {1},", "reconvergation", circuitParameters.reconvergation);
            writer.WriteLine("\t\"{0}\": {1},", "reliability", circuitParameters.reliability.ToString(CultureInfo.InvariantCulture));
            writer.WriteLine("\t\"{0}\": {1},", "size", circuitParameters.size.ToString(CultureInfo.InvariantCulture));
            writer.WriteLine("\t\"{0}\": {1},", "area", circuitParameters.area.ToString(CultureInfo.InvariantCulture));
            writer.WriteLine("\t\"{0}\": {1},", "longest_path", circuitParameters.longest_path.ToString(CultureInfo.InvariantCulture));
            writer.WriteLine("\t\"{0}\": {1},", "gates", circuitParameters.gates.ToString(CultureInfo.InvariantCulture));
            writer.WriteLine("\t\"{0}\": {1},", "sensitivity_factor", circuitParameters.sensitivity_factor.ToString(CultureInfo.InvariantCulture));
            writer.WriteLine("\t\"{0}\": {1},", "sensitivity_factor_percent", circuitParameters.reliability_percent.ToString(CultureInfo.InvariantCulture));
            writer.WriteLine("\t\"{0}\": {1},", "sensitive_area", circuitParameters.sensitive_area.ToString(CultureInfo.InvariantCulture));
            writer.WriteLine("\t\"{0}\": {1},", "sensitive_area_percent", circuitParameters.sensitive_area_percent.ToString(CultureInfo.InvariantCulture));
            //writer.WriteLine("\t\"{0}\": {1},", "averageDistanceBetweenElements", circuitParameters.averageDistanceBetweenElements);
            writer.WriteLine("\t\"{0}\": \"{1}\",", "hashCode", circuitParameters.hashCode);

            writer.WriteLine("\t\"{0}\": {{","numElementsOfEachType");
            foreach (string key in circuitParameters.numElementsOfEachType.Keys)
            {
                if (circuitParameters.numElementsOfEachType[key] != 0)
                    writer.WriteLine("\t\t\"{0}\": {1},", key, circuitParameters.numElementsOfEachType[key]);
            }
            writer.WriteLine($"\t}},");

            writer.WriteLine("\t\"{0}\": {{", "numEdgesOfEachType");
            foreach (var key in circuitParameters.numEdgesOfEachType.Keys)
            {
                if (circuitParameters.numEdgesOfEachType[key] != 0)
                    writer.WriteLine("\t\t\"{0}\": {1},", key.Item1 + "-" + key.Item2, circuitParameters.numEdgesOfEachType[key]);
            }
            writer.WriteLine($"\t}},");


            writer.Write($"}}");

            writer.Close();
            file.Close();

            return true;
        }
        private bool checkExistingHash()
        {
            var dir = new DirectoryInfo(this.path);
            string path = dir.Parent.FullName;
            if (!File.Exists(path))
                return false;

            StreamReader sr = new StreamReader(path);
            string line = sr.ReadLine();
            while (line != null)
            {
                if (line == this.circuitParameters.hashCode)
                {
                    sr.Close();
                    return true;
                }
                line = sr.ReadLine();
            }
            sr.Close();
            return false;
        }
        public bool generate(bool pathExists = false)
        {
            if (!pathExists)
                path += this.circuitName;
            if (!this.GraphToVerilog(path, pathExists))
                return false;

            this.UpdateCircuitParameters();

            if (!this.saveParameters())
                return false;
            if (checkExistingHash() || (this.circuitParameters.reliability == 0) || (this.circuitParameters.gates == 0))
            {
                if (!pathExists)
                    Directory.Delete(path, true);
            }
            else
            {
                var dir = new DirectoryInfo(this.path);
                string path = dir.Parent.FullName + "\\hashCodes.txt";
                StreamWriter sw = new StreamWriter(path, true);
                sw.WriteLine(this.circuitParameters.hashCode);
                sw.Close();
            }

            return true;
        }
    }
}
