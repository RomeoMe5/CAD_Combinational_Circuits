using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Properties;

namespace Graph
{
    /// <summary>
    /// Вершина графа
    /// </summary>
    public class GraphVertex
    {

        /// <summary>
        /// Описание основных переменных класса.
        /// </summary>
        private string logicExpression;
        public string operation;
        private int level = 0;
        private bool value = false;
        private string wireName = "wr";

        static public int count = 0;
        private Settings settings;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="expr">Итоговое логическое выражения</param>
        /// <param name="operation">Выполняемая логическая операция</param>
        public GraphVertex(string expr, string operation, bool value = false, string wireName = null)
        {
            this.logicExpression = expr;
            this.operation = operation;
            this.settings = Settings.GetInstance();
            this.value = value;
            if (operation == "input" || operation == "output" || operation == "const")
                this.wireName = expr;
            else
                if (wireName == null)
                this.wireName += $"{count++}";
            else
                this.wireName = wireName;
        }

        public int Level
        {
            get
            {
                return this.level;
            }

            set
            {
                this.level = value;
            }
        }

        public bool Value
        {
            get { return this.value; }
            set
            {
                this.value = value;
            }
        }

        public string LogicExpression
        {
            get
            {
                return logicExpression;
            }
        }

        public string Operation
        {
            get
            {
                return operation;
            }
        }

        public string WireName
        {
            get
            {
                return wireName;
            }
        }

    }
}
