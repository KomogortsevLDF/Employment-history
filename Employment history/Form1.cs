﻿using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace Employment_history
{
    public partial class Form1 : Form
    {
        private Timer inactivityTimer;
        public Form1()
        {
            InitializeComponent();
            button2.Visible = false;

            // Инициализация таймера
            inactivityTimer = new Timer();
            inactivityTimer.Interval = 4000;
            inactivityTimer.Tick += InactivityTimer_Tick;
            inactivityTimer.Start();

            // Добавление обработчиков событий для мыши
            this.MouseMove += _MouseMove;
            this.KeyPress += _KeyPress;
            this.MouseWheel += _MouseWheel;
            menuStrip1.MouseMove += _MouseMove;
            menuStrip1.KeyPress += _KeyPress;
            toolStripMenuItem1.MouseMove += _MouseMove;
            toolStripMenuItem2.MouseMove += _MouseMove;
            button1.MouseMove += _MouseMove;
            button1.KeyPress += _KeyPress;
            textBox1.MouseMove += _MouseMove;
            textBox1.KeyPress += _KeyPress;
            label1.MouseMove += _MouseMove;
            label1.KeyPress += _KeyPress;

            textBox1.Validating += _Validating;       // ??????
            textBox2.Validating += _Validating;       // ??????
            textBox3.Validating += _Validating;       // ??????

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

        private void _Validating(object sender, CancelEventArgs e)
        {
            ResetInactivityTimer();
        }


        private void InactivityTimer_Tick(object sender, EventArgs e)
        {
            //MessageBox.Show("Обнаружено бездействие", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ResetInactivityTimer()
        {
            inactivityTimer.Stop();
            inactivityTimer.Start();
        }

        public string snils { get; set; }

        private string DataFileName = "data.xml";
        //private string filePath_user = "users.txt";
        private string filePath_accountant = "accountants.txt";

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            button2.Visible = false;
            button1.Visible = true;
            textBox3.Visible = true;
            label2.Visible = true;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            button2.Visible = true;
            button1.Visible = false;
            textBox3.Visible = false;
            label2.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "user1";
            textBox2.Text = "pass1";
            textBox3.Text = "111-111-111 11";

            string username = textBox1.Text;
            string password = textBox2.Text;
            string SNILS = textBox3.Text;

            snils = SNILS;
            // Проверяем, существует ли файл с данными пользователей
            if (!File.Exists(DataFileName))
            {
                MessageBox.Show("Файл с данными не найден!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Проверяем, правильно ли введен логин и пароль для файла data.xml (т.е. для верификации пользователя)
            if (IsLoginValid(username, password, SNILS))
            {
                //MessageBox.Show("Вы успешно вошли в систему!", "Вход", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Hide();
                Form3 form3 = new Form3(snils);
                inactivityTimer.Stop();
                form3.ShowDialog();                

                form3.Close();
                this.Show();
                inactivityTimer.Start();
            }
            else
            {
                MessageBox.Show("Неправильный логин, пароль или СНИЛС!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsLoginValid(string username, string password, string SNILS)
        {
            // Читаем данные из файла и проверяем, существует ли пользователь с таким логином и паролем и снилсом
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(DataFileName);
            XmlNodeList employeeNodes = xmlDoc.SelectNodes("//Employee");
            
            foreach (XmlNode employeeNode in employeeNodes)
            {
                if (employeeNode.SelectSingleNode("User").InnerText == username &&
                    employeeNode.SelectSingleNode("Pass").InnerText == password &&
                    employeeNode.SelectSingleNode("SNILS").InnerText == SNILS) 
                { return true; }
            }

            return false;
        }

        private bool IsLoginValid(string username, string password)
        {
            using (StreamReader sr = new StreamReader(filePath_accountant))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split(':');
                    if (parts[0] == username && parts[1] == password)
                    {
                        return true;
                    }

                }
            }

            return false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "acc1";
            textBox2.Text = "pass1";
            string username = textBox1.Text;
            string password = textBox2.Text;

            // Проверяем, существует ли файл с данными работодателей
            if (!File.Exists(filePath_accountant))
            {
                MessageBox.Show("Файл с данными работодателей не найден!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Проверяем, правильно ли введен логин и пароль для файла accountants.txt (т.е. для верификации работодателя)
            if (IsLoginValid(username, password))
            {
                //MessageBox.Show("Вы успешно вошли в систему!", "Вход", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Hide();
                Form2 form2 = new Form2();
                inactivityTimer.Stop();
                form2.ShowDialog();
                form2.Close();
                inactivityTimer.Start();
                this.Show();
            }
            else
            {
                MessageBox.Show("Неправильный логин или пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
