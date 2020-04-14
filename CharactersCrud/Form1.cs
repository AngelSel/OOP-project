using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using CharactersCrud.Elements;

namespace CharactersCrud
{
    public partial class Form1 : Form
    {
        public static ISerializations[] serializers = { new BinarySerialization(), new JSONSerialization(), new MySerialization() };
        public Form1()
        {
            InitializeComponent();
            InitCombo();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            saveFileDialog1.Filter = "Binary(bin)|*.bin|Json(json)|*.json|Text(txt)|*.txt";
            openFileDialog1.Filter = "Binary(bin)|*.bin|Json(json)|*.json|Text(txt)|*.txt";
        }

        public List<Character> objList = new List<Character>();
        public List<Armour> armourList = new List<Armour>();
        public List<object> arList = new List<object>();

        internal List<Armour> ArmourList { get => armourList; set => armourList = value; }


        //инициализация ComboBox
        private void InitCombo()
        {
            Assembly asm = Assembly.Load("CharactersCrud");
            foreach (Type type in asm.GetTypes())
            {
                foreach (var el in Enum.GetValues(typeof(Character.TCategorys)))
                {
                    if (type.Name.Equals(el.ToString()))
                        comboBox1.Items.Add(type);
                }
            }

            this.comboBox1.SelectedIndex = 0;
        }


        //добавление новой строки в лист
        public static void AddLine(ListView some)
        {
            ListViewItem lvi = new ListViewItem();
            ListViewItem.ListViewSubItem lvSub1 = new ListViewItem.ListViewSubItem();
            ListViewItem.ListViewSubItem lvSub2 = new ListViewItem.ListViewSubItem();
            ListViewItem.ListViewSubItem lvSub3 = new ListViewItem.ListViewSubItem();
            lvi.SubItems.Add(lvSub1);
            lvi.SubItems.Add(lvSub2);
            lvi.SubItems.Add(lvSub3);
            some.Items.Add(lvi);
        }

        //обновление листа
        private void Update(List<Character> objList)
        {
            listView1.Items.Clear();
            for (int j = 0; j < objList.Count; j++)
            {
                AddLine(listView1);
                listView1.Items[j].SubItems[0].Text = objList[j].Name;
                listView1.Items[j].SubItems[1].Text = objList[j].RaceType.ToString();
                listView1.Items[j].SubItems[2].Text = objList[j].GetType().Name.ToString();
                listView1.Items[j].SubItems[3].Text = objList[j].Level.ToString();
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Type type = this.comboBox1.SelectedItem as Type;
            CreateObject(type);

        }

            
        //полное создание объекта
        private void CreateObject(Type type)
        {
            ConstructorInfo[] csInfo = type.GetConstructors();
            ParameterInfo[] prmInfo = csInfo[0].GetParameters();

            int i = 0;
            Form form = new Form
            {
                Width = 300,
                Height = (prmInfo.Length + 3) * 30,
                FormBorderStyle = FormBorderStyle.FixedSingle
            };

            int deltaleft = 100;
            int compDelta = 10;
            int tbWidth = 130;
            int tbHeight = 30;

            int j = 0;
            for (i = 0; i < prmInfo.Length; i++)
            {
                if (prmInfo[i].ParameterType.IsEnum)
                    j++;
            }

            TextBox[] tb = new TextBox[(prmInfo.Length)-j];
            Label[] lb = new Label[prmInfo.Length];
            ComboBox[] cm = new ComboBox[j];

            int textCounter = 0;
            int boxCOunter = 0;

            //генерация формы создания
            for (i = 0; i < prmInfo.Length; i++)
            {

                if (prmInfo[i].ParameterType.IsEnum)
                {
                    cm[boxCOunter] = new ComboBox
                    {
                        Left = deltaleft,
                        Top = compDelta,
                        Width = tbWidth,
                        Height = tbHeight
                    };
                    lb[i] = new Label
                    {
                        Top = compDelta + 5,
                        Text = prmInfo[i].Name
                    };
                    form.Controls.Add(cm[boxCOunter]);
                    form.Controls.Add(lb[i]);

                    foreach (var prm in Enum.GetValues(prmInfo[i].ParameterType))
                    {
                        cm[boxCOunter].Items.Add(prm);
                    }

                    cm[boxCOunter].SelectedIndex = 0;
                    boxCOunter++;
                    compDelta += tbHeight;
                }
                else if(prmInfo[i].ParameterType.IsClass && !(prmInfo[i].ParameterType.Equals(typeof(string))))
                {
                    lb[i] = new Label
                    {
                        Top = compDelta + 5,
                        Text = prmInfo[i].Name
                    };
                    form.Controls.Add(cm[boxCOunter]);
                    form.Controls.Add(lb[i]);

                    Button btnObject = new Button
                    {
                        Text = "Armour",
                        Left = deltaleft,
                        Top = compDelta,
                        Width = tbWidth,
                        Height = 20
                    };

                    form.Controls.Add(btnObject);
                    CreateObject(prmInfo[i].ParameterType);
                    compDelta += tbHeight;
                }
                
                else
                {
                    tb[textCounter] = new TextBox
                    {
                        Left = deltaleft,
                        Top = compDelta,
                        Width = tbWidth,
                        Height = tbHeight
                    };

                    lb[i] = new Label
                    {
                        Top = compDelta + 5,
                        Text = prmInfo[i].Name
                    };

                    form.Controls.Add(tb[textCounter]);
                    form.Controls.Add(lb[i]);
                    textCounter++;
                    compDelta += tbHeight;
                }
            }

            Button btn = new Button
            {
                Text = "Ok",
                Width = 100,
                Height = 30
            };
            btn.Left = form.Width / 2 - btn.Width / 2;
            btn.Top = form.Height - (int)(btn.Height * 2.5);
            form.Controls.Add(btn);
            btn.Click += new System.EventHandler(CreateModalClick);
            Object obj = null;
            form.Show(this);


            //сохранение объекта
            void CreateModalClick(object sender, EventArgs e)
            {
                textCounter = 0;
                boxCOunter = 0; 
                bool valid = true;
                Object[] prms = new Object[prmInfo.Length];

                for (i = 0; i < prmInfo.Length; i++)
                {
                    if (!valid)
                    {
                        break;
                    }

                    if (prmInfo[i].ParameterType.IsEnum)
                    {
                        valid = cm[boxCOunter].Text != "";
                    }
                    else
                    {
                        valid = tb[textCounter].Text != "";
                    }

                    try
                    {
                        if (prmInfo[i].ParameterType == typeof(int))
                        {
                            prms[i] = int.Parse(tb[textCounter].Text);
                            textCounter++;
                        }
                        if (prmInfo[i].ParameterType == typeof(string))
                        {
                            prms[i] = tb[textCounter].Text;
                            textCounter++;
                        }
                       
                        if (prmInfo[i].ParameterType.IsEnum) 
                        {
                            prms[i] = Enum.Parse(prmInfo[i].ParameterType,cm[boxCOunter].Text);
                            boxCOunter++;
                        }
                        if(prmInfo[i].ParameterType.IsClass && !(prmInfo[i].ParameterType.Equals(typeof(string))))
                        {
                            if (armourList != null)
                            {
                                prms[i] = armourList[0];
                                armourList = null;
                            }
                            else
                                prms[i]=null;
                        }
                        
                    }
                    catch
                    {
                        valid = false;
                    }
                }

                if (valid)
                {

                    obj = Activator.CreateInstance(type, prms);
                    if ((obj).GetType().Equals(typeof(Armour)))
                    {
                        this.armourList.Add((Armour)obj);
                    }
                    else
                    {
                        this.objList.Add((Character)obj);
                        Update(objList);

                    }
                        
                    form.Hide();
                    form.Dispose();
                }
                else
                {
                    MessageBox.Show("Error", "Error",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Error);
                }
               
            }



        }

        private void DeleteModalClick(object sender, EventArgs e)
        {
            listView1.Clear();
        }

        //полное редактирование объекта
        private void EditObj(object obj)
        {
            IEnumerable<PropertyInfo> props = obj.GetType().GetRuntimeProperties();
            PropertyInfo[] props1 = obj.GetType().GetProperties();
            List<PropertyInfo> publicProps = new List<PropertyInfo>();

            //генерация формы редактирования существующего объекта
            foreach (PropertyInfo inf in props)
            {
                if (inf.GetSetMethod() != null || inf.PropertyType.Equals(typeof(Armour)))
                    publicProps.Add(inf);
            }

            int j = 0;
            for (int i = 0; i < publicProps.Count; i++)
            {
                if (publicProps[i].PropertyType.IsEnum)
                {
                    j++;
                }
            }

            Form form = new Form
            {
                Width = 250
            };
            form.Height = form.Height = (publicProps.Count + 3) * 30;
            form.FormBorderStyle = FormBorderStyle.FixedSingle;

            int deltaleft = 100;
            int compDelta = 10;
            int tbWidth = 130;
            int tbHeight = 30;

            TextBox[] tb = new TextBox[(publicProps.Count) - j];
            Label[] lb = new Label[publicProps.Count];
            ComboBox[] cmb = new ComboBox[j];
            int textCounter = 0;
            int boxCounter = 0;
            for (int i = 0; i < publicProps.Count; i++)
            {
                if (publicProps[i].PropertyType.IsEnum)
                {
                    cmb[boxCounter] = new ComboBox
                    {
                        Left = deltaleft,
                        Top = compDelta,
                        Width = tbWidth,
                        Height = tbHeight
                    };

                    lb[i] = new Label
                    {
                        Top = compDelta + 5,
                        Text = publicProps[i].Name
                    };

                    form.Controls.Add(cmb[boxCounter]);
                    form.Controls.Add(lb[i]);
                    object value = publicProps[i].GetValue(obj);
                    int index = 0;
                    int counter = 0;
                    foreach (var prm in Enum.GetValues(publicProps[i].PropertyType))
                    {
                        cmb[boxCounter].Items.Add(prm);
                        if (prm.Equals(value))
                        {
                            index = counter;
                        }
                        counter++;
                    }

                    cmb[boxCounter].SelectedIndex = index;
                    boxCounter++;
                    compDelta += tbHeight;
                }
                else if (publicProps[i].PropertyType.IsClass && !(publicProps[i].PropertyType.Equals(typeof(string))))
                {
                    lb[i] = new Label
                    {
                        Top = compDelta + 5,
                        Text = publicProps[i].Name
                    };
                    form.Controls.Add(lb[i]);
                    compDelta += tbHeight;
                    Object inObj = publicProps[i].GetValue(obj);
                    arList.Add(inObj);
                    EditObj(inObj);


                }


                else
                {

                    tb[textCounter] = new TextBox
                    {
                        Left = deltaleft,
                        Top = compDelta,
                        Width = tbWidth,
                        Height = tbHeight
                    };

                    if (publicProps[i].GetGetMethod() != null)
                    {
                        object value = publicProps[i].GetValue(obj);
                        if (value != null) tb[textCounter].Text = value.ToString();
                        else tb[textCounter].Text = "";
                    }

                    lb[i] = new Label
                    {
                        Top = compDelta + 5,
                        Text = publicProps[i].Name
                    };

                    form.Controls.Add(tb[textCounter]);
                    form.Controls.Add(lb[i]);

                    compDelta += tbHeight;
                    textCounter++;
                }
            }

            Button btn = new Button
            {
                Text = "Ok",
                Width = 100,
                Height = 30
            };
            btn.Left = form.Width / 2 - btn.Width / 2;
            btn.Top = form.Height - (int)(btn.Height * 2.5);
            form.Controls.Add(btn);
            btn.Click += new System.EventHandler(EditModalClick);

            form.Show(this);


            //сохранение внесенных изменений
            void EditModalClick(object snd, EventArgs ev)
            {
                Object prm = null;
                bool success = true;
                textCounter = 0;
                boxCounter = 0;
                for (int i = 0; i < publicProps.Count; i++)
                {

                    try
                    {
                        if (publicProps[i].PropertyType == typeof(int))
                        {
                            prm = int.Parse(tb[textCounter].Text);
                            textCounter++;
                        }
                        if (publicProps[i].PropertyType == typeof(string))
                        {
                            prm = tb[textCounter].Text;
                            textCounter++;
                        }

                        if (publicProps[i].PropertyType.IsEnum)
                        {
                            prm = Enum.Parse(publicProps[i].PropertyType, cmb[boxCounter].Text);
                            boxCounter++;
                        }
                        if(publicProps[i].PropertyType.IsClass && !(publicProps[i].PropertyType.Equals(typeof(string))))
                        {
                            if (arList != null)
                            {
                                prm = arList[0];
                                arList.Clear();
                            }
                            else
                                prm = null;

                        }

                        publicProps[i].SetValue(obj, prm);
                    }
                    catch
                    {
                        success = false;
                        MessageBox.Show(publicProps[i].Name + " is invalid!", "Error",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Error);
                    }
                }

                if (success)
                {
                    if(obj.GetType().Equals(typeof(Armour)))
                    {
                        arList.Add(obj);
                        form.Hide();
                        form.Dispose();

                    }
                    else
                    {
                        form.Hide();
                        form.Dispose();
                        Update(objList);

                    }

                }
            }

        }


        private void button2_Click(object sender, EventArgs e)
        {
            int a = listView1.SelectedIndices[0];
            EditObj(objList[a]);
           
        }

        private void button3_Click(object sender, EventArgs e)
        {

            int a = listView1.SelectedIndices[0];
            objList.RemoveAt(a);
            Update(objList);
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string fileName = openFileDialog1.FileName;
            objList = serializers[openFileDialog1.FilterIndex - 1].Deserializations(fileName);
            Update(objList);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string fileName = saveFileDialog1.FileName;
            serializers[(saveFileDialog1.FilterIndex) - 1 ].Serialization(fileName,objList);
            
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
