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

        System.Data.OleDb.OleDbCommandBuilder scb;
        DataSet ds = new DataSet();

        public static System.Data.OleDb.OleDbConnection CreateConnection()
        {

            System.Data.OleDb.OleDbConnection MyConnection;
            System.Data.OleDb.OleDbCommand mycommand = new System.Data.OleDb.OleDbCommand();
            //string database_loc = "'G:\\N016_17 - Dogradnja Cognex DM čitača datamatrix koda na stroju za mjerenje sile uprešavanja\\databaza.xlsx'";
            //string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Extended Properties=Excel 12.0; Data Source="+database_loc +";";
        
            string database_loc = "'G:\\N016_17 - Dogradnja Cognex DM čitača datamatrix koda na stroju za mjerenje sile uprešavanja\\database_access.accdb'";

            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;  Persist Security Info = False; Data Source=" + database_loc +";";


            //Provider = Microsoft.ACE.OLEDB.12.0; Data Source = C:\myFolder\myAccessFile.accdb;
           

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

        private void button1_Click(object sender, EventArgs e)
        {
            System.Data.OleDb.OleDbConnection MyConnection = CreateConnection();

            string m_selectSQL = "SELECT * FROM Popis_komada";
           
            dataAdapter = new System.Data.OleDb.OleDbDataAdapter(m_selectSQL, MyConnection);


            dataAdapter.Fill(dataTable);
       

            dataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

            dataGridView1.DataSource = dataTable;
       
            MyConnection.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                System.Data.OleDb.OleDbConnection MyConnection = CreateConnection();

                scb = new System.Data.OleDb.OleDbCommandBuilder(dataAdapter);

               // dataAdapter.DeleteCommand = scb.GetDeleteCommand(true);
                dataAdapter.UpdateCommand = scb.GetUpdateCommand(true);
                dataAdapter.UpdateCommand = MyConnection.CreateCommand();
              //  dataAdapter.InsertCommand = scb.GetInsertCommand(true);

                //dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns["ID"] };
                //dataAdapter.UpdateCommand = scb.GetUpdateCommand();          
                dataAdapter.Update(dataTable);
                MyConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void open_viewer_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.ShowDialog();
        }
    }
}
