using Npgsql;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Employment_history
{
    public partial class Form2 : Form
    {
        private Timer inactivityTimer;
        private Timer WarningTimer;
        private string connectionStr = "Host=localhost;Username=postgres;Password=triPonnA5;Database=employeedb";
        private DataTable dataTable = new DataTable();
        private DataTable awardsTable = new DataTable();
        private string prevSnils = " ", username;
        Logger logger;

        public string[] row { get; set; }
        
        public Form2(string _username, Logger _logger)
        {
            logger = _logger;
            username = _username; 

            InitializeComponent();
            this.FormClosing += _FormClosing;

            // Инициализация таймеров
            inactivityTimer = new Timer();
            inactivityTimer.Interval = 5 * 60 * 1000;
            inactivityTimer.Tick += InactivityTimer_Tick;
            inactivityTimer.Start();

            WarningTimer = new Timer();
            WarningTimer.Interval = 1 * 60 * 1000;
            WarningTimer.Tick += WarningTimer_Tick;
            WarningTimer.Start();


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

        private void WarningTimer_Tick(object sender, EventArgs e)
        {
            MessageBox.Show($"Обнаружено бездействие\n" +
                $"Пользователь будет разлогирован через {WarningTimer.Interval / 1000} сек.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }

        // Метод для сброса таймера при активности пользователя
        private void ResetInactivityTimer()
        {
            inactivityTimer.Stop();
            inactivityTimer.Start();

            WarningTimer.Stop();
            WarningTimer.Start();
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

            if (!IsSnilsValid(textBox1.Text.Trim()))
            {
                MessageBox.Show("СНИЛС сотрудника введен неверно", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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
                        $" FROM empinfo where id = {empId};";

                    using (NpgsqlCommand commandEmpifno = new NpgsqlCommand(sqlEmpinfo, connection))
                    using (NpgsqlCommand commandEntries = new NpgsqlCommand(sqlEntries, connection))
                    using (NpgsqlCommand commandAwards = new NpgsqlCommand(sqlAwards, connection))
                    {
                        NpgsqlDataAdapter adapterEntries = new NpgsqlDataAdapter(commandEntries);
                        NpgsqlDataAdapter adapterAwards = new NpgsqlDataAdapter(commandAwards);

                        DataView dataview = new DataView(null);
                        dataTable.Rows.Clear();
                        awardsTable.Rows.Clear();
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

            if (!IsSnilsValid(textBox1.Text))
            {
                MessageBox.Show("СНИЛС сотрудника введен неверно",
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


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

        void InsertIntoDB(object sender)
        {
            if (row == null) return;
            else
            {
                DateTime dateBirth = DateTime.ParseExact(row[4], "dd.MM.yyyy", null);
                DateTime dateReg = DateTime.ParseExact(row[7], "dd.MM.yyyy", null);
                DateTime dateValue = DateTime.ParseExact(row[0], "dd.MM.yyyy", null);

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionStr))
                {
                    connection.Open();
                    if (row.Length > 5)
                    {
                        string sqlEmp = "INSERT INTO empinfo (tk_number, name, date_birth, education, profession, date_registration, " +
                            "login, pass, snils) VALUES (@tk_number, @name, @date_birth, @education, @profession, @date_registration," +
                            " @login, @pass, @snils);";

                        using (NpgsqlCommand command = new NpgsqlCommand(sqlEmp, connection))
                        {
                            command.Parameters.AddWithValue("@tk_number", GetTkNumber(connection)); // function
                            command.Parameters.AddWithValue("@name", row[3]);
                            command.Parameters.AddWithValue("@date_birth", dateBirth);
                            command.Parameters.AddWithValue("@education", row[5]);
                            command.Parameters.AddWithValue("@profession", row[6]);
                            command.Parameters.AddWithValue("@date_registration", dateReg);
                            command.Parameters.AddWithValue("@login", row[8]);
                            command.Parameters.AddWithValue("@pass", row[9]);
                            command.Parameters.AddWithValue("@snils", textBox1.Text.Trim());

                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0) { MessageBox.Show("Done!"); }
                            // rowsAffected содержит количество добавленных строк (должно быть 1, если вставка прошла успешно)
                        }

                    }
                    string sqlEntr = "INSERT INTO entries (id_emp, date, entry, document) VALUES (@id_emp, @date, @entry, @document);";

                    if (sender == toolStripMenuItem5)
                    sqlEntr = "INSERT INTO awards (id_emp, date, entry, document) VALUES (@id_emp, @date, @entry, @document);";

                    using (NpgsqlCommand command = new NpgsqlCommand(sqlEntr, connection))
                    {
                        command.Parameters.AddWithValue("@id_emp", GetEmployeeIdBySnils(connection, textBox1.Text.Trim()));
                        command.Parameters.AddWithValue("@date", dateValue);
                        command.Parameters.AddWithValue("@entry", row[1]);
                        command.Parameters.AddWithValue("@document", row[2]);

                        int rowsAffected = command.ExecuteNonQuery();
                        //if (rowsAffected > 0) { MessageBox.Show("Done!"); }
                    }
                }
            }

            row = null;
            return;
        }

        private string GetTkNumber(NpgsqlConnection connection)
        { 
            string sql = $"select tk_number from empinfo order by id desc limit 1;";
            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
            {
                object result = cmd.ExecuteScalar();
                string number = "1111111"; // В БД нет ни одного сотрудника
                if (result != null) { number = (int.Parse(result.ToString()) + 1).ToString("D7"); }

                return number;
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            inactivityTimer.Stop();

            if (!IsSnilsValid(textBox1.Text.Trim())) MessageBox.Show("СНИЛС сотрудника введен неверно", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (!IsUserExist(textBox1.Text.Trim()))
            {
                Form4 form4 = new Form4(username, logger);
                form4.Owner = this;
                form4.ShowDialog();

                InsertIntoDB(sender);
            }
            else if (FirstWordFired(textBox1.Text.Trim()))
            {
                Form5 form5 = new Form5("toolStripMenuItem2");
                form5.Owner = this;
                form5.ShowDialog();

                InsertIntoDB(sender);
            }
            else MessageBox.Show("Данный сотрудник уже числится в базе данных", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            
            inactivityTimer.Start();
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
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionStr))
            {
                connection.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT id FROM empinfo WHERE snils = @snils", connection))
                {
                    cmd.Parameters.AddWithValue("snils", SNILS);
                    var result = cmd.ExecuteScalar();
                    if (Convert.ToInt32(result) > 0) { return  true; }
                    else { return false; }
                }
            }
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            inactivityTimer.Stop();

            if (!IsSnilsValid(textBox1.Text.Trim()))
            {
                MessageBox.Show("СНИЛС сотрудника введен неверно", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            toolStripMenuItem3.Tag = "toolStripMenuItem3";
            if (!IsUserExist(textBox1.Text.Trim())) MessageBox.Show("Сотрудника с таким СНИЛС не найдено", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (!FirstWordFired(textBox1.Text.Trim()))
            {
                Form5 form5 = new Form5("toolStripMenuItem3");
                form5.Owner = this;
                form5.ShowDialog();

                if (row == null) return;

                InsertIntoDB(sender);

                row = null;
            }
            else MessageBox.Show("Данный сотрудник не числится в базе", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            inactivityTimer.Start();
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            inactivityTimer.Stop();

            if (!IsSnilsValid(textBox1.Text.Trim()))
            {
                MessageBox.Show("СНИЛС сотрудника введен неверно", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (IsUserExist(textBox1.Text.Trim()))
            {
                toolStripButton1.Enabled = true;
                toolStripButton2.Enabled = false;
            }
            else
            {
                toolStripButton1.Enabled = false;
                toolStripButton2.Enabled = true;
            }

            if (!IsUserExist(textBox1.Text.Trim())) MessageBox.Show("Сотрудника с таким СНИЛС не найдено", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (!FirstWordFired(textBox1.Text.Trim()))
            {
                Form5 form5 = new Form5("toolStripMenuItem5");
                form5.Owner = this;
                form5.ShowDialog();

                if (row == null) return;

                InsertIntoDB(sender);

                row = null;
            }
            else MessageBox.Show("Данный сотрудник не числится в базе", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

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

            if (!IsSnilsValid(textBox1.Text.Trim()))
            {
                MessageBox.Show("СНИЛС сотрудника введен неверно", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsUserExist(textBox1.Text.Trim())) MessageBox.Show("Сотрудника с таким СНИЛС не найдено", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (!FirstWordFired(textBox1.Text.Trim()))
            {
                Form5 form5 = new Form5("toolStripMenuItem4");
                form5.Owner = this;
                form5.ShowDialog();

                if (row == null) return;

                InsertIntoDB(sender);

                row = null;
            }
            else MessageBox.Show("Данный сотрудник не числится в базе", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            inactivityTimer.Start();
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            inactivityTimer.Stop();

            if (!IsSnilsValid(textBox1.Text.Trim()))
            {
                MessageBox.Show("СНИЛС сотрудника введен неверно", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsUserExist(textBox1.Text.Trim())) MessageBox.Show("Сотрудника с таким СНИЛС не найдено", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (!FirstWordFired(textBox1.Text.Trim()))
            {
                Form5 form5 = new Form5("toolStripMenuItem6");
                form5.Owner = this;
                form5.ShowDialog();

                if (row == null) return;

                InsertIntoDB(sender);

                row = null;
            }
            else MessageBox.Show("Данный сотрудник не числится в базе", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            inactivityTimer.Start();
        }

        public bool FirstWordFired(string targetValue)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionStr))
            {
                connection.Open();
                int empId = GetEmployeeIdBySnils(connection, targetValue);
                string sql = $"select entry from entries where id_emp = {empId} order by id desc limit 1;";
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        string entry = result.ToString();
                        if (entry.StartsWith("Уволен", StringComparison.OrdinalIgnoreCase)) { return true; }
                    }
                }
                return false;
            }
        }

        private void _FormClosing(object sender, FormClosingEventArgs e)
        {
            inactivityTimer.Stop();
            WarningTimer.Stop();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }
    }
}
