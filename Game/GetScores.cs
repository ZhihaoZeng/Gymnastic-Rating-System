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
    public partial class GetScores : Form
    {

        OleDbConnection conn = null;//定义数据库连接
        string teamname;//正在执行操作的队名
        public GetScores()
        {
            InitializeComponent();
        }
        public GetScores(string teamname)
        {
            InitializeComponent();
            conn = database.dbcon.getConnection();//获取数据库连接
            this.teamname = teamname;
            updateListView();
        }

        public void updateListView()
        {
            this.listView1.Show();
            this.listView1.Items.Clear();
            this.listView1.Columns.Clear();
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("姓名", this.listView1.Width / 12, HorizontalAlignment.Left);
            listView1.Columns.Add("项目", this.listView1.Width / 12, HorizontalAlignment.Left);
            listView1.Columns.Add("初/决赛", this.listView1.Width / 12, HorizontalAlignment.Left);
            listView1.Columns.Add("排名", this.listView1.Width / 12, HorizontalAlignment.Left);
            listView1.Columns.Add("成绩1", this.listView1.Width / 12, HorizontalAlignment.Left);
            listView1.Columns.Add("成绩2", this.listView1.Width / 12, HorizontalAlignment.Left);
            listView1.Columns.Add("成绩3", this.listView1.Width / 12, HorizontalAlignment.Left);
            listView1.Columns.Add("成绩4", this.listView1.Width / 12, HorizontalAlignment.Left);
            listView1.Columns.Add("成绩5", this.listView1.Width / 12, HorizontalAlignment.Left);
            listView1.Columns.Add("D", this.listView1.Width / 12, HorizontalAlignment.Left);
            listView1.Columns.Add("P", this.listView1.Width / 12, HorizontalAlignment.Left);
            listView1.Columns.Add("总成绩", this.listView1.Width / 11, HorizontalAlignment.Left);
            string getscores = string.Format("select name, type, preorfin,rank, score1,score2,score3,score4" +
                ",score5,D,P,finalscore, peopleid  id from result , gametype where result.gameid = gametype.gameid and exists" +
                "(select * from people where result.peopleid = people.peopleid and teamname = '{0}')order by name ", teamname);
            //getscores = "select name, preorfin,rank, score1,score2,score3,score4,score5,D,P,finalscore, peopleid  id from result" +
            //    "";
            OleDbDataAdapter adapter = new OleDbDataAdapter(getscores, conn);
            DataTable table = new DataTable();
            adapter.Fill(table);
            for(int i =0;i<table.Rows.Count;i++)
            {
                ListViewItem temp = new ListViewItem(table.Rows[i].ItemArray[0].ToString());
                temp.SubItems.Add(table.Rows[i].ItemArray[1].ToString());
                temp.SubItems.Add(table.Rows[i].ItemArray[2].ToString());
                temp.SubItems.Add(table.Rows[i].ItemArray[3].ToString());
                temp.SubItems.Add(table.Rows[i].ItemArray[4].ToString());
                temp.SubItems.Add(table.Rows[i].ItemArray[5].ToString());
                temp.SubItems.Add(table.Rows[i].ItemArray[6].ToString());
                temp.SubItems.Add(table.Rows[i].ItemArray[7].ToString());
                temp.SubItems.Add(table.Rows[i].ItemArray[8].ToString());
                temp.SubItems.Add(table.Rows[i].ItemArray[9].ToString());
                temp.SubItems.Add(table.Rows[i].ItemArray[10].ToString());
                temp.SubItems.Add(table.Rows[i].ItemArray[11].ToString());
                listView1.Items.Add(temp);
            }
        }

        private void GetScores_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OptionsForTeam o = new OptionsForTeam(teamname);
            o.Show();
            this.Close();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            updateListView();
        }

        private void GetScores_Load_1(object sender, EventArgs e)
        {

        }
    }
}
