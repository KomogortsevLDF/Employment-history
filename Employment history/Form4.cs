using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Employment_history
{
    public partial class Form4 : Form
    {
        private Timer inactivityTimer;
        public Form4()
        {
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

        // Метод для сброса таймера при активности пользователя
        private void ResetInactivityTimer()
        {
            inactivityTimer.Stop();
            inactivityTimer.Start();
        }

        private string DataFileName = "data.xml";
        private void Form4_Load(object sender, EventArgs e)
        {
            textBox3.Text = DateTime.Now.ToString().Substring(0, DateTime.Now.ToString().Length - 8);
            textBox4.Text = "Принят в административный отдел на должность секретаря";
            textBox5.Text = "Приказ от 13.09.2023 №4 - k";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 form2 = this.Owner as Form2;
            // Создаем массив строк со значениями из TextBox
            string[] row = new string[] { textBox1.Text, textBox2.Text, "", "", textBox3.Text, textBox4.Text, textBox5.Text };
            form2.row = row;

            this.Close();
        }

        private void _FormClosing(object sender, FormClosingEventArgs e)
        {
            inactivityTimer.Stop();
        }
    }
}
