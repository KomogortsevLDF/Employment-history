using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;

//< Awards > Почетная грамота за помощь в подготовке к проекту "Инновация"</Awards>
namespace Employment_history
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            button2.Visible = false;
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

            string username = textBox1.Text;
            string password = textBox2.Text;
            string SNILS = textBox3.Text;

            username = "user1";
            password = "pass1";
            SNILS = "111-111-111 11";

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
                form3.ShowDialog();

                form3.Close();
                this.Show();
            }
            else
            {
                MessageBox.Show("Неправильный логин, пароль или СНИЛС!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsLoginValid(string username, string password, string SNILS)
        {
            // Читаем данные из файла и проверяем, существует ли пользователь с таким логином и паролем
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(DataFileName);
            XmlNodeList employeeNodes = xmlDoc.SelectNodes("//Employee");
            
            foreach (XmlNode employeeNode in employeeNodes)
            {
                if (employeeNode.SelectSingleNode("Login/User").InnerText == username &&
                    employeeNode.SelectSingleNode("Login/Pass").InnerText == password &&
                    employeeNode.SelectSingleNode("Login/SNILS").InnerText == SNILS) 
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
        //private void button3_Click(object sender, EventArgs e)
        //{
        //    string username = textBox1.Text;
        //    string password = textBox2.Text;
        //    string INN = textBox3.Text;
        //    string SNILS = textBox4.Text;

        //    // Проверяем, правильно ли введен логин и пароль
        //    if (!IsInputValid(username, password, INN, SNILS))
        //    {
        //        MessageBox.Show("Логин и пароль не могут быть пустыми!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return;
        //    }

        //    // Проверяем, существует ли файл с данными пользователей
        //    if (!File.Exists(filePath_user))
        //    {
        //        // Создаем файл с данными пользователей, если он не существует
        //        using (StreamWriter sw = new StreamWriter(filePath_user))
        //        {
        //            sw.WriteLine($"{username}:{password}");
        //        }
        //    }
        //    else
        //    {
        //        // Проверяем, существует ли пользователь с таким логином
        //        if (IsUserExists(username))
        //        {
        //            MessageBox.Show("Пользователь с таким логином уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            return;
        //        }

        //        // Добавляем нового пользователя в файл с данными пользователей
        //        using (StreamWriter sw = File.AppendText(filePath_user))
        //        {
        //            sw.WriteLine($"{username}:{password}:{INN}:{SNILS}");
        //        }
        //    }

        //    MessageBox.Show("Вы успешно зарегистрировались!", "Регистрация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //}

        //private bool IsUserExists(string username)
        //{
        //    // Читаем данные из файла и проверяем, существует ли пользователь с
        //    // таким логином
        //    using (StreamReader sr = new StreamReader(filePath_user))
        //    {
        //        string line;
        //        while ((line = sr.ReadLine()) != null)
        //        {
        //            string[] parts = line.Split(':');
        //            if (parts[0] == username)
        //            {
        //                return true;
        //            }
        //        }
        //    }

        //    return false;
        //}

        //private bool IsInputValid(string username, string password, string INN, string SNILS)
        //{
        //    return !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password)
        //        && !string.IsNullOrEmpty(INN) && !string.IsNullOrEmpty(SNILS);
        //}

        private void button2_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            string password = textBox2.Text;

            username = "acc1";
            password = "pass1";
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
                form2.ShowDialog();
                form2.Close();
                this.Show();
            }
            else
            {
                MessageBox.Show("Неправильный логин или пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
