using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Properties;
using ConsoleTables;

namespace Structure
{
    class Structure
    {
        private List<List<bool>> adjacencyMatrix;
        private Dictionary<string, bool> error;
        private Settings settings;
        private int input, output;
        private List<string> inputs;
        private List<string> outputs;
        private Dictionary<string, Tuple<string, string[], int>> elements;

        /// <summary>
        /// Конструктор
        /// </summary>
        public Structure()
        {
            adjacencyMatrix = new List<List<bool>>();
            elements = new Dictionary<string, Tuple<string, string[], int>>();
            this.inputs = new List<string>();
            this.outputs = new List<string>();
            this.settings = Settings.GetInstance();
        }

        /// <summary>
        /// Добавление входа
        /// </summary>
        public bool addInput(string inputName)
        { 
            if ((!(inputs.Contains(inputName))) | (!(elements.ContainsKey(inputName))))
            {
                inputs.Add(inputName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Добавление выходов
        /// </summary>
        public bool addOutput(string outputName)
        {
            if ((!(outputs.Contains(outputName))) & ((elements.ContainsKey(outputName))) | (inputs.Contains(outputName)))
            {
                outputs.Add(outputName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Добавление элемента
        /// </summary>
        /// <param logicExpression="elementName">Имя вершины</param>
        public bool addElement(string elementName, string operation, string[] perentsElement, int level = 0)
        {
            if ((operation == "input") | (operation == "output") | (elements.ContainsKey(elementName)))
            {
                return false;
            }

            elements.Add(elementName, new Tuple<string, string[], int>(operation, perentsElement, level));
            return true;
        }

        public List<string> getElementsByType(string type)
        {
            List<string> names = new List<string>();
            if (type == "input")
            {
                return inputs;
            }
            if (type == "output")
            {
                return outputs;
            }
            foreach (var el in elements)
            {
                if (el.Value.Item1 == type)
                {
                    names.Add(el.Key);
                }
            }
            return names;
        }

        public List<string> getElementsByLevel(int level)
        {
            List<string> names = new List<string>();
            if (level == 0)
            {
                return inputs;
            }
            foreach (var el in elements)
            {
                if (el.Value.Item3 == level)
                    names.Add(el.Key);
            }
            return names;
        }

        public Dictionary<string, Tuple<string, string[], int>> Elements
        {
            get
            {
                return elements;
            }
        }
    }
}

