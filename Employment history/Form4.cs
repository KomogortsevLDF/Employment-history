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
        public Form4()
        {
            InitializeComponent();
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
    }
}
