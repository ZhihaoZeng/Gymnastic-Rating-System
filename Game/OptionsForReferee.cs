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
    public partial class OptionsForReferee : Form
    {
        OleDbConnection conn = null;//定义数据库连接
        string refereename;
        int gametograde = -1;
        int peopletograde = -1;
        int peopleno = -1;
        public OptionsForReferee()
        {
            InitializeComponent();
            

        }
  
        public OptionsForReferee(string refereename)
        {
            InitializeComponent();
            conn = database.dbcon.getConnection();//获取数据库连接
            this.refereename = refereename;
            label2.Text = refereename;
            updateGameToGrade();
            welcome();
        }
        private void OptionsForReferee_Load(object sender, EventArgs e)
        {

        }
        public void welcome()
        {
            //检测自己的gametograde与peopletograde
            //有比赛需要自己打分才显示listview
            if(gametograde!=-1&&peopletograde!=-1)
            {
                updateListView();
            }
            else
            {
                MessageBox.Show("您当前无需要打分的比赛！");
                return;
            }
        }
        public void updateGameToGrade()
        {
            //从referee表中查询更新gametograde,peopletograde
            string findGameAndPeopleID = "select * from referee where name = '"+refereename+"' ";
            OleDbCommand command = new OleDbCommand(findGameAndPeopleID, conn);
            OleDbDataReader myreader = command.ExecuteReader();
            while (myreader.Read())
            {
                try
                {
                    gametograde = Convert.ToInt32(myreader["GAMETOGRADE"]);
                    peopletograde = Convert.ToInt32(myreader["PEOPLETOGRADE"]);
                }
               catch(Exception e)
                {//当暂时没有需要打分的比赛时，直接退出该函数
                    return;
                }

            }
        }
        public void updateListView()
        {
            this.listView1.Items.Clear();
            this.listView1.Columns.Clear();
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("姓名", this.listView1.Width / 3, HorizontalAlignment.Left);
            listView1.Columns.Add("ID", this.listView1.Width / 3, HorizontalAlignment.Left);
            listView1.Columns.Add("项目", this.listView1.Width / 3, HorizontalAlignment.Left);
            string emm = string.Format("select * from result where peopleid  = {0} AND gameid = {1}", peopletograde, gametograde);
            OleDbDataAdapter adapter = new OleDbDataAdapter(emm, conn);
            DataTable table = new DataTable();
            adapter.Fill(table);
            ListViewItem temp = null;


            string type = "";
            string getType = string.Format("select type T from gametype where gameid = {0}", gametograde);
            OleDbCommand command = new OleDbCommand(getType, conn);
            OleDbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                type = reader["T"].ToString();
            }
            temp = new ListViewItem(table.Rows[0].ItemArray[1].ToString());
            temp.SubItems.Add(table.Rows[0].ItemArray[2].ToString());
            temp.SubItems.Add(type);
            listView1.Items.Add(temp);
        }
        public bool grade()
        {
            //目前无待打分比赛而强行打分
            if(gametograde==-1||peopletograde==-1)
            {
                MessageBox.Show("您当前无需要打分的比赛！");
                return false;
            }
            //实现打分细节
            //获取textbox
            if (textBox1.Text == "")
            {
                MessageBox.Show("请输入分数！");
                return false;
            }
            int score = Convert.ToInt32(textBox1.Text);
            //去gametype中一个个比对得到自己是referee几
            string findRefereeRank = string.Format("select referee1,referee2,referee3,referee4,referee5"
                +" from gametype where gameid={0}",gametograde);
            OleDbCommand command = new OleDbCommand(findRefereeRank,conn);
            OleDbDataReader myreader = command.ExecuteReader();
            int rank = 0;
            while(myreader.Read())
            {
                if(refereename==myreader["referee1"].ToString())
                {
                    rank = 1;
                }
                else if (refereename == myreader["referee2"].ToString())
                {
                    rank = 2;
                }
                else if (refereename == myreader["referee3"].ToString())
                {
                    rank = 3;
                }
                else if (refereename == myreader["referee4"].ToString())
                {
                    rank = 4;
                }
                else if (refereename == myreader["referee5"].ToString())
                {
                    rank = 5;
                }
            }
            //分数更新到result中对应位数的score几中
            string getIfScored = string.Format("select score{0} s from result where peopleid = {1} and gameid = {2}", rank, peopletograde, gametograde);
            OleDbCommand com = new OleDbCommand(getIfScored, conn);
            OleDbDataReader reader = com.ExecuteReader();
            while(reader.Read())
            {
                if (Convert.ToInt32(reader["s"]) != -1)
                {
                    MessageBox.Show("已经打过分，不可重新打分！");
                    return false;
                }
            }
            string updatescore = string.Format("update result set score{0} = {1},finalscore = finalscore +1"
                +" where peopleid = {2} and gameid = {3}", rank, score, peopletograde, gametograde);
            command = new OleDbCommand(updatescore, conn);
            int x = command.ExecuteNonQuery();
            if(x<=0)
            {
                MessageBox.Show("上传分数失败");
                return false;
            }
            //打完分将referee表中自己的gametograde,peopletograde设置为-1
            string updateSelf = string.Format("update referee set gametograde = -1,peopletograde = -1 where name = '{0}'", refereename);
            command = new OleDbCommand(updateSelf, conn);
            x = command.ExecuteNonQuery();
            MessageBox.Show("打分成功！！"); 
            return true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {//打分
            bool indicator = grade();
            //if (!indicator)
            //{
            //    return;
            //}
            ////检测是否所有裁判都打分完成(不考虑裁判数小于五的情况)
            //string find = string.Format("select * from result where peopleid = {0} and gameid = {1}",peopletograde,gametograde);
            //OleDbCommand command = new OleDbCommand(find, conn);
            //OleDbDataReader myreader = command.ExecuteReader();
            //while(myreader.Read())
            //{
            //    if(Convert.ToInt32(myreader["score1"])==-1)
            //    {
            //        indicator = false;
            //    }
            //    else if(Convert.ToInt32(myreader["score2"]) == -1)
            //    {
            //        indicator = false;
            //    }
            //    else if (Convert.ToInt32(myreader["score3"]) == -1)
            //    {
            //        indicator = false;
            //    }
            //    else if (Convert.ToInt32(myreader["score4"]) == -1)
            //    {
            //        indicator = false;
            //    }
            //    else if (Convert.ToInt32(myreader["score5"]) == -1)
            //    {
            //        indicator = false;
            //    }
            //}
            //if(indicator)//所有裁判都打分完成,完成了后将finalscore设置为101
            //{
            //    string updateself = string.Format("update result set finalscore = 101 where peopleid = {0} and gameid = {1}", peopletograde, gametograde);
            //    command = new OleDbCommand(updateself, conn);
            //    int x = command.ExecuteNonQuery();
            //    if(x<=0)
            //    {
            //        MessageBox.Show("更新101失败");
            //    }
            //}
        }

        private void button3_Click(object sender, EventArgs e)
        {
            updateGameToGrade();
            welcome();
        }
    }
}
