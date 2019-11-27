using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
namespace Game
{
    public partial class Screen : Form
    {
        int gameid; 
        OleDbConnection conn = null;//定义数据库连接
        public Screen(int id)
        {
            InitializeComponent();
            conn = database.dbcon.getConnection();//获取数据库连接
            gameid = id;
            updateListView(gameid);
        }
        public Screen()
        {
            InitializeComponent();
            conn = database.dbcon.getConnection();//获取数据库连接
            gameid = 0;
            this.listView1.Hide();
            this.label2.Text = "暂时无最新完成的比赛";

        }
        public void updateListView(int gameid)
        {
            this.listView1.Show();
            this.label2.Text = "当前最新完成的比赛赛况如下";
            this.listView1.Items.Clear();
            this.listView1.Columns.Clear();
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("项目", 2 * this.listView1.Width / 8, HorizontalAlignment.Left);
            listView1.Columns.Add("排名", this.listView1.Width / 8, HorizontalAlignment.Left);
            listView1.Columns.Add("姓名", this.listView1.Width / 8, HorizontalAlignment.Left);
            listView1.Columns.Add("ID", this.listView1.Width / 8, HorizontalAlignment.Left);
            listView1.Columns.Add("D分", this.listView1.Width / 8, HorizontalAlignment.Left);
            listView1.Columns.Add("P分", this.listView1.Width / 8, HorizontalAlignment.Left);
            listView1.Columns.Add("总分", this.listView1.Width / 8, HorizontalAlignment.Left);
            string getAll = string.Format("select rank,name,peopleid,d,p,finalscore from result where gameid = {0} order by rank", gameid);
            OleDbDataAdapter adapter = new OleDbDataAdapter(getAll, conn);
            DataTable table = new DataTable();
            adapter.Fill(table);
            OleDbDataReader reader;
            for (int i = 0;i<table.Rows.Count;i++)
            {
                string getTypeAndPF = string.Format("select type,preorfin from gametype where gameid = {0}",gameid);
                OleDbCommand command = new OleDbCommand(getTypeAndPF, conn);
                reader = command.ExecuteReader();
                string game = "";
                while (reader.Read())
                {
                     game = reader["type"].ToString();
                    if (reader["preorfin"].ToString() == "p")
                        game = game + "（初赛）";
                    else
                        game = game + "（决赛）";
                }
                ListViewItem temp = new ListViewItem(game);
                temp.SubItems.Add(table.Rows[i].ItemArray[0].ToString());
                temp.SubItems.Add(table.Rows[i].ItemArray[1].ToString());
                temp.SubItems.Add(table.Rows[i].ItemArray[2].ToString());
                temp.SubItems.Add(table.Rows[i].ItemArray[3].ToString());
                temp.SubItems.Add(table.Rows[i].ItemArray[4].ToString());
                temp.SubItems.Add(table.Rows[i].ItemArray[5].ToString());
                listView1.Items.Add(temp);
            }
        }
        private void Screen_Load(object sender, EventArgs e)
        {

        }
    }
}
