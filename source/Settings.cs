using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Properties
{
    /*enum generationTypes
    {
        none,
        truthTableToGraphWithoutOptimization,
        truthTableToGraphWithOptimization
    }*/

    [Serializable]
    class Settings
    {

        public static Dictionary<string, string> generationMethodsToPrefix = new Dictionary<string, string>{
            {"FromRandomTruthTable", "ftt"},
            {"RandLevel", "rl"},
            {"NumOperation", "nop"},
            {"Genetic", "gen"}
        };

        /// <summary>
        /// Название файла с базой данных.
        /// </summary>
        public static string csvdataset = "dataset.csv";
        /// <summary>
        /// Путь для сохранения настроек.
        /// </summary>
        private static string fileName = "settings.dat";
        /// <summary>
        /// Путь к папке с датасетом
        /// </summary>
        public static string datasetPath = Directory.GetCurrentDirectory() + "\\database";
        /// <summary>
        /// Экземпляр настроек.
        /// </summary>
        private static Settings instance;
        /// <summary>
        /// Путь к программе рассчета параметров схемы.
        /// </summary>
        public static string pathNadezhda = "data\\Nadezhda";
        /// <summary>
        /// Словарь с основными путями к файлами программы.
        /// </summary>
        public static Dictionary<string, string> nadezhda = new Dictionary<string, string>{ 
            {"python", ".\\src\\python37-32\\python.exe"},
            {"resynthesis", "Nadezhda\\Scripts\\resynthesis_local_rewriting.pyc"},
            {"reliability", "Nadezhda\\Scripts\\check_reliability.pyc"},
            {"liberty", "Nadezhda\\Test\\Nangate.lib"}
        };

        public static int numThreads { get; private set; }
        
        /// <summary>
        /// Перечисляются используемые логические операции. Формат: {операция: (символ, уровень выполнения)}.
        /// </summary>
        public Dictionary<string, Tuple<string, int>> logicOperations;
        /// <summary>
        /// Соотносят операции и уровень выполнения. Формат: {операция: уровень выполнения}.
        /// </summary>
        public Dictionary<int, List<string>> operationsToHierarchy;
        /// <summary>
        /// Соотносят символ операции и ее обозначение. Формат: {символ: операция}.
        /// </summary>
        public Dictionary<string, string> operationsToName;

        public int maxInputs = 50;
        public int maxOutputs = 50;

        enum generationTypes
        {
            none,
            truthTableToGraphWithoutOptimization,
            truthTableToGraphWithOptimization
        }


        /// <summary>
        /// Конструктор для инициализации настроек.
        /// </summary>
        private Settings()
        {
            numThreads = 4;

            logicOperations = new Dictionary<string, Tuple<string, int>>{
                { "input",  new Tuple<string, int>("",      10)},
                { "output", new Tuple<string, int>("=",     0)},
                { "const",  new Tuple<string, int>("1'b",     9)},
                { "and",    new Tuple<string, int>("and",   4)},
                { "nand",   new Tuple<string, int>("nand",  3)},
                { "or",     new Tuple<string, int>("or",    2)},
                { "nor",    new Tuple<string, int>("nor",   1)},
                { "not",    new Tuple<string, int>("not",   7)},
                { "buf",    new Tuple<string, int>("buf",   8)},
                { "xor",    new Tuple<string, int>("xor",   6)},
                { "xnor",   new Tuple<string, int>("xnor",  5)}
            };

            operationsToHierarchy = new Dictionary<int, List<string>>();
            operationsToName = new Dictionary<string, string>();

            foreach (var item in logicOperations)
            {
                int i = item.Value.Item2;
                if (!operationsToHierarchy.ContainsKey(i))
                    operationsToHierarchy.Add(i, new List<string>());
                operationsToHierarchy[i].Add(item.Value.Item1);
                
            }

            foreach (var item in logicOperations)
            {
                operationsToName.Add(item.Value.Item1, item.Key);
            }
        }

        /// <summary>
        /// Метод, возвращающий экземпляр настроек.
        /// </summary>
        /// <returns>Возвращает экземпляр настроек.</returns>
        public static Settings GetInstance()
        {
            if (instance == null)
                instance = Settings.LoadSettings();
            return instance;
        }


        /// <summary>
        /// Метод загрузки настроек из файла. Происходит поптыка считывания настроек из файла. При неудаче создается базовый экземпляр настроек.
        /// </summary>
        /// <returns>Возвращает класс с настройками.</returns>
        public static Settings LoadSettings()
        {
            Settings settings = null;
            BinaryFormatter bf = new BinaryFormatter();

            if (File.Exists(fileName))
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                {
                    try
                    {
                        settings = (Settings)bf.Deserialize(fs);
                        fs.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"Exception Handler in LoadSettings: {ex}");
                        settings = new Settings();
                    }
                }
            }
            else
            {
                settings = new Settings();
            }

            return settings;
        }

        /// <summary>
        /// Метод сохранения настроек в файл.
        /// </summary>
        public void Save()
        {            
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                bf.Serialize(fs, this);
                fs.Close();
            }
        }

    }
}
