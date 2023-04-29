using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Employment_history
{
    public partial class Form3 : Form
    {
        public Form3(string Snils)
        {
            snils = Snils;
            InitializeComponent();
        }
        private string snils;
        XmlDocument xmlDoc;
        DataTable dataTable;

        private void Form3_Load(object sender, EventArgs e)
        {

            //bindingNavigator1.Enabled = true;
            // Создаем объект XmlDocument и загружаем в него XML-файл
            xmlDoc = new XmlDocument();
            xmlDoc.Load("data.xml");

            // Выбираем все узлы "Employee"
            XmlNodeList employeeNodes = xmlDoc.SelectNodes("//Table");

            // Создаем объект DataTable и добавляем в него необходимые столбцы
            dataTable = new DataTable();
            dataTable.Columns.Add("User");
            dataTable.Columns.Add("Pass");
            dataTable.Columns.Add("SNILS");
            dataTable.Columns.Add("Number");
            dataTable.Columns.Add("Date");
            dataTable.Columns.Add("Entries");
            dataTable.Columns.Add("Document");

            // Проходим по всем узлам "Employee" и добавляем данные в DataTable
            foreach (XmlNode employeeNode in employeeNodes)
            {
                string user = employeeNode.SelectSingleNode("User").InnerText;
                string pass = employeeNode.SelectSingleNode("Pass").InnerText;
                string snils = employeeNode.SelectSingleNode("SNILS").InnerText;
                string number = employeeNode.SelectSingleNode("Number").InnerText;
                string date = employeeNode.SelectSingleNode("Date").InnerText;
                string entries = employeeNode.SelectSingleNode("Entries").InnerText;
                string document = employeeNode.SelectSingleNode("Document").InnerText;

                dataTable.Rows.Add(user, pass, snils, number, date, entries, document);
            }

            bindingSource1.DataSource = dataTable;
            this.bindingNavigator1.BindingSource = bindingSource1;

            DataView dataView = new DataView(dataTable);
            dataView.RowFilter = $"SNILS = '{snils}'";

            // Назначаем DataTable источником данных для DataGridView
            dataGridView1.DataSource = dataView;
            dataGridView1.Columns["SNILS"].Visible = false;
            dataGridView1.ReadOnly = true;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Создание диалогового окна сохранения файла
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.Title = "Save file as";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Открытие потока для записи данных в файл
                using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                {
                    // Определяем ширину столбца для выравнивания
                    int columnWidth = 25;

                    // Запись заголовков столбцов
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        // Выравнивание по центру
                        string header = String.Format("{0," + columnWidth + "}", dataGridView1.Columns[i].HeaderText);
                        writer.Write(header);

                        if (i != dataGridView1.Columns.Count - 1)
                        {
                            writer.Write("\t");
                        }
                    }
                    writer.WriteLine();

                    // Запись данных
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        for (int i = 0; i < dataGridView1.Columns.Count; i++)
                        {
                            // Выравнивание по центру
                            string value = String.Format("{0," + columnWidth + "}", row.Cells[i].Value);
                            writer.Write(value);

                            if (i != dataGridView1.Columns.Count - 1)
                            {
                                writer.Write("\t");
                            }
                        }
                        writer.WriteLine();
                    }

                }
            }
        }

        private void bindingNavigatorMoveLastItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("HiHelloHi");
        }
    }
}
