using Npgsql;
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
        private Timer inactivityTimer;
        private string connectionStr = "Host=localhost;Username=postgres;Password=triPonnA5;Database=employeedb";
        private DataTable dataTable = new DataTable();
        private DataTable awardsTable = new DataTable();
        private string prevSnils = " ";
        private string DataFileName = "data.xml";

        public string[] row { get; set; }
        
        public Form2()
        {
            InitializeComponent();
            this.FormClosing += _FormClosing;

            // Инициализация таймера
            inactivityTimer = new Timer();
            inactivityTimer.Interval = 40000000;
            inactivityTimer.Tick += InactivityTimer_Tick;
            inactivityTimer.Start();


            // Добавление обработчиков событий для мыши
            this.MouseMove += _MouseMove;
            this.KeyPress += _KeyPress;
            this.MouseWheel += _MouseWheel;
            dataGridView1.MouseMove += _MouseMove;
            dataGridView1.KeyPress += _KeyPress;
            menuStrip1.MouseMove += _MouseMove;
            menuStrip1.KeyPress += _KeyPress;
            bindingNavigator1.MouseMove += _MouseMove;
            bindingNavigator1.KeyPress += _KeyPress;
            panel1.MouseMove += _MouseMove;
            panel1.KeyPress += _KeyPress;
            button1.MouseMove += _MouseMove;
            button1.KeyPress += _KeyPress;
            textBox1.MouseMove += _MouseMove;
            textBox1.KeyPress += _KeyPress;
            label1.MouseMove += _MouseMove;
            label1.KeyPress += _KeyPress;
            toolStripMenuItem1.MouseMove += _MouseMove;
            toolStripMenuItem2.MouseMove += _MouseMove;
            toolStripMenuItem3.MouseMove += _MouseMove;
            toolStripMenuItem4.MouseMove += _MouseMove;
            toolStripMenuItem5.MouseMove += _MouseMove;
            toolStripMenuItem6.MouseMove += _MouseMove;
            toolStripMenuItem7.MouseMove += _MouseMove;
            toolStripButton1.MouseMove += _MouseMove;
            toolStripButton2.MouseMove += _MouseMove;
            bindingNavigatorSeparator.MouseMove += _MouseMove;
            bindingNavigatorSeparator1.MouseMove += _MouseMove;

            // Добавление обработчиков событий для клавиатуры
            this.KeyPreview = true;
            this.KeyDown += _KeyDown;
        }

        private void _MouseMove(object sender, MouseEventArgs e)
        {
            ResetInactivityTimer();
        }

        private void _KeyPress(object sender, KeyPressEventArgs e)
        {
            ResetInactivityTimer();
        }

        private void _KeyDown(object sender, KeyEventArgs e)
        {
            ResetInactivityTimer();
        }

        private void _MouseWheel(object sender, MouseEventArgs e)
        {
            ResetInactivityTimer();
        }

        private void InactivityTimer_Tick(object sender, EventArgs e)
        {
            this.Close();
        }

        // Метод для сброса таймера при активности пользователя
        private void ResetInactivityTimer()
        {
            inactivityTimer.Stop();
            inactivityTimer.Start();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            toolStripButton1.Enabled = false;
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            DataGridViewTextBoxColumn idColumn = new DataGridViewTextBoxColumn();
            idColumn.Name = "id";
            dataGridView1.Columns.Add(idColumn);
            dataGridView1.Columns["id"].Visible = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
        }

        int GetEmployeeIdBySnils(NpgsqlConnection conn, string snils)
        {
            using (var cmd = new NpgsqlCommand("SELECT id FROM empinfo WHERE snils = @snils", conn))
            {
                cmd.Parameters.AddWithValue("snils", snils);
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    return Convert.ToInt32(result);
                }
                else
                {
                    return -1; // Возвращаем -1 если сотрудник не найден
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == prevSnils || textBox1.Text == "") { return; }
            else { prevSnils = textBox1.Text; }

            toolStripButton1.Enabled = false;
            toolStripButton2.Enabled = true;

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionStr))
                {
                    connection.Open();
                    int empId = GetEmployeeIdBySnils(connection, textBox1.Text.Trim());

                    if (empId == -1) 
                    {
                        MessageBox.Show("Сотрудника с таким СНИЛС не найдено", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return; 
                    }
                    dataGridView1.Columns["id"].Visible = true;

                    string sqlEntries = $"SELECT date, entry, document FROM entries where id_emp = {empId};"; // запрос
                    string sqlAwards = $"SELECT date, entry, document FROM awards where id_emp = {empId};";
                    string sqlEmpinfo = $"SELECT tk_number, name, date_birth, education, profession, date_registration" +
                        $" FROM empinfo where id = {empId};"; ;

                    using (NpgsqlCommand commandEmpifno = new NpgsqlCommand(sqlEmpinfo, connection))
                    using (NpgsqlCommand commandEntries = new NpgsqlCommand(sqlEntries, connection))
                    using (NpgsqlCommand commandAwards = new NpgsqlCommand(sqlAwards, connection))
                    {
                        NpgsqlDataAdapter adapterEntries = new NpgsqlDataAdapter(commandEntries);
                        NpgsqlDataAdapter adapterAwards = new NpgsqlDataAdapter(commandAwards);

                        DataView dataview = new DataView(null);
                        //if (dataGridView1.Rows.Count != 0 ) { dataGridView1.Rows.Clear(); }
                        dataTable.Rows.Clear();
                        awardsTable.Rows.Clear();   
                        //dataGridView1.DataSource = dataTable;
                        adapterEntries.Fill(dataTable);
                        adapterAwards.Fill(awardsTable);

                        dataGridView1.DataSource = dataTable;

                        dataGridView1.Columns["id"].Width = 100;
                        dataGridView1.Columns["date"].Width = 100;
                        dataGridView1.Columns["id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        dataGridView1.Columns["date"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;

                        dataGridView1.Columns["id"].Resizable = DataGridViewTriState.False;
                        dataGridView1.Columns["date"].Resizable = DataGridViewTriState.False;


                        dataGridView1.Columns["id"].HeaderText = "№ записи";
                        dataGridView1.Columns["date"].HeaderText = "Дата";
                        dataGridView1.Columns["entry"].HeaderText = "Сведения";
                        dataGridView1.Columns["document"].HeaderText = "Документ";

                        foreach (DataGridViewColumn column in dataGridView1.Columns)
                        {
                            column.SortMode = DataGridViewColumnSortMode.NotSortable;
                        }

                        using (var reader = commandEmpifno.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string[] separatedName = reader["name"].ToString().Split(' ');
                                textBox8.Text = "№ " + reader["tk_number"].ToString().TrimEnd();
                                textBox2.Text = separatedName[0];
                                textBox3.Text = separatedName[1];
                                textBox4.Text = separatedName[2];
                                textBox5.Text = DateTime.Parse(reader["date_birth"].ToString()).ToString("dd.MM.yyyy");
                                textBox6.Text = reader["education"].ToString();
                                textBox7.Text = reader["profession"].ToString();
                                textBox9.Text = DateTime.Parse(reader["date_registration"].ToString()).ToString("dd.MM.yyyy");
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }

            NumberRows();
            //string filterValue = textBox1.Text;
            //DataView dataView = new DataView(dataTable);
            //dataView.RowFilter = $"SNILS = '{filterValue}'";
            //dataGridView1.DataSource = dataView;

            //if (!IsSnilsValid(textBox1.Text))
            //{
            //    MessageBox.Show("СНИЛС сотрудника введен неверно",
            //        "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}
        }

        private void NumberRows()
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Cells["id"].Value = (i + 1).ToString();
            }
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            inactivityTimer.Stop();

            if(dataGridView1.DataSource == null) { return; }

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
                    int[] maxWidth = new int[4];

                    int count = 0;
                    //Определяем самые широкие строки
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        count++;
                        if (count == dataGridView1.RowCount && dataGridView1.RowCount != 1) continue;

                        for (int i = 0; i < dataGridView1.Columns.Count; i++)
                        {
                            len = row.Cells[i].Value.ToString().Length;

                            if (len > maxWidth[i]) maxWidth[i] = len;
                        }
                    }

                    for (int i = 0; i < maxWidth.Length; i++)
                    {
                        maxWidth[i] += coeffWidth;
                    }

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Номер ТК:                   " + textBox8.Text);
                    sb.AppendLine("Фамилия:                    " + textBox2.Text);
                    sb.AppendLine("Имя:                        " + textBox3.Text);
                    sb.AppendLine("Отчество:                   " + textBox4.Text);
                    sb.AppendLine("Дата рождения:              " + textBox5.Text);
                    sb.AppendLine("Образование:                " + textBox6.Text);
                    sb.AppendLine("Профессия, Специальность:   " + textBox7.Text);
                    sb.AppendLine("Дата заполнения:            " + textBox9.Text);

                    string formattedString = sb.ToString();
                    writer.Write(formattedString);
                    writer.WriteLine();

                    //Запись заголовков столбцов
                    string header = String.Format("{0," + maxWidth[0] + "}\t{1," + maxWidth[1] + "}\t{2," + maxWidth[2] + "}" +
                                    "\t{3," + maxWidth[3] + "}", dataGridView1.Columns[0].HeaderText,
                                    dataGridView1.Columns[1].HeaderText, dataGridView1.Columns[2].HeaderText,
                                    dataGridView1.Columns[3].HeaderText);
                    int removedSpaces = header.Length - header.TrimStart().Length;
                    writer.Write(header.TrimStart());
                    writer.WriteLine();

                    try
                    {
                        //Запись строк
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            string value = String.Format("{0," + maxWidth[0] + "}\t{1," + maxWidth[1] + "}\t{2," + maxWidth[2] + "}" +
                                           "\t{3," + maxWidth[3] + "}", row.Cells[0].Value, DateTime.Parse(row.Cells[1].Value
                                           .ToString()).ToString("dd.MM.yyyy"), row.Cells[2].Value, row.Cells[3].Value);
                            writer.Write(value.Substring(removedSpaces));
                            writer.WriteLine();
                        }
                    }
                    catch (Exception ex) { MessageBox.Show("Ошибка " + ex.Message); }
                }
            }

            inactivityTimer.Start();
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

                //dataSet.WriteXml(DataFileName, XmlWriteMode.IgnoreSchema);
            }

            row = null;

            return;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            inactivityTimer.Stop();

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
            
            inactivityTimer.Start();
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
            inactivityTimer.Stop();

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

                //dataSet.WriteXml(DataFileName, XmlWriteMode.IgnoreSchema);
                row = null;
            }
            else MessageBox.Show("Данный сотрудник уже НЕ числится в базе ПФР", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            inactivityTimer.Start();
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            inactivityTimer.Stop();

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

            //DataView dataView = new DataView(TableAwards);
            //dataView.RowFilter = $"SNILS = '{textBox1.Text}'";

            // Назначаем DataTable источником данных для DataGridView
            //dataGridView1.DataSource = dataView;
            //dataGridView1.Columns["SNILS"].Visible = false;
            //dataGridView1.Columns["User"].Visible = false;
            //dataGridView1.Columns["Pass"].Visible = false;
            //dataGridView1.ReadOnly = true;

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
                //TableAwards.Rows.Add(row);

                //AwardsSet.WriteXml(AwardsFilename, XmlWriteMode.IgnoreSchema);
                row = null;
            }
            else MessageBox.Show("Данный сотрудник уже НЕ числится в базе ПФР", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            inactivityTimer.Start();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            toolStripButton1.Enabled = false;
            toolStripButton2.Enabled = true;

            DataView dataView = new DataView(dataTable);
            dataGridView1.DataSource = dataView;
            dataGridView1.ReadOnly = true;
            NumberRows();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            toolStripButton1.Enabled = true;
            toolStripButton2.Enabled = false;

            DataView dataView = new DataView(awardsTable);
            dataGridView1.DataSource = dataView;
            dataGridView1.ReadOnly = true;
            NumberRows();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            inactivityTimer.Stop();

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

                //dataSet.WriteXml(DataFileName, XmlWriteMode.IgnoreSchema);
                row = null;
            }
            else MessageBox.Show("Данный сотрудник уже НЕ числится в базе ПФР", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            inactivityTimer.Start();
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            inactivityTimer.Stop();

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

                //dataSet.WriteXml(DataFileName, XmlWriteMode.IgnoreSchema);
                row = null;
            }
            else MessageBox.Show("Данный сотрудник уже НЕ числится в базе ПФР", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            inactivityTimer.Start();
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

        private void _FormClosing(object sender, FormClosingEventArgs e)
        {
            inactivityTimer.Stop();
            Application.Exit();
        }

    }
}
