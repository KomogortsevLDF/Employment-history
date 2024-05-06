using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Employment_history
{
    public partial class Form5 : Form
    {
        private Timer inactivityTimer;
        public Form5(string sender)
        {
            Form5_Sender = sender;
            InitializeComponent();
            this.FormClosing += _FormClosing;

            // Инициализация таймера
            inactivityTimer = new Timer();
            inactivityTimer.Interval = 4000;
            inactivityTimer.Tick += InactivityTimer_Tick;
            inactivityTimer.Start();

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

        // Метод для сброса таймера при активности пользователя
        private void ResetInactivityTimer()
        {
            inactivityTimer.Stop();
            inactivityTimer.Start();
        }


        private string Form5_Sender;
        private void Form5_Load(object sender, EventArgs e)
        {
            textBox3.Text = DateTime.Now.ToString().Substring(0, DateTime.Now.ToString().Length - 8);
            
            if (Form5_Sender == "toolStripMenuItem2")
            {
                textBox4.Text = "Принят в административный отдел на должность секретаря";
                textBox5.Text = "Приказ от 13.09.2023 №4 - k";
            }
            if (Form5_Sender == "toolStripMenuItem3")
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
            string[] row = new string[] { "0", "0", "", "", textBox3.Text, textBox4.Text, textBox5.Text };
            form2.row = row;

            this.Close();
        }

        private void _FormClosing(object sender, FormClosingEventArgs e)
        {
            inactivityTimer.Stop();
        }
    }
}
