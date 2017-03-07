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


        DataTable dataTable = new DataTable("excelImport");
        OdbcDataAdapter dataAdapter;
        OdbcCommandBuilder scb;
        DataSet ds = new DataSet();

        public static System.Data.Odbc.OdbcConnection CreateConnection()
        {
            System.Data.Odbc.OdbcConnection MyConnection;
            System.Data.Odbc.OdbcCommand mycommand = new System.Data.Odbc.OdbcCommand();

            //string database_loc = "C:\\databaza.xlsx;";
            string database_loc = "G:\\N016_17 - Dogradnja Cognex DM čitača datamatrix koda na stroju za mjerenje sile uprešavanja\\databaza.xlsx;";

            string connectionString = @"Driver={Microsoft Excel Driver (*.xls, *.xlsx, *.xlsm, *.xlsb)};DBQ=" + database_loc + ";";
            MyConnection = new System.Data.Odbc.OdbcConnection(connectionString);
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

                System.Data.Odbc.OdbcConnection MyConnection = CreateConnection();

                MyConnection.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Data.Odbc.OdbcConnection MyConnection = CreateConnection();

            string m_selectSQL = "SELECT * FROM [Sheet1$]";

            dataAdapter = new OdbcDataAdapter(m_selectSQL, MyConnection);
            //dataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

            dataAdapter.Fill(dataTable);
            dataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

            DataColumn[] columns = new DataColumn[1];
            columns[0] = dataTable.Columns["ID"];
            dataTable.PrimaryKey = columns;
            dataGridView1.DataSource = dataTable;

            MyConnection.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                OdbcConnection MyConnection = CreateConnection();

                //scb = new OdbcCommandBuilder(MyConnection);
                scb = new OdbcCommandBuilder(dataAdapter);

                dataAdapter.DeleteCommand = scb.GetDeleteCommand(true);
                dataAdapter.UpdateCommand = scb.GetUpdateCommand(true);
                dataAdapter.InsertCommand = scb.GetInsertCommand(true);


                dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns["ID"] };
                dataAdapter.UpdateCommand = scb.GetUpdateCommand();


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
    }
}
