using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cognex_tesanj
{
    public partial class Form1 : Form
    {

        DataSet myset = new DataSet("Excel import");
        DataTable dataTable = new DataTable("excelImport");
        System.Data.OleDb.OleDbDataAdapter dataAdapter;
        //System.Data.

        System.Data.OleDb.OleDbCommandBuilder scb;
        DataSet ds = new DataSet();

        public static System.Data.OleDb.OleDbConnection CreateConnection()
        {

            System.Data.OleDb.OleDbConnection MyConnection;
            System.Data.OleDb.OleDbCommand mycommand = new System.Data.OleDb.OleDbCommand();
            string Db_Password = "0000";
            string database_loc = "'G:\\N016_17 - Dogradnja Cognex DM čitača datamatrix koda na stroju za mjerenje sile uprešavanja\\database_access.accdb'";
            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0; Jet OLEDB:Database Password=" + Db_Password + "; Persist Security Info = False; Data Source=" + database_loc +";";       

            MyConnection = new System.Data.OleDb.OleDbConnection(connectionString);
            MyConnection.Open();
            return MyConnection;
        }

        public static void ReadData(DataTable data)
        {
            foreach (DataRow dataRow in data.Rows)
            {
                foreach (var item in dataRow.ItemArray)
                {
                    Console.WriteLine(item);
                }
            }
        }

        public Form1()
        {
            InitializeComponent();

            try
            {

                System.Data.OleDb.OleDbConnection MyConnection = CreateConnection();
                MyConnection.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void open_viewer_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainForm cognex_form = new MainForm();
            cognex_form.Show();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Button pressed", "Title"); 
            //MessageBox.Show()
        }
    }
}
