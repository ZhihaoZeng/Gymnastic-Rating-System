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
    public partial class OptionsForMainReferee : Form
    {
        Screen screen;
        OleDbConnection conn = null;//定义数据库连接
        string mainrefereename;
        int nextgameid;//现在要打分的比赛
        int peopleid;//现在要打分的人
        int peopleno;
        double prescore = 0;
        bool indicator = false;//指示是否可以确认打分
        DataTable table;
        public OptionsForMainReferee(Screen s)
        {
            InitializeComponent();
            conn = database.dbcon.getConnection();//获取数据库连接
            screen = s;
           
            updateIndicator();
            updateListView();
        }
        public void updateIndicator()
        {
            //从nextgameID中去获取gameid,peopleno到自己的nextgameid,peopleid中
            string findGameAndPeopleID = "select * from NEXTGAMEID ";
            OleDbCommand command = new OleDbCommand(findGameAndPeopleID, conn);
            OleDbDataReader myreader = command.ExecuteReader();
            while (myreader.Read())
            {
                nextgameid = Convert.ToInt32(myreader["GAMEID"]);
                peopleno = Convert.ToInt32(myreader["PEOPLENO"]);
            }

            //查找maxgameid，看是否全部都评完了
            string getmax = "select id from maxgameid";
            command = new OleDbCommand(getmax, conn);
            myreader = command.ExecuteReader();
            while(myreader.Read())
            {
                if (nextgameid == Convert.ToInt32(myreader["id"]))
                {
                    MessageBox.Show("当前阶段(初赛或决赛)全部比赛已经打完");
                    return;
                }
            }

            //通过peopleno初始化peopleid
            string initiatePeopleid = string.Format("select peopleid from result where gameid = {0}", nextgameid);
            OleDbDataAdapter adapter = new OleDbDataAdapter(initiatePeopleid, conn);
            table = new DataTable();
            adapter.Fill(table);
            try
            {
                peopleid = Convert.ToInt32(table.Rows[peopleno].ItemArray[0]);

            }
            catch (Exception e)
            {
                MessageBox.Show("初赛（决赛）已经全部完成");
            }


            //通过nextgameID和peopleid去result中查看finalscore的值
            //如果值是-4，则表示可以裁定分数了
            string findFinalScore = string.Format("select FINALSCORE from result where peopleid = {0} and gameid = {1}", peopleid, nextgameid);
             command = new OleDbCommand(findFinalScore, conn);
             myreader = command.ExecuteReader();
            int finalscore;
            while (myreader.Read())
            {
                finalscore = Convert.ToInt32(myreader["finalscore"]);
                if (finalscore == -4)
                {
                    indicator = true;
                    MessageBox.Show("可以确定打分了！");
                }
                else
                {  //否则
                    indicator = false;
                    MessageBox.Show("您当前不能打分");
                    return;
                }
            }
        }
        
        public void updateListView()
        {
            //此处先预先计算好五个裁判的预处理分数存入prescore
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


            this.listView1.Items.Clear();
            this.listView1.Columns.Clear();
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("项目",2*this.listView1.Width / 9, HorizontalAlignment.Left);
            listView1.Columns.Add("名称", this.listView1.Width / 9, HorizontalAlignment.Left);
            listView1.Columns.Add("分数1", this.listView1.Width /9, HorizontalAlignment.Left);
            listView1.Columns.Add("分数2", this.listView1.Width /9, HorizontalAlignment.Left);
            listView1.Columns.Add("分数3", this.listView1.Width /9, HorizontalAlignment.Left);
            listView1.Columns.Add("分数4", this.listView1.Width /9, HorizontalAlignment.Left);
            listView1.Columns.Add("分数5", this.listView1.Width /9, HorizontalAlignment.Left);
            listView1.Columns.Add("总分", this.listView1.Width / 9, HorizontalAlignment.Left);
            string emm = string.Format("select * from result where peopleid = {0} AND gameid = {1}", peopleid, nextgameid);
            OleDbCommand command = new OleDbCommand(emm, conn);
            OleDbDataReader myreader = command.ExecuteReader();
            while (myreader.Read())
            {
                string a = string.Format("select type,preorfin from gametype where gameid = {0}",nextgameid);
                string type = "";
                OleDbCommand c = new OleDbCommand(a,conn);
                OleDbDataReader reader2 = c.ExecuteReader();
                string pf = "(初赛）";
                while (reader2.Read())
                {
                    type = reader2["type"].ToString();
                    if (reader2["PREORFIN"].ToString() != "p")
                    {
                        pf = "（决赛）";
                    }
                }
                ListViewItem temp = new ListViewItem(type+pf);
                temp.SubItems.Add(myreader["name"].ToString());
                temp.SubItems.Add(myreader["score1"].ToString());
                temp.SubItems.Add(myreader["score2"].ToString());
                temp.SubItems.Add(myreader["score3"].ToString());
                temp.SubItems.Add(myreader["score4"].ToString());
                temp.SubItems.Add(myreader["score5"].ToString());
                prescore = CalculateScore(Convert.ToInt32(myreader["score1"]), Convert.ToInt32(myreader["score2"]), Convert.ToInt32(myreader["score3"]), Convert.ToInt32(myreader["score4"]), Convert.ToInt32(myreader["score5"]));
                temp.SubItems.Add(prescore.ToString());
                listView1.Items.Add(temp);
            }
        }
        public double CalculateScore(int a, int b, int c, int d, int e)
        {//计算粗总分
            double score = 0;
            int[] g = new int[5];
            g[0] = a;
            g[1] = b;
            g[2] = c;
            g[3] = d;
            g[4] = e;
            Array.Sort(g);
            score = 5 * (g[1]+g[2]+g[3])/(3.0);
            return score;
        }
        public OptionsForMainReferee(string mainrefereename)
        {
            InitializeComponent();
            this.mainrefereename = mainrefereename;
        }
        public bool confirm()
        {//确认打分
            int p, d;
            //获取p，d分（文本框中的string转换成int）
            if (textBox1.Text == "")
            {
                MessageBox.Show("请输入奖励分！");
                return false;
            }
            else
            {
                d = Convert.ToInt32(textBox1.Text);
            }
            if (textBox2.Text == "")
            {
                MessageBox.Show("请输入惩罚分！");
                return false;
            }
            else
            {
                p = Convert.ToInt32(textBox2.Text);
            }

            //把之前计算出的prescore+D-P,存入prescore
            prescore = prescore + d - p;

            //更新prescore到finalscore
            string setFinalScore = string.Format("update result set finalscore = {0} ,D={3},P={4}"
                + "where peopleid = {1} and gameid = {2}", prescore, peopleid, nextgameid,d,p);
            OleDbCommand command = new OleDbCommand(setFinalScore, conn);
            int x = command.ExecuteNonQuery();
            if (x <= 0)
            //如果影响行数大于0则说明插入成功，否则的话插入失败
            {
                MessageBox.Show("更新finalscore和D,P失败！请尝试重新确认");
                return false;
            }

            //判断peopleno是否等于result表中查询到的这场比赛参加的人数
            int peoplecount = 0;
            string findPEOPLECOUNT = string.Format("select  peoplecount from gametype where gameid = {0}", nextgameid);
            command = new OleDbCommand(findPEOPLECOUNT, conn);
            OleDbDataReader myreader = command.ExecuteReader();
            while (myreader.Read())
            {
                peoplecount = Convert.ToInt32(myreader["peoplecount"]);
            }
            if (peoplecount  == peopleno+1)
            {//等于则gameid+1并且更新到nextgameID中，peopleno更新为0
                string updateNextgameid = string.Format("update NEXTGAMEID set gameid = gameid + 1,peopleno = 0 where gameid = {0}", nextgameid);
                command = new OleDbCommand(updateNextgameid, conn);
                x = command.ExecuteNonQuery();
                if (x <= 0)
                //如果影响行数大于0则说明插入成功，否则的话插入失败
                {
                    MessageBox.Show("更新nextgameid失败！");
                    return false;
                }
            }
            else
            {//否则更新nextgameid中的peopleid+1为下一个人
                string updatepeopleid = string.Format("update nextgameid set peopleno = peopleno + 1 where gameid = {0}", nextgameid);
                command = new OleDbCommand(updatepeopleid, conn);
                x = command.ExecuteNonQuery();
                if (x <= 0)
                //如果影响行数大于0则说明插入成功，否则的话插入失败
                {
                    MessageBox.Show("更新peopleID失败！");
                    return false;
                }
            }
            indicator = false;
            
            return true;
        }
        public void reject()
        {//重新打分
         //用nextgameid去gametype中查找到所有的裁判，
            string[] referees = new string[5];
            string findrefereee = string.Format("select referee1,referee2,referee3,referee4,referee5 from gametype where gameid = {0}", nextgameid);
            OleDbCommand command = new OleDbCommand(findrefereee, conn);
            OleDbDataReader myreader = command.ExecuteReader();
            while (myreader.Read())
            {
                referees[0] = myreader["referee1"].ToString();
                referees[1] = myreader["referee2"].ToString();
                referees[2] = myreader["referee3"].ToString();
                referees[3] = myreader["referee4"].ToString();
                referees[4] = myreader["referee5"].ToString();
            }
            //通过循环，将他们的检测自己的gametograde,peopletograde更新成nextgameid,peopleno
            for (int i = 0; i < 5; i++)
            {
                string updateGAMETOGRADEandPEOPLETOGRADE = string.Format("update referee set"
                    +" GAMETOGRADE = {0},PEOPLETOGRADE = {1} where name = '{2}'",nextgameid,peopleid,referees[i]);
                command = new OleDbCommand(updateGAMETOGRADEandPEOPLETOGRADE, conn);
                int a = command.ExecuteNonQuery();
                if (a <= 0)
                //如果影响行数大于0则说明插入成功，否则的话插入失败
                {
                    MessageBox.Show("通知裁判重新打分失败！");
                    return;
                }
            }
            //将finalscore从101设置为-9
            string setbackFINALSCORE = string.Format("update result set "
                    + "FINALSCORE = -9 where peopleid = {0} and gameid = {1}",  peopleid, nextgameid);
            command = new OleDbCommand(setbackFINALSCORE, conn);
            int x = command.ExecuteNonQuery();
            if (x <= 0)
            //如果影响行数大于0则说明插入成功，否则的话插入失败
            {
                MessageBox.Show("取消finalscore为-5失败！");
                return;
            }
            string setbackScore1to5 = string.Format("update result set score1=-1,score2=-1,score3=-1,score4=-1,score5=-1");
            command = new OleDbCommand(setbackScore1to5, conn);
            int s = command.ExecuteNonQuery();
            if (s <= 0)
            {
                MessageBox.Show("设置score1-5重新为-1失败");
                return ;
            }
            MessageBox.Show("已通知裁判重新打分！");

            updateListView();
        }
        private void OptionsForMainReferee_Load(object sender, EventArgs e)
        {

        }
        public bool rank( int gameid )
        {
            string getPeopleCount = string.Format("select count(*) C from result where finalscore = -9 and gameid = {0}", gameid);
            OleDbCommand command = new OleDbCommand(getPeopleCount, conn);
            OleDbDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                if (Convert.ToInt32(reader["C"]) != 0)//该组比赛的运动员还未全部打分，无法排名，直接返回
                {
                    return false;
                }
            }
            string getAllCompetitors = "select peopleid,finalscore from result where gameid = " + gameid + "order by finalscore DESC";
            OleDbDataAdapter adapter = new OleDbDataAdapter(getAllCompetitors, conn);
            DataTable table = new DataTable();
            adapter.Fill(table);
            for(int i =0;i<table.Rows.Count;i++)
            {
                int pid = Convert.ToInt32(table.Rows[i].ItemArray[0]);
                string updateRank = string.Format("update result set rank = {0} where peopleid = {1} and gameid = {2}", i + 1, pid, gameid);
                command = new OleDbCommand(updateRank, conn);
                int x = command.ExecuteNonQuery();
                if(x<=0)
                {
                    MessageBox.Show("给运动员排名失败");
                    return false;
                }
            }
            //此时表示该组比赛全部确认打分完毕，则在大屏幕上输出所有运动员成绩
            OutputToScreen();
            return true;
        }

        public void OutputToScreen()
        {
            screen.updateListView(nextgameid);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (indicator)
            {
                if (confirm())
                    MessageBox.Show("确定打分成功！");
                else
                    return;
                textBox1.Text = "";
                textBox2.Text = "";
                updateListView();
            }
            else
            {
                MessageBox.Show("您不能进行确认打分操作！");
            }

            if (rank(nextgameid))//排名，函数中检查是否该组比赛所有的运动员都比赛完成，未完成则直接返回
            {
                MessageBox.Show("该项目所有运动员打分完毕");
                //打分完毕则在大屏幕上输出成绩
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (indicator)
            {
                reject();
            }
            else
            {
                MessageBox.Show("(裁判还未打分或是您已经打完分)您不能进行重新打分操作！");
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(!rank(nextgameid))
            {
                MessageBox.Show("还有运动员未打分，无法排名");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            updateIndicator();
            updateListView();
            if (indicator)
            {
                MessageBox.Show("可以确定打分了！");

            }
            else
            {
                MessageBox.Show("暂时还不可以确认打分");
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
