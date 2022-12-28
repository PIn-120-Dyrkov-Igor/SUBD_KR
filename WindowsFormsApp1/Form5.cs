using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class Form5 : Form
    {
        private sqliteclass mydb = null;    //Класс
        private string sSql = string.Empty; //Запрос
        int id;                             //ID текущей записи
        
        public Form5()
        {
            InitializeComponent();
            textBox4.MaxLength = 4;         //Год рождения
            textBox5.MaxLength = 11;        //Номер телефона
            textBox6.MaxLength = 6;         //Номер Мед.карты
            Text = Form2.operName;
            button2.Text = Form2.operName;
            KeyPreview = true;
            //Запуск события нажатой кнопки при нажатии Enter
            KeyDown += (s, e) => { if (e.KeyValue == (char)Keys.Enter) button2_Click(button1, null); };
            //Закрытие окна при нажатии Escape
            KeyDown += (s, e) => { if (e.KeyValue == (char)Keys.Escape) this.Close(); };

            if (Text == "Изменить")
            {
                string commandText = "select fam,name,otchestvo,yearofbird,phonenumber,medecinecard,id,photo from driver where medecinecard = '" + Form2.transit.ToString() + "'";
                SQLiteConnection conn = new SQLiteConnection(@"Data Source=" + db_connect.path + ";New=True;Version=3");
                SQLiteCommand cmd = new SQLiteCommand(commandText, conn);
                conn.Open();

                IDataReader sqlReader = cmd.ExecuteReader();
                while (sqlReader.Read())
                {
                    textBox1.Text = sqlReader.GetValue(0).ToString();//Фамилия
                    textBox2.Text = sqlReader.GetValue(1).ToString();//Имя
                    textBox3.Text = sqlReader.GetValue(2).ToString();//Отчество
                    textBox4.Text = sqlReader.GetValue(3).ToString();//Год рождения
                    textBox5.Text = sqlReader.GetValue(4).ToString();//Номер телефона
                    textBox6.Text = sqlReader.GetValue(5).ToString();//Номер мед карты
                    id = Convert.ToInt32(sqlReader.GetValue(6).ToString());//Номер мед карты
                    try
                    {
                        byte[] a = (System.Byte[])sqlReader[7];
                        pictureBox1.Image = ByteToImage(a);//Фото
                        pictureBox1.Image = photo;
                    }
                    catch { }                  
                }
                conn.Close();
                textBox6.ReadOnly = true;
            }
        }

        public Image ByteToImage(byte[] imageBytes)
        {
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = new Bitmap(ms);
            photo = image;
            return image;
        }

        Array pic;
        Image photo;

        private void button1_Click(object sender, EventArgs e)//Добавление изображения
        {
            //Код добавления изображения в бд
            OpenFileDialog openDialog = new OpenFileDialog();

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(openDialog.FileName);
                MemoryStream ms = new MemoryStream();
                pictureBox1.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);//Png
                pic = ms.ToArray();
            }
        }

        public byte[] ImageToByte(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();
                return imageBytes;
            }
        }

        private void button2_Click(object sender, EventArgs e)//Добавление записи в таблицу
        {
            //Проверка на уникальное значение
            int count = 0;
            bool isExist = false;
            string commandText = "select medecinecard from driver where medecinecard = '" + textBox6.Text + "';";
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

            //Добавление записи в БД
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text.Length != 4 || textBox5.Text.Length != 11 || textBox6.Text.Length != 6 || Convert.ToInt32(textBox4.Text) >= 2004 || (isExist == true && Text != "Изменить"))
            {
                if (textBox1.Text == "") MessageBox.Show("Не указана фамилия водителя!", "Ошибка при заполнении");
                else if (textBox2.Text == "") MessageBox.Show("Не указано имя водителя!", "Ошибка при заполнении");
                else if (textBox3.Text == "") MessageBox.Show("Не указано отчество водителя!", "Ошибка при заполнении");
                else if (textBox4.Text.Length != 4 || Convert.ToInt32(textBox4.Text) >= 2004) MessageBox.Show("Год рождения должен содержать 4 цифры!\r\nВодитель должен быть старше 18 лет!", "Ошибка при заполнении");
                else if (textBox5.Text.Length != 11) MessageBox.Show("Номер телефона должен содержать 11 цифр!", "Ошибка при заполнении");
                else if (textBox6.Text.Length != 6) MessageBox.Show("Номер мед карты должен содержать 6 цифр!", "Ошибка при заполнении");
                else if (isExist == true) MessageBox.Show("Номер мед карты c таким значением уже существует!", "Ошибка при заполнении");
            }
            else
            {
                if (Text != "Изменить")
                {
                    mydb = new sqliteclass();
                    sSql = @"insert into driver (fam,name,otchestvo,yearofbird,phonenumber,medecinecard,photo) 
                        values('" + textBox1.Text.ToUpper() + "','" + textBox2.Text.ToUpper() + "'," +
                        "'" + textBox3.Text.ToUpper() + "','" + textBox4.Text + "'," +
                        "'" + textBox5.Text + "','" + textBox6.Text + "','');";                

                    mydb.iExecuteNonQuery(db_connect.path, sSql, 0);
                    mydb = null;

                    string query = "update driver set (photo) = (@photo) where medecinecard = '" + textBox6.Text + "';";
                    string conString = @"Data Source=" + db_connect.path + ";datetimeformat=CurrentCulture";
                    SQLiteConnection con = new SQLiteConnection(conString);
                    SQLiteCommand cmd = new SQLiteCommand(query, con);
                    cmd.Parameters.Add("@photo", DbType.Binary, 8000).Value = pic;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    Close();
                }
                else
                {
                    mydb = new sqliteclass();         
                    sSql = @"update driver set (fam,name,otchestvo,yearofbird,phonenumber,medecinecard,photo) = 
                        ('" + textBox1.Text.ToUpper() + "','" + textBox2.Text.ToUpper() + "'," +
                        "'" + textBox3.Text.ToUpper() + "','" + textBox4.Text + "'," +
                        "'" + textBox5.Text + "','" + textBox6.Text + "','') where id = '" + id + "';";
                    mydb.iExecuteNonQuery(db_connect.path, sSql, 0);
                    mydb = null;

                    string query = "update driver set (photo) = (@photo) where medecinecard = '" + textBox6.Text + "';";
                    string conString = @"Data Source=" + db_connect.path + ";datetimeformat=CurrentCulture";
                    SQLiteConnection con = new SQLiteConnection(conString);
                    SQLiteCommand cmd = new SQLiteCommand(query, con);
                    cmd.Parameters.Add("@photo", DbType.Binary, 8000).Value = pic;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    Close();
                }
            }
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)//Ввод фамилии (только буквы)
        {
            if (!Char.IsLetter(e.KeyChar) && e.KeyChar != Convert.ToChar(8))
            { e.Handled = true; }
        }
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)//Ввод имени (только буквы)
        {
            if (!Char.IsLetter(e.KeyChar) && e.KeyChar != Convert.ToChar(8))
            { e.Handled = true; }
        }
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)//Ввод отчества (только буквы)
        {
            if (!Char.IsLetter(e.KeyChar) && e.KeyChar != Convert.ToChar(8))
            { e.Handled = true; }
        }
        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)//Ввод года рождения (только цифры)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(8))
            { e.Handled = true; }
        }
        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)//Ввод номера телефона (только цифры)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(8))
            { e.Handled = true; }
        }
        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)//Ввод года мед карты (только цифры)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(8))
            { e.Handled = true; }
        }     
    }
}
