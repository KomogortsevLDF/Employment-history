using Npgsql;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Employment_history
{
    public partial class Form1 : Form
    {
        private System.Windows.Forms.Timer inactivityTimer;
        private string connectionStr = "Host=localhost;Username=postgres;Password=triPonnA5;Database=employeedb";
        private string filePath = "wmpi.dmp";
        private int maxAttempt = 3;
        private static int lockDurationSeconds = 300;

        private DateTime breakeTime = new DateTime(1952, 5, 11, 22, 51, 0);
        private static DateTime lockTime = new DateTime(1952, 5, 11, 22, 51, 0);
        int attempt = 0;
        string[] parts = { "1", "1952.05.11 22:51:00" };

        public string snils { get; set; }

        private Logger logger;

        public Form1()
        {
            InitializeComponent();
            button2.Visible = false;
            this.FormClosing += _FormClosing;

            logger = new Logger("eventlog.txt"); // Инициализация Logger с указанием пути к файлу логов

            // Инициализация таймера
            inactivityTimer = new System.Windows.Forms.Timer();
            inactivityTimer.Interval = 4000000;
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
            //logger.LogEvent(username, "Inactivity", "Form closed due to inactivity");
        }

        private void ResetInactivityTimer()
        {
            //inactivityTimer.Stop();
            //inactivityTimer.Start();
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
            if (IsUserBlocked())
            {
                UserLocked();
                return;
            }

            //if (textBox1.Text == "")
            //{
            //    textBox1.Text = "user1";
            //    textBox2.Text = "pass1";
            //    textBox3.Text = "123-456-789 01";
            //}

            string username = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();
            string SNILS = textBox3.Text;

            // Замена пробелов на тире
            SNILS = Regex.Replace(SNILS, @"^\d+\s\d+\s\d+(?=\s\d)", m => m.Value.Replace(' ', '-'));

            snils = SNILS;           

            if (AuthenticateUser(username, password, SNILS))
            {
                logger.LogEvent(username, "успешный вход", "Пользователь успешно вошел в систему.");
                this.Hide();
                Form3 form3 = new Form3(username, logger, snils);
                inactivityTimer.Stop();
                form3.ShowDialog();

                form3.Close();
                this.Show();
                inactivityTimer.Start();

                FileReset(true);
            }
            else
            {
                logger.LogEvent(username, "неуспешный вход", "Не удалось войти. Неправильный логин, пароль или СНИЛС.");
                MessageBox.Show("Неправильный логин, пароль или СНИЛС", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UserLocked();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (IsUserBlocked())
            {
                UserLocked();
                return;
            }

            //textBox1.Text = "admin1";
            //textBox2.Text = "password1";

            string username = textBox1.Text;
            string password = textBox2.Text;

            if (AuthenticateAccounter(username, password))
            {
                logger.LogEvent(username, "успешный вход", "Кадровик успешно вошел в систему.");
                this.Hide();
                Form2 form2 = new Form2(username, logger);
                inactivityTimer.Stop();
                form2.ShowDialog();
                form2.Close();
                inactivityTimer.Start();
                this.Show();

                FileReset(true);
            }
            else
            {
                logger.LogEvent(username, "неуспешный вход", "Не удалось войти. Неправильный логин или пароль.");
                MessageBox.Show("Неправильный логин или пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UserLocked();
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

            string snils = textBox3.Text;
            string formattedSnils = string.Join("", snils.Where(char.IsDigit));

            if (formattedSnils.Length > 14)
            {
                formattedSnils = formattedSnils.Substring(0, 14);
            }

            string formattedWithSpaces = string.Empty;
            for (int i = 0; i < formattedSnils.Length; i++)
            {
                if (i > 0 && i % 3 == 0 && i < 11)
                {
                    formattedWithSpaces += " ";
                }
                formattedWithSpaces += formattedSnils[i];
            }

            textBox3.Text = formattedWithSpaces;

            textBox3.SelectionStart = textBox3.Text.Length;

           // textBox3.TextChanged += textBox3_TextChanged; // Восстанавливаем обработчик

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            textBox4.Text = "Вы входите как сотрудник";
            FileReset(false);
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

        private void UserLocked()
        {
            FileRead();

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                if (lockTime == breakeTime) // Первая неправильная попытка входа
                {
                    writer.WriteLine("1 " + DateTime.Now.ToString());
                }
                else if (attempt >= maxAttempt)
                {
                    if ((DateTime.Now - lockTime).TotalSeconds < lockDurationSeconds) //отсчет времени уже идет
                    {
                        TimeSpan timeSpan = TimeSpan.FromSeconds(lockDurationSeconds - (int)(DateTime.Now - lockTime).TotalSeconds);

                        MessageBox.Show($"Превышено число попыток входа\n" +
                            $"Оставшееся время ожидания {(timeSpan).ToString(@"mm\:ss")}", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        writer.WriteLine((Convert.ToInt32(parts[0])).ToString() + " " + lockTime.ToString());
                    }
                    else
                    {
                        writer.WriteLine($"1 " + DateTime.Now.ToString());
                    }
                }
                else
                {
                    writer.WriteLine((Convert.ToInt32(parts[0]) + 1).ToString() + " " + lockTime.ToString());
                }

            }
        }

        private bool IsUserBlocked()
        {
            FileRead();
            if (attempt >= maxAttempt) { return true; }

            return false;

        }

        private void FileReset(bool IsattemptSuccess)
        {
            if (!File.Exists(filePath) || IsattemptSuccess)
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    string line = "1 " + breakeTime.ToString();
                    writer.WriteLine(line);
                }
            }

            FileRead();
        }

        private void FileRead()
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line = reader.ReadLine();
                parts = line.Split(new char[] { ' ' }, 2);
                attempt = int.Parse(parts[0]); // Преобразуем первый элемент в int
                lockTime = DateTime.Parse(parts[1]);
            }
        }

        void _FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();

        }







        private void cpypt()
        {
            string originalText = "Ы данные";

            byte[] key = Encoding.UTF8.GetBytes("nDwBKdttJMj9ewON");
            byte[] iv = Encoding.UTF8.GetBytes("7XP0dU4IxhqQb157");

            // Генерируем ключ и вектор инициализации

            // Шифруем данные
            byte[] encryptedBytes = EncryptStringToBytes(originalText, key, iv);

            // Сохраняем зашифрованные данные в файл
            File.WriteAllBytes("encryptedFile.dat", encryptedBytes);

            Console.WriteLine("Файл успешно зашифрован \n");

            // Дешифруем данные из файла
            string decryptedText = DecryptBytesToString(File.ReadAllBytes("encryptedFile.dat"), key, iv);

            Console.WriteLine("Расшифрованный текст: " + decryptedText + "\n");
        }

        static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        return msEncrypt.ToArray();
                    }
                }
            }
        }

        static string DecryptBytesToString(byte[] cipherText, byte[] key, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
