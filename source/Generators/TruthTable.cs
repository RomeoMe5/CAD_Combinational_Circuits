using System;
using ConsoleTables;
using Properties;
using Genetics;

namespace Generators
{
    class TruthTable : Chromosome<TruthTableParameters>
    {
        private int input, output, size;
        public bool[][] array { get; set; }
        private Settings settings;

        public TruthTable()
        {
            this.settings = Settings.GetInstance();
        }
        public TruthTable(Chromosome<TruthTableParameters> chr)
        {
            this.settings = Settings.GetInstance();
        }

        /// Конструктор класса.
        public TruthTable(int input, int output, bool[][] array = null)
        {
            this.settings = Settings.GetInstance();
            this.input = input;
            this.output = output;
            size = (int)Math.Pow(2, this.input);
            if (array == null || array.Length != size || array[0].Length != output)
                this.GenerateRandom(new TruthTableParameters(input, output));
            else
                this.array = array;
        }

        public TruthTable(TruthTable tt, bool[][] array = null)
        {
            this.settings = Settings.GetInstance();
            this.input = tt.input;
            this.output = tt.output;
            size = (int)Math.Pow(2, this.input);
            if (array == null || array.Length != size || array[0].Length != output)
                this.GenerateRandom(new TruthTableParameters(input, output));
            else
                this.array = array;
        }
        public TruthTable(int input, int output, double p = 0.5)
        {
            this.settings = Settings.GetInstance();
            this.input = input;
            this.output = output;
            size = (int)Math.Pow(2, this.input);
            this.generateTable(p);
        }

        /// Взаимодействие с переменной input.
        public int Input
        {
            get
            {
                return this.input;
            }
        }

        /// Взаимодействие с переменной output.
        public int Output
        {
            get
            {
                return this.output;
            }
        }

        /// Взаимодействие с переменной size.
        public int Size
        {
            get
            {
                return (int)Math.Pow(2, this.input);
            }
        }

        /// Взаимодействие с переменной array.
        public bool[][] OutTable
        {
            get
            {
                return this.array;
            }
        }

        /// Генерация случайных значений таблицы истинности.
        public void generateTable(double p = 0)
        {
            if (p == 0)
            {
                this.array = new bool[this.size][];
                Random rnd = new Random();
                for (int i = 0; i < this.size; i++)
                {
                    this.array[i] = new bool[this.output];
                    for (int j = 0; j < this.output; j++)
                    {
                        this.array[i][j] = rnd.Next(0, 2) == 1;
                    }
                }
                return;
            }
            if (p > 0 && p <= 1)
            {
                this.array = new bool[this.size][];
                Random rnd = new Random();
                for (int i = 0; i < this.size; i++)
                {
                    this.array[i] = new bool[this.output];
                    for (int j = 0; j < this.output; j++)
                    {
                        this.array[i][j] = rnd.NextDouble() < p;
                    }
                }
                return;
            }
        }


        public void GenerateRandom(TruthTableParameters gp = null)
        {
            Random rnd = new Random();
            if (gp == null)
            {
                gp = new TruthTableParameters();
                gp.inputs = rnd.Next(0, settings.maxInputs);
                gp.outputs = rnd.Next(0, settings.maxOutputs);
            }
            this.input = gp.inputs;
            this.output = gp.outputs;

            this.array = new bool[gp.size][];            
            for (int i = 0; i < gp.size; i++)
            {
                this.array[i] = new bool[gp.outputs];
                for (int j = 0; j < gp.outputs; j++)
                {
                    this.array[i][j] = rnd.Next(0, 2) == 1;
                }
            }
        }

        /// Формирование части таблицы истинности с входными сигналами.
        /// Перевод чисел в бинарный массив.
        public bool[,] convToBinary()
        {
            bool[,] bin = new bool[Size, input];

            for (int i = 0; i < this.Size; i++)
            {
                for (int j = this.input - 1, tmp = i; j >= 0; j--)
                {
                    bin[i, j] = (tmp % 2) == 1;
                    tmp = tmp / 2;
                }
            }
            return bin;
        }

        /// Вывод сгенерированной таблицы истинности в консоль.
        public void printTable()
        {
            var consTable = new ConsoleTable();
            string[] cols = new string[input + output];
            string[][] row = new string[size][];

            for (int i = 0; i < row.Length; i++)
                row[i] = new string[input + output];

            for (int i = 0; i < input; i++)
                cols[i] = String.Format($"x{i,-5}");
            for (int i = 0; i < output; i++)
                cols[i + input] = String.Format($"f{i,-5}");
            consTable.AddColumn(cols);

            // Вывод значений таблицы истинности.
            bool[,] bin = this.convToBinary();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < input; j++)
                {
                    row[i][j] = String.Format($"{Convert.ToInt32(bin[i, j])}");
                }
                for (int j = 0; j < output; j++)
                {
                    row[i][input + j] = String.Format($"{Convert.ToInt32(array[i][j])}");
                }
                consTable.AddRow(row[i]);
            }
            consTable.Write(Format.Alternative);
        }
    }
}
