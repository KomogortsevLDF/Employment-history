using Npgsql;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Employment_history
{
    public partial class Form4 : Form
    {
        private Timer inactivityTimer;
        private Timer WarningTimer;
        private string connectionStr = "Host=localhost;Username=postgres;Password=triPonnA5;Database=employeedb";
        private SecurityManager securityManager;
        private Logger logger;
        private string username;

        public Form4(string _username, Logger _logger)
        {
            logger = _logger;
            username = _username;

            InitializeComponent();
            this.FormClosing += _FormClosing;

            securityManager = new SecurityManager();

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

            if (char.IsLetter(e.KeyChar)) { e.Handled = true; }
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
            logger.LogEvent(username, "Inactivity", "Form closed due to inactivity");
        }

        private void WarningTimer_Tick(object sender, EventArgs e)
        {
            MessageBox.Show($"Обнаружено бездействие\n" +
                $"Пользователь будет разлогирован через {WarningTimer.Interval / 1000} сек.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            logger.LogEvent(username, "InactivityWarning", "Показано предупреждение о бездействии");
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
            textBox5.Text = DateTime.Now.ToString("dd.MM.yyyy");
            textBox9.Text = DateTime.Now.ToString("dd.MM.yyyy");
            textBox10.Text = "Принят в административный отдел на должность секретаря";
            textBox11.Text = "Приказ от 13.09.2023 №4 - k";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Создаем массив строк со значениями из TextBox
            string[] row = new string[]
            {
                textBox9.Text.Trim(),
                textBox10.Text.Trim(),
                textBox11.Text.Trim(),
                textBox1.Text.Trim(),
                textBox2.Text.Trim(),
                textBox3.Text.Trim(),
                textBox4.Text.Trim(),
                textBox5.Text.Trim(),
                textBox6.Text.Trim(),
                textBox7.Text.Trim()
            };

            if (row.Any(field => string.IsNullOrEmpty(field)))
            {
                MessageBox.Show("Некоторые поля не заполнены", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                logger.LogEvent(username, "RegistrationError", "Некоторые поля не заполнены");
                return;
            }

            DateTime dateBirth, dateReg, dateValue;
            try
            {
                dateValue = DateTime.ParseExact(row[4], "dd.MM.yyyy", null);
                dateBirth = DateTime.ParseExact(row[7], "dd.MM.yyyy", null);
                dateReg = DateTime.ParseExact(row[0], "dd.MM.yyyy", null);
            }
            catch
            {
                MessageBox.Show("Формат даты введен неверно\nОжидаемый формат: dd.MM.yyyy", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                logger.LogEvent(username, "RegistrationError", "Неверный формат даты");
                return;
            }

            if (!securityManager.ValidateLogin(textBox6.Text.Trim()))
            {
                MessageBox.Show("Логин должен содержать не менее 8 символов!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                logger.LogEvent(username, "RegistrationError", "Логин не соответствует требованиям");
                return;
            }

            if (!securityManager.ValidatePassword(textBox7.Text.Trim()))
            {
                MessageBox.Show("Пароль должен содержать не менее 8 символов, включать заглавные и строчные буквы, цифры и специальные символы!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                logger.LogEvent(username, "RegistrationError", "Пароль не соответствует требованиям");
                return;
            }

            if (textBox7.Text.Trim() != textBox8.Text.Trim())
            {
                MessageBox.Show("Пароли не совпадают!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                logger.LogEvent(username, "RegistrationError", "Пароли не совпадают");
                return;
            }

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionStr))
            {
                connection.Open();
                bool isFindL = false;
                bool isFindP = false;
                string sql = "SELECT login, pass FROM empinfo;";

                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string valueL = reader["login"].ToString();
                            if (valueL == textBox6.Text.Trim())
                            {
                                isFindL = true;
                                break;
                            }
                            string valueP = reader["pass"].ToString();
                            if (valueP == textBox7.Text.Trim())
                            {
                                isFindP = true;
                                break;
                            }
                        }
                    }
                }

                if (isFindL)
                {
                    MessageBox.Show("Сотрудник с таким логином уже существует!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    logger.LogEvent(username, "RegistrationError", "Логин уже существует");
                    return;
                }
                else if (isFindP)
                {
                    MessageBox.Show("Сотрудник с таким паролем уже существует!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    logger.LogEvent(username, "RegistrationError", "Пароль уже существует");
                    return;
                }
            }

            Form2 form2 = this.Owner as Form2;
            form2.row = row;
            logger.LogEvent(username, "RegistationSuccess", "Пользователь успешно зарегистрирован");
            this.Close();
        }

        private void _FormClosing(object sender, FormClosingEventArgs e)
        {
            inactivityTimer.Stop();
            WarningTimer.Stop();
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // Получаем текст из текстового поля
            string dateInput = textBox2.Text;

            // Оставляем только цифры
            string formattedDate = string.Join("", dateInput.Where(char.IsDigit));

            // Ограничиваем длину строки до 8 цифр
            if (formattedDate.Length > 8)
            {
                formattedDate = formattedDate.Substring(0, 8);
            }

            // Форматируем строку в формат даты
            string formattedWithDots = string.Empty;
            if (formattedDate.Length >= 5)
            {
                formattedWithDots = $"{formattedDate.Substring(0, 2)}.{formattedDate.Substring(2, 2)}.{formattedDate.Substring(4)}";
            }
            else if (formattedDate.Length >= 3)
            {
                formattedWithDots = $"{formattedDate.Substring(0, 2)}.{formattedDate.Substring(2)}";
            }
            else if (formattedDate.Length >= 1)
            {
                formattedWithDots = formattedDate.Substring(0, formattedDate.Length);
            }

            // Установите флаг, чтобы избежать рекурсивного вызова TextChanged
            textBox2.TextChanged -= textBox2_TextChanged;

            // Обновляем текст в текстовом поле
            textBox2.Text = formattedWithDots;

            // Перемещаем курсор в конец текста
            textBox2.SelectionStart = textBox2.Text.Length;

            // Включаем обработчик обратно
            textBox2.TextChanged += textBox2_TextChanged;
        }
    }
}
