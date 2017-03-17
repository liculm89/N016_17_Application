using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cognex_tesanj
{
    public partial class Form2 : Form
    {

        static string database_loc = "'G:\\N016_17 - Dogradnja Cognex DM čitača datamatrix koda na stroju za mjerenje sile uprešavanja\\database_access.accdb'";
        static string Db_Password = "0000";
        static string conString = "Provider=Microsoft.ACE.OLEDB.12.0; Jet OLEDB:Database Password="+ Db_Password +"; Persist Security Info = False; Data Source=" + database_loc + ";";
        

        OleDbConnection con = new OleDbConnection(conString);
        OleDbCommand cmd;
        OleDbDataAdapter adapter;
        DataTable dt = new DataTable();

        public Form2()
        {
            InitializeComponent();

            //DATAGRIDVIEW PROPERTIES
            dataGridView1.ColumnCount = 3;
            dataGridView1.Columns[0].Name = "ID";
            dataGridView1.Columns[1].Name = "Datamatrix";
            dataGridView1.Columns[2].Name = "Graf";
            //dataGridView1.Columns[3].Name = "Team";
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            //SELECTION MODE
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            //String timeStamp = DateTime.Now.ToString();
           // Console.WriteLine(timeStamp);
        }

        //INSERT INTO DB
        private void add(string dm, string graf)
        {
            string sql = "INSERT INTO Popis_komada(Datamatrix,Graf) VALUES(@Datamatrix,@Graf)";

            cmd = new OleDbCommand(sql, con);

            //ADD PARAMS
            cmd.Parameters.AddWithValue("@Datamatrix", dm);
            cmd.Parameters.AddWithValue("@Graf", graf);
            //cmd.Parameters.AddWithValue("@TEAM", team);
            //OPEN CON AND EXEC insert
            try
            {
                con.Open();
                if (cmd.ExecuteNonQuery() > 0)
                {
                    clearTxts();
                    MessageBox.Show("Successfully Inserted");
                }
                con.Close();
                retrieve();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
        }

        //FILL DGVIEW
        private void populate(string id, string dm, string graf)
        {
            dataGridView1.Rows.Add(id, dm, graf);
        }
        //RETRIEVAL OF DATA
        private void retrieve()
        {
            dataGridView1.Rows.Clear();
            //SQL STMT
            String sql = "SELECT * FROM Popis_komada";
            cmd = new OleDbCommand(sql, con);
            try
            {
                con.Open();
                adapter = new OleDbDataAdapter(cmd);
                adapter.Fill(dt);
                //LOOP THRU DT
                foreach (DataRow row in dt.Rows)
                {
                    populate(row[0].ToString(), row[1].ToString(), row[2].ToString());
                }
                dt.PrimaryKey = new DataColumn[] { dt.Columns["ID"] };

                con.Close();

                //CLEAR DT 
                dt.Rows.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
        }
        //UPDATE DB
        private void update(int id, string dm, string graf)
        {
            //SQL STMT
            string sql = "UPDATE Popis_komada SET Datamatrix='" + dm + "',Graf='" + graf + "' WHERE ID=" + id + "";
            cmd = new OleDbCommand(sql, con);
            //OPEN CON,UPDATE,RETRIEVE DGVIEW
            try
            {
                con.Open();
                adapter = new OleDbDataAdapter(cmd);
                adapter.UpdateCommand = con.CreateCommand();
                adapter.UpdateCommand.CommandText = sql;
                if (adapter.UpdateCommand.ExecuteNonQuery() > 0)
                {
                    clearTxts();
                    MessageBox.Show("Successfully Updated");
                }
                con.Close();
                //REFRESH
                retrieve();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
        }
        //DELETE FROM DB
        private void delete(int id)
        {
            //SQL STMT
            String sql = "DELETE FROM Popis_komada WHERE ID=" + id + "";
            cmd = new OleDbCommand(sql, con);
            //'OPEN CON,EXECUTE DELETE,CLOSE CON
            try
            {
                con.Open();
                adapter = new OleDbDataAdapter(cmd);
                adapter.DeleteCommand = con.CreateCommand();
                adapter.DeleteCommand.CommandText = sql;
                //PROMPT FOR CONFIRMATION
                if (MessageBox.Show("Sure ??", "DELETE", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("Successfully deleted");
                    }
                }
                con.Close();
                retrieve();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
        }
        //CLEAR TXT
        private void clearTxts()
        {
            datamatrix_txt.Text = "";
            graf_txt.Text = "";
           // teamtxt.Text = "";
        }
        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
           datamatrix_txt.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
           graf_txt.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
            //teamtxt.Text = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
        }
        private void addBtn_Click(object sender, EventArgs e)
        {
            add(datamatrix_txt.Text, graf_txt.Text);
        }
        private void retrieveBtn_Click(object sender, EventArgs e)
        {
            retrieve();
        }
        private void updateBtn_Click(object sender, EventArgs e)
        {
            String selected = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            int id = Convert.ToInt32(selected);
            update(id, datamatrix_txt.Text, graf_txt.Text);
        }
        private void deleteBtn_Click(object sender, EventArgs e)
        {
            String selected = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            int id = Convert.ToInt32(selected);
            delete(id);
        }
        private void clearBtn_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            clearTxts();
        }

        private void graf_txt_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            //datamatrix_txt.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
           // graf_txt.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
           // datamatrix_txt.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
           // graf_txt.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
        }

        private void dataGridView1_MouseClick_1(object sender, MouseEventArgs e)
        {
            if (dataGridView1.SelectedRows[0].Cells[1].Value == null)
            {
                datamatrix_txt.Text = "";
                graf_txt.Text = "";
            }
            else
            {
                datamatrix_txt.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                graf_txt.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
            }
        }
        /*
private void retrieveBtn_Click(object sender, EventArgs e)
{
retrieve();
}*/
    }
}
