using System;
using System.Windows.Forms;
using System.Data.SQLite;

namespace WindowsFormsApp1
{
    public partial class Form10 : Form
    {
        private sqliteclass mydb = null;    //Класс
        private string sSql = string.Empty; //Запрос
        int id;                             //ID текущей записи
        public static string operName = ""; //Нажатая кнопка
        public static string buffOne = "";
        public static string buffTwo = "";
        public static string buffThree = "";
        public static string driver1 = "";
        public static string driver2 = "";
        public Form10()
        {
            InitializeComponent();
            comboBox1.Enabled = false;
            comboBox1.SelectedIndex = 0;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;   //Запрет на ввод текста в комбоБокс1
            button4.Enabled = false;
            if(comboBox1.SelectedIndex == 1) button4.Enabled = true;
            else button4.Enabled = false;
            Text = Form2.operName;

            if (Text == "Изменить")
            {
                string commandText = "select busnumber,pathfrom,pathto,busplaces,driveridfirst,driveridsecond,freeplaces from trip where id = '" + Form2.transit + "'";
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
                    textBox6.Text = sqlReader.GetValue(3).ToString();//Номер паспорта
                    textBox7.Text = sqlReader.GetValue(4).ToString();//Год рождения

                }
                conn.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            operName = "Маршрут";
            Form11 Add = new Form11();
            if (Add.ShowDialog(this) == DialogResult.Cancel)
            {
                textBox1.Text = buffOne;
                textBox2.Text = buffTwo;
                textBox3.Text = buffThree;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            operName = "Автобус";
            Form11 Add = new Form11();
            if (Add.ShowDialog(this) == DialogResult.Cancel)
            {
                textBox4.Text = buffOne;
                textBox5.Text = buffTwo;
            }
            if (textBox3.Text == "Москва") comboBox1.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            operName = "Водитель1";
            Form11 Add = new Form11();
            if (Add.ShowDialog(this) == DialogResult.Cancel)
            {
                textBox6.Text = buffOne;
                driver1 = buffOne;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            operName = "Водитель2";
            Form11 Add = new Form11();
            if (Add.ShowDialog(this) == DialogResult.Cancel)
            {
                textBox7.Text = buffOne;
                driver2 = buffOne;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 1 && textBox3.Text == "Москва") button4.Enabled = true;
            else button4.Enabled = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (Text != "Изменить")
            {
                mydb = new sqliteclass();
                if (textBox7.Text == "")
                {
                    sSql = @"insert into trip (busnumber,pathfrom,pathto,busplaces,driveridfirst,freeplaces) 
            values('" + textBox1.Text + "','" + textBox2.Text + "','" + textBox3.Text + "'," +
                "'" + Convert.ToInt32(textBox5.Text) + "','" + Convert.ToInt32(textBox6.Text) + "'," +
                "'" + Convert.ToInt32(textBox5.Text) + "');";
                }
                else
                {
                    sSql = @"insert into trip (busnumber,pathfrom,pathto,busplaces,driveridfirst,driveridsecond,freeplaces) 
            values('" + textBox1.Text + "','" + textBox2.Text + "','" + textBox3.Text + "'," +
                "'" + Convert.ToInt32(textBox5.Text) + "','" + Convert.ToInt32(textBox6.Text) + "'," +
                "'" + Convert.ToInt32(textBox6.Text) + "','" + Convert.ToInt32(textBox5.Text) + "');";
                }

                mydb.iExecuteNonQuery(db_connect.path, sSql, 0);
                mydb = null;
                Close();
            }
            else
            {               
                mydb = new sqliteclass();
                if (textBox7.Text == "")
                {
                    sSql = @"update trip set (busnumber,pathfrom,pathto,busplaces,driveridfirst,freeplaces) =
                ('" + textBox1.Text + "','" + textBox2.Text + "','" + textBox3.Text + "'," +
                "'" + Convert.ToInt32(textBox5.Text) + "','" + Convert.ToInt32(textBox6.Text) + "'," +
                "'" + Convert.ToInt32(textBox5.Text) + "') where id = '" + Form2.transit + "';";
                }
                else
                {
                    sSql = @"update trip set (busnumber,pathfrom,pathto,busplaces,driveridfirst,driveridsecond,freeplaces) =
                ('" + textBox1.Text + "','" + textBox2.Text + "','" + textBox3.Text + "'," +
                "'" + Convert.ToInt32(textBox5.Text) + "','" + Convert.ToInt32(textBox6.Text) + "'," +
                "'" + Convert.ToInt32(textBox6.Text) + "','" + Convert.ToInt32(textBox5.Text) + "') where id = '" + Form2.transit + "';";
                }

                mydb.iExecuteNonQuery(db_connect.path, sSql, 0);
                mydb = null;
                Close();
            }
        }
    }
}
