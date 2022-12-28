using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        }
    }
}
