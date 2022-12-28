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

        public Form4()
        {
            InitializeComponent();
            numericUpDown1.Minimum = 1;
            dateTimePicker1.MinDate = DateTime.Today;
            textBox4.MaxLength = 6;         //Номер паспорта
            textBox5.MaxLength = 4;         //Год рождения
            Text = Form3.operName;
            button1.Text = Form3.operName;
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
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

                string path = "";

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

                if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "" || textBox5.Text == "")
                {
                    MessageBox.Show("Не все поля заполнены!");
                }
                else
                {
                    if(Text != "Изменить")
                    {
                        DateTime now = DateTime.Now;
                        string dataTime = now.ToString("dd.MM.yyyy");
                        mydb = new sqliteclass();
                        sSql = @"insert into ticket (numberticet,fam,name,otchestvo,numbermarsh,path,data,datatwo,value,yearofbird,passport) 
                        values('" + maxTicket + "','" + textBox1.Text + "','" + textBox2.Text + "','" + textBox3.Text + "','105','" + path + "'," +
                        "'" + dataTime + "','" + dateTimePicker1.Value.ToString("dd.MM.yyyy") + "','500','" + textBox5.Text + "','" + textBox4.Text + "');";       
                    
                        mydb.iExecuteNonQuery(db_connect.path, sSql, 0);
                        mydb = null;

                        Close();
                    }
                    else
                    {
                        mydb = new sqliteclass();
                        sSql = @"update ticket set (fam,name,otchestvo,datatwo,value,yearofbird,passport) = 
                        ('" + textBox1.Text + "','" + textBox2.Text + "','" + textBox3.Text + "','" + dateTimePicker1.Value.ToString("dd.MM.yyyy") + "'," +
                        "'500','" + textBox5.Text + "','" + textBox4.Text + "') where numberticet = '" + Form3.transit.ToString() + "';";
                        
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
