using System;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
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
        //public string snils { get; set; }
        public string[] row{ get; set; }
        private string DataFileName = "data.xml";
        XmlDocument xmlDoc;
        DataTable dataTable;
        DataSet dataSet = new DataSet("Employment_History");

        private void Form2_Load(object sender, EventArgs e)
        {
            toolStripButton1.Enabled = false;
            
            //// Задаем отступы DataGridView
            //dataGridView1.Margin = new Padding(0, 0, 0, 50);

            //// Привязываем DataGridView к нижнему краю родительского элемента
            //dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;


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

            DataTable tempTable = new DataTable();
            tempTable.Columns.Add("User");
            tempTable.Rows.Add("t1");
            tempTable.Rows.Add("t2");
            tempTable.Rows.Add("t3");
            tempTable.Rows.Add("t4");

            bindingSource1.DataSource = tempTable;
            bindingNavigator1.BindingSource = bindingSource1;
            //bindingNavigatorMoveNextItem.PerformClick();

            DataView dataView = new DataView(dataTable);
            dataView.RowFilter = $"SNILS = ''";

            // Назначаем DataTable источником данных для DataGridView
            dataGridView1.DataSource = dataView;
            dataGridView1.Columns["SNILS"].Visible = false;
            dataGridView1.Columns["User"].Visible = false;
            dataGridView1.Columns["Pass"].Visible = false;
            dataGridView1.ReadOnly = true;

            dataGridView1.Columns["Number"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridView1.Columns["Date"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //dataGridView1.Columns["Entries"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //dataGridView1.Columns["Document"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            // Устанавливаем режим автоматического изменения ширины столбцов

        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

                // Установить режим переноса строк
                cell.Style.WrapMode = DataGridViewTriState.True;

                // Установить высоту строки равной высоте содержимого
                var textSize = e.Graphics.MeasureString(cell.Value.ToString(), cell.Style.Font, e.CellBounds.Width);
                dataGridView1.Rows[e.RowIndex].Height = (int)textSize.Height + 2;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            
            toolStripButton1.Enabled = false;
            toolStripButton2.Enabled = true;

            string filterValue = textBox1.Text;
            //DataView dataView = (DataView)dataGridView1.DataSource;
            DataView dataView = new DataView(dataTable);
            dataView.RowFilter = $"SNILS = '{filterValue}'";
            dataGridView1.DataSource = dataView;

            if (!IsSnilsValid(textBox1.Text))
            {
                MessageBox.Show("СНИЛС сотрудника введен неверно", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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
                    int columnWidth = 15; int coeffWidth = columnWidth; int coeffWidthEntries = 75;
                    //int columnWidthEntries = 100;

                    // Запись заголовков столбцов
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        if (i == 0 || i == 1 || i == 2) continue;
                        if (i == 5) columnWidth = coeffWidthEntries;
                        string header = String.Format("{0," + columnWidth + "}", dataGridView1.Columns[i].HeaderText);
                        writer.Write(header);

                        if (i != dataGridView1.Columns.Count - 1)
                        {
                            //writer.Write("\t");
                        }
                    }
                    writer.WriteLine();

                    // Запись данных
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        columnWidth = coeffWidth;
                        for (int i = 0; i < dataGridView1.Columns.Count; i++)
                        {
                            if (i == 0 || i == 1 || i == 2) continue;
                            if (i == 5) columnWidth = coeffWidthEntries;
                            string value = String.Format("{0," + columnWidth + "}", row.Cells[i].Value);
                            writer.Write(value);

                            if (i != dataGridView1.Columns.Count - 1)
                            {
                               // writer.Write("\t");
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

                if (row == null) return;

                if (row[0] == "" || row[1] == "" || row[4] == "" || row[5] == "" || row[6] == "")
                {
                    MessageBox.Show("Некоторые поля не заполнены", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    row = null;
                    return;
                }

                row[2] = textBox1.Text;
                row[3] = "1";
                dataTable.Rows.Add(row);

                //DataSet dataSet = new DataSet();
                //dataSet.Tables.Add(dataTable);
                dataSet.WriteXml(DataFileName, XmlWriteMode.IgnoreSchema);

                //dataTable.WriteXml("data.xml", XmlWriteMode.WriteSchema);
            }
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
            else
            {
                Form5 form5 = new Form5("toolStripMenuItem3");
                form5.Owner = this;
                form5.ShowDialog();

                if (row == null) return;

                if (row[4] == "" || row[5] == "" || row[6] == "")
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
            }
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

            XmlDocument xmlAwards = new XmlDocument();
            DataTable TableAwards = new DataTable("Employee");
            DataSet AwardsSet = new DataSet("Awards");
            xmlAwards.Load("awards.xml");
            string AwardsFilename = "awards.xml";

            XmlNodeList employeeNodes = xmlAwards.SelectNodes("//Employee");

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
            //bindingSource1.DataSource = TableAwards;
            //this.bindingNavigator1.BindingSource = bindingSource1;

            DataView dataView = new DataView(TableAwards);
            dataView.RowFilter = $"SNILS = '{textBox1.Text}'";

            // Назначаем DataTable источником данных для DataGridView
            dataGridView1.DataSource = dataView;
            dataGridView1.Columns["SNILS"].Visible = false;
            dataGridView1.Columns["User"].Visible = false;
            dataGridView1.Columns["Pass"].Visible = false;
            dataGridView1.ReadOnly = true;

            if (!IsUserExist(textBox1.Text))
            {   
                MessageBox.Show("СНИЛС сотрудника введен неверно (Данный сотрудник не числится в базе ПФР)", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (sender == toolStripButton2) { }
            else
            {
                Form5 form5 = new Form5("toolStripMenuItem5");
                form5.Owner = this;
                form5.ShowDialog();

                if (row == null) return;

                if (row[4] == "" || row[5] == "" || row[6] == "")
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
            }
        }

        //private void bindingNavigatorMoveNextItem_Click(object sender, EventArgs e)
        //{
        //    //bindingNavigatorMoveNextItem.Enabled = false;
        //    //bindingNavigatorMovePreviousItem.Enabled = true;

        //    toolStripMenuItem5_Click(sender, e);

        //    bindingNavigatorMoveNextItem.Enabled = false;
        //}

        //private void bindingNavigatorMovePreviousItem_Click(object sender, EventArgs e)
        //{
        //    //MessageBox.Show("Hands Up");
        //    bindingNavigatorMoveNextItem.Enabled = true;
        //    bindingNavigatorMovePreviousItem.Enabled = false;

        //    DataView dataView = new DataView(dataTable);
        //    dataView.RowFilter = $"SNILS = '{textBox1.Text}'";

        //    // Назначаем DataTable источником данных для DataGridView
        //    dataGridView1.DataSource = dataView;
        //    dataGridView1.Columns["SNILS"].Visible = false;
        //    dataGridView1.Columns["User"].Visible = false;
        //    dataGridView1.Columns["Pass"].Visible = false;
        //    dataGridView1.ReadOnly = true;
        //}

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
            else
            {
                Form5 form5 = new Form5("toolStripMenuItem4");
                form5.Owner = this;
                form5.ShowDialog();

                if (row == null) return;

                if (row[4] == "" || row[5] == "" || row[6] == "")
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
            }
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            if (!IsUserExist(textBox1.Text)) MessageBox.Show("СНИЛС сотрудника введен неверно (Данный сотрудник не числится в базе ПФР)", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                Form5 form5 = new Form5("toolStripMenuItem6");
                form5.Owner = this;
                form5.ShowDialog();

                if (row == null) return;

                if (row[4] == "" || row[5] == "" || row[6] == "")
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
            }
        }

        //private void toolStripButton3_Click(object sender, EventArgs e)
        //{
        //    //DataView dataView = new DataView(dataTable);
        //    //dataView.RowFilter = $"SNILS = '{textBox1.Text}'";

        //    //// Назначаем DataTable источником данных для DataGridView
        //    //dataGridView1.DataSource = dataView;
        //    //dataGridView1.Columns["SNILS"].Visible = false;
        //    //dataGridView1.Columns["User"].Visible = false;
        //    //dataGridView1.Columns["Pass"].Visible = false;
        //    //dataGridView1.ReadOnly = true;
        //}

        //private void toolStripButton2_Click(object sender, EventArgs e)
        //{
        //    toolStripMenuItem5_Click(sender, e);
        //}

        //private void toolStripButton4_Click(object sender, EventArgs e)
        //{
        //    //DataView dataView = new DataView(dataTable);
        //    //dataView.RowFilter = $"SNILS = '{textBox1.Text}'";

        //    //// Назначаем DataTable источником данных для DataGridView
        //    //dataGridView1.DataSource = dataView;
        //    //dataGridView1.Columns["SNILS"].Visible = false;
        //    //dataGridView1.Columns["User"].Visible = false;
        //    //dataGridView1.Columns["Pass"].Visible = false;
        //    //dataGridView1.ReadOnly = true;
        //}
    }
}
