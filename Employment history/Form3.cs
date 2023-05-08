﻿using System;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
            toolStripButton1.Enabled = false;
            //bindingNavigator1.Enabled = true;
            // Создаем объект XmlDocument и загружаем в него XML-файл
            xmlDoc = new XmlDocument();
            xmlDoc.Load("data.xml");

            // Выбираем все узлы "Employee"
            XmlNodeList employeeNodes = xmlDoc.SelectNodes("//Employee");

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
            dataGridView1.Columns["User"].Visible = false;
            dataGridView1.Columns["Pass"].Visible = false;
            dataGridView1.ReadOnly = true;

            dataGridView1.Columns["Number"].Width = 50;
            dataGridView1.Columns["Date"].Width = 75;
            dataGridView1.Columns["Number"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridView1.Columns["Date"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
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
                    int columnWidth = 50;

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

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            toolStripButton1.Enabled = false;
            toolStripButton2.Enabled = true;

            DataView dataView = new DataView(dataTable);
            dataView.RowFilter = $"SNILS = '{snils}'";

            dataGridView1.DataSource = dataView;
            dataGridView1.Columns["SNILS"].Visible = false;
            dataGridView1.Columns["User"].Visible = false;
            dataGridView1.Columns["Pass"].Visible = false;
            dataGridView1.ReadOnly = true;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            toolStripButton1.Enabled = true;
            toolStripButton2.Enabled = false;

            XmlDocument xmlAwards = new XmlDocument();
            DataTable TableAwards = new DataTable("Employee");
            DataSet AwardsSet = new DataSet("Awards");
            xmlAwards.Load("awards.xml");

            XmlNodeList employeeNodes = xmlAwards.SelectNodes("//Employee");

            TableAwards.Columns.Add("User");
            TableAwards.Columns.Add("Pass");
            TableAwards.Columns.Add("SNILS");
            TableAwards.Columns.Add("Number");
            TableAwards.Columns.Add("Date");
            TableAwards.Columns.Add("Entries");
            TableAwards.Columns.Add("Document");

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

                TableAwards.Rows.Add(user, pass, snils, number, date, entries, document);
            }

            AwardsSet.Tables.Add(TableAwards);

            DataView dataView = new DataView(TableAwards);
            dataView.RowFilter = $"SNILS = '{snils}'";

            // Назначаем DataTable источником данных для DataGridView
            dataGridView1.DataSource = dataView;
            dataGridView1.Columns["SNILS"].Visible = false;
            dataGridView1.Columns["User"].Visible = false;
            dataGridView1.Columns["Pass"].Visible = false;
            dataGridView1.ReadOnly = true;

            //if (!IsUserExist(textBox1.Text))
            //{
            //    MessageBox.Show("СНИЛС сотрудника введен неверно (Данный сотрудник не числится в базе ПФР)", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}
        }
    }
}
