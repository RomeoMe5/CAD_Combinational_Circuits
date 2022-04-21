using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graph;
using Properties;

namespace Generators
{
    class SimpleGenerators
    {
        private Settings settings;
        Random random;

        public SimpleGenerators()
        {
            this.settings = Settings.GetInstance();
            this.random = new Random();
        }

        /// Генерация логического выражения в канонической нормальной форме.
        /// tp: 1 - КДНФ, 0 - ККНФ
        public List<string> cnfFromTruthTable(TruthTable table, bool tp = true)
        {
            List<string> fun = new List<string>();
            bool[,] bin = table.convToBinary();

            for (int j = 0; j < table.Output; j++) //цикл для генерации уравнения каждого выхода
            {
                fun.Add($"f{j} = ");
                int mem = 0; //Будут хранится количество единиц для дальнейших циклов
                int tmp = 0; //Будет хранится текущее расположения строки, которой мы рассматриваем

                for (int i = 0; i < table.Size; i++) //цикл подсчёта единиц
                {
                    if (!(table.OutTable[i][j] ^ tp))
                    {
                        mem++;
                    }
                }
                if (mem == 0)
                {
                    fun[j] += $"1'b{(tp ? 0 : 1)}";
                    continue;
                }

                if (mem == table.Size)
                {
                    fun[j] += $"1'b{(tp ? 1 : 0)}";
                    continue;
                }

                for (int i = 0; i < mem; i++) //основной цикл создания логического уравнения
                {
                    fun[j] += '(';
                    while ((table.OutTable[tmp][j] ^ tp) && tmp < table.Size) //находим номер строки, где выход "1"
                    {
                        tmp++;
                    }

                    for (int k = 0; k < table.Input; k++) //Цикл, который переводит таблицу истинности в уравнение
                    {
                        if (bin[tmp, k] ^ tp) //Делаем "Не", если "0"
                        {
                            fun[j] += settings.logicOperations["not"].Item1 + " ";
                        }
                        fun[j] += 'x';
                        fun[j] += k.ToString();
                        if (k != table.Input - 1)
                        {
                            fun[j] += " " + (tp ? settings.logicOperations["and"].Item1 : settings.logicOperations["or"].Item1) + " ";
                        }
                    }

                    fun[j] += ')';

                    if (i != mem - 1)
                    {
                        fun[j] += tp ? settings.logicOperations["or"].Item1 : settings.logicOperations["and"].Item1;
                    }

                    tmp++;
                }
            }

            return fun;
        }

        public OrientedGraph generatorRandLevel(int mxLevel, int maxElements, int inputs, int outputs) //Задаём максимальный возможный уровень (не учитываем входы), элементов на уровне, входы и выходы 
        {
            int maxLevel = random.Next(0, mxLevel);
            int[] elemlevel = new int[maxLevel + 1];
            Random rand = new Random();
            List<string> logoper = settings.logicOperations.Keys.ToList<String>();
            logoper.Remove("input");
            logoper.Remove("output");
            logoper.Remove("const");

            //maxLevel++;
            elemlevel[0] = inputs;
            elemlevel[maxLevel] = outputs;
            for (int i = 1; i < maxLevel; i++)
            {
                elemlevel[i] = rand.Next(2, maxElements + 1);
            }

            int choice; //Индекс случайной операции
            string expression;
            OrientedGraph graph = new OrientedGraph();
            int ch1, ch2;//Индекс случайно выбранного элемента с предыдущего уровня
            for (int i = 0; i < elemlevel[0]; i++)
            {
                expression = "x" + Convert.ToString(i);
                graph.addVertex(expression, "input");
            }
            int currentindex = elemlevel[0];
            int previndex = 0;
            for (int i = 1; i < maxLevel - 1; i++)
            {
                int position = 0;
                for (int j = 0; j < elemlevel[i]; j++)
                {
                    choice = rand.Next(0, logoper.Count);
                    if (logoper[choice] == "not")
                    {
                        ch1 = rand.Next(currentindex);
                        expression = settings.operationsToName[logoper[choice]] + " (" + graph.Vertices[ch1].LogicExpression + ")";
                        if (graph.addVertex(expression, "not"))
                        {
                            graph.addEdge(graph.Vertices[ch1].LogicExpression, graph.Vertices[currentindex + position].LogicExpression);
                        }
                        else
                        {
                            position--;
                            j--;
                        }
                    }
                    else
                    {
                        ch1 = rand.Next(previndex, currentindex);
                        ch2 = rand.Next(previndex, currentindex);


                        expression = "(" + graph.Vertices[ch2].LogicExpression + ") " + settings.operationsToName[logoper[choice]] + " (" + graph.Vertices[ch1].LogicExpression + ")";
                        if (graph.addVertex(expression, logoper[choice]))
                        {
                            graph.addDoubleEdge(graph.Vertices[ch2].LogicExpression, graph.Vertices[ch1].LogicExpression, graph.Vertices[currentindex + position].LogicExpression);
                        }
                        else
                        {
                            position--;
                            j--;
                        }

                    }
                    position++;
                }

                previndex += currentindex - previndex;
                currentindex += position;

            }

            for (int i = 0; i < outputs; i++)
            {
                ch1 = rand.Next(previndex, currentindex);
                expression = "f" + Convert.ToString(i + 1);// + " = " + graph.Vertices[ch1].LogicExpression;
                graph.addVertex(expression, "output");
                graph.addEdge(graph.Vertices[ch1].LogicExpression, graph.Vertices[currentindex + i].LogicExpression);
            }
            return graph;
        }

        public OrientedGraph generatorNumOperation(int input, int output, Dictionary<string, int> logicOper, bool leaveEmptyOut = true)
        {
            int sumOper = 0, maxLvl; //sumOper - общее кол-во операций, maxLvl - наибольший уровень;
            string name; // name - вспомогательная переменная для сохранения имен вершин
            // leaveEmptyOut - флаг задающий что делать, если выходов больше, чем возможных вершин
            Dictionary<string, int> copyLogicOper, levelName;
            //logicOper - "название операции":"кол-во данной операции", levelName -"имя вершины":"её уровень";
            List<string> nameOut, nameInput; //nameOut - список названий выходов; nameInput - список для названий входов
            
            copyLogicOper = new Dictionary<string, int>(logicOper);
            levelName = new Dictionary<string, int>();
            nameOut = new List<string>();
            nameInput = new List<string>();

            OrientedGraph Graph = new OrientedGraph();
            for (int i = 0; i < input; i++)
            {
                name = $@"x{i}";
                Graph.addVertex(name, "input");
                levelName.Add(name, Graph.Vertices[Graph.getIndexOfExpression(name)].Level);
                if (leaveEmptyOut == false)
                {
                    nameInput.Add(name);
                }
            }

            for (int i = 0; i < output; i++)
            {
                name = $@"f{i}";
                Graph.addVertex(name, "output");
                nameOut.Add(name);
            }

            foreach (var kvp in copyLogicOper)
            {
                sumOper += kvp.Value;
            }

            for (int i = 0; i < sumOper; i++)
            {
                copyLogicOper = delNull(copyLogicOper); // удаление операций, чьё кол-во стало = 0 
                string oper = randomGenerator(copyLogicOper.Keys); // рандомный выбор операции 
                copyLogicOper[oper]--;

                if (oper == "not")
                {
                    string ver1 = randomGenerator(levelName.Keys); // рандомный выбор вершины
                    name = $@"{this.settings.logicOperations[oper].Item1}({ver1})";
                    if (Graph.addVertex(name, oper))
                    {                        
                        Graph.addEdge(ver1, name);
                        levelName.Add(name, Graph.Vertices[Graph.getIndexOfExpression(name)].Level);
                    }
                    else
                    {
                        copyLogicOper[oper]++;
                        sumOper++;
                    }
                }
                else
                {
                    string ver1 = randomGenerator(levelName.Keys); // рандомный выбор вершины
                    string ver2 = randomGenerator(levelName.Keys); // рандомный выбор вершины
                    
                    name = $@"({ver1}) {this.settings.logicOperations[oper].Item1} ({ver2})";
                    string reverseName = $@"({ver2}) {this.settings.logicOperations[oper].Item1} ({ver1})";

                    if (Graph.addVertex(name, oper))
                    {                        
                        Graph.addDoubleEdge(ver1, ver2, name);
                        levelName.Add(name, Graph.Vertices[Graph.getIndexOfExpression(name)].Level);
                    }
                    else
                    {
                        copyLogicOper[oper]++;
                        sumOper++;
                    }
                }
            }

            // соединение выходов и вершин с максимальным уровнем
            while ((nameOut.Count > 0) & ((levelName.Count > 0) | (leaveEmptyOut == false)))
            {
                if (levelName.Count > 0)
                {
                    List<string> help = new List<string>();
                    maxLvl = levelName.Values.Max<int>();
                    // добавляем вершины с максимальным уровнем в список для дальнейшего случайного выбора вершины
                    foreach (var item in levelName)
                    {
                        if (item.Value == maxLvl)
                        {
                            help.Add(item.Key);
                        }
                    }
                    while ((help.Count > 0) & (nameOut.Count > 0))
                    {
                        int R1 = random.Next(help.Count);
                        int R2 = random.Next(nameOut.Count);
                        Graph.addEdge(help[R1], nameOut[R2]);
                        levelName.Remove(help[R1]);
                        help.Remove(help[R1]);
                        nameOut.Remove(nameOut[R2]);
                    }
                }
                else
                {
                    int R1 = random.Next(nameInput.Count);
                    int R2 = random.Next(nameOut.Count);
                    Graph.addEdge(nameInput[R1], nameOut[R2]);
                    nameOut.Remove(nameOut[R2]);
                }
            }
            return Graph;
        }

        // удаление операций, чьё кол-во стало = 0 
        private Dictionary<string, int> delNull(Dictionary<string, int> copyLogicOper)
        {
            List<string> delList = new List<string>();
            foreach (var kvp in copyLogicOper)
            {
                if (kvp.Value == 0)
                {
                    delList.Add(kvp.Key);
                }
            }
            foreach (string opName in delList)
            {
                copyLogicOper.Remove(opName);
            }
            delList.Clear();
            return copyLogicOper;
        }

        // рандомный выбор элемента из коллекции
        private string randomGenerator(ICollection O)
        {
            int indx = random.Next(O.Count);
            string[] keyArray = new string[O.Count];
            O.CopyTo(keyArray, 0);
            return keyArray[indx];
        }
    }
}
