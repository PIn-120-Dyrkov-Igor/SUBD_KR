using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;


namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        string filePath = string.Empty;     //Путь к файлу
        string fileExt = string.Empty;      //Проверка на существование БД

        private sqliteclass mydb = null;    //Класс
        private string sSql = string.Empty; //Запрос
        public static int transit;          //Переменная для передачи текущей записи в новую форму
        public static string transitStr;    //Переменная для передачи текущей записи типа String в новую форму
        public static string operName = ""; //Действие при нажатии Добавить/Изменить

        public Form2()
        {
            InitializeComponent();
            KeyPreview = true;
            //Закрытие окна при нажатии Escape
            KeyDown += (s, e) => { if (e.KeyValue == (char)Keys.Escape) this.Close(); };
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;         //Запрет на изменение размера формы

            if (db_connect.path == null)//Закрытие доступа к кнопакам до подключения к БД
            {
                button3.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
                button6.Enabled = false;
                button16.Enabled = false;
            }
            else
            {
                string commandText = "select numberticet as 'Номер билета',fam as 'Фамилия',name as 'Имя'," +
                    "otchestvo as 'Отчество',numbermarsh as 'Номер маршрута',path as 'Путь',data as 'Дата покупки'," +
                    "datatwo as 'Дата отправления',value as 'Стоимость' from ticket";
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
        }

        private void TabControl1_Selecting(Object sender, TabControlCancelEventArgs e)//Запрет на изменение вкладки до подключения к бд
        {
            if (db_connect.path == null)
            {
                MessageBox.Show("БД еще не выбрана");
                e.Cancel = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)      //Поиск уже существующей базы данных
        {
            openFileDialog1.Filter = "Database(*.db)|*.db";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileExt = Path.GetExtension(openFileDialog1.FileName);
                if (fileExt.CompareTo(".db") == 0)
                {
                    try
                    {
                        filePath = openFileDialog1.FileName;//
                        db_connect.path = openFileDialog1.FileName;

                        //Вывод данных на форму
                        string commandText = "select numberticet as 'Номер билета',fam as 'Фамилия',name as 'Имя'," +
                            "otchestvo as 'Отчество',numbermarsh as 'Номер маршрута',path as 'Путь',data as 'Дата покупки'," +
                            "datatwo as 'Дата отправления',value as 'Стоимость' from ticket";
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
                        catch
                        {
                            MessageBox.Show("Данная БД не работает с этим приложением","Ошибка");//Это событие возникает при не соответствующей БД
                            db_connect.path = null;
                            return;
                        }
                        conn.Close();

                        //////

                        MessageBox.Show("База данных добавлена");
                        button3.Enabled = true;
                        button4.Enabled = true;
                        button5.Enabled = true;
                        button6.Enabled = true;
                        button16.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Невозможно открыть выбранный файл\nОшибка: " + ex);
                    }
                }
                else
                {
                    MessageBox.Show("Выберите файл с расширением .db", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)      //Создание новой базы данных
        {
            db_connect.firstAdd = true;
            DateTime now = DateTime.Now;
            string dataTime = now.ToString();
            dataTime = (dataTime.Replace(" ", "-")).Replace(":", ".");
            string fileName = "kursovayadb" + dataTime + ".db";

            filePath = Path.Combine(Application.StartupPath, fileName);
            db_connect.path = filePath;
            //Подключение и создание базы данных
            mydb = new sqliteclass();
            sSql  = @"CREATE TABLE if not exists [ticket] ([id] INTEGER PRIMARY KEY AUTOINCREMENT, [numberticet] INTEGER,  [fam] TEXT,
            [name] TEXT, [otchestvo] TEXT, [yearofbird] INTEGER, [passport] INTEGER, [numbermarsh] INTEGER, [path] TEXT, [stoppoint] TEXT,
            [data] TEXT, [datatwo] TEXT, [timemarsh] TEXT, [tickcount] INTEGER, [value] INTEGER);";//Таблица "Билеты"

            sSql += @"CREATE TABLE if not exists [bus] ([id] INTEGER PRIMARY KEY AUTOINCREMENT, [busnumber] TEXT, [bustype] TEXT, [places] INTEGER);";//Таблица "Автобусы"

            sSql += @"CREATE TABLE if not exists [driver] ([id] INTEGER PRIMARY KEY AUTOINCREMENT, [fam] TEXT, [name] TEXT, [otchestvo] TEXT,
            [yearofbird] INTEGER, [phonenumber] INTEGER, [medecinecard] INTEGER, [photo] BLOP);";//Таблица "Водители"

            sSql += @"CREATE TABLE if not exists [path] ([id] INTEGER PRIMARY KEY AUTOINCREMENT, [pathnumber] INTEGER, [pathfrom] TEXT,  
            [pathto] TEXT, [pathtime] INTEGER);";//Таблица "Маршруты"

            sSql += @"CREATE TABLE if not exists [stops] ([id] INTEGER PRIMARY KEY AUTOINCREMENT, [stopfrom] TEXT, [stopto] TEXT, 
            [stopname] TEXT, [price] INTEGER);";//Таблица "Остановки"

            sSql += @"CREATE TABLE if not exists [trip] ([id] INTEGER PRIMARY KEY AUTOINCREMENT, [busnumber] TEXT, [pathfrom] TEXT,  
            [pathto] TEXT, [busplaces] INTEGER, [driveridfirst] INTEGER, [driveridsecond] INTEGER, [freeplaces] INTEGER, [datatrip] TEXT, [timetrip] text);";//Таблица "Рейсы"

            

            //sSql += @"create trigger new_item after insert on ticket for each row begin insert into driver(name) values (new.id); end;";

            mydb.iExecuteNonQuery(db_connect.path, sSql, 0);
            //Text = "Таблица создана!";

            

            
            //////////////////////////////
            MessageBox.Show($"База данных создана\r\nИмя файла: {fileName}");
            mydb = null;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            button16.Enabled = true;

            string commandText = "select numberticet as 'Номер билета',fam as 'Фамилия',name as 'Имя'," +
                "otchestvo as 'Отчество',numbermarsh as 'Номер маршрута',path as 'Путь'," +
                "data as 'Дата покупки',datatwo as 'Дата отправления',value as 'Стоимость' from ticket";
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

            return;
        }


        private void button3_Click(object sender, EventArgs e)//Пассажиры
        {      
            string commandText = "select numberticet as 'Номер билета',fam as 'Фамилия',name as 'Имя'," +
                "otchestvo as 'Отчество',numbermarsh as 'Номер маршрута',path as 'Путь'," +
                "data as 'Дата покупки',datatwo as 'Дата отправления',value as 'Стоимость' from ticket";
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

        private void button4_Click(object sender, EventArgs e)//Водители
        {
            string commandText = "select fam as 'Фамилия',name as 'Имя',otchestvo as 'Отчество'," +
                "yearofbird as 'Год рождения',phonenumber as 'Номер тел',medecinecard as 'Номер мед карты' from driver";
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
        private void button5_Click(object sender, EventArgs e)//Автобусы
        {
            string commandText = "select busnumber as 'Номер автобуса',bustype as 'Тип автобуса',places as 'Количество мест' from bus";
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

        private void button6_Click(object sender, EventArgs e)//Маршруты
        {
            string commandText = "select pathnumber as 'Номер маршрута',pathfrom as 'Откуда',pathto as 'Куда',pathtime as 'Время в пути' from path";
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

        public static int selectedTab = 0;//Запись индекса выбранной вкладки таблицы

        private void tabControl1_Selected(object sender, TabControlEventArgs e)//Изменение вкладки после подключения к БД
        {
            DateTime now = DateTime.Now;

            switch (e.TabPageIndex)
            {
                case 0://Проверка БД

                    selectedTab = 0;
                    string commandText = "select numberticet as 'Номер билета',fam as 'Фамилия',name as 'Имя'," +
                        "otchestvo as 'Отчество',numbermarsh as 'Номер маршрута',path as 'Путь',data as 'Дата покупки'," +
                        "datatwo as 'Дата отправления',value as 'Стоимость' from ticket";
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

                case 1://Водители

                    selectedTab = 1;
                    commandText = "select fam as 'Фамилия',name as 'Имя',otchestvo as 'Отчество',yearofbird as 'Год рождения'," +
                        "phonenumber as 'Номер телефона',medecinecard as 'Номер мед.карты' from driver";
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

                case 2://Автобусы

                    selectedTab = 2;
                    commandText = "select busnumber as 'Номер автобуса',bustype as 'Тип автобуса'," +
                        "places as 'Количество мест' from bus";
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

                case 3://Маршруты

                    selectedTab = 3;
                    commandText = "select pathnumber as 'Номер маршрута',pathfrom as 'Место отправления'," +
                        "pathto as 'Место назначения',pathtime as 'Время в пути' from path";
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

                case 4://Остановки
                    tabControl2.SelectedTab = tabPage7;
                    selectedTab = 4;
                    commandText = "select stopfrom as 'Место отправления',stopto as 'Место назначения'," +
                        "stopname as 'Название остановки',price as 'Цена' from stops where stopto = 'Владимир' ORDER BY price";
                    SQLiteConnection conn5 = new SQLiteConnection(@"Data Source=" + db_connect.path + ";New=True;Version=3");
                    SQLiteCommand cmd5 = new SQLiteCommand(commandText, conn5);
                    conn5.Open();

                    try
                    {
                        SQLiteDataReader sqlReader = cmd5.ExecuteReader();
                        DataTable dt = new DataTable();
                        dataGridView5.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        dataGridView5.AllowUserToAddRows = false;//Убираем пустую строку
                        dt.Load(sqlReader);
                        dataGridView5.DataSource = dt;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Не удалось выбрать данные из таблицы\nОшибка: " + ex);
                        return;
                    }

                    conn5.Close();
                    break;

                case 5://Рейсы

                    selectedTab = 5;
                    commandText = "select id as 'ID', busnumber as 'Номер маршрута',pathfrom as 'Место отправления'," +
                        "pathto as 'Место назначения',busplaces as 'Количество мест'," +
                        "driveridfirst as 'Первый водитель',driveridsecond as 'Второй водитель' from trip";
                    SQLiteConnection conn6 = new SQLiteConnection(@"Data Source=" + db_connect.path + ";New=True;Version=3");
                    SQLiteCommand cmd6 = new SQLiteCommand(commandText, conn6);
                    conn6.Open();

                    try
                    {
                        SQLiteDataReader sqlReader = cmd6.ExecuteReader();
                        DataTable dt = new DataTable();
                        dataGridView9.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        dataGridView9.AllowUserToAddRows = false;//Убираем пустую строку
                        dt.Load(sqlReader);
                        dataGridView9.DataSource = dt;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Не удалось выбрать данные из таблицы\nОшибка: " + ex);
                        return;
                    }

                    conn6.Close();
                    break;

                default:
                    MessageBox.Show("Что-то пошло не так!");
                    break;
            }
        }

        public static int selectedTab2 = 0;//Запись индекса выбранной вкладки таблицы остановки

        private void tabControl2_Selected(object sender, TabControlEventArgs e)//Изменение вкладки для таблицы рейсы
        {
            switch (e.TabPageIndex)
            {
                case 0://Владимир

                    selectedTab2 = 0;
                    string commandText = "select stopfrom as 'Место отправления',stopto as 'Место назначения'," +
                        "stopname as 'Название остановки',price as 'Цена' from stops where stopto = 'Владимир' ORDER BY price";
                    SQLiteConnection conn = new SQLiteConnection(@"Data Source=" + db_connect.path + ";New=True;Version=3");
                    SQLiteCommand cmd = new SQLiteCommand(commandText, conn);
                    conn.Open();

                    try
                    {
                        SQLiteDataReader sqlReader = cmd.ExecuteReader();
                        DataTable dt = new DataTable();
                        dataGridView5.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        dataGridView5.AllowUserToAddRows = false;//Убираем пустую строку
                        dt.Load(sqlReader);
                        dataGridView5.DataSource = dt;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Не удалось выбрать данные из таблицы\nОшибка: " + ex);
                        return;
                    }
                    conn.Close();
                    break;

                case 1://Москва

                    selectedTab2 = 1;
                    commandText = "select stopfrom as 'Место отправления',stopto as 'Место назначения'," +
                        "stopname as 'Название остановки',price as 'Цена' from stops where stopto = 'Москва' ORDER BY price";
                    SQLiteConnection conn2 = new SQLiteConnection(@"Data Source=" + db_connect.path + ";New=True;Version=3");
                    SQLiteCommand cmd2 = new SQLiteCommand(commandText, conn2);
                    conn2.Open();

                    try
                    {
                        SQLiteDataReader sqlReader = cmd2.ExecuteReader();
                        DataTable dt = new DataTable();
                        dataGridView5.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        dataGridView5.AllowUserToAddRows = false;//Убираем пустую строку
                        dt.Load(sqlReader);
                        dataGridView5.DataSource = dt;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Не удалось выбрать данные из таблицы\nОшибка: " + ex);
                        return;
                    }
                    conn2.Close();
                    break;

                case 2://Нижний новгород

                    selectedTab2 = 2;
                    commandText = "select stopfrom as 'Место отправления',stopto as 'Место назначения'," +
                        "stopname as 'Название остановки',price as 'Цена' from stops where stopto = 'Нижний новгород' ORDER BY price";
                    SQLiteConnection conn3 = new SQLiteConnection(@"Data Source=" + db_connect.path + ";New=True;Version=3");
                    SQLiteCommand cmd3 = new SQLiteCommand(commandText, conn3);
                    conn3.Open();

                    try
                    {
                        SQLiteDataReader sqlReader = cmd3.ExecuteReader();
                        DataTable dt = new DataTable();
                        dataGridView5.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        dataGridView5.AllowUserToAddRows = false;//Убираем пустую строку
                        dt.Load(sqlReader);
                        dataGridView5.DataSource = dt;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Не удалось выбрать данные из таблицы\nОшибка: " + ex);
                        return;
                    }
                    conn3.Close();
                    break;

                case 3://Рязань

                    selectedTab2 = 3;
                    commandText = "select stopfrom as 'Место отправления',stopto as 'Место назначения'," +
                        "stopname as 'Название остановки',price as 'Цена' from stops where stopto = 'Рязань' ORDER BY price"; 
                    SQLiteConnection conn4 = new SQLiteConnection(@"Data Source=" + db_connect.path + ";New=True;Version=3");
                    SQLiteCommand cmd4 = new SQLiteCommand(commandText, conn4);
                    conn4.Open();

                    try
                    {
                        SQLiteDataReader sqlReader = cmd4.ExecuteReader();
                        DataTable dt = new DataTable();
                        dataGridView5.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        dataGridView5.AllowUserToAddRows = false;//Убираем пустую строку
                        dt.Load(sqlReader);
                        dataGridView5.DataSource = dt;
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

        private void button16_Click(object sender, EventArgs e)//Выводи информации о бд
        {
            //Открытие новой формы
            Form9 info = new Form9();
            info.ShowDialog();
            if(db_connect.firstAdd is true)
            {
                db_connect.firstAdd = false;
                MessageBox.Show("Тестовые данные добавлены в таблицы");
                //Добавление тестовых данных в таблицу
                DateTime now = DateTime.Now;
                string dataTime = now.ToString();
                dataTime = (dataTime.Replace(" ", "-")).Replace(":", ".");

                now = DateTime.Now;//Форма для даты
                dataTime = now.ToString("dd/MM/yyyy");

                //Подключение и создание базы данных
                mydb = new sqliteclass();

                sSql = @"insert into ticket (numberticet,fam,name,otchestvo,numbermarsh,path,data,datatwo,value) 
                values('123','Фамилия','Имя','Отчество','105','Владимир',         '" + dataTime + "','" + dataTime + "','500');";
                sSql += @"insert into ticket (numberticet,fam,name,otchestvo,numbermarsh,path,data,datatwo,value)
                values('124','Фамилия','Имя','Отчество','105','Москва',           '" + dataTime + "','" + dataTime + "','500');";
                sSql += @"insert into ticket (numberticet,fam,name,otchestvo,numbermarsh,path,data,datatwo,value)
                values('125','Фамилия','Имя','Отчество','105','Нижний новгород',  '" + dataTime + "','" + dataTime + "','500');";
                sSql += @"insert into ticket (numberticet,fam,name,otchestvo,numbermarsh,path,data,datatwo,value)
                values('126','Фамилия','Имя','Отчество','105','Рязань',           '" + dataTime + "','" + dataTime + "','500');";

                sSql += @"insert into bus    (busnumber,bustype,places) values('А999АА','ЗиЛ','35');";
                sSql += @"insert into driver (fam,name,otchestvo,yearofbird,phonenumber,medecinecard) 
                values('Фамилия','Имя','Отчество','2001','88009601850','999666');";
                sSql += @"insert into path   (pathnumber,pathfrom,pathto,pathtime) values('105','Муром','Рязань','90');";
                sSql += @"insert into stops  (stopfrom,stopto,stopname,price) values('Муром','Москва','Алея','50');";
                sSql += @"insert into trip   (busnumber,pathfrom,pathto,busplaces,driveridfirst,driveridsecond) 
                values('А228УЕ','Муром','Рязань','35','1','2');";

                if (mydb.iExecuteNonQuery(db_connect.path, sSql, 1) == 0)
                {
                    MessageBox.Show($"Ошибка при создании БД");
                    mydb = null;
                    return;
                }

            }           
        }

        private void button7_Click(object sender, EventArgs e)
        {
            operName = "Добавить";
            Form5 Add = new Form5();
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

        private void button8_Click(object sender, EventArgs e)
        {

            // Номер выбранной строки
            int selectedRow = Convert.ToInt32(dataGridView2[5, dataGridView2.CurrentRow.Index].Value);// Столбец, в который выводится id
            transit = selectedRow;
            operName = "Изменить";
            Form5 Add = new Form5();
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

        private void button10_Click(object sender, EventArgs e)//Добавить автобус
        {
            operName = "Добавить";
            Form6 Add = new Form6();
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

        private void button13_Click(object sender, EventArgs e)//Добавить маршрут
        {
            operName = "Добавить";
            Form7 Add = new Form7();
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

        private void button17_Click(object sender, EventArgs e)
        {
            operName = "Добавить";
            Form8 Add = new Form8();
            if (Add.ShowDialog(this) == DialogResult.Cancel)
            {
                if (selectedTab2 == 0)
                {
                    int thisTab = selectedTab2;
                    tabControl2.SelectTab(1);
                    tabControl2.SelectTab(thisTab);
                }
                else
                {
                    int thisTab = selectedTab2;
                    tabControl2.SelectTab(0);
                    tabControl2.SelectTab(thisTab);
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            // Номер выбранной строки
            int selectedRow = Convert.ToInt32(dataGridView2[5, dataGridView2.CurrentRow.Index].Value);// Столбец, в который выводится id

            string show = "\r\nФамилия: " + Convert.ToString(dataGridView2[0, dataGridView2.CurrentRow.Index].Value) + 
                          "\r\nИмя: " + Convert.ToString(dataGridView2[1, dataGridView2.CurrentRow.Index].Value) +
                          "\r\nОтчество: " + Convert.ToString(dataGridView2[2, dataGridView2.CurrentRow.Index].Value);

            DialogResult dr = MessageBox.Show($"Вы действительно хотите удалить запись {show}?", "Удаление записи", MessageBoxButtons.YesNo);

            switch (dr)
            {
                case DialogResult.Yes:

                    mydb = new sqliteclass();
                    sSql = "delete from driver where medecinecard = '" + selectedRow + "' ";
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

        private void button11_Click(object sender, EventArgs e)//Изменить автобус
        {
            string selectedRow = Convert.ToString(dataGridView3[0, dataGridView3.CurrentRow.Index].Value);// Столбец, в который выводится id
            transitStr = selectedRow;
            operName = "Изменить";
            Form6 Add = new Form6();
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

        private void button12_Click(object sender, EventArgs e)//Удалить автобус
        {
            // Номер выбранной строки
            /*int*/ string selectedRow = Convert.ToString(dataGridView3[0, dataGridView3.CurrentRow.Index].Value);// Столбец, в который выводится id

            string show = "\r\nНомер автобуса: " + Convert.ToString(dataGridView3[0, dataGridView3.CurrentRow.Index].Value) +
                "\r\nТип автобуса: " + Convert.ToString(dataGridView3[1, dataGridView3.CurrentRow.Index].Value) +
                "\r\nКоличество мест: " + Convert.ToString(dataGridView3[2, dataGridView3.CurrentRow.Index].Value);

            DialogResult dr = MessageBox.Show($"Вы действительно хотите удалить запись {show}?", "Удаление записи", MessageBoxButtons.YesNo);

            switch (dr)
            {
                case DialogResult.Yes:

                    mydb = new sqliteclass();
                    sSql = "delete from bus where busnumber = '" + selectedRow + "' ";
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

        private void button14_Click(object sender, EventArgs e)
        {
            // Номер выбранной строки
            int selectedRow = Convert.ToInt32(dataGridView4[0, dataGridView4.CurrentRow.Index].Value);// Столбец, в который выводится id
            transit = selectedRow;
            operName = "Изменить";
            Form7 Add = new Form7();
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

        private void button15_Click(object sender, EventArgs e)//Удалить маршрут
        {
            // Номер выбранной строки
            int selectedRow = Convert.ToInt32(dataGridView4[0, dataGridView4.CurrentRow.Index].Value);// Столбец, в который выводится id

            string show = "\r\nНомер маршрута: " + Convert.ToInt32(dataGridView4[0, dataGridView4.CurrentRow.Index].Value) +
                "\r\nМесто отправления: " + Convert.ToString(dataGridView4[1, dataGridView4.CurrentRow.Index].Value) +
                "\r\nМесто назначения: " + Convert.ToString(dataGridView4[2, dataGridView4.CurrentRow.Index].Value) +
                "\r\nВремя в пути: " + Convert.ToInt32(dataGridView4[3, dataGridView4.CurrentRow.Index].Value);

            DialogResult dr = MessageBox.Show($"Вы действительно хотите удалить запись {show}?", "Удаление записи", MessageBoxButtons.YesNo);

            switch (dr)
            {
                case DialogResult.Yes:

                    mydb = new sqliteclass();
                    sSql = "delete from path where pathnumber = '" + selectedRow + "' ";
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

        private void button19_Click(object sender, EventArgs e)//Удалить остановку
        {

            //Выбор компОнента датаГритВью при смене таблицы


            // Номер выбранной строки
            string selectedRow = Convert.ToString(dataGridView5[0, dataGridView5.CurrentRow.Index].Value);// Столбец, в который выводится id
            string secondParam = Convert.ToString(dataGridView5[1, dataGridView5.CurrentRow.Index].Value);
            string thirdParam = Convert.ToString(dataGridView5[2, dataGridView5.CurrentRow.Index].Value);

            string show = "\r\nМесто отправления: " + Convert.ToString(dataGridView5[0, dataGridView5.CurrentRow.Index].Value) +
                "\r\nМесто назначения: " + Convert.ToString(dataGridView5[1, dataGridView5.CurrentRow.Index].Value) +
                "\r\nНазвание остановки: " + Convert.ToString(dataGridView5[2, dataGridView5.CurrentRow.Index].Value) +
                "\r\nЦена: " + Convert.ToInt32(dataGridView5[3, dataGridView5.CurrentRow.Index].Value);

            DialogResult dr = MessageBox.Show($"Вы действительно хотите удалить запись {show}?", "Удаление записи", MessageBoxButtons.YesNo);

            switch (dr)
            {
                case DialogResult.Yes:

                    mydb = new sqliteclass();
                    sSql = "delete from stops where stopfrom = '" + selectedRow + "' and stopto = '" + secondParam + "' and stopname = '" + thirdParam + "' ";
                    if (mydb.iExecuteNonQuery(db_connect.path, sSql, 1) == 0)
                    {
                        MessageBox.Show("Ошибка удаления записи!", "Ошибка");
                        mydb = null;
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Запись удалена", "Готово");
                        if (selectedTab2 == 0)
                        {
                            int thisTab = selectedTab2;
                            tabControl2.SelectTab(1);
                            tabControl2.SelectTab(thisTab);
                        }
                        else
                        {
                            int thisTab = selectedTab2;
                            tabControl2.SelectTab(0);
                            tabControl2.SelectTab(thisTab);
                        }
                    }
                    break;
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            string selectedRow = Convert.ToString(dataGridView5[1, dataGridView5.CurrentRow.Index].Value);// Столбец, в который выводится id
            transitStr = selectedRow;
            operName = "Изменить";
            Form8 Add = new Form8();
            if (Add.ShowDialog(this) == DialogResult.Cancel)
            {
                if (selectedTab2 == 0)
                {
                    int thisTab = selectedTab2;
                    tabControl2.SelectTab(1);
                    tabControl2.SelectTab(thisTab);
                }
                else
                {
                    int thisTab = selectedTab2;
                    tabControl2.SelectTab(0);
                    tabControl2.SelectTab(thisTab);
                }
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            operName = "Добавить";
            Form10 Add = new Form10();
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

        private void button21_Click(object sender, EventArgs e)
        {
            int selectedRow = Convert.ToInt32(dataGridView9[0, dataGridView9.CurrentRow.Index].Value);// Столбец, в который выводится id
            transit = selectedRow;
            operName = "Изменить";
            Form10 Add = new Form10();
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

        private void button22_Click(object sender, EventArgs e)
        {
            int selectedRow = Convert.ToInt32(dataGridView9[0, dataGridView9.CurrentRow.Index].Value);// Столбец, в который выводится id

            string show = "\r\nНомер маршрута: " + Convert.ToInt32(dataGridView9[1, dataGridView9.CurrentRow.Index].Value) +
                "\r\nМесто отправления: " + Convert.ToString(dataGridView9[2, dataGridView9.CurrentRow.Index].Value) +
                "\r\nМесто назначения: " + Convert.ToString(dataGridView9[3, dataGridView9.CurrentRow.Index].Value) +
                "\r\nКоличество мест: " + Convert.ToInt32(dataGridView9[4, dataGridView9.CurrentRow.Index].Value) +
                "\r\nID первого водителя: " + Convert.ToInt32(dataGridView9[5, dataGridView9.CurrentRow.Index].Value);



            DialogResult dr = MessageBox.Show($"Вы действительно хотите удалить запись {show}?", "Удаление записи", MessageBoxButtons.YesNo);

            switch (dr)
            {
                case DialogResult.Yes:

                    mydb = new sqliteclass();
                    sSql = "delete from trip where id = '" + selectedRow + "' ";
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
    }
}
