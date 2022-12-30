using System;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Globalization;

namespace WindowsFormsApp1
{
    public partial class Form4 : Form
    {
        private sqliteclass mydb = null;        //Класс
        private string sSql = string.Empty;     //Запрос
        public int thisTab = Form3.selectedTab; //Номер выбранной вкладки
        string path = "";

        public Form4()
        {
            InitializeComponent();
            switch (thisTab)
            {
                case 0:
                    path = "Владимир";
                    break;
                case 1:
                    path = "Москва";
                    break;
                case 2:
                    path = "Нижний новгород";
                    break;
                case 3:
                    path = "Рязань";
                    break;
            }
            numericUpDown1.Minimum = 1;
            dateTimePicker1.MinDate = DateTime.Today;
            textBox4.MaxLength = 6;         //Номер паспорта
            textBox5.MaxLength = 4;         //Год рождения
            Text = Form3.operName;
            button1.Text = Form3.operName;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;   //Запрет на ввод текста в комбоБокс1
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;   //Запрет на ввод текста в комбоБокс2
            KeyPreview = true;              
            //Запуск события нажатой кнопки при нажатии Enter
            KeyDown += (s, e) => { if (e.KeyValue == (char)Keys.Enter) button1_Click(button1, null); };
            //Закрытие окна при нажатии Escape
            KeyDown += (s, e) => { if (e.KeyValue == (char)Keys.Escape) this.Close(); };

            if (Text == "Изменить")
            {
                string commandText = "select fam,name,otchestvo,passport,yearofbird,datatwo from ticket where numberticet = '" + Form3.transit.ToString() + "'";
                SQLiteConnection conn = new SQLiteConnection(@"Data Source=" + db_connect.path + ";New=True;Version=3");
                SQLiteCommand cmd = new SQLiteCommand(commandText, conn);
                conn.Open();

                SQLiteDataReader sqlReader = cmd.ExecuteReader();

                while (sqlReader.Read())
                {
                    textBox1.Text = sqlReader.GetValue(0).ToString();//Фамилия
                    textBox2.Text = sqlReader.GetValue(1).ToString();//Имя
                    textBox3.Text = sqlReader.GetValue(2).ToString();//Отчество
                    textBox4.Text = sqlReader.GetValue(3).ToString();//Номер паспорта
                    textBox5.Text = sqlReader.GetValue(4).ToString();//Год рождения
                    dateTimePicker1.Value = DateTime.ParseExact(sqlReader.GetValue(5).ToString(), "dd.MM.yyyy",CultureInfo.InvariantCulture);
                }

                conn.Close();

                string firstText = "select pathnumber from path where pathto = '" + path + "';";
                SQLiteCommand cmd1 = new SQLiteCommand(firstText, conn);
                conn.Open();

                SQLiteDataReader sqlReader1 = cmd1.ExecuteReader();

                while (sqlReader1.Read())
                {
                    comboBox2.Items.Add(sqlReader1.GetValue(0).ToString());
                }

                conn.Close();

                string secondText = "select stopname from stops where stopto = '" + path + "';";
                SQLiteCommand cmd2 = new SQLiteCommand(secondText, conn);

                conn.Open();
                SQLiteDataReader sqlReader2 = cmd2.ExecuteReader();

                while (sqlReader2.Read())
                {
                    comboBox1.Items.Add(sqlReader2.GetValue(0).ToString());
                }

                conn.Close();
            }
            else
            {
                string commandText = "select pathnumber from path where pathto = '" + path + "';";
                SQLiteConnection conn = new SQLiteConnection(@"Data Source=" + db_connect.path + ";New=True;Version=3");
                SQLiteCommand cmd = new SQLiteCommand(commandText, conn);
                conn.Open();

                SQLiteDataReader sqlReader = cmd.ExecuteReader();

                while (sqlReader.Read())
                {
                    comboBox2.Items.Add(sqlReader.GetValue(0).ToString());                                                 
                }

                conn.Close();

                string secondText = "select stopname from stops where stopto = '" + path + "';";
                SQLiteCommand cmd2 = new SQLiteCommand(secondText, conn);

                conn.Open();
                SQLiteDataReader sqlReader2 = cmd2.ExecuteReader();

                while (sqlReader2.Read())
                {
                    comboBox1.Items.Add(sqlReader2.GetValue(0).ToString());
                }

                conn.Close();
            }
            

        }

        private void button1_Click(object sender, EventArgs e)
        {
                int maxTicket;
                string commandText = "select max(numberticet) from ticket";//Выбор максимума 
                SQLiteConnection conn = new SQLiteConnection(@"Data Source=" + db_connect.path + ";New=True;Version=3");
                SQLiteCommand cmd = new SQLiteCommand(commandText, conn);
                conn.Open();

            try
            {
                maxTicket = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
            }
            catch
            {
                maxTicket = 1;
            }
         
                conn.Close();


                if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "" || textBox5.Text == "")
                {
                    MessageBox.Show("Не все поля заполнены!");
                }
                else
                {
                    if(Text != "Изменить")
                    {
                    int price = 0;
                    string commandText1 = "select price from stops where stopname = '" + comboBox1.Text + "';";
                    SQLiteConnection conn1 = new SQLiteConnection(@"Data Source=" + db_connect.path + ";New=True;Version=3");
                    SQLiteCommand cmd1 = new SQLiteCommand(commandText1, conn1);
                    conn1.Open();

                    SQLiteDataReader sqlReader = cmd1.ExecuteReader();

                    while (sqlReader.Read())
                    {
                        price = Convert.ToInt32(sqlReader.GetValue(0).ToString());
                    }

                    conn1.Close();

                    price = price * Convert.ToInt32(numericUpDown1.Value);

                    DateTime now = DateTime.Now;
                        string dataTime = now.ToString("dd.MM.yyyy");
                        mydb = new sqliteclass();
                        sSql = @"insert into ticket (numberticet,fam,name,otchestvo,numbermarsh,path,data,datatwo,value,yearofbird,passport,stoppoint,tickcount) 
                        values('" + maxTicket + "','" + textBox1.Text + "','" + textBox2.Text + "','" + textBox3.Text + "','" + Convert.ToInt32(comboBox2.SelectedItem) + "','" + path + "'," +
                        "'" + dataTime + "','" + dateTimePicker1.Value.ToString("dd.MM.yyyy") + "','" + price + "','" + textBox5.Text + "','" + textBox4.Text + "'," +
                        "'" + Convert.ToString(comboBox2.SelectedItem) + "','" + Convert.ToInt32(numericUpDown1.Value) + "');";       
                    
                        mydb.iExecuteNonQuery(db_connect.path, sSql, 0);
                        mydb = null;

                        Close();
                    }
                    else
                    {
                    int price = 0;
                    string commandText1 = "select price from stops where stopname = '" + comboBox1.Text + "';";
                    SQLiteConnection conn1 = new SQLiteConnection(@"Data Source=" + db_connect.path + ";New=True;Version=3");
                    SQLiteCommand cmd1 = new SQLiteCommand(commandText1, conn1);
                    conn1.Open();

                    SQLiteDataReader sqlReader = cmd1.ExecuteReader();

                    while (sqlReader.Read())
                    {
                        price = Convert.ToInt32(sqlReader.GetValue(0).ToString());
                    }

                    conn1.Close();

                    price = price * Convert.ToInt32(numericUpDown1.Value);

                    mydb = new sqliteclass();
                        sSql = @"update ticket set (fam,name,otchestvo,numbermarsh,path,datatwo,value,yearofbird,passport,stoppoint,tickcount) = 
                        ('" + textBox1.Text + "','" + textBox2.Text + "','" + textBox3.Text + "','" + Convert.ToInt32(comboBox2.SelectedItem) + "','" + path + "'," +
                        "'" + dateTimePicker1.Value.ToString("dd.MM.yyyy") + "','" + price + "','" + textBox5.Text + "','" + textBox4.Text + "'," +
                        "'" + Convert.ToString(comboBox2.SelectedItem) + "','" + Convert.ToInt32(numericUpDown1.Value) + "') " +
                        "where numberticet = '" + Form3.transit.ToString() + "';";
                        
                        mydb.iExecuteNonQuery(db_connect.path, sSql, 0);
                        mydb = null;

                        Close();
                    }                   
                }      
        }


        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)//Ввод только букв
        { if (!Char.IsLetter(e.KeyChar) && e.KeyChar != Convert.ToChar(8)) e.Handled = true; }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)//Ввод только букв
        { if (!Char.IsLetter(e.KeyChar) && e.KeyChar != Convert.ToChar(8)) e.Handled = true; }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)//Ввод только букв
        { if (!Char.IsLetter(e.KeyChar) && e.KeyChar != Convert.ToChar(8)) e.Handled = true; }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)//Ввод только цифр
        { if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(8)) e.Handled = true; }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)//Ввод только цифр
        { if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(8)) e.Handled = true; }
    }
}
