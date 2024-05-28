using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Npgsql;

namespace Employment_history
{
    public partial class Form3 : Form
    {
        private Timer inactivityTimer;
        private Timer WarningTimer;
        private string connectionStr = "Host=localhost;Username=postgres;Password=triPonnA5;Database=employeedb";
        DataTable dataTable = new DataTable();
        DataTable awardsTable = new DataTable();
        private string snils, username;
        Logger logger;


        public Form3(string _username, Logger _logger, string _Snils)
        {
            snils = _Snils;
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
            toolStripMenuItem1.MouseMove += _MouseMove;
            toolStripButton1.MouseMove += _MouseMove;
            toolStripButton2.MouseMove += _MouseMove;
            bindingNavigatorSeparator1.MouseMove += _MouseMove;
            bindingNavigatorSeparator2.MouseMove += _MouseMove;

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
                $"Пользователь будет разлогирован через {WarningTimer.Interval/1000} сек.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            
        }

        // Метод для сброса таймера при активности пользователя
        private void ResetInactivityTimer()
        {
            inactivityTimer.Stop();
            inactivityTimer.Start();

            WarningTimer.Stop();
            WarningTimer.Start();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            toolStripButton1.Enabled = false;
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            LoadData();
            NumberRows();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            inactivityTimer.Stop();

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
                        if (count == dataGridView1.RowCount) continue;

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
                    sb.AppendLine("Фамилия:                    " + textBox1.Text);
                    sb.AppendLine("Имя:                        " + textBox2.Text);
                    sb.AppendLine("Отчество:                   " + textBox3.Text);
                    sb.AppendLine("Дата рождения:              " + textBox4.Text);
                    sb.AppendLine("Образование:                " + textBox5.Text);
                    sb.AppendLine("Профессия, Специальность:   " + textBox6.Text);
                    sb.AppendLine("Дата заполнения:            " + textBox7.Text);

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
            }

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

        private void _FormClosing(object sender, FormClosingEventArgs e)
        {
            inactivityTimer.Stop();
            WarningTimer.Stop();
        }

        private void LoadData()
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionStr))
                {
                    connection.Open();

                    int empId = GetEmployeeIdBySnils(connection);

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

                        adapterEntries.Fill(dataTable);
                        adapterAwards.Fill(awardsTable);

                        DataGridViewTextBoxColumn idColumn = new DataGridViewTextBoxColumn();
                        idColumn.Name = "id";
                        dataGridView1.Columns.Add(idColumn);

                        dataGridView1.DataSource = dataTable;
                        dataGridView1.ReadOnly = true;
                        dataGridView1.RowHeadersVisible = false;

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
                                textBox1.Text = separatedName[0];
                                textBox2.Text = separatedName[1];
                                textBox3.Text = separatedName[2];
                                textBox4.Text = DateTime.Parse(reader["date_birth"].ToString()).ToString("dd.MM.yyyy");
                                textBox5.Text = reader["education"].ToString();
                                textBox6.Text = reader["profession"].ToString();
                                textBox7.Text = DateTime.Parse(reader["date_registration"].ToString()).ToString("dd.MM.yyyy");
                            }
                        }
                     
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }

        private void NumberRows()
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Cells["id"].Value = (i + 1).ToString();
            }
        }

        int GetEmployeeIdBySnils(NpgsqlConnection conn)
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

    }
}
