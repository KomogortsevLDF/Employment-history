using System;
using System.Windows.Forms;

namespace Employment_history
{
    public partial class Form5 : Form
    {
        private Timer inactivityTimer;
        private Timer WarningTimer;
        public Form5(string sender)
        {
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
            textBox4.KeyPress += _KeyPress;
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
                return;
            }

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
