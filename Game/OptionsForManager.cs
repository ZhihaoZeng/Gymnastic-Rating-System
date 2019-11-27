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
    public partial class OptionsForManager : Form
    {
        int peopletograde = -1;
        int gametograde = -1;
        int peopleno = -1;
        OleDbConnection conn = null;//定义数据库连接
        public OptionsForManager()
        {
            InitializeComponent();
            conn = database.dbcon.getConnection();//获取数据库连接
        }
        private void delegateReferee()
        {//分配裁判
         //获取gametype表的count
            int count = 0, refereecount = 0 ;
            string getcount = string.Format("select count(*) C from gametype where gender in ('男','女')");
            OleDbCommand command = new OleDbCommand(getcount, conn);
            OleDbDataReader myreader = command.ExecuteReader();
            while(myreader.Read())
            {
                count = Convert.ToInt32(myreader["C"]);
            }
            
            //顺序循环扫描整个gametype表，每个项都分配裁判
            //获取整个裁判表，并且计算裁判个数
            string getRefereeTable = string.Format("select * from referee");
            OleDbDataAdapter adapter = new OleDbDataAdapter(getRefereeTable, conn);
            DataTable table = new DataTable();
            adapter.Fill(table);
            refereecount = table.Rows.Count;
            int Nowcount = refereecount - 1;
            //从所有的裁判中，倒数第一个，第二个...直到第五个，分配给该比赛，
            //选择的裁判为第0位时，下一位设置为倒数第一个
            //记录下这次的裁判的第n位到refereeno表中，下次继续使用
            //将这五个裁判更新到gametype的referee1-5中
            //进入下一次循环
            //int game = 0;
            for (int i =0;i<count;i++)
            {
                string n1 = table.Rows[Nowcount].ItemArray[0].ToString();
                Nowcount--;
                if (Nowcount == -1)
                    Nowcount = refereecount - 1;

                string n2 = table.Rows[Nowcount].ItemArray[0].ToString();
                Nowcount--;
                if (Nowcount == -1)
                    Nowcount = refereecount - 1;

                string n3 = table.Rows[Nowcount].ItemArray[0].ToString();
                Nowcount--;
                if (Nowcount == -1)
                    Nowcount = refereecount - 1;

                string n4 = table.Rows[Nowcount].ItemArray[0].ToString();
                Nowcount--;
                if (Nowcount == -1)
                    Nowcount = refereecount - 1;

                string n5 = table.Rows[Nowcount].ItemArray[0].ToString();
                Nowcount--;
                if (Nowcount == -1)
                    Nowcount = refereecount - 1;

                string gaveReferee = string.Format("update gametype set "
                    + "referee1 = '{0}',referee2='{1}',referee3 = '{2}'"
                    + ",referee4 = '{3}',referee5='{4}' where gameid = {5}", n1, n2, n3, n4, n5, /*game*/i);
                OleDbCommand command0 = new OleDbCommand(gaveReferee, conn);
                int x = command0.ExecuteNonQuery();
                if (x <= 0)
                {
                    MessageBox.Show("分配裁判失败");
                    return;
                }
                ////game++;
            }
            MessageBox.Show("成功为所有比赛分配裁判");
        }
        private void updateGamePeople()
        {
            //从nextgameID中去获取gameid,peopleno到自己的nextgameid,peopleid中
            string findGameAndPeopleID = "select * from NEXTGAMEID ";
            OleDbCommand command = new OleDbCommand(findGameAndPeopleID, conn);
            OleDbDataReader myreader = command.ExecuteReader();
            while (myreader.Read())
            {
                gametograde = Convert.ToInt32(myreader["GAMEID"]);
                peopleno = Convert.ToInt32(myreader["PEOPLENO"]);
            }

            //通过peopleno初始化peopletograde
            string initiatePeopleid = string.Format("select peopleid from result where gameid = {0}", gametograde);
            OleDbDataAdapter adapter = new OleDbDataAdapter(initiatePeopleid, conn);
            DataTable table = new DataTable();
            adapter.Fill(table);
            peopletograde = Convert.ToInt32(table.Rows[peopleno].ItemArray[0]);
        }
        private void Grade()
        {//发出打分指令
            //获取nextgameid中的gameid（int）peopleno(int)
            //////////////////判断peopleno是否等于result表中查询到的这场比赛参加的人数
            //////////////////等于则gameid+1并且更新到nextgameID中，peopleno更新为0
            //设置裁判表中的gametograde和peopletograde为待打分的gametograde,peopletograde

            updateGamePeople();

            //裁判名表
            string[] referees = new string[5];
            string findrefereee = string.Format("select referee1,referee2,referee3,referee4,referee5 from gametype where gameid = {0}", gametograde);
            OleDbCommand command = new OleDbCommand(findrefereee, conn);
            OleDbDataReader myreader = command.ExecuteReader();
            while (myreader.Read())
            {
                string a = myreader["referee1"].ToString();
                referees[0] = myreader["referee1"].ToString();
                referees[1] = myreader["referee2"].ToString();
                referees[2] = myreader["referee3"].ToString();
                referees[3] = myreader["referee4"].ToString();
                referees[4] = myreader["referee5"].ToString();
            }
            //通过循环，将他们的检测自己的gametograde,peopletograde更新成nextgameid,peopleno
            for (int i = 0; i < 5; i++)
            {
                string updateGAMETOGRADEandPEOPLETOGRADE = string.Format("update referee set "
                    + "GAMETOGRADE = {0},PEOPLETOGRADE = {1} where name = '{2}'", gametograde, peopletograde, referees[i]);
                command = new OleDbCommand(updateGAMETOGRADEandPEOPLETOGRADE, conn);
                int a = command.ExecuteNonQuery();
                if (a <= 0)
                //如果影响行数大于0则说明插入成功，否则的话插入失败
                {
                    MessageBox.Show("通知裁判打分失败！");
                    return;
                }
            }
            
            //将finalscore从-1设置为-2
            string setbackFINALSCORE = string.Format("update result set "
                    + "FINALSCORE = -9,score1 = -1,score2 = -1,score3 = -1,score4 = -1,score5 = -1 where peopleid = {0} and gameid = {1}", peopletograde, gametograde);
            command = new OleDbCommand(setbackFINALSCORE, conn);
            int x = command.ExecuteNonQuery();
            if (x <= 0)
            //如果影响行数大于0则说明插入成功，否则的话插入失败
            {
                MessageBox.Show("取消finalscore为101失败！");
                return;
            }
            MessageBox.Show("已通知裁判打分！");
        }
        public void FinalGame()
        {
            string preCount = "select count(*) C from result where finalscore = -9";
            OleDbCommand command = new OleDbCommand(preCount, conn);
            OleDbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (Convert.ToInt32(reader["C"]) == 0)
                {
                    //进入决赛部分
                    final();
                    MessageBox.Show("成功生成决赛场次表！！");
                }
                else
                {
                    MessageBox.Show("还有初赛未完成，无法进入决赛部分");
                    return;
                }
            }
        }
        public bool final()
        {
            int gameid = 0;
            string getmaxid = string.Format("select id from maxgameid");
            OleDbCommand command = new OleDbCommand(getmaxid, conn);
            OleDbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                gameid = Convert.ToInt32(reader["id"]);
            }
            string getGameTable = string.Format("select * from gametype order by gameid ");
            OleDbDataAdapter adapter = new OleDbDataAdapter(getGameTable, conn);
            DataTable gametable = new DataTable();
            adapter.Fill(gametable);
            //预赛全部存入gametable中
            //循环所有的预赛
            
            for(int i = 0;i<gameid;i++)
            {//第i号预赛
                //查找当前比赛的前四名并且加入到表中
                string getfour = string.Format("select rank,name,peopleid,gameid,type,gender,agegroup from result natural join gametype where gameid = {0} and rank < 5", i);
                OleDbDataAdapter ada = new OleDbDataAdapter(getfour, conn);
                DataTable fourpeople = new DataTable();
                ada.Fill(fourpeople);
                int fourpeoplesize = fourpeople.Rows.Count;
                //将四名加入到新的表中
                string typehere = fourpeople.Rows[0].ItemArray[4].ToString();
                string genderhere = fourpeople.Rows[0].ItemArray[5].ToString();
                int agegrouphere = Convert.ToInt32(fourpeople.Rows[0].ItemArray[6]);
                
                //查询是否有同类比赛（年龄段，性别，种类）
                string getSameFinal = string.Format(
                    "select count(*) C from gametype where type = '{0}' and " +
                    "gender = '{1}' and agegroup = {2} and preorfin = 'f'", typehere, genderhere, agegrouphere);
                command = new OleDbCommand(getSameFinal, conn);
                OleDbDataReader readSameFinal = command.ExecuteReader();
                while (readSameFinal.Read())
                {
                    if (Convert.ToInt32(readSameFinal["C"]) == 0)
                    {//没找到相同的决赛
                     //为决赛分配新的id
                        int newgameid = getGameId();
                        //无-》新建gametype   peoplecount = 表中前四名人数****************
                        string createFinal = string.Format("insert into gametype(type,gender,agegroup,preorfin,gameid,peoplecount) " +
                            "values('{0}','{1}',{2},'f',{3},{4})", typehere, genderhere, agegrouphere, newgameid, fourpeoplesize);
                        command = new OleDbCommand(createFinal, conn);
                        command.ExecuteNonQuery();

                        //循环四名加入到result表中*********************
                        for (int j = 0;j<fourpeoplesize;j++)
                        {
                            string newname = fourpeople.Rows[j].ItemArray[1].ToString();
                            int newpeopleid = Convert.ToInt32(fourpeople.Rows[j].ItemArray[2]);
                            string addToResult = string.Format("insert into result(name,peopleid,gameid,score1,score2,score3,score4,score5,finalscore) " +
                                "values('{0}',{1},{2},-1,-1,-1,-1,-1,-9)" ,newname,newpeopleid,newgameid);
                            command = new OleDbCommand(addToResult, conn);
                            command.ExecuteNonQuery();

                        }
                    }
                    else
                    {//找到决赛
                     //决赛的id和人数
                        string getFinal = string.Format(
                      "select gameid, peoplecount from gametype where type = '{0}' and " +
                      "gender = '{1}' and agegroup = {2} and preorfin = 'f' ", typehere, genderhere, agegrouphere);
                        command = new OleDbCommand(getFinal, conn);
                        OleDbDataReader readFinal = command.ExecuteReader();
                        int newgameid = 0;
                        while (readFinal.Read())
                        {
                            newgameid =Convert.ToInt32(readFinal["gameid"]);
                        }
                        // int finalpeoplecount = Convert.ToInt32(readSameFinal["peoplecount"]);
                        //有-》插入
                        //增加peoplecount+++++++++++++++++++++++++++++
                        string updatePeoplecount = string.Format("update gametype set" +
                            " peoplecount = peoplecount + {0} where gameid = {1}", fourpeoplesize, newgameid);
                        command = new OleDbCommand(updatePeoplecount, conn);
                        command.ExecuteNonQuery();

                        //插入result
                        for(int j = 0;j<fourpeoplesize;j++)
                        {
                            string newname = fourpeople.Rows[j].ItemArray[1].ToString();
                            int newpeopleid = Convert.ToInt32(fourpeople.Rows[j].ItemArray[2]);
                            string addToResult = string.Format("insert into result(name,peopleid,gameid,score1,score2,score3,score4,score5,finalscore) " +
                                "values('{0}',{1},{2},-1,-1,-1,-1,-1,-9)", newname, newpeopleid, newgameid);
                            command = new OleDbCommand(addToResult, conn);
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            return true;
        }
        public int getGameId()
        {
            int id;
            string findID = "select * from maxgameid ";
            OleDbCommand command = new OleDbCommand(findID, conn);
            OleDbDataReader myreader = command.ExecuteReader();
            while (myreader.Read())
            {
                id = Convert.ToInt32(myreader["ID"]);
                //更新id表
                string setID = "";
                setID = string.Format("update MAXGAMEID set ID ='{0}'", (id + 1).ToString());
                command = new OleDbCommand(setID, conn);
                int x = command.ExecuteNonQuery();
                if (x <= 0)
                //如果影响行数大于0则说明插入成功，否则的话插入失败
                {
                    MessageBox.Show("更新MAXGAMEID表失败！");
                }
                return id;
            }
            return -1;
        }
        private void initiate()
        {
            OleDbCommand command;
            string[] ini = new string[12];
            ini[0] = "delete from people ";
            ini[1] = "delete from gametype ";
            ini[2] = "update nextgameid set gameid = 0";
            ini[3] = "update nextgameid set peopleno = 0";
            ini[4] = "update maxgameid set id = 0";
            ini[5] = "delete from result";
            ini[6] = "update nowid set peopleID = 0 where gender= '女'";
            ini[7] = "update nowid set peopleID = 1 where gender= '男'";            ini[7] = "update nowid set peopleID = 1 where gender= '男'";
            ini[8] = "delete from team";
            ini[9] = "delete from idandpassword where name<>'manager'";
            ini[10] = "insert into idandpassword(id, name, password, status) values('main', 'main', 'main', 'mainreferee')" ;
            ini[11] = "delete from referee where name <> 'main'";
            for (int i = 0; i < 12; i++)
            {
                command = new OleDbCommand(ini[i], conn);
                int x = command.ExecuteNonQuery();
                if (x <= 0)
                {
                    MessageBox.Show("删除失败");
                }
            }
            MessageBox.Show("删除所有数据成功");
        }
        public bool CalculateScore(string preorfin)
        {
            return true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Grade();
        }

        private void OptionsForManager_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            delegateReferee();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FinalGame();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            initiate();
        }
       
    }
}
