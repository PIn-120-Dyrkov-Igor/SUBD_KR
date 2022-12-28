using System;
using System.Windows.Forms;
using System.Data.SQLite;

namespace WindowsFormsApp1
{
    public partial class Form8 : Form
    {
        private sqliteclass mydb = null;    //Класс
        private string sSql = string.Empty; //Запрос
        int id;                             //ID текущей записи
        public Form8()
        {
            InitializeComponent();
            Text = Form2.operName;
            button1.Text = Form2.operName;
            comboBox1.SelectedIndex = 0;                            //Выбор 0 записи комбоБокс1
            comboBox2.SelectedIndex = Form2.selectedTab2;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;   //Запрет на ввод текста в комбоБокс1
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;   //Запрет на ввод текста в комбоБокс2
            numericUpDown1.Maximum = 1000;

            KeyPreview = true;
            //Запуск события нажатой кнопки при нажатии Enter
            KeyDown += (s, e) => { if (e.KeyValue == (char)Keys.Enter) button1_Click(button1, null); };
            //Закрытие окна при нажатии Escape
            KeyDown += (s, e) => { if (e.KeyValue == (char)Keys.Escape) this.Close(); };

            if (Text == "Изменить")
            {
                string commandText = "select stopfrom,stopto,stopname,price,id from stops where stopto = '" + Form2.transitStr + "'";
                SQLiteConnection conn = new SQLiteConnection(@"Data Source=" + db_connect.path + ";New=True;Version=3");
                SQLiteCommand cmd = new SQLiteCommand(commandText, conn);
                conn.Open();

                SQLiteDataReader sqlReader = cmd.ExecuteReader();
                while (sqlReader.Read())
                {
                    comboBox1.Text = sqlReader.GetValue(0).ToString();  //Место отправления
                    comboBox2.Text = sqlReader.GetValue(1).ToString();  //Место прибытия
                    textBox1.Text = sqlReader.GetValue(2).ToString();   //Название остановки
                    numericUpDown1.Value = Convert.ToInt32(sqlReader.GetValue(3).ToString());//Время в пути
                    id = Convert.ToInt32(sqlReader.GetValue(4).ToString());
                }

                conn.Close();
                textBox1.ReadOnly = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Проверка на уникальное значение
            int count = 0;
            bool isExist = false;
            string commandText = "select stopname from stops where stopname = '" + textBox1.Text + "';";
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

            if (comboBox1.Text == "" || comboBox2.Text == "" || textBox1.Text == "" || numericUpDown1.Value == 0 || (isExist == true && Text != "Изменить"))
            {
                if(comboBox1.Text == "") MessageBox.Show("Не выбрано место отправления");
                else if (comboBox2.Text == "") MessageBox.Show("Не выбрано место прибытия");
                else if (textBox1.Text == "") MessageBox.Show("Не указанно название остановки");
                else if (numericUpDown1.Value == 0) MessageBox.Show("Не указана стоимость");
                else if (isExist == true) MessageBox.Show("Остановка с таким именем уже существует");

            }
            else
            {
                if (Text != "Изменить")
                {
                    mydb = new sqliteclass();
                    sSql = @"insert into stops (stopfrom,stopto,stopname,price) 
                    values('" + comboBox1.Text + "','" + comboBox2.Text + "','" + textBox1.Text + "','" + numericUpDown1.Value + "');";
                    mydb.iExecuteNonQuery(db_connect.path, sSql, 0);
                    mydb = null;
                    Close();
                }
                else
                {
                    mydb = new sqliteclass();
                    sSql = @"update stops set (stopfrom,stopto,stopname,price) = 
                    ('" + comboBox1.Text + "','" + comboBox2.Text + "','" + textBox1.Text + "','" + numericUpDown1.Value + "') where id = '" + id + "';";
                    mydb.iExecuteNonQuery(db_connect.path, sSql, 0);
                    mydb = null;
                    Close();
                }
            }
        }
    }
}
