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
    public partial class Form7 : Form
    {
        private sqliteclass mydb = null;    //Класс
        private string sSql = string.Empty; //Запрос
        int id;                             //ID текущей записи
        public Form7()
        {
            InitializeComponent();
            Text = Form2.operName;
            button1.Text = Form2.operName;
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;   //Запрет на ввод текста в комбоБокс1
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;   //Запрет на ввод текста в комбоБокс2
            KeyPreview = true;
            //Запуск события нажатой кнопки при нажатии Enter
            KeyDown += (s, e) => { if (e.KeyValue == (char)Keys.Enter) button1_Click(button1, null); };
            //Закрытие окна при нажатии Escape
            KeyDown += (s, e) => { if (e.KeyValue == (char)Keys.Escape) this.Close(); };

            if (Text == "Изменить")
            {
                string commandText = "select pathnumber,pathfrom,pathto,pathtime,id from path where pathnumber = '" + Form2.transit + "'";
                SQLiteConnection conn = new SQLiteConnection(@"Data Source=" + db_connect.path + ";New=True;Version=3");
                SQLiteCommand cmd = new SQLiteCommand(commandText, conn);
                conn.Open();

                SQLiteDataReader sqlReader = cmd.ExecuteReader();
                while (sqlReader.Read())
                {
                    numericUpDown1.Value = Convert.ToInt32(sqlReader.GetValue(0).ToString());//Номер маршрута
                    comboBox1.Text = sqlReader.GetValue(1).ToString();//Место направления
                    comboBox2.Text = sqlReader.GetValue(2).ToString();//Место прибытия
                    numericUpDown2.Value = Convert.ToInt32(sqlReader.GetValue(3).ToString());//Время в пути
                    id = Convert.ToInt32(sqlReader.GetValue(4).ToString());
                }

                conn.Close();
                numericUpDown1.ReadOnly = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Проверка на уникальное значение
            int count = 0;
            bool isExist = false;
            string commandText = "select pathnumber from path where pathnumber = '" + numericUpDown1.Value + "';";
            SQLiteConnection conn0 = new SQLiteConnection(@"Data Source=" + db_connect.path + ";New=True;Version=3");
            SQLiteCommand cmd0 = new SQLiteCommand(commandText, conn0);
            conn0.Open();

            SQLiteDataReader sqlReader = cmd0.ExecuteReader();
            while (sqlReader.Read())
            {
                count++;
            }

            conn0.Close();

            if (count > 0)
            {
                isExist = true;
            }
            else
                isExist = false;

            if (numericUpDown1.Value == 0 || comboBox1.Text == "" || comboBox2.Text == "" || numericUpDown2.Value == 0 || (isExist == true && Text != "Изменить"))
            {
                if (numericUpDown1.Value == 0) MessageBox.Show("Номер маршрута не должен быть равен 0", "Ошибка при заполнении");//Добавить проверку на уже существующую запись, при инициализации добавляем список всех существующих записей и сравниваем с существующей
                else if (comboBox1.Text == "") MessageBox.Show("Не выбрано место отправления!", "Ошибка при заполнении");
                else if (comboBox2.Text == "") MessageBox.Show("не выбрано место прибытия!", "Ошибка при заполнении");
                else if (numericUpDown1.Value == 0) MessageBox.Show("Время в пути не должно быть равно 0.", "Ошибка при заполнении");
            }
            else
            {
                if (Text != "Изменить")
                {
                    mydb = new sqliteclass();
                    sSql = @"insert into path (pathnumber,pathfrom,pathto,pathtime) values('" + numericUpDown1.Value + "','" + comboBox1.Text + "','" + comboBox2.Text + "','" + numericUpDown2.Value + "');";
                    mydb.iExecuteNonQuery(db_connect.path, sSql, 0);
                    mydb = null;
                    Close();
                }
                else
                {
                    mydb = new sqliteclass();
                    sSql = @"update path set (pathnumber,pathfrom,pathto,pathtime) = ('" + numericUpDown1.Value + "','" + comboBox1.Text + "','" + comboBox2.Text + "','" + numericUpDown2.Value + "') where pathnumber = '" + Form2.transit + "';";
                    //@"CREATE TABLE if not exists [ticket]([id] INTEGER PRIMARY KEY AUTOINCREMENT,[numberticet] INTEGER,[fam] TEXT,[name] TEXT,[otchestvo] TEXT,[numbermarsh] INTEGER,[path] TEXT,[data] REAl,[datatwo] REAl,[value] INTEGER);";
                    mydb.iExecuteNonQuery(db_connect.path, sSql, 0);
                    mydb = null;

                    //Form3 Update = new Form3();
                    //Update.Refresh();
                    Close();
                }
            }
        }
    }
}
