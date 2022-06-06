using System;
using System.Collections.Generic;
using Graph;
using Properties;

namespace Generators
{

    /// <summary>
    /// Парсинг логического выражения.
    /// </summary>
    class Parser
    {

        /// <summary>
        /// Описание основных переменных класса.
        /// </summary>
        private List<string> logExpressions;
        
        private OrientedGraph graph;
        private Settings settings;

        /// <summary>
        /// Конструктор парсера.
        /// </summary>
        /// <param name="logExpression">Логическое выражения для парсинга.</param>
        public Parser(string logExpression)
        {
            GraphVertex.count = 0;
            this.logExpressions = new List<string>();
            this.logExpressions.Add(logExpression.Replace("  ", " "));
            this.graph = new OrientedGraph();
            this.settings = Settings.GetInstance();
        }

        /// <summary>
        /// Конструктор парсера.
        /// </summary>
        /// <param name="logExpression">Список логических выражений для парсинга.</param>
        public Parser(List<string> logExpressions)
        {
            GraphVertex.count = 0;
            this.logExpressions = new List<string>();
            for (int i = 0; i < logExpressions.Count; i++)
                this.logExpressions.Add(logExpressions[i].Replace(" ", string.Empty));
            this.graph = new OrientedGraph();
            this.settings = Settings.GetInstance();
        }

        public OrientedGraph Graph
        {
            get { return this.graph; }
        }

        //TODO: при переводе из таблицы истинности в выражение иногда вместо входа идет константа

        /// <summary>
        /// Метод для создания структуры скобок в выражении.
        /// </summary>
        /// <param name="expr">Логическое выражения.</param>
        /// <returns>Формат: (корректность скобок, список из пар индексов открытия и закрытия скобок).</returns>
        private Tuple<bool, List<Tuple<int, int>>> CreateBracketsList(string expr)
        {
            List<Tuple<int, int>> brackets = new List<Tuple<int, int>>();
            for (int i = 0; i < expr.Length; i++)
            {
                if (expr[i] == '(')
                {
                    brackets.Add(new Tuple<int, int>(i, -1));
                }
                if (expr[i] == ')')
                {
                    int j = brackets.Count - 1;
                    while (j >=0 && brackets[j].Item2 != -1) j--;
                    if (j == -1)
                        return new Tuple<bool, List<Tuple<int, int>>>(false, null);
                    brackets[j] = new Tuple<int, int>(brackets[j].Item1, i);
                }
            }

            int u = brackets.Count - 1;
            while (u >= 0 && brackets[u].Item2 != -1)
                u--;
            if (u == -1)
                return new Tuple<bool, List<Tuple<int, int>>>(true, brackets);
            return new Tuple<bool, List<Tuple<int, int>>>(false, null);
        }

        /// <summary>
        /// Метод для проверки нахождения логической операции внутри скобок.
        /// </summary>
        /// <param name="brackets">Список пар из индексов открытия и закрытия скобок.</param>
        /// <param name="position">Позиция символа для проверки.</param>
        /// <returns>Результат нахождения символа внутри скобок.</returns>
        private bool InBrackets(List<Tuple<int, int>> brackets, int position)
        {
            int j = brackets.Count - 1;
            while (j >= 0 && !(position > brackets[j].Item1 && position < brackets[j].Item2))
                j--;
            if (j == -1)
                return false;
            return true;
        }

        private string DeleteExtraSpaces(string s)
        {
            while (s[0] == ' ')
                s = s.Remove(0, 1);

            while (s[s.Length - 1] == ' ')
                s = s.Remove(s.Length - 1, 1);

            return s;
        }

        /// <summary>
        /// Разбиение выражения на операцию и ее операнды
        /// </summary>
        /// <param name="expr">Исходное логическое выражение</param>
        /// <returns>Спсиок из логической операции и ее операндов</returns>
        public Tuple<int, List<string>> SplitLogicExpression(string expr)
        {
            bool f = true;
            int l = 0;

            while (f && l <= this.settings.logicOperations["input"].Item2)
            {
                List<string> operations = this.settings.operationsToHierarchy[l];
                if (expr == "1'b1")
                    expr = expr;
                foreach (string op in operations)
                {
                    int index = expr.IndexOf(op);

                    while (index != -1)
                    {
                        Tuple<bool, List<Tuple<int, int>>> brackets = this.CreateBracketsList(expr);
                        if (!InBrackets(brackets.Item2, index))
                        {
                            List<string> lst = new List<string>();
                            string newOp = this.settings.operationsToName[op];
                            lst.Add(this.DeleteExtraSpaces(newOp));
                            switch (newOp)
                            {
                                case "not":
                                    lst.Add(this.DeleteExtraSpaces(expr.Substring(index + op.Length)));
                                    break;
                                case "buf":
                                    lst.Add(this.DeleteExtraSpaces(expr.Substring(index + op.Length)));
                                    break;
                                case "input":
                                    lst.Add(this.DeleteExtraSpaces(expr));
                                    break;
                                case "const":
                                    lst.Add(this.DeleteExtraSpaces(expr));
                                    break;
                                default:
                                    lst.Add(this.DeleteExtraSpaces(expr.Substring(0, index)));
                                    lst.Add(this.DeleteExtraSpaces(expr.Substring(index + op.Length)));
                                    break;
                            }
                            return new Tuple<int, List<string>>(index, lst);
                        }
                        index = expr.IndexOf(op, index + 1);
                    }
                }
                l++;
            }
            return null;
        }

        /// <summary>
        /// Метод запуска парсинга выражения и перевода его в граф.
        /// </summary>
        /// <returns>Возвращает созданный экземпляр графа. При ошибке в создании графа возвращает null.</returns>
        public bool ParseAll()
        {
            this.graph = new OrientedGraph();
            for (int i = 0; i < this.logExpressions.Count; i++)
                if (this.CreateBracketsList(this.logExpressions[i]).Item1)
                    if (!Parse(this.logExpressions[i]))
                        return false;
            
            return true;
        }

        /// <summary>
        /// Функция рекурсивного парсинга логического выражения
        /// </summary>
        /// <param name="expression">Логическое выражение</param>
        /// <returns>Возвращает созданный экземпляр графа. При ошибке в создании графа возвращает null.</returns>
        public bool Parse(string expression)
        {            
            Tuple<int, List<string>> t = this.SplitLogicExpression(expression);
            if (t == null)
                return false;
            if (t.Item2[0] == "output")
            {
                List<Tuple<int, int>> bl = this.CreateBracketsList(t.Item2[2]).Item2;
                foreach (Tuple<int, int> tl in bl)
                    if (tl.Item1 == 0 && tl.Item2 == t.Item2[2].Length - 1)
                        t.Item2[2] = t.Item2[2].Substring(1, t.Item2[2].Length - 2);

                Tuple<int, List<string>> tt = this.SplitLogicExpression(t.Item2[2]);
                if (tt == null)
                    return false;             

                this.graph.addVertex(t.Item2[1], t.Item2[0]);
                this.graph.addVertex(t.Item2[2], tt.Item2[0]);
                this.graph.addEdge(t.Item2[2], t.Item2[1]);
                if (tt.Item2[0] != "input" && tt.Item2[0] != "const")
                    this.Parse(t.Item2[2]);
            }
            else
            {
                this.graph.addVertex(expression, t.Item2[0]);
                for (int i = 1; i < t.Item2.Count; i++)
                {
                    string part = t.Item2[i];

                    List<Tuple<int, int>> bl = this.CreateBracketsList(part).Item2;
                    foreach (Tuple<int, int> tl in bl)
                        if (tl.Item1 == 0 && tl.Item2 == part.Length - 1)
                            part = part.Substring(1, part.Length - 2);

                    Tuple<int, List<string>> tt = this.SplitLogicExpression(part);
                    if (tt == null)
                        return false;
                    this.graph.addVertex(part, tt.Item2[0]);
                    this.graph.addEdge(part, expression);
                    if (tt.Item2[0] != "input" && tt.Item2[0] != "const")
                        this.Parse(part);
                }
            }            
            return true;
        }
    }
}
