using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace WindowsFormsApp1
{
    public partial class Form6 : Form
    {
        private sqliteclass mydb = null;    //Класс
        private string sSql = string.Empty; //Запрос
        int id;                             //ID текущей записи
        string thisCell = "";               //Данные из выбранной вкладки

        public Form6()
        {
            InitializeComponent();
            textBox1.MaxLength = 9;//Номер автобуса
            numericUpDown1.Maximum = 300;//Максимальная вместимость автобуса
            Text = Form2.operName;
            button1.Text = Form2.operName;
            KeyPreview = true;
            //Запуск события нажатой кнопки при нажатии Enter
            KeyDown += (s, e) => { if (e.KeyValue == (char)Keys.Enter) button1_Click(button1, null); };
            //Закрытие окна при нажатии Escape
            KeyDown += (s, e) => { if (e.KeyValue == (char)Keys.Escape) this.Close(); };

            if (Text == "Изменить")
            {
                string commandText = "select busnumber,bustype,places,id from bus where busnumber = '" + Form2.transitStr + "'";//photo
                SQLiteConnection conn = new SQLiteConnection(@"Data Source=" + db_connect.path + ";New=True;Version=3");
                SQLiteCommand cmd = new SQLiteCommand(commandText, conn);
                conn.Open();

                SQLiteDataReader sqlReader = cmd.ExecuteReader();
                while (sqlReader.Read())
                {
                    textBox1.Text = sqlReader.GetValue(0).ToString();//Номер автобуса
                    textBox2.Text = sqlReader.GetValue(1).ToString();//Тип автобуса
                    numericUpDown1.Value = Convert.ToInt32(sqlReader.GetValue(2).ToString());//Вместимость автобуса
                    id = Convert.ToInt32(sqlReader.GetValue(3).ToString());//Номер мед карты
                }

                conn.Close();
                textBox1.Text = textBox1.Text.ToUpper();
                textBox1.ReadOnly = true;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //Проверка на уникальное значение
            int count = 0;
            bool isExist = false;           
            string commandText = "select busnumber from bus where busnumber = '" + textBox1.Text.ToUpper() + "';";
            SQLiteConnection conn0 = new SQLiteConnection(@"Data Source=" + db_connect.path + ";New=True;Version=3");
            SQLiteCommand cmd0 = new SQLiteCommand(commandText, conn0);
            conn0.Open();

            SQLiteDataReader sqlReader = cmd0.ExecuteReader();
            while (sqlReader.Read())
            {
                count++;
                thisCell = sqlReader.GetValue(0).ToString();
            }

            conn0.Close();

            if (count > 0)
            {
                isExist = true;
            }
            else
                isExist = false;

            
            bool car = false;
            textBox1.Text = textBox1.Text.ToUpper();
            Regex regex = new Regex(@"^[А-Я]\d{3}[А-Я]{2}\d{2,3}");
            MatchCollection matches = regex.Matches(textBox1.Text);
            if (matches.Count == 1) car = true;

            if (textBox1.Text == "" || textBox2.Text == "" || numericUpDown1.Value == 0 || car == false || (isExist == true && Text != "Изменить"))
            {
                if (car == false) MessageBox.Show("Номер автобуса должен быть следующего формата:\r\nА111АА11 или А111АА111","Ошибка при заполнении");
                else if (textBox1.Text == "") MessageBox.Show("Номер автобуса не заполнен!", "Ошибка при заполнении");
                else if (textBox2.Text == "") MessageBox.Show("Тип автобуса не заполнен!", "Ошибка при заполнении");
                else if (numericUpDown1.Value == 0) MessageBox.Show("Количество мест в автобусе не должно быть равно 0.", "Ошибка при заполнении");
                else if (isExist == true) MessageBox.Show("Автобус с таким номером уже существует", "Ошибка при заполнении");
            }
            else
            {
                if (Text != "Изменить")
                {
                    mydb = new sqliteclass();
                    sSql = @"insert into bus (busnumber,bustype,places) values('" + textBox1.Text + "','" + textBox2.Text + "','" + numericUpDown1.Value + "');";
                    mydb.iExecuteNonQuery(db_connect.path, sSql, 0);
                    mydb = null;
                    Close();
                }
                else 
                {
                    mydb = new sqliteclass();
                    sSql = @"update bus set (busnumber,bustype,places) = ('" + textBox1.Text + "','" + textBox2.Text + "','" + numericUpDown1.Value + "') where id = '" + id + "';";
                    mydb.iExecuteNonQuery(db_connect.path, sSql, 0);
                    mydb = null;
                    Close();
                }

            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsLetter(e.KeyChar) && e.KeyChar != Convert.ToChar(8))
            {
                e.Handled = true;
            }
        }
    }
}
