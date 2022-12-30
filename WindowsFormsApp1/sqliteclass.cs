using System.Data;
using System.Data.SQLite;

namespace WindowsFormsApp1
{
    internal class db_connect
    {
        public static SQLiteConnection conn;
        public static SQLiteCommand cmd;
        public static SQLiteDataReader reader;
        public static string commandText;
        public static string path;
        public static bool firstAdd = false;
    }
    class sqliteclass
    {
        //Конструктор
        public sqliteclass(){}
        public int iExecuteNonQuery(string FileData, string sSql, int where)
        {
            int n = 0;
            try
            {
                using (SQLiteConnection con = new SQLiteConnection())
                {
                    if (where == 0)
                    {
                        con.ConnectionString = @"Data Source=" + FileData + ";New=True;Version=3";
                    }
                    else
                    {
                        con.ConnectionString = @"Data Source=" + FileData + ";New=False;Version=3";
                    }
                    con.Open();
                    using (SQLiteCommand sqlCommand = con.CreateCommand())
                    {
                        sqlCommand.CommandText = sSql;
                        n = sqlCommand.ExecuteNonQuery();
                    }
                    con.Close();
                }
            }
            catch 
            {
                n = 0;
            }
            return n;
        }
        /*public DataRow[] drExecute(string FileData, string sSql)
        {
            DataRow[] datarows = null;
            SQLiteDataAdapter dataadapter = null;
            DataSet dataset = new DataSet();
            DataTable datatable = new DataTable();
            try
            {
                using (SQLiteConnection con = new SQLiteConnection())
                {
                    con.ConnectionString = @"Data Source=" + FileData + ";New=False;Version=3";
                    con.Open();
                    using (SQLiteCommand sqlCommand = con.CreateCommand())
                    {
                        dataadapter = new SQLiteDataAdapter(sSql, con);
                        dataset.Reset();
                        dataadapter.Fill(dataset);
                        datatable = dataset.Tables[0];
                        datarows = datatable.Select();
                    }
                    con.Close();
                }
            }
            catch 
            {
                datarows = null;
            }
            return datarows;
        }*/
    }
}
