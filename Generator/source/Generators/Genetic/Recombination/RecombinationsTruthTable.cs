using System;
using System.Collections.Generic;
using Generators;
using Genetics.Parent;
using source;
using Genetics.Mutation;

namespace Genetics.Recombination
{
    public partial class Recombinations<Type, ParametersType> where ParametersType : GeneticParameters
                                                    where Type : Chromosome<ParametersType>, new()
    {
        /// <summary>
        /// В данном методе реализуется алгоритм скрещивания, который представляет из себя многоточечный кроссинговер. 
        /// Вначале сохраняются все необходимые параметры, такие как количество входов, выходов, размер переданной популяции и количество опорных точек для кроссинговера.
        /// Далее запускается цикл с генерацией потомков. В первую очередь происходит выбор родителей, далее выбор опорных точек. Опорные точки одинаковы для каждого выхода.
        /// Теперь запускается цикл заполнения таблицы истинности потомка. Вначале берутся элементы первого родителя, когда цикл доходит до опорной точки, элементы начинают
        /// доставаться уже у второго родителя и так далее. В конце добавляем в лист сгенерированных потомков и начинается следующая итерация. 
        /// </summary>
        /// <param name="recombinationParameters"></param> Объект содержит необходимые элементы для генерации, такие как количество опорных точек и способ выбора родителей. 
        /// <param name="population"></param> Объект, который содержит в себе потенциальных родителей.
        /// <returns></returns>Возвращается список сгенерированных потомков.
        private List<ChromosomeType<TruthTable, TruthTableParameters>> RecombinationCrossingEachExitInTurnMany(   RecombinationParameters recombinationParameters,
                                                                                                            List<ChromosomeType<TruthTable, TruthTableParameters>> population)
        {
            Parents<TruthTable, TruthTableParameters> P = new Parents<TruthTable, TruthTableParameters>();
            int input = population[0].chromosome.Input;
            int output = population[0].chromosome.Output;
            int size = population[0].chromosome.Size;
            int ref_point = recombinationParameters.refPoints;

            List<int> referencePoints;
            List<ChromosomeType<TruthTable, TruthTableParameters>> survivors = new List<ChromosomeType<TruthTable, TruthTableParameters>>();

            for (int cr = 0; cr < recombinationParameters.recombinationNumber; cr++)
            {
                List<int> parentsInt = P.ParentsTypes(recombinationParameters.ParentsParameter, population);
                int ch1 = parentsInt[0];
                int ch2 = parentsInt[1];

                referencePoints = AuxiliaryMethods.GetRandomIntList(ref_point, 0, size, false);

                List<bool[][]> parents = new List<bool[][]>();
                parents.Add(population[ch1].chromosome.OutTable);
                parents.Add(population[ch2].chromosome.OutTable);
                bool[][] child = new bool[size][];
                int stage = 0;
                for (int j = 0; j < size; j++)
                {
                    if (stage < referencePoints.Count && j == referencePoints[stage])
                        stage++;
                    child[j] = parents[stage % parents.Count][j];
                }
                TruthTable tt = new TruthTable(input, output, child);
                ChromosomeType<TruthTable, TruthTableParameters> tmp = new ChromosomeType<TruthTable, TruthTableParameters>($@"ind{cr + 1}", tt);
                survivors.Add(tmp);
            }
            return survivors;
        }
        /// <summary>
        /// В данном методе реализуется алгоритм скрещивания, который представляет из себя однородный кроссинговер.
        /// Вначале сохраняются все необходимые параметры: количество входов, выходов и размер популяции. Цикл генерации потомков начинается с выбора родителей.
        /// Далее генерируется "маска". Далее происходит заполнения элементов потомка. Если в маске на текущем элементе стоит "1"(т.е. true), то вставляется элемент первого родителя, 
        /// в противном случае берётся элемент у второго родителя. Потом данный элмент добавляется в сгенерированную популяцию, и цикл переходит к следующей итерации.
        /// </summary>
        /// <param name="recombinationParameters"></param>Объект содержит необходимые параметры: выбор родителя, способ генерации маски.
        /// <param name="population"></param> Объект, который содержит в себе потенциальных родителей.
        /// <returns></returns>Возвращается список сгенерированных потомков.
        private List<ChromosomeType<TruthTable, TruthTableParameters>> RecombinationCrossingUniform(  RecombinationParameters recombinationParameters,
                                                                                                List<ChromosomeType<TruthTable, TruthTableParameters>> population)
        {
            Parents<TruthTable, TruthTableParameters> P = new Parents<TruthTable, TruthTableParameters>();
            int input = population[0].chromosome.Input;
            int output = population[0].chromosome.Output;
            int size = population[0].chromosome.Size;

            List<ChromosomeType<TruthTable, TruthTableParameters>> survivors = new List<ChromosomeType<TruthTable, TruthTableParameters>>();

            for (int cr = 0; cr < recombinationParameters.recombinationNumber; cr++)
            {
                List<int> parentsInt = P.ParentsTypes(recombinationParameters.ParentsParameter, population);
                int ch1 = parentsInt[0];
                int ch2 = parentsInt[1];

                TruthTable mask = new TruthTable(input, output, recombinationParameters.maskProbability); //генерации маски

                List<bool[][]> parents = new List<bool[][]>();
                parents.Add(population[ch1].chromosome.OutTable);
                parents.Add(population[ch2].chromosome.OutTable);
                bool[][] child = new bool[size][];

                for (int j = 0; j < size; j++)
                {
                    child[j] = new bool[output];
                    for (int i = 0; i < output; i++)
                        child[j][i] = parents[mask.array[j][i] ? 1 : 0][j][i];
                }
                TruthTable tt = new TruthTable(input, output, child);
                ChromosomeType<TruthTable, TruthTableParameters> tmp = new ChromosomeType<TruthTable, TruthTableParameters>($@"ind{cr + 1}", tt);
                survivors.Add(tmp);
            }
            return survivors;
        }
        /// <summary>
        /// В данном методе реализуется алгоритм скрещивания, который представляет из себя триадный кроссинговер.
        /// Вначале сохраняются все необходимые параметры: количество входов, выходов и размер популяции. Цикл генерации потомков начинается с выбора родителей.
        /// В качестве маски выбирается третий родитель из популяции, однако у него происходит мутация 10% генов. Далее происходит заполнение потомка.
        /// Если в маске на текущем элементе стоит "1"(т.е. true), то вставляется элемент первого родителя, 
        /// в противном случае берётся элемент у второго родителя. Потом данный элмент добавляется в сгенерированную популяцию, и цикл переходит к следующей итерации.
        /// </summary>
        /// <param name="recombinationParameters"></param>Объект содержит необходимые параметры: выбор родителя, способ генерации маски.
        /// <param name="population"></param> Объект, который содержит в себе потенциальных родителей.
        /// <returns></returns>Возвращается список сгенерированных потомков.
        private List<ChromosomeType<TruthTable, TruthTableParameters>> RecombinationCrossingTriadic(RecombinationParameters recombinationParameters,
                                                                        List<ChromosomeType<TruthTable, TruthTableParameters>> population)
        {
            Parents<TruthTable, TruthTableParameters> P = new Parents<TruthTable, TruthTableParameters>();
            int input = population[0].chromosome.Input;
            int output = population[0].chromosome.Output;
            int size = population[0].chromosome.Size;

            int ch3;
            Random random = new Random();
            for (int cr = 0; cr < recombinationParameters.recombinationNumber; cr++) //Можно подумать ещё сколько потомков создавать
            {
                ch3 = 0;
                List<int> parentsInt = P.ParentsTypes(recombinationParameters.ParentsParameter, population);
                int ch1 = parentsInt[0];
                int ch2 = parentsInt[1];
                while (ch1 == ch3 || ch2 == ch3)
                {
                    ch3 = random.Next(0, population.Count);
                }
                TruthTable mask = new TruthTable(input, output, Mutations<TruthTable, TruthTableParameters>.MutationTable(recombinationParameters.maskProbability, population[ch3].chromosome.OutTable));

                List<bool[][]> parents = new List<bool[][]>();
                parents.Add(population[ch1].chromosome.OutTable);
                parents.Add(population[ch2].chromosome.OutTable);
                bool[][] child = new bool[size][];

                for (int j = 0; j < size; j++)
                {
                    child[j] = new bool[output];
                    for (int i = 0; i < output; i++)
                        child[j][i] = parents[mask.array[j][i] ? 0 : 1][j][i];
                }
                TruthTable tt = new TruthTable(input, output, child);
                ChromosomeType<TruthTable, TruthTableParameters> tmp = new ChromosomeType<TruthTable, TruthTableParameters>($@"ind{cr + 1}", tt);
                population.Add(tmp);
            }
            population.Clear();
            return population;
        }
        /// <summary>
        /// В данном методе реализуется алгоритм скрещивания, который представляет из себя кроссинговер с уменьшением замены. 
        /// Вначале сохраняются все необходимые параметры, такие как количество входов, выходов, размер переданной популяции и количество опорных точек для кроссинговера.
        /// Далее запускается цикл с генерацией потомков. В первую очередь происходит выбор родителей, далее выбор опорных точек. В отличае от многоточечного кроссинговера
        /// опорными точками могут быть те элементы, на которых значения у родителей отличаются. Кроме того, опорные точки для каждого выхода выбираются отдельно.
        /// Вначале берутся элементы первого родителя, когда цикл доходит до опорной точки, элементы начинают доставаться уже у второго родителя и так далее. 
        /// В конце добавляем в лист сгенерированных потомков и начинается следующая итерация. 
        /// </summary>
        /// <param name="recombinationParameters"></param> Объект содержит необходимые элементы для генерации, такие как количество опорных точек и способ выбора родителей. 
        /// <param name="population"></param> Объект, который содержит в себе потенциальных родителей.
        /// <returns></returns>Возвращается список сгенерированных потомков.
        private List<ChromosomeType<TruthTable, TruthTableParameters>> RecombinationCrossingReducedReplacemnt(RecombinationParameters recombinationParameters,
                                                                                List<ChromosomeType<TruthTable, TruthTableParameters>> population)
        {
            Parents<TruthTable, TruthTableParameters> P = new Parents<TruthTable, TruthTableParameters>();
            Random random = new Random();
            int input = population[0].chromosome.Input;
            int output = population[0].chromosome.Output;
            int size = population[0].chromosome.Size;
            int reference_point;

            List<ChromosomeType<TruthTable, TruthTableParameters>> survivors = new List<ChromosomeType<TruthTable, TruthTableParameters>>();

            for (int cr = 0; cr < recombinationParameters.recombinationNumber; cr++)
            {
                List<int> parentsInt = P.ParentsTypes(recombinationParameters.ParentsParameter, population);
                int ch1 = parentsInt[0];
                int ch2 = parentsInt[1];

                List<bool[][]> parents = new List<bool[][]>();
                parents.Add(population[ch1].chromosome.OutTable);
                parents.Add(population[ch2].chromosome.OutTable);
                bool[][] child = new bool[size][];                

                for (int i = 0; i < output; i++)
                {
                    do
                    {
                        reference_point = random.Next(0, size);
                    }
                    while (parents[0][reference_point][i] == parents[1][reference_point][i]);

                    for (int j = 0; j < size; j++) 
                    {
                        child[j][i] = parents[j < reference_point ? 0 : 1][j][i];
                    }
                }
                TruthTable tt = new TruthTable(input, output, child);
                ChromosomeType<TruthTable, TruthTableParameters> tmp = new ChromosomeType<TruthTable, TruthTableParameters>($@"ind{cr + 1}", tt);
                survivors.Add(tmp);
            }
            return survivors;
        }
        /// <summary>
        /// В данном методе реализуется алгоритм скрещивания, который представляет из себя перестановочный кроссинговер.  
        /// Вначале сохраняются все необходимые параметры, такие как количество входов, выходов и размер переданной популяции.
        /// Далее запускается цикл с генерацией потомков. В первую очередь происходит выбор родителей, далее выбор опорной точки.
        /// Потом происходит обмен генов у родителей с вероятностью 50%. После этого происходит кросинговер с одной опорной точкой.
        /// В конце добавляем в лист сгенерированных потомков и начинается следующая итерация.
        /// </summary>
        /// <param name="recombinationParameters"></param> Объект содержит необходимые элемент для генерации, такой как способ выбора родителей. 
        /// <param name="population"></param> Объект, который содержит в себе потенциальных родителей.
        /// <returns></returns>Возвращается список сгенерированных потомков.
        private List<ChromosomeType<TruthTable, TruthTableParameters>> RecombinationCrossingShuffling(RecombinationParameters recombinationParameters,
                                                                        List<ChromosomeType<TruthTable, TruthTableParameters>> population)
        {
            Parents<TruthTable, TruthTableParameters> P = new Parents<TruthTable, TruthTableParameters>();
            TruthTable ttTest = population[0].chromosome as TruthTable;
            int input = ttTest.Input;
            int output = ttTest.Output;
            int size = ttTest.Size;

            List<ChromosomeType<TruthTable, TruthTableParameters>> survivors = new List<ChromosomeType<TruthTable, TruthTableParameters>>();

            bool temp;
            Random random = new Random();
            for (int cr = 0; cr < recombinationParameters.recombinationNumber; cr++)//Можно подумать ещё сколько потомков создавать
            {
                List<int> parentsInt = P.ParentsTypes(recombinationParameters.ParentsParameter, population);
                int ch1 = parentsInt[0];
                int ch2 = parentsInt[1];

                int reference_point = random.Next(0, size);
                List<bool[][]> parents = new List<bool[][]>();
                parents.Add(population[ch1].chromosome.OutTable);
                parents.Add(population[ch2].chromosome.OutTable);
                bool[][] child = new bool[size][];

                for (int j = 0; j < size; j++)
                {
                    for (int i = 0; i < output; i++)
                    {
                        if (random.Next(0, 2) == 1)
                        {
                            temp = parents[0][j][i];
                            parents[0][j][i] = parents[1][j][i];
                            parents[1][j][i] = temp;
                        }
                    }
                }

                for (int j = 0; j < size; j++)
                {
                    child[j] = new bool[output];
                    for (int i = 0; i < output; i++)
                        child[j][i] = parents[j < reference_point ? 0 : 1][j][i];
                }
                TruthTable tt = new TruthTable(input, output, child);
                ChromosomeType<TruthTable, TruthTableParameters> tmp = new ChromosomeType<TruthTable, TruthTableParameters>($@"ind{cr + 1}", tt);
                survivors.Add(tmp);
            }
            return survivors;
        }
    }

}
