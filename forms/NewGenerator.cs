using Properties;

using DataBaseGenerators;

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using source;
using Genetics;

namespace CombinationalCircuitDatabaseGenerator.forms
{
    public partial class NewGenerator : Form
    {
        private GenerationTypes gt;
        private int tab;
        private DataBaseGeneratorParameters dbgp;
        public DataBaseGeneratorParameters dataBaseGeneratorParameters { get { return dbgp; } }
        private Dictionary<string, int> logicOper;
        private Settings settings;

        public NewGenerator(GenerationTypes gt)
        {
            InitializeComponent();
            this.settings = Settings.GetInstance();
            this.gt = gt;
            label1.Text += gt.ToString();
            this.tab = (int)gt;
            tbcAllPanels.SelectedIndex = tab;
            comboBox1.Items.Clear();

            logicOper = new Dictionary<string, int>();
            foreach (string s in settings.logicOperations.Keys)
                if (s != "input" & s != "output" & s != "const")
                    logicOper.Add(s, 0);
            comboBox1.Items.AddRange(logicOper.Keys.ToArray());
            comboBox1.SelectedIndex = 0;
            numericUpDown1.Value = 0;
            numericUpDown5.Value = 1;

            comboBox2.Items.Clear();
            comboBox2.Items.AddRange(Enum.GetValues(typeof(SelectionTypes))
                .Cast<SelectionTypes>()
                .Select(v => v.ToString())
                .ToArray());
            comboBox2.SelectedIndex = 0;

            comboBox3.Items.Clear();
            comboBox3.Items.AddRange(Enum.GetValues(typeof(MutationTypes))
                .Cast<MutationTypes>()
                .Select(v => v.ToString())
                .ToArray());
            comboBox3.SelectedIndex = 0;

            comboBox4.Items.Clear();
            comboBox4.Items.AddRange(Enum.GetValues(typeof(ParentsTypes))
                .Cast<ParentsTypes>()
                .Select(v => v.ToString())
                .ToArray());
            comboBox4.SelectedIndex = 0;

            comboBox5.Items.Clear();
            comboBox5.Items.AddRange(Enum.GetValues(typeof(RecombinationTypes))
                .Cast<RecombinationTypes>()
                .Select(v => v.ToString())
                .ToArray());
            comboBox5.SelectedIndex = 0;

            comboBox6.Items.Clear();
            comboBox6.Items.AddRange(Enum.GetValues(typeof(genotypeParametersTypes))
                .Cast<genotypeParametersTypes>()
                .Select(v => v.ToString())
                .ToArray());
            comboBox6.SelectedIndex = 0;


        }

        private void nmrcMinInputs_ValueChanged(object sender, EventArgs e)
        {
            if (nmrcMinInputs.Value > nmrcMaxInputs.Value)
                nmrcMaxInputs.Value = nmrcMinInputs.Value;
        }

        private void nmrcMinOutputs_ValueChanged(object sender, EventArgs e)
        {
            if (nmrcMinOutputs.Value > nmrcMaxOutputs.Value)
                nmrcMaxOutputs.Value = nmrcMinOutputs.Value;
        }

        private void tbcAllPanels_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbcAllPanels.SelectedIndex = tab;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            dbgp = new DataBaseGeneratorParameters();
            dbgp.minInputs = (int)nmrcMinInputs.Value;
            dbgp.maxInputs = (int)nmrcMaxInputs.Value;
            dbgp.minOutputs = (int)nmrcMinOutputs.Value;
            dbgp.maxOutputs = (int)nmrcMaxOutputs.Value;
            dbgp.eachIteration = (int)nmrcEachIteration.Value;
            dbgp.generationTypes = gt;

            if (gt == GenerationTypes.CNFFromTruthTable)
            {
                dbgp.generationParameters.cnfFromTruthTableParameters.generateLimitation = checkBox1.Checked;
                dbgp.generationParameters.cnfFromTruthTableParameters.CNFF = checkBox3.Checked;
                dbgp.generationParameters.cnfFromTruthTableParameters.CNFT = checkBox4.Checked;
            }

            if (gt == GenerationTypes.RandLevel)
            {
                dbgp.generationParameters.generatorRandLevelParameters.maxElements = (int)nmrcMaxElements.Value;
                dbgp.generationParameters.generatorRandLevelParameters.maxLevel = (int)nmrcMaxLevel.Value;
            }

            if (gt == GenerationTypes.NumOperation)
            {
                dbgp.generationParameters.generatorNumOperationParameters.leaveEmptyOut = checkBox2.Checked;
                dbgp.generationParameters.generatorNumOperationParameters.logicOper = logicOper;
            }

            if (gt == GenerationTypes.Genetic)
            {
                dbgp.generationParameters.geneticParameters.populationSize = (int)numericUpDown2.Value;
                dbgp.generationParameters.geneticParameters.numOfCycles = (int)numericUpDown3.Value;
                dbgp.generationParameters.geneticParameters.keyEndProcessIndex = (double)numericUpDown4.Value;

                dbgp.generationParameters.geneticParameters.RecombinationParameter.RecombinationType = AuxiliaryMethods.ToEnum<RecombinationTypes>(comboBox5.SelectedItem.ToString());
                dbgp.generationParameters.geneticParameters.RecombinationParameter.ParentsParameter.ParentsType = AuxiliaryMethods.ToEnum<ParentsTypes>(comboBox4.SelectedItem.ToString());
                dbgp.generationParameters.geneticParameters.RecombinationParameter.ParentsParameter.TournematnNumber = (int)numericUpDown9.Value;
                dbgp.generationParameters.geneticParameters.RecombinationParameter.refPoints = (int)numericUpDown11.Value;
                dbgp.generationParameters.geneticParameters.RecombinationParameter.maskProbability = (double)numericUpDown12.Value;
                dbgp.generationParameters.geneticParameters.RecombinationParameter.recombinationNumber = (int)numericUpDown10.Value;

                dbgp.generationParameters.geneticParameters.MutationParameter.MutationType = AuxiliaryMethods.ToEnum<MutationTypes>(comboBox3.SelectedItem.ToString());
                dbgp.generationParameters.geneticParameters.MutationParameter.probabilityGen = (double)numericUpDown6.Value;
                dbgp.generationParameters.geneticParameters.MutationParameter.exchangeType = (int)numericUpDown7.Value;
                dbgp.generationParameters.geneticParameters.MutationParameter.probabilityTruthTable = (double)numericUpDown8.Value;


                dbgp.generationParameters.geneticParameters.SelectionParameter.SelectionType = AuxiliaryMethods.ToEnum<SelectionTypes>(comboBox2.SelectedItem.ToString());
                dbgp.generationParameters.geneticParameters.SelectionParameter.numOfSurvivors = (int)numericUpDown5.Value;
            }

            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            logicOper[comboBox1.SelectedItem.ToString()] = (int)numericUpDown1.Value;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            numericUpDown1.Value = logicOper[comboBox1.SelectedItem.ToString()];
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown2.Value < numericUpDown5.Value)
                numericUpDown5.Value = numericUpDown2.Value;
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown2.Value < numericUpDown5.Value)
                numericUpDown5.Value--;
        }
    }
}
