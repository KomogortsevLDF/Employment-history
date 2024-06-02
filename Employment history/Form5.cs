using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Employment_history
{
    public partial class Form5 : Form
    {
        private Timer inactivityTimer;
        private Timer WarningTimer;
        private Logger logger;
        private string username;

        SecurityManager securityManager = new SecurityManager();

        public Form5(string _username, Logger _logger, string sender)
        {
            logger = _logger;
            username = _username;
            Form5_Sender = sender;
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
            textBox3.MouseMove += _MouseMove;
            textBox3.KeyPress += _KeyPress;
            textBox4.MouseMove += _MouseMove;
            textBox4.KeyPress += textBox4_KeyPress;
            textBox5.MouseMove += _MouseMove;
            textBox5.KeyPress += _KeyPress;
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


        private string Form5_Sender;
        private void Form5_Load(object sender, EventArgs e)
        {
            textBox3.Text = DateTime.Now.ToString().Substring(0, DateTime.Now.ToString().Length - 8).Trim();
            
            if (Form5_Sender == "toolStripMenuItem2")
            {
                textBox4.Text = "Принят в административный отдел на должность секретаря";
                textBox5.Text = "Приказ от 13.09.2023 №4 - k";
            }
            else if (Form5_Sender == "toolStripMenuItem3")
            {
                textBox4.Text = "Переведен на должность маркетолога";
                textBox5.Text = "Приказ от 13.09.2001 №134 j";
            }
            else if (Form5_Sender == "toolStripMenuItem4")
            {
                textBox4.Text = "Принят на работу по совместительству на должность интернет-маркетолога";
                textBox5.Text = "Приказ от 13.09.2009 №5 - k";
            }
            else if (Form5_Sender == "toolStripMenuItem5")
            {
                textBox4.Text = "Почетная грамота за помощь в подготовке к проекту \"Инновация\"";
                textBox5.Text = "Приказ №02334 от 23.01.2015";
            }
            else if (Form5_Sender == "toolStripMenuItem6")
            {
                textBox4.Text = "Уволен по собственному желанию пункт 3 части 1 статьи 77 ТК РФ";
                textBox5.Text = "Приказ №16 от 23.01.2018";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 form2 = this.Owner as Form2;
            // Создаем массив строк со значениями из TextBox
            string[] row = new string[] { textBox3.Text.Trim(), textBox4.Text.Trim(), textBox5.Text.Trim() };

            DateTime dateValue = new DateTime(2024, 5, 25);
            try
            {
                dateValue = DateTime.ParseExact(row[0], "dd.MM.yyyy", null);
            }
            catch
            {
                MessageBox.Show("Формат даты введен неверно", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                logger.LogEvent(username, "InvalidDateFormat", "Формат даты введен неверно");
                return;
            }

            if(row.Any(field => string.IsNullOrEmpty(field)))
{
                MessageBox.Show("Некоторые поля не заполнены", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                logger.LogEvent(username, "RegistrationError", "Некоторые поля не заполнены");
                return;
            }   

            if (!securityManager.ValidateDate(textBox3.Text.Trim()))
            {
                MessageBox.Show("Введите корректную дату", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                logger.LogEvent(username, "RegistrationError", "Дата не соответсвтует требованиям");
                return;
            }

            form2.row = row;
            logger.LogEvent(username, "DataSaved", "Данные успешно сохранены");


            this.Close();
        }

        private void _FormClosing(object sender, FormClosingEventArgs e)
        {
            inactivityTimer.Stop();
            WarningTimer.Stop();
        }

        private void Form5_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            // Получаем текст из текстового поля
            string dateInput = textBox3.Text;

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
            textBox3.TextChanged -= textBox3_TextChanged;

            // Обновляем текст в текстовом поле
            textBox3.Text = formattedWithDots;

            // Перемещаем курсор в конец текста
            textBox3.SelectionStart = textBox3.Text.Length;

            // Включаем обработчик обратно
            textBox3.TextChanged += textBox3_TextChanged;
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            ResetInactivityTimer();

            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ' ') { e.Handled = true; }
        }
    }
}
