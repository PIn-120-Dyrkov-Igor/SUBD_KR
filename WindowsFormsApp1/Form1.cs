using System;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            KeyPreview = true;
            //Закрытие окна при нажатии Escape
            KeyDown += (s, e) => { if (e.KeyValue == (char)Keys.Escape) this.Close(); };
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;         //Запрет на изменение размера формы
            comboBox1.SelectedIndex = 1;                            //Выбор второй строки листБокса при открытии формы
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;   //Запрет на ввод текста в листБокс

            textBox1.PasswordChar = '*';                            //Маска для поля ввода пароля
            textBox1.MaxLength = 12;                                //Максимальная длина пароля
        }

        private void selectedItem(object sender, EventArgs e)
        {
            textBox1.Text = "";                                     //Очистка поля с паролем
        }

        private void enterButton(object sender, EventArgs e)
        {                                                           //Запрет на вход при неавторизованной БД
            if (comboBox1.SelectedIndex == 0 && db_connect.path == null)
            {
                MessageBox.Show("БД еще не выбрана");
            }
            else if (comboBox1.SelectedIndex == 0 && textBox1.Text == "1")
            {
                Form3 start = new Form3();

                textBox1.Text = "";                                 //Очистка поля с паролем

                if (start.ShowDialog(this) == DialogResult.Cancel)  //Открытие формы "Рабочая касса"
                {
                    if (comboBox1.SelectedIndex == 0) comboBox1.SelectedIndex = 1;
                }
            }
            else if (comboBox1.SelectedIndex == 1 && textBox1.Text == "1")
            {
                Form2 admin = new Form2();

                textBox1.Text = "";                                 //Очистка поля с паролем

                if (admin.ShowDialog(this) == DialogResult.Cancel)  //Открытие формы "Администрирование"
                {
                    if (comboBox1.SelectedIndex == 1) comboBox1.SelectedIndex = 0;
                }                                              
            }
            else
                MessageBox.Show("Не верный пароль для выбранного пользователя.", "Ошибка");
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyValue == (char)Keys.Enter)
            {
                enterButton(button1,null);
            }
        }
    }
}
