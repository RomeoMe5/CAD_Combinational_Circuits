using System;
using System.Collections.Generic;
using ConsoleTables;
using Genetics;
using Properties;
using Generators;

namespace Graph
{
    /// <summary>
    /// Граф
    /// </summary>
    public class OrientedGraph : Chromosome<OrientedGraphParameters>
    {
        /// <summary>
        /// Список вершин графа
        /// </summary>
        private List<GraphVertex> vertices;
        private List<List<bool>> adjacencyMatrix;
        private Dictionary<string, bool> error;
        private Settings settings;

        /// <summary>
        /// Конструктор
        /// </summary>
        public OrientedGraph()
        {
            this.vertices = new List<GraphVertex>();
            adjacencyMatrix = new List<List<bool>>();
            this.settings = Settings.GetInstance();
        }

        public List<GraphVertex> Vertices
        {
            get
            {
                return vertices;
            }
        }
        
        public List<List<bool>> AdjacencyMatrix
        {
            get
            {
                return adjacencyMatrix;
            }
        }

        public int MaxLevel
        {
            get {
                var outputs = this.getVerticesByType("output");
                int maxLevel = 0;
                for (int i = 0; i < outputs.Count; i++)
                {
                    int n = this.getIndexOfExpression(outputs[i]);
                    n = Vertices[n].Level;
                    maxLevel = n > maxLevel ? n : maxLevel;
                }
                return vertices.Count; 
            }
        }

        public void GenerateRandom(OrientedGraphParameters gp = null)
        {
            Random rnd = new Random();
            int inputs;
            int outputs;
            int maxLevel;
            int maxElements;
            if (gp != null)
            {
                inputs = gp.inputs;
                outputs = gp.outputs;
                maxLevel = gp.maxLevel;
                maxElements = gp.maxElements;
            }
            else
            {
                inputs = rnd.Next(0, settings.maxInputs); ;
                outputs = rnd.Next(0, settings.maxOutputs);
                maxLevel = gp.maxLevel;
                maxElements = gp.maxElements;
            }

            SimpleGenerators sg = new SimpleGenerators();
            OrientedGraph grd = sg.generatorRandLevel(maxLevel, maxElements, inputs, outputs);
            this.vertices = grd.vertices;
            this.adjacencyMatrix = grd.adjacencyMatrix;
        }

        /// <summary>
        /// Поиска индекса вершины в списке по выполняемому логическому выражению.
        /// </summary>
        /// <param name="expression">Логическое выражение</param>
        /// <returns></returns>
        public int getIndexOfExpression(string expression)
        {
            //TODO: провести поиск не по полномму совпадению, а по перестановкам.
            for (int i = 0; i < this.vertices.Count; i++)
            {
                if (vertices[i].LogicExpression == expression)
                {
                    return i;
                }
            }
            return -1;
        }
        public int getIndexOfWireName(string wireName)
        {
            for (int i = 0; i < this.vertices.Count; i++)
            {
                if (vertices[i].WireName == wireName)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Добавление вершины
        /// </summary>
        /// <param logicExpression="vertexName">Имя вершины</param>
        public bool addVertex(string vertexName, string operation)
        {
            if (this.getIndexOfExpression(vertexName) != -1)
                return false;
            vertices.Add(new GraphVertex(vertexName, operation));
            for (int i = 0; i < vertices.Count - 1; i++)
                adjacencyMatrix[i].Add(false);
            adjacencyMatrix.Add(new List<bool>());
            for (int i = 0; i < adjacencyMatrix.Count; i++)
                adjacencyMatrix[vertices.Count - 1].Add(false);
            return true;
        }

        /// <summary>
        /// Добавление ребра
        /// </summary>
        /// <param logicExpression="vertexFrom">Имя первой вершины</param>
        /// <param logicExpression="vertexTo">Имя второй вершины</param>
        public bool addEdge(string vertexFrom, string vertexTo)
        {
            int v1 = this.getIndexOfExpression(vertexFrom);
            int v2 = this.getIndexOfExpression(vertexTo);
            if (v1 != -1 && v2 != -1)
            {
                vertices[v2].Level = Math.Max(vertices[v1].Level + 1, vertices[v2].Level);
                adjacencyMatrix[v1][v2] = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Добавление двух ребер
        /// </summary>
        /// <param logicExpression="vertexFrom">Имя первой вершины</param>
        /// <param logicExpression="vertexTo">Имя второй вершины</param>
        public bool addDoubleEdge(string vertexFromFirst, string vertexFromSecond, string vertexTo)
        {
            int v1 = this.getIndexOfExpression(vertexFromFirst);
            int v2 = this.getIndexOfExpression(vertexFromSecond);
            int v3 = this.getIndexOfExpression(vertexTo);
            if (v1 != -1 && v2 != -1 && v3 != -1)
            {
                vertices[v3].Level = Math.Max(vertices[v1].Level + 1, vertices[v3].Level);
                vertices[v3].Level = Math.Max(vertices[v2].Level + 1, vertices[v3].Level);
                adjacencyMatrix[v1][v3] = true;
                adjacencyMatrix[v2][v3] = true;

                return true;
            }
            return false;
        }


        /// <summary>
        /// Вывод таблицы смежности графа в консоль.
        /// </summary>
        /// <param name="isExpressions">Вывод названий вершин. false - номера в списке. true - логические выражения.</param>
        public void printAdjacencyMatrix(bool isExpressions = false)
        {
            var consTable = new ConsoleTable();

            string[] cols = new string[vertices.Count + 1];
            string[][] row = new string[vertices.Count][];

            cols[0] = "";
            for (int i = 0; i < this.vertices.Count; i++)
                cols[i + 1] = isExpressions ? vertices[i].WireName : vertices[i].LogicExpression;
            consTable.AddColumn(cols);

            for (int i = 0; i < this.vertices.Count; i++)
            {
                row[i] = new string[vertices.Count + 1];
                row[i][0] = isExpressions ? vertices[i].WireName : vertices[i].LogicExpression;
                for (int j = 1; j < this.vertices.Count + 1; j++)
                    row[i][j] = String.Format($"{Convert.ToInt32(this.adjacencyMatrix[i][j-1])}");
                consTable.AddRow(row[i]);
            }
            consTable.Write(Format.Alternative);
        }

        public List<string> getVerticesByType(string type)
        {
            List<string> names = new List<string>();
            foreach (var vert in this.vertices)
            {
                if (vert.Operation == type)
                    names.Add(vert.LogicExpression);
            }
            return names;
        }

        public List<string> getVerticesByTypeToWireName(string type)
        {
            List<string> names = new List<string>();
            foreach (var vert in this.vertices)
            {
                if (vert.Operation == type)
                    names.Add(vert.WireName);
            }
            return names;
        }

        public List<string> getLogicVerticesToWireName()
        {
            List<string> names = new List<string>();
            foreach (var vert in this.vertices)
            {
                if (vert.Operation != "input" &&
                    vert.Operation != "output" &&
                    vert.Operation != "const")
                    names.Add(vert.WireName);
            }
            return names;
        }

        public List<string> getVerticesByLevel(int level)
        {
            List<string> names = new List<string>();
            foreach (var vert in this.vertices)
            {
                if (vert.Level == level)
                    names.Add(vert.LogicExpression);
            }
            return names;
        }

        private List<int> getConnectedTo(int k)
        {
            List<int> lst = new List<int>();

            for (int i = 0; i < this.vertices.Count; i++)
            {
                if (this.adjacencyMatrix[k][i])
                    lst.Add(i);
            }

            return lst;
        }

        private List<int> getConnectedFrom(int k)
        {
            List<int> lst = new List<int>();

            for (int i = 0; i < this.vertices.Count; i++)
            {
                if (this.adjacencyMatrix[i][k])
                    lst.Add(i);
            }

            return lst;
        }

        public void updateLevels(bool isFull = true, int k = 0)
        {
            if (isFull)
            {
                for (int i = 0; i < this.vertices.Count; i++)
                {
                    if (this.vertices[i].Operation == "input" || this.vertices[i].Operation == "const")
                    {
                        this.vertices[i].Level = 0;
                        updateLevels(false, i);
                    }
                }
            }
            else
            {
                List<int> ver = getConnectedTo(k);
                foreach (int j in ver)
                {
                    this.vertices[j].Level = Math.Max(this.vertices[j].Level, this.vertices[k].Level + 1);
                    updateLevels(isFull, j);
                }
            }
        }
        private List<bool> vertsToValues(List<int> verts)
        {
            List<bool> val = new List<bool>();
            foreach (int i in verts)
                val.Add(this.vertices[i].Value);
            return val;
        }
        private bool calc(List<bool> inputs, string op)
        {
            bool res = false;
            if (inputs.Count == 0)
                return res;
            switch (op)
            {
                case "not":
                    {
                        res = !inputs[0];
                        break;
                    }
                case "and":
                    {
                        res = inputs[0];
                        for (int i = 1; i < inputs.Count; i++)
                            res &= inputs[i];
                        break;
                    }
                case "nand":
                    {
                        res = inputs[0];
                        for (int i = 1; i < inputs.Count; i++)
                            res &= inputs[i];
                        res = !res;
                        break;
                    }
                case "or":
                    {
                        res = inputs[0];
                        for (int i = 1; i < inputs.Count; i++)
                            res |= inputs[i];
                        break;
                    }
                case "nor":
                    {
                        res = inputs[0];
                        for (int i = 1; i < inputs.Count; i++)
                            res |= inputs[i];
                        res = !res;
                        break;
                    }
                case "xor":
                    {
                        res = inputs[0];
                        for (int i = 1; i < inputs.Count; i++)
                            res ^= inputs[i];
                        break;
                    }
                case "xnor":
                    {
                        res = inputs[0];
                        for (int i = 1; i < inputs.Count; i++)
                            res ^= inputs[i];
                        res = !res;
                        break;
                    }
            }
            return res;
        }
        public Dictionary<string, bool> calcGraph(  Dictionary<string, bool> inputValues, 
                                                    bool withErrorValues = false, 
                                                    Dictionary<string, bool> errorValues = null,
                                                    bool withErrorSertting = false,
                                                    Dictionary<string, bool> setError = null)
        {
            this.updateLevels();
            Dictionary<string, bool> dict = new Dictionary<string, bool>();
            if (inputValues.Count != this.getVerticesByType("input").Count)
                return null;
            if (withErrorValues && errorValues != null && errorValues.Count != this.vertices.Count
                                        - this.getVerticesByType("input").Count
                                        - this.getVerticesByType("const").Count
                                        - this.getVerticesByType("output").Count)
                return null;
            if (withErrorSertting && setError != null && setError.Count != this.vertices.Count
                                        - this.getVerticesByType("input").Count
                                        - this.getVerticesByType("const").Count
                                        - this.getVerticesByType("output").Count)
                return null;
            int n = this.MaxLevel;
            for (int level = 0; level < n; level++)
            {
                List<string> verts = this.getVerticesByLevel(level);
                foreach (var v in verts)
                {
                    int i = this.getIndexOfExpression(v);
                    switch (this.vertices[i].Operation)
                    {
                        case "input":
                            {
                                this.vertices[i].Value = inputValues[this.vertices[i].WireName];
                                break;
                            }
                        case "output":
                            {
                                List<int> from = this.getConnectedFrom(i);
                                if (from.Count > 0)
                                    dict.Add(this.vertices[i].WireName, this.vertices[from[0]].Value);
                                break;
                            }
                        case "const":
                            {
                                this.vertices[i].Value = this.vertices[i].WireName.Contains("1'b1") ? true : false;
                                break;
                            }
                        default:
                            {
                                List<int> from = this.getConnectedFrom(i);
                                List<bool> lst = new List<bool>();
                                if (withErrorValues)
                                    lst.Add(errorValues[this.vertices[i].WireName]);
                                lst.AddRange(this.vertsToValues(from));
                                this.vertices[i].Value = this.calc(lst, this.vertices[i].Operation);
                                if (withErrorSertting)
                                    this.vertices[i].Value ^= setError[this.vertices[i].WireName];
                                break;
                            }
                    }
                }
            }
            
            return dict; 
        }
        

        //TODO: отредактировать подсчет уровня каждой вершины

        //TODO: проверка на корректность.

        //TODO: оптимизация графа.
    }    
}
