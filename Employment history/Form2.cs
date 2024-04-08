using System;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Employment_history
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public string[] row{ get; set; }
        private string DataFileName = "data.xml";

        DataTable dataTable;
        DataSet dataSet = new DataSet("Employment_History");

        DataTable TableAwards;
        DataSet AwardsSet;

        private void Form2_Load(object sender, EventArgs e)
        {
            toolStripButton1.Enabled = false;
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            // Создаем объект XmlDocument и загружаем в него XML-файл
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(DataFileName);

            // Выбираем все узлы "Employee"
            XmlNodeList employeeNodes = xmlDoc.SelectNodes("//Employee");

            // Создаем объект DataTable и добавляем в него необходимые столбцы
            dataTable = new DataTable("Employee");
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

            DataView dataView = new DataView(dataTable);
            dataView.RowFilter = $"SNILS = ''";

            // Назначаем DataTable источником данных для DataGridView
            dataGridView1.DataSource = dataView;
            dataGridView1.Columns["SNILS"].Visible = false;
            dataGridView1.Columns["User"].Visible = false;
            dataGridView1.Columns["Pass"].Visible = false;
            dataGridView1.ReadOnly = true;

            dataGridView1.Columns["Number"].Width = 50;
            dataGridView1.Columns["Date"].Width = 75;
            dataGridView1.Columns["Number"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridView1.Columns["Date"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;




            XmlDocument xmlAwards = new XmlDocument();
            TableAwards = new DataTable("Employee");
            AwardsSet = new DataSet("Awards");
            xmlAwards.Load("awards.xml");

            employeeNodes = xmlAwards.SelectNodes("//Employee");

            TableAwards.Columns.Add("User");
            TableAwards.Columns.Add("Pass");
            TableAwards.Columns.Add("SNILS");
            TableAwards.Columns.Add("Number");
            TableAwards.Columns.Add("Date");
            TableAwards.Columns.Add("Entries");
            TableAwards.Columns.Add("Document");

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

                TableAwards.Rows.Add(user, pass, snils, number, date, entries, document);
            }
            AwardsSet.Tables.Add(TableAwards);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            toolStripButton1.Enabled = false;
            toolStripButton2.Enabled = true;

            string filterValue = textBox1.Text;
            DataView dataView = new DataView(dataTable);
            dataView.RowFilter = $"SNILS = '{filterValue}'";
            dataGridView1.DataSource = dataView;

            if (!IsSnilsValid(textBox1.Text))
            {
                MessageBox.Show("СНИЛС сотрудника введен неверно",
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.Title = "Save file as";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                {
                    // Определяем ширину столбца для выравнивания
                    int columnWidth = 15; int coeffWidth = columnWidth;
                    int len;
                    int[] maxWidth = new int[7];

                    int count = 0;
                    //Определяем самые широкие строки
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        count++;
                        if (count == dataGridView1.RowCount) continue;

                        for (int i = 3; i < dataGridView1.Columns.Count; i++)
                        {
                            len = row.Cells[i].Value.ToString().Length;

                            if (len > maxWidth[i]) maxWidth[i] = len;
                        }
                    }

                    for (int i = 0; i < maxWidth.Length; i++)
                    {
                        maxWidth[i] += coeffWidth;
                    }

                    //Запись заголовков столбцов
                    string header = String.Format("{0," + maxWidth[3] + "}\t{1," + maxWidth[4] + "}\t{2," + maxWidth[5] + "}" +
                                    "\t{3," + maxWidth[6] + "}", dataGridView1.Columns[3].HeaderText,
                                    dataGridView1.Columns[4].HeaderText, dataGridView1.Columns[5].HeaderText,
                                    dataGridView1.Columns[6].HeaderText);
                    writer.Write(header);
                    writer.WriteLine();

                    //Запись строк
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        string value = String.Format("{0," + maxWidth[3] + "}\t{1," + maxWidth[4] + "}\t{2," + maxWidth[5] + "}" +
                                       "\t{3," + maxWidth[6] + "}", row.Cells[3].Value, row.Cells[4].Value, row.Cells[5].Value, 
                                       row.Cells[6].Value);
                        writer.Write(value);
                        writer.WriteLine();
                    }
                }
            }
        }

        void MenuItem2_Support()
        {
            if (row == null) return;
            else if (row[0] == "" || row[1] == "" || row[4] == "" || row[5] == "" || row[6] == "")
            {
                MessageBox.Show("Некоторые поля не заполнены", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (row[0] == "0") row[0] = SnilsUsername(textBox1.Text);
                if (row[1] == "0") row[1] = SnilsPassword(textBox1.Text);
                row[2] = textBox1.Text;
                row[3] = NextNumber(textBox1.Text, DataFileName);
                dataTable.Rows.Add(row);

                dataSet.WriteXml(DataFileName, XmlWriteMode.IgnoreSchema);
            }

            row = null;

            return;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (!IsSnilsValid(textBox1.Text)) MessageBox.Show("СНИЛС сотрудника введен неверно", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (!IsUserExist(textBox1.Text))
            {
                Form4 form4 = new Form4();
                form4.Owner = this;
                form4.ShowDialog();

                MenuItem2_Support();
            }
            else if (FirstWordFired(DataFileName, textBox1.Text))
            {
                Form5 form5 = new Form5("toolStripMenuItem2");
                form5.Owner = this;
                form5.ShowDialog();

                MenuItem2_Support();
            }
            else MessageBox.Show("Данный сотрудник уже числится в базе ПФР", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private string SnilsUsername(string Snils)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(DataFileName);
            XmlNodeList employeeNodes = xmlDoc.SelectNodes("//Employee");

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
            XmlNodeList employeeNodes = xmlDoc.SelectNodes("//Employee");

            foreach (XmlNode employeeNode in employeeNodes)
            {
                if (employeeNode.SelectSingleNode("SNILS").InnerText == Snils)
                {
                    return employeeNode.SelectSingleNode("Pass").InnerText;
                }
            }

            return null;
        }

        private string NextNumber(string Snils, string filename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filename);

            // Выбираем все узлы "Employee"
            XmlNodeList employeeNodes = xmlDoc.SelectNodes("//Employee");
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
            XmlNodeList employeeNodes = xmlDoc.SelectNodes("//Employee");

            foreach (XmlNode employeeNode in employeeNodes)
            {
                if (employeeNode.SelectSingleNode("SNILS").InnerText == SNILS)
                { return true; }
            }

            return false;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            toolStripMenuItem3.Tag = "toolStripMenuItem3";
            if (!IsUserExist(textBox1.Text)) MessageBox.Show("СНИЛС сотрудника введен неверно (Данный сотрудник не числится в базе ПФР)", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (!FirstWordFired(DataFileName, textBox1.Text))
            {
                Form5 form5 = new Form5("toolStripMenuItem3");
                form5.Owner = this;
                form5.ShowDialog();

                if (row == null) return;
                else if (row[4] == "" || row[5] == "" || row[6] == "")
                {
                    MessageBox.Show("Некоторые поля не заполнены", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    row = null;
                    return;
                }

                row[0] = SnilsUsername(textBox1.Text);
                row[1] = SnilsPassword(textBox1.Text);
                row[2] = textBox1.Text;
                row[3] = NextNumber(textBox1.Text, DataFileName);
                dataTable.Rows.Add(row);

                dataSet.WriteXml(DataFileName, XmlWriteMode.IgnoreSchema);
                row = null;
            }
            else MessageBox.Show("Данный сотрудник уже НЕ числится в базе ПФР", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            if (IsUserExist(textBox1.Text))
            {
                toolStripButton1.Enabled = true;
                toolStripButton2.Enabled = false;
            }
            else
            {
                toolStripButton1.Enabled = false;
                toolStripButton2.Enabled = true;
            }

            string AwardsFilename = "awards.xml";

            DataView dataView = new DataView(TableAwards);
            dataView.RowFilter = $"SNILS = '{textBox1.Text}'";

            // Назначаем DataTable источником данных для DataGridView
            dataGridView1.DataSource = dataView;
            dataGridView1.Columns["SNILS"].Visible = false;
            dataGridView1.Columns["User"].Visible = false;
            dataGridView1.Columns["Pass"].Visible = false;
            dataGridView1.ReadOnly = true;

            if (!IsUserExist(textBox1.Text)) MessageBox.Show("СНИЛС сотрудника введен неверно (Данный сотрудник не числится в базе ПФР)", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (sender == toolStripButton2) { }
            else if (!FirstWordFired(DataFileName, textBox1.Text))
            {
                Form5 form5 = new Form5("toolStripMenuItem5");
                form5.Owner = this;
                form5.ShowDialog();

                if (row == null) return;
                else if (row[4] == "" || row[5] == "" || row[6] == "")
                {
                    MessageBox.Show("Некоторые поля не заполнены", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    row = null;
                    return;
                }

                row[0] = SnilsUsername(textBox1.Text);
                row[1] = SnilsPassword(textBox1.Text);
                row[2] = textBox1.Text;
                row[3] = NextNumber(textBox1.Text, AwardsFilename);
                TableAwards.Rows.Add(row);

                AwardsSet.WriteXml(AwardsFilename, XmlWriteMode.IgnoreSchema);
                row = null;
            }
            else MessageBox.Show("Данный сотрудник уже НЕ числится в базе ПФР", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            toolStripButton1.Enabled = false;
            toolStripButton2.Enabled = true;

            DataView dataView = new DataView(dataTable);
            dataView.RowFilter = $"SNILS = '{textBox1.Text}'";

            dataGridView1.DataSource = dataView;
            dataGridView1.Columns["SNILS"].Visible = false;
            dataGridView1.Columns["User"].Visible = false;
            dataGridView1.Columns["Pass"].Visible = false;
            dataGridView1.ReadOnly = true;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            toolStripMenuItem5_Click(sender, e);
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (!IsUserExist(textBox1.Text)) MessageBox.Show("СНИЛС сотрудника введен неверно (Данный сотрудник не числится в базе ПФР)", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (!FirstWordFired(DataFileName, textBox1.Text))
            {
                Form5 form5 = new Form5("toolStripMenuItem4");
                form5.Owner = this;
                form5.ShowDialog();

                if (row == null) return;
                else if (row[4] == "" || row[5] == "" || row[6] == "")
                {
                    MessageBox.Show("Некоторые поля не заполнены", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    row = null;
                    return;
                }

                row[0] = SnilsUsername(textBox1.Text);
                row[1] = SnilsPassword(textBox1.Text);
                row[2] = textBox1.Text;
                row[3] = NextNumber(textBox1.Text, DataFileName);
                dataTable.Rows.Add(row);

                dataSet.WriteXml(DataFileName, XmlWriteMode.IgnoreSchema);
                row = null;
            }
            else MessageBox.Show("Данный сотрудник уже НЕ числится в базе ПФР", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            if (!IsUserExist(textBox1.Text)) MessageBox.Show("СНИЛС сотрудника введен неверно (Данный сотрудник не числится в базе ПФР)", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (!FirstWordFired(DataFileName, textBox1.Text))
            {
                Form5 form5 = new Form5("toolStripMenuItem6");
                form5.Owner = this;
                form5.ShowDialog();

                if (row == null) return;
                else if (row[4] == "" || row[5] == "" || row[6] == "")
                {
                    MessageBox.Show("Некоторые поля не заполнены", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    row = null;
                    return;
                }

                row[0] = SnilsUsername(textBox1.Text);
                row[1] = SnilsPassword(textBox1.Text);
                row[2] = textBox1.Text;
                row[3] = NextNumber(textBox1.Text, DataFileName);
                dataTable.Rows.Add(row);

                dataSet.WriteXml(DataFileName, XmlWriteMode.IgnoreSchema);
                row = null;
            }
            else MessageBox.Show("Данный сотрудник уже НЕ числится в базе ПФР", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public bool FirstWordFired(string filePath, string targetValue)
        {
            // Загрузка XML-файла
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(filePath);

            XmlNode lastEmployee = null;

            string xPathQuery = $"/Employment_History/Employee[SNILS = '{targetValue}']";
            XmlNodeList matchingEmployees = xmlDocument.SelectNodes(xPathQuery);
            if (matchingEmployees.Count > 0)
            {
                lastEmployee = matchingEmployees[matchingEmployees.Count - 1];

                // Получение текстового содержимого поля
                string lastEntries = lastEmployee.SelectSingleNode("Entries").InnerText.Trim();

                // Разделение содержимого поля на слова
                string[] words = lastEntries.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (words.Length > 0)
                {
                    if (words[0] == "Уволен" || words[0] == "уволен") return true;
                }
            }
            
            return false;
        }
    }
}
