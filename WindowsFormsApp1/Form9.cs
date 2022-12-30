using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form9 : Form
    {
        public Form9()
        {
            InitializeComponent();
            DateTime now = DateTime.Now;//Форма для даты
            string dataTime = now.ToString("dd/MM/yyyy");
            dataTime = (dataTime.Replace(" ", "-")).Replace(":", ".");
            label1.Text += dataTime;
            label2.Text += db_connect.path;
            comboBox1.SelectedIndex = 0;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;   //Запрет на ввод текста в комбоБокс1
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox3.Text = "Водители работающие в данный промежуток времени: ";
            textBox4.Text = "Автобусы используемые в данный промежуток времени:";

            DateTime now = DateTime.Now;
            DateTime past = DateTime.Now;
            string str = null;
            string dataTime = now.ToString("dd.MM.yyyy");
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    str = now.ToString("dd.MM.yyyy");
                    break;
                case 1:
                    past = DateTime.Now.AddDays(-7);
                    str = past.ToString("dd.MM.yyyy");
                    break;
                case 2:
                    str = now.ToString("dd.MM.yyyy");
                    str = str.Remove(0, 2);
                    str = "01" + str;
                    break;
                case 3:
                    str = now.ToString("dd.MM.yyyy");
                    str = str.Remove(0, 5);
                    str = "01.01" + str;
                    break;
            }

            textBox3.Text += $"\r\n{str} - {dataTime}";
            textBox4.Text += $"\r\n{str} - {dataTime}";


            string commandText = "select sum(value) from ticket where data BETWEEN '" + str + "' AND '" + dataTime + "'";
            SQLiteConnection conn = new SQLiteConnection(@"Data Source=" + db_connect.path + ";New=True;Version=3");
            SQLiteCommand cmd = new SQLiteCommand(commandText, conn);
            conn.Open();

            SQLiteDataReader sqlReader = cmd.ExecuteReader();

            while (sqlReader.Read())
            {
                textBox1.Text = sqlReader.GetValue(0).ToString();//Фамилия               
            }

            conn.Close();

            string secondText = "select count(*) from ticket where data BETWEEN '" + str + "' AND '" + dataTime + "'";
            SQLiteCommand cmd2 = new SQLiteCommand(secondText, conn);

            conn.Open();
            SQLiteDataReader sqlReader2 = cmd2.ExecuteReader();

            while (sqlReader2.Read())
            {
                textBox2.Text = sqlReader2.GetValue(0).ToString();
            }

            conn.Close();

            string thirdText = "select fam,name,otchestvo from driver where id in (select driveridfirst from trip where datatrip BETWEEN '" + str + "' AND '" + dataTime + "')";
            SQLiteCommand cmd3 = new SQLiteCommand(thirdText, conn);

            conn.Open();
            SQLiteDataReader sqlReader3 = cmd3.ExecuteReader();

            while (sqlReader3.Read())
            {
                textBox3.Text += $"\r\n{sqlReader3.GetValue(0).ToString()}";
                textBox3.Text += $" {sqlReader3.GetValue(1).ToString()}";
                textBox3.Text += $" {sqlReader3.GetValue(2).ToString()}";
            }

            conn.Close();

            string fothText = "select busnumber,bustype from bus where places in (select busplaces from trip where datatrip BETWEEN '" + str + "' AND '" + dataTime + "')";
            SQLiteCommand cmd4 = new SQLiteCommand(fothText, conn);

            conn.Open();
            SQLiteDataReader sqlReader4 = cmd4.ExecuteReader();

            while (sqlReader4.Read())
            {
                textBox4.Text += $"\r\n{sqlReader4.GetValue(0).ToString()}";
                textBox4.Text += $" {sqlReader4.GetValue(1).ToString()}";
            }

            conn.Close();

        }
    }
}
