using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SQLite;

namespace WindowsFormsApp1
{
    public partial class Form3 : Form
    {
        private sqliteclass mydb = null;    //Класс
        private string sSql = string.Empty; //Запрос
        public static int selectedTab = 0;  //Запись индекса выбранной вкладки таблицы
        public static int transit;          //Переменная для передачи текущей записи в новую форму                      
        public static string operName = ""; //Действие при нажатии Добавить/Изменить

        public Form3()
        {
            InitializeComponent();
            KeyPreview = true;
            //Закрытие окна при нажатии Escape
            KeyDown += (s, e) => { if (e.KeyValue == (char)Keys.Escape) this.Close(); };

            DateTime now = DateTime.Now;
            string dataTime = now.ToString("dd/MM/yyyy");

            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;         //Запрет на изменение размера формы        

            string commandText = "select numberticet as 'Номер билета',fam as 'Фамилия',name as 'Имя'," +
                "otchestvo as 'Отчество',numbermarsh as 'Номер маршрута',path as 'Путь'," +
                "data as 'Дата покупки',datatwo as 'Дата отправления',value as 'Стоимость' " +
                "from ticket where path = 'Владимир' and data = '" + dataTime + "'";
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

        private void button1_Click(object sender, EventArgs e)//Добавить
        {
            operName = "Добавить";

            Form4 Add = new Form4();
            if(Add.ShowDialog(this) == DialogResult.Cancel)//Обновление формы после изменеия
            {
                if (selectedTab == 0)
                {
                    int thisTab = selectedTab;
                    tabControl1.SelectTab(1);
                    tabControl1.SelectTab(thisTab);
                }
                else
                {
                    int thisTab = selectedTab;
                    tabControl1.SelectTab(0);
                    tabControl1.SelectTab(thisTab);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)//Изменить
        {         
            // Номер выбранной строки
            int selectedRow = 0/* = Convert.ToInt32(dataGridView1[0,dataGridView1.CurrentRow.Index].Value)*/;// Столбец, в который выводится id           

            switch (selectedTab)//Выбор таблицы из которой берется строка для удаления
            {
                case 0:
                    if (dataGridView1.CurrentRow is null)
                    {
                        MessageBox.Show("Не выбрана строка");
                        return;
                    }
                    else
                        selectedRow = Convert.ToInt32(dataGridView1[0, dataGridView1.CurrentRow.Index].Value);
                    break;

                case 1:
                    if (dataGridView2.CurrentRow is null)
                    {
                        MessageBox.Show("Не выбрана строка");
                        return;
                    }
                    else
                        selectedRow = Convert.ToInt32(dataGridView2[0, dataGridView2.CurrentRow.Index].Value);
                    break;

                case 2:
                    if (dataGridView3.CurrentRow is null)
                    {
                        MessageBox.Show("Не выбрана строка");
                        return;
                    }
                    else
                        selectedRow = Convert.ToInt32(dataGridView3[0, dataGridView3.CurrentRow.Index].Value);
                    break;

                case 3:
                    if (dataGridView4.CurrentRow is null)
                    {
                        MessageBox.Show("Не выбрана строка");
                        return;
                    }
                    else
                        selectedRow = Convert.ToInt32(dataGridView4[0, dataGridView4.CurrentRow.Index].Value);
                    break;

            }


            transit = selectedRow;
            operName = "Изменить";

            Form4 Add = new Form4();
            Add.thisTab = selectedRow;
         
            if (Add.ShowDialog(this) == DialogResult.Cancel)
            {
                if (selectedTab == 0)
                {
                    int thisTab = selectedTab;
                    tabControl1.SelectTab(1);
                    tabControl1.SelectTab(thisTab);
                }
                else
                {
                    int thisTab = selectedTab;
                    tabControl1.SelectTab(0);
                    tabControl1.SelectTab(thisTab);
                }
            }
            
        }

        private void button3_Click(object sender, EventArgs e)//Удалить
        {

            // Номер выбранной строки
            // Столбец, в который выводится id
            int selectedRow = 0 /*Convert.ToInt32(dataGridView1[0, dataGridView1.CurrentRow.Index].Value)*/;

            switch (selectedTab)//Выбор таблицы из которой берется строка для удаления
            {
                case 0:
                    if (dataGridView1.CurrentRow is null)
                    {
                        MessageBox.Show("Не выбрана строка");
                        return;
                    }
                    else
                        selectedRow = Convert.ToInt32(dataGridView1[0, dataGridView1.CurrentRow.Index].Value);
                    break;

                case 1:
                    if (dataGridView2.CurrentRow is null)
                    {
                        MessageBox.Show("Не выбрана строка");
                        return;
                    }
                    else
                        selectedRow = Convert.ToInt32(dataGridView2[0, dataGridView2.CurrentRow.Index].Value);
                    break;

                case 2:
                    if (dataGridView3.CurrentRow is null)
                    {
                        MessageBox.Show("Не выбрана строка");
                        return;
                    }
                    else
                        selectedRow = Convert.ToInt32(dataGridView3[0, dataGridView3.CurrentRow.Index].Value);
                    break;

                case 3:
                    if (dataGridView4.CurrentRow is null)
                    {
                        MessageBox.Show("Не выбрана строка");
                        return;
                    }
                    else
                        selectedRow = Convert.ToInt32(dataGridView4[0, dataGridView4.CurrentRow.Index].Value);
                    MessageBox.Show($"{selectedRow}", "Готово");
                    break;

            }

            DialogResult dr = MessageBox.Show($"Вы действительно хотите удалить запись {selectedRow}?", "Удаление записи", MessageBoxButtons.YesNo);

            switch (dr)
            {
                case DialogResult.Yes:

                    mydb = new sqliteclass();
                    sSql = "delete from ticket where numberticet = '" + selectedRow + "' ";
                    if (mydb.iExecuteNonQuery(db_connect.path, sSql, 1) == 0)
                    {
                        MessageBox.Show("Ошибка удаления записи!", "Ошибка");
                        mydb = null;
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Запись удалена", "Готово");
                        if (selectedTab == 0)
                        {
                            int thisTab = selectedTab;
                            tabControl1.SelectTab(1);
                            tabControl1.SelectTab(thisTab);
                        }
                        else
                        {
                            int thisTab = selectedTab;
                            tabControl1.SelectTab(0);
                            tabControl1.SelectTab(thisTab);
                        }
                    }
                    break;
            }

        }

        public void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            DateTime now = DateTime.Now;
            string dataTime = now.ToString("dd/MM/yyyy");

            switch (e.TabPageIndex)
            {
                case 0:

                    selectedTab = 0;
                    string commandText = "select numberticet as 'Номер билета',fam as 'Фамилия',name as 'Имя'," +
                        "otchestvo as 'Отчество',numbermarsh as 'Номер маршрута',path as 'Путь'," +
                        "data as 'Дата покупки',datatwo as 'Дата отправления',value as 'Стоимость' " +
                        "from ticket where path = 'Владимир' and data = '" + dataTime + "'";
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
                    break;

                case 1:

                    selectedTab = 1;
                    commandText = "select numberticet as 'Номер билета',fam as 'Фамилия',name as 'Имя'," +
                        "otchestvo as 'Отчество',numbermarsh as 'Номер маршрута',path as 'Путь'," +
                        "data as 'Дата покупки',datatwo as 'Дата отправления',value as 'Стоимость' " +
                        "from ticket where path = 'Москва' and data = '" + dataTime + "'";                 
                    SQLiteConnection conn2 = new SQLiteConnection(@"Data Source=" + db_connect.path + ";New=True;Version=3");
                    SQLiteCommand cmd2 = new SQLiteCommand(commandText, conn2);
                    conn2.Open();

                    try
                    {
                        SQLiteDataReader sqlReader = cmd2.ExecuteReader();
                        DataTable dt = new DataTable();
                        dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        dataGridView2.AllowUserToAddRows = false;//Убираем пустую строку
                        dt.Load(sqlReader);

                        dataGridView2.DataSource = dt;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Не удалось выбрать данные из таблицы\nОшибка: " + ex);
                        return;
                    }
                    conn2.Close();
                    break;

                case 2:

                    selectedTab = 2;
                    commandText = "select numberticet as 'Номер билета',fam as 'Фамилия',name as 'Имя'," +
                        "otchestvo as 'Отчество',numbermarsh as 'Номер маршрута',path as 'Путь'," +
                        "data as 'Дата покупки',datatwo as 'Дата отправления',value as 'Стоимость' " +
                        "from ticket where path = 'Нижний новгород' and data = '" + dataTime + "'";
                    SQLiteConnection conn3 = new SQLiteConnection(@"Data Source=" + db_connect.path + ";New=True;Version=3");
                    SQLiteCommand cmd3 = new SQLiteCommand(commandText, conn3);
                    conn3.Open();

                    try
                    {
                        SQLiteDataReader sqlReader = cmd3.ExecuteReader();
                        DataTable dt = new DataTable();
                        dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        dataGridView3.AllowUserToAddRows = false;//Убираем пустую строку
                        dt.Load(sqlReader);

                        dataGridView3.DataSource = dt;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Не удалось выбрать данные из таблицы\nОшибка: " + ex);
                        return;
                    }
                    conn3.Close();
                    break;

                case 3:

                    selectedTab = 3;
                    commandText = "select numberticet as 'Номер билета',fam as 'Фамилия',name as 'Имя'," +
                        "otchestvo as 'Отчество',numbermarsh as 'Номер маршрута',path as 'Путь'," +
                        "data as 'Дата покупки',datatwo as 'Дата отправления',value as 'Стоимость' " +
                        "from ticket where path = 'Рязань' and data = '" + dataTime + "'";
                    SQLiteConnection conn4 = new SQLiteConnection(@"Data Source=" + db_connect.path + ";New=True;Version=3");
                    SQLiteCommand cmd4 = new SQLiteCommand(commandText, conn4);
                    conn4.Open();

                    try
                    {
                        SQLiteDataReader sqlReader = cmd4.ExecuteReader();
                        DataTable dt = new DataTable();
                        dataGridView4.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        dataGridView4.AllowUserToAddRows = false;//Убираем пустую строку
                        dt.Load(sqlReader);

                        dataGridView4.DataSource = dt;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Не удалось выбрать данные из таблицы\nОшибка: " + ex);
                        return;
                    }
                    conn4.Close();
                    break;

                default:
                    MessageBox.Show("Что-то пошло не так!");
                    break;
            }          
        }
    }
}
