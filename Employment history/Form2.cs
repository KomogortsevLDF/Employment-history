using System;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Employment_history
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        //public string snils { get; set; }
        public string[] row{ get; set; }
        private string DataFileName = "data.xml";
        XmlDocument xmlDoc;
        DataTable dataTable;
        DataSet dataSet = new DataSet();

        private void Form2_Load(object sender, EventArgs e)
        {
            //dataTable = new DataTable();
            //dataTable.ReadXml(DataFileName);

            //DataXML = new DataSet();

            //FileStream fs = new FileStream(file_name, FileMode.Open);
            //xml_read = new XmlTextReader(fs);
            //DataXML.ReadXml(xml_read, XmlReadMode.InferSchema);
            //bindingSource1.DataMember = DataXML.Tables[0].ToString();
            //bindingSource1.DataSource = DataXML.Tables[0];

            //this.bindingNavigator1.BindingSource = bindingSource1;
            //dataGridView1.DataSource = bindingSource1;
            //dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;

            //dataGridView1.Columns[0].Width = 25;
            //dataGridView1.Columns[0].HeaderText = "№";
            //dataGridView1.Columns[1].Width = 25;
            //dataGridView1.Columns[1].HeaderText = "Число";
            //dataGridView1.Columns[2].Width = 25;
            //dataGridView1.Columns[2].HeaderText = "Месяц";
            //dataGridView1.Columns[3].Width = 25;
            //dataGridView1.Columns[3].HeaderText = "Год";
            //dataGridView1.Columns[4].Width = 25;
            //dataGridView1.Columns[4].HeaderText = "Записи";
            //dataGridView1.Columns[5].Width = 25;
            //dataGridView1.Columns[5].HeaderText = "Документ";
            //fs.Close();

            // Создаем объект XmlDocument и загружаем в него XML-файл
            xmlDoc = new XmlDocument();
            xmlDoc.Load(DataFileName);

            // Выбираем все узлы "Employee"
            XmlNodeList employeeNodes = xmlDoc.SelectNodes("//Table");

            // Создаем объект DataTable и добавляем в него необходимые столбцы
            dataTable = new DataTable("Table");
            dataTable.Columns.Add("User"); 
            dataTable.Columns.Add("Pass");
            dataTable.Columns.Add("SNILS");
            dataTable.Columns.Add("Number");
            dataTable.Columns.Add("Date");
            dataTable.Columns.Add("Entries");
            dataTable.Columns.Add("Document");
            
            // Проходим по всем узлам "Employee" и добавляем данные в DataTable
            foreach (XmlNode employeeNode in employeeNodes)
            {
                string user = employeeNode.SelectSingleNode("User").InnerText;
                string pass = employeeNode.SelectSingleNode("Pass").InnerText;
                string snils = employeeNode.SelectSingleNode("SNILS").InnerText;
                string number = employeeNode.SelectSingleNode("Number").InnerText;
                string date = employeeNode.SelectSingleNode("Date").InnerText;
                string entries = employeeNode.SelectSingleNode("Entries").InnerText;
                string document = employeeNode.SelectSingleNode("Document").InnerText;

                dataTable.Rows.Add(user, pass, snils, number, date, entries, document);
            }

            dataSet.Tables.Add(dataTable);
            bindingSource1.DataSource = dataTable;
            this.bindingNavigator1.BindingSource = bindingSource1;
            
            DataView dataView = new DataView(dataTable);
            dataView.RowFilter = $"SNILS = ''";

            // Назначаем DataTable источником данных для DataGridView
            dataGridView1.DataSource = dataView;
            dataGridView1.Columns["SNILS"].Visible = false;
            dataGridView1.Columns["User"].Visible = false;
            dataGridView1.Columns["Pass"].Visible = false;
            dataGridView1.ReadOnly = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Фильтруем данные по значению SNILS
            //DataView dataView = new DataView(dataTable);
            //dataView.RowFilter = $"SNILS = '{textBox1}'";

            //dataGridView1.DataSource = dataView;

            if (IsSnilsValid(textBox1.Text))
            {
                string filterValue = textBox1.Text;
                DataView dataView = (DataView)dataGridView1.DataSource;
                dataView.RowFilter = $"SNILS = '{filterValue}'";
                dataGridView1.DataSource = dataView;
            }
            else MessageBox.Show("СНИЛС сотрудника введен неверно", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            // Создание диалогового окна сохранения файла
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.Title = "Save file as";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Открытие потока для записи данных в файл
                using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                {
                    // Определяем ширину столбца для выравнивания
                    int columnWidth = 25;

                    // Запись заголовков столбцов
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        // Выравнивание по центру
                        string header = String.Format("{0," + columnWidth + "}", dataGridView1.Columns[i].HeaderText);
                        writer.Write(header);

                        if (i != dataGridView1.Columns.Count - 1)
                        {
                            writer.Write("\t");
                        }
                    }
                    writer.WriteLine();

                    // Запись данных
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        for (int i = 0; i < dataGridView1.Columns.Count; i++)
                        {
                            // Выравнивание по центру
                            string value = String.Format("{0," + columnWidth + "}", row.Cells[i].Value);
                            writer.Write(value);

                            if (i != dataGridView1.Columns.Count - 1)
                            {
                                writer.Write("\t");
                            }
                        }
                        writer.WriteLine();
                    }

                }
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (!IsSnilsValid(textBox1.Text)) MessageBox.Show("СНИЛС сотрудника введен неверно", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (IsUserExist(textBox1.Text)) MessageBox.Show("Данный сотрудник уже числится в базе ПФР", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                Form4 form4 = new Form4();
                form4.Owner = this;
                form4.ShowDialog();

                row[2] = textBox1.Text;
                row[3] = NextNumber(textBox1.Text);
                dataTable.Rows.Add(row);

                DataSet dataSet = new DataSet();
                dataSet.Tables.Add(dataTable);
                dataSet.WriteXml(DataFileName, XmlWriteMode.IgnoreSchema);

                //dataTable.WriteXml("data.xml", XmlWriteMode.WriteSchema);
            }
        }

        private string SnilsUsername(string Snils)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(DataFileName);
            XmlNodeList employeeNodes = xmlDoc.SelectNodes("//Table");

            foreach (XmlNode employeeNode in employeeNodes)
            {
                if (employeeNode.SelectSingleNode("SNILS").InnerText == Snils)
                {
                    return employeeNode.SelectSingleNode("User").InnerText;
                }
            }

            return null;
        }

        private string SnilsPassword(string Snils)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(DataFileName);
            XmlNodeList employeeNodes = xmlDoc.SelectNodes("//Table");

            foreach (XmlNode employeeNode in employeeNodes)
            {
                if (employeeNode.SelectSingleNode("SNILS").InnerText == Snils)
                {
                    return employeeNode.SelectSingleNode("Pass").InnerText;
                }
            }

            return null;
        }

        private string NextNumber(string Snils)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(DataFileName);

            // Выбираем все узлы "Employee"
            XmlNodeList employeeNodes = xmlDoc.SelectNodes("//Table");
            int count = 0;
            foreach (XmlNode employeeNode in employeeNodes)
            {
                if (employeeNode.SelectSingleNode("SNILS").InnerText == Snils)
                {
                    count++;
                }
            }

            return (count+1).ToString();    
        }
        private bool IsSnilsValid(string input)
        {
            // Создаем регулярное выражение, соответствующее образцу
            string pattern = @"^\d{3}-\d{3}-\d{3} \d{2}$";
            Regex regex = new Regex(pattern);

            // Проверяем, соответствует ли входная строка образцу
            return regex.IsMatch(input);
        }


        private bool IsUserExist(string SNILS)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(DataFileName);
            XmlNodeList employeeNodes = xmlDoc.SelectNodes("//Table");

            foreach (XmlNode employeeNode in employeeNodes)
            {
                if (employeeNode.SelectSingleNode("SNILS").InnerText == SNILS)
                { return true; }
            }

            return false;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (!IsUserExist(textBox1.Text)) MessageBox.Show("СНИЛС сотрудника введен неверно (Данный сотрудник не числится в базе ПФР)", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                Form4 form4 = new Form4();
                form4.Owner = this;
                form4.ShowDialog();

                row[0] = SnilsUsername(textBox1.Text);
                row[1] = SnilsPassword(textBox1.Text);
                row[2] = textBox1.Text;
                row[3] = NextNumber(textBox1.Text);
                dataTable.Rows.Add(row);

                dataSet.WriteXml(DataFileName, XmlWriteMode.IgnoreSchema);

                //dataTable.WriteXml("data.xml", XmlWriteMode.WriteSchema);
            }
        }
    }
}
