using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CombinationalCircuitDatabaseGenerator.forms
{
    public partial class CircuitForm : Form
    {
        public CircuitForm(string filename)
        {
            InitializeComponent();
            treeView1.Nodes.Clear();
            string s = File.ReadAllText(filename + ".json");
            JObject obj = JObject.Parse(s);
            TreeNode parent = Json2Tree(obj);
            foreach (TreeNode node in parent.Nodes)
                treeView1.Nodes.Add(node);

            s = File.ReadAllText(filename + ".v");
            richTextBox1.Text = s;
        }

        private TreeNode Json2Tree(JObject obj)
        {
            // Если объекта нет, то возвращаем null.
            if (obj is null)
            {
                return null;
            }
            //Создаем родительскую вершину.
            TreeNode parent = new TreeNode();
            //Итерируемся по obj. Все токены явлются <key, value>
            foreach (var token in obj)
            {
                //change the display Content of the parent
                if (token.Key.ToString() == "hashCode")
                {
                    parent.Text = token.Value.ToString().Substring(0, 10);
                }
                //create the child node
                TreeNode child = new TreeNode();
                child.Text = token.Key.ToString();
                //check if the value is of type obj recall the method
                if (token.Value.Type.ToString() == "Object")
                {
                    //create a new JObject using the the Token.value
                    JObject o = (JObject)token.Value;
                    //recall the method
                    child = Json2Tree(o);
                    child.Text = token.Key.ToString();
                    //add the child to the parentNode
                    parent.Nodes.Add(child);
                }
                //if type is of array
                else if (token.Value.Type.ToString() == "Array")
                {
                    int ix = -1;
                    //  child.Text = token.Key.ToString();
                    //loop though the array
                    foreach (var itm in token.Value)
                    {
                        //check if value is an Array of objects
                        if (itm.Type.ToString() == "Object")
                        {
                            TreeNode objTN = new TreeNode();
                            //child.Text = token.Key.ToString();
                            //call back the method
                            ix++;

                            JObject o = (JObject)itm;
                            objTN = Json2Tree(o);
                            objTN.Text = token.Key.ToString() + "[" + ix + "]";
                            child.Nodes.Add(objTN);
                            //parent.Nodes.Add(child);
                        }
                        //regular array string, int, etc
                        else if (itm.Type.ToString() == "Array")
                        {
                            ix++;
                            TreeNode dataArray = new TreeNode();
                            foreach (var data in itm)
                            {
                                dataArray.Text = token.Key.ToString() + "[" + ix + "]";
                                dataArray.Nodes.Add(data.ToString());
                            }
                            child.Nodes.Add(dataArray);
                        }

                        else
                        {
                            child.Nodes.Add(itm.ToString());
                        }
                    }
                    parent.Nodes.Add(child);
                }
                else
                {
                    child.Text = token.Key.ToString();
                    //change the value into N/A if value == null or an empty string 
                    if (token.Value.ToString() == "")
                        child.Nodes.Add("N/A");
                    else
                        child.Nodes.Add(token.Value.ToString());
                    parent.Nodes.Add(child);

                }
            }
            return parent;

        }

    }
}
