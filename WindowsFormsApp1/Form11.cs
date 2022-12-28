using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace WindowsFormsApp1
{
    public partial class Form11 : Form
    {
        private sqliteclass mydb = null;    //Класс
        private string sSql = string.Empty; //Запрос
        int id;                             //ID текущей записи
        public Form11()
        {
            InitializeComponent();
            Text = Form10.operName;


            if (Text == "Маршрут")
            {
                string commandText = "select id as 'ID', pathnumber as 'Номер маршрута',pathfrom as 'Место отправления'," +
                        "pathto as 'Место назначения',pathtime as 'Время в пути' from path";//photo
                SQLiteConnection conn = new SQLiteConnection(@"Data Source=" + db_connect.path + ";New=True;Version=3");
                SQLiteCommand cmd = new SQLiteCommand(commandText, conn);
                conn.Open();
                try
                {
                    SQLiteDataReader sqlReader = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dataGridView1.AllowUserToAddRows = false;//Убираем пустую строку
                    dt.Load(sqlReader);
                    dataGridView1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Не удалось выбрать данные из таблицы\nОшибка: " + ex);
                    return;
                }
                conn.Close();
            }

            if (Text == "Автобус")
            {
                string commandText = "select id as 'ID', busnumber as 'Номер автобуса',bustype as 'Тип автобуса'," +
                        "places as 'Количество мест' from bus";//bus
                SQLiteConnection conn = new SQLiteConnection(@"Data Source=" + db_connect.path + ";New=True;Version=3");
                SQLiteCommand cmd = new SQLiteCommand(commandText, conn);
                conn.Open();
                try
                {
                    SQLiteDataReader sqlReader = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dataGridView1.AllowUserToAddRows = false;//Убираем пустую строку
                    dt.Load(sqlReader);
                    dataGridView1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Не удалось выбрать данные из таблицы\nОшибка: " + ex);
                    return;
                }
                conn.Close();
            }

            if (Text == "Водитель1")
            {
                string commandText = "select id as 'ID', fam as 'Фамилия',name as 'Имя',otchestvo as 'Отчество',yearofbird as 'Год рождения'," +
                        "phonenumber as 'Номер телефона',medecinecard as 'Номер мед.карты' from driver where id not like '" + Form10.driver2 + "'";//bus
                SQLiteConnection conn = new SQLiteConnection(@"Data Source=" + db_connect.path + ";New=True;Version=3");
                SQLiteCommand cmd = new SQLiteCommand(commandText, conn);
                conn.Open();
                try
                {
                    SQLiteDataReader sqlReader = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dataGridView1.AllowUserToAddRows = false;//Убираем пустую строку
                    dt.Load(sqlReader);
                    dataGridView1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Не удалось выбрать данные из таблицы\nОшибка: " + ex);
                    return;
                }
                conn.Close();
            }

            if (Text == "Водитель2")
            {
                string commandText = "select id as 'ID', fam as 'Фамилия',name as 'Имя',otchestvo as 'Отчество',yearofbird as 'Год рождения'," +
                        "phonenumber as 'Номер телефона',medecinecard as 'Номер мед.карты' from driver where id not like '" + Form10.driver1 + "'";//bus
                SQLiteConnection conn = new SQLiteConnection(@"Data Source=" + db_connect.path + ";New=True;Version=3");
                SQLiteCommand cmd = new SQLiteCommand(commandText, conn);
                conn.Open();
                try
                {
                    SQLiteDataReader sqlReader = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dataGridView1.AllowUserToAddRows = false;//Убираем пустую строку
                    dt.Load(sqlReader);
                    dataGridView1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Не удалось выбрать данные из таблицы\nОшибка: " + ex);
                    return;
                }
                conn.Close();
            }



        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (Text == "Маршрут")
            {
                Form10.buffOne = Convert.ToString(dataGridView1[1, dataGridView1.CurrentRow.Index].Value);//Номер маршрута
                Form10.buffTwo = Convert.ToString(dataGridView1[2, dataGridView1.CurrentRow.Index].Value);//Откуда
                Form10.buffThree = Convert.ToString(dataGridView1[3, dataGridView1.CurrentRow.Index].Value);//Куда

            }
            if (Text == "Автобус")
            {
                Form10.buffOne = Convert.ToString(dataGridView1[1, dataGridView1.CurrentRow.Index].Value);//Номер маршрута
                Form10.buffTwo = Convert.ToString(dataGridView1[3, dataGridView1.CurrentRow.Index].Value);//Откуда
            }
            if (Text == "Водитель1")
            {
                Form10.buffOne = Convert.ToString(dataGridView1[0, dataGridView1.CurrentRow.Index].Value);//ID 1водителя
            }
            if (Text == "Водитель2")
            {
                Form10.buffOne = Convert.ToString(dataGridView1[0, dataGridView1.CurrentRow.Index].Value);//ID 2водителя
            }
            Close();
        }
    }
}
