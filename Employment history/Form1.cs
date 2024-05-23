using Npgsql;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace Employment_history
{
    public partial class Form1 : Form
    {
        private Timer inactivityTimer;
        private string connectionStr = "Host=localhost;Username=postgres;Password=triPonnA5;Database=employeedb";

        public string snils { get; set; }

        public Form1()
        {
            InitializeComponent();
            //button2.Visible = false;

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

            textBox1.Validating += _Validating;
            textBox2.Validating += _Validating;
            textBox3.Validating += _Validating;

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

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            button2.Visible = false;
            button1.Visible = true;
            textBox3.Visible = true;
            label2.Visible = true;
            textBox4.Text = "Вы входите как сотрудник";
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            button2.Visible = true;
            button1.Visible = false;
            textBox3.Visible = false;
            label2.Visible = false;
            textBox4.Text = "Вы входите как кадровик";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "user1";
            textBox2.Text = "pass1";
            textBox3.Text = "123-456-789 01";

            string username = textBox1.Text;
            string password = textBox2.Text;
            string SNILS = textBox3.Text;

            snils = SNILS;

            if (AuthenticateUser(username, password, SNILS))
            {
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

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "admin1";
            textBox2.Text = "password1";
            string username = textBox1.Text;
            string password = textBox2.Text;

            if (AuthenticateAccounter(username, password))
            {
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
                MessageBox.Show("Неправильный логин или пароль!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox4.Text = "Вы входите как сотрудник";
        }

        private bool AuthenticateUser(string login, string password, string snils)
        {
            using (var conn = new NpgsqlConnection(connectionStr))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM empinfo WHERE login = @login AND pass = @password AND snils = @snils", conn))
                {
                    cmd.Parameters.AddWithValue("login", login);
                    cmd.Parameters.AddWithValue("password", password);
                    cmd.Parameters.AddWithValue("snils", snils);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        private bool AuthenticateAccounter(string login, string password)
        {
            using (var conn = new NpgsqlConnection(connectionStr))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM admins WHERE username = @login AND password = @password", conn))
                {
                    cmd.Parameters.AddWithValue("login", login);
                    cmd.Parameters.AddWithValue("password", password);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }
    }
}
