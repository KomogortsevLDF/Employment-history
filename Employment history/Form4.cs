using Npgsql;
using System;
using System.Windows.Forms;

namespace Employment_history
{
    public partial class Form4 : Form
    {
        private Timer inactivityTimer;
        private Timer WarningTimer;
        private string connectionStr = "Host=localhost;Username=postgres;Password=triPonnA5;Database=employeedb";

        public Form4()
        {
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
            textBox1.MouseMove += _MouseMove;
            textBox1.KeyPress += _KeyPress;
            textBox2.MouseMove += _MouseMove;
            textBox2.KeyPress += _KeyPress;
            textBox3.MouseMove += _MouseMove;
            textBox3.KeyPress += _KeyPress;
            textBox4.MouseMove += _MouseMove;
            textBox4.KeyPress += _KeyPress;
            textBox5.MouseMove += _MouseMove;
            textBox5.KeyPress += _KeyPress;
            label1.MouseMove += _MouseMove;
            label1.KeyPress += _KeyPress;
            label2.MouseMove += _MouseMove;
            label2.KeyPress += _KeyPress;
            label3.MouseMove += _MouseMove;
            label3.KeyPress += _KeyPress;
            label4.MouseMove += _MouseMove;
            label4.KeyPress += _KeyPress;
            label5.MouseMove += _MouseMove;
            label5.KeyPress += _KeyPress;
            button1.MouseMove += _MouseMove;
            button1.KeyPress += _KeyPress;

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
            Form2 form2 = this.Owner as Form2;
            form2.Close();
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

        private void Form4_Load(object sender, EventArgs e)
        {
            textBox5.Text = DateTime.Now.ToString().Substring(0, DateTime.Now.ToString().Length - 8).Trim();
            textBox9.Text = DateTime.Now.ToString().Substring(0, DateTime.Now.ToString().Length - 8).Trim();
            textBox10.Text = "Принят в административный отдел на должность секретаря";
            textBox11.Text = "Приказ от 13.09.2023 №4 - k";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Создаем массив строк со значениями из TextBox
            string[] row = new string[] { textBox9.Text.Trim(), textBox10.Text.Trim(), textBox11.Text.Trim(), textBox1.Text.Trim(),
                            textBox2.Text.Trim(), textBox3.Text.Trim(), textBox4.Text.Trim(), textBox5.Text.Trim(), textBox6.Text.Trim()
                            , textBox7.Text.Trim()};

            if (row[0] == "" || row[1] == "" || row[2] == "" || row[3] == "" || row[4] == "" || row[5] == "" || row[6] == ""
                 || row[7] == "" || row[8] == "" || row[9] == "")
            {
                MessageBox.Show("Некоторые поля не заполнены", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime dateBirth = new DateTime(2024, 5, 25),
                     dateReg = new DateTime(2024, 5, 25),
                     dateValue = new DateTime(2024, 5, 25);
            try
            {
                dateValue = DateTime.ParseExact(row[4], "dd.MM.yyyy", null);
                dateBirth = DateTime.ParseExact(row[7], "dd.MM.yyyy", null);
                dateReg = DateTime.ParseExact(row[0], "dd.MM.yyyy", null);
            }
            catch
            {
                MessageBox.Show("Формат даты введен неверно\nОжидаемый формат: dd.MM.yyyy", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (textBox7.Text.Trim() != textBox8.Text.Trim())
            {
                MessageBox.Show("Пароли не совпадают!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionStr))
            {
                connection.Open();
                bool isFindL = false;
                bool isFindP = false;
                string sql = $"SELECT login, pass FROM empinfo ;";

                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string valueL = reader["login"].ToString(); // Получаем значение из первого столбца текущей строки
                            if (valueL == textBox6.Text.Trim())
                            {
                                isFindL = true; // Если найдено совпадение, устанавливаем флаг в false
                                break; // Прерываем цикл, так как совпадение найдено
                            }
                            string valueP = reader["pass"].ToString(); // Получаем значение из первого столбца текущей строки
                            if (valueP == textBox7.Text.Trim())
                            {
                                isFindP = true; // Если найдено совпадение, устанавливаем флаг в false
                                break; // Прерываем цикл, так как совпадение найдено
                            }
                        }
                    }
                }

                if (isFindL)
                {
                    MessageBox.Show("Сотрудник с таким логином уже существует!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (isFindP)
                {
                    MessageBox.Show("Сотрудник с таким паролем уже существует!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            Form2 form2 = this.Owner as Form2;
            form2.row = row;

            this.Close();
        }

        private void _FormClosing(object sender, FormClosingEventArgs e)
        {
            inactivityTimer.Stop();
            WarningTimer.Stop();
        }
    }
}
