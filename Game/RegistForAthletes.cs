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
    public partial class RegistForAthletes : Form
    {

        OleDbConnection conn = null;//定义数据库连接
        //OleDbCommand command = null;
        //OleDbDataReader myreader = null;
        string teamname;//正在执行报名操作的队名

        public RegistForAthletes()
        {
            InitializeComponent();
        }

        public RegistForAthletes(string teamname)
        {
            InitializeComponent();
            conn = database.dbcon.getConnection();//获取数据库连接
            this.teamname = teamname;
            updateListView();
        }

        public void insertIntoPeople()
        {//添加至people表
            //获取各textbox的值
            string name = this.textBox1.Text;
            string age = this.textBox2.Text;
            string identification = this.textBox3.Text;
            // string gender = this.textBox4.Text;
            string gender = "";
            if (radioButton1.Checked)
                gender = radioButton1.Text;
            else
                gender = radioButton2.Text;
            //避免文本框为空
            if (name == "" || age == "" || identification == "" || gender == "")
            {
                MessageBox.Show("请输入完整信息！！！");
                return;
            }


            //检测性别是否满足要求,不满足要求返回

            string gender1;
            int AGE = int.Parse(age), agegroup = -1;
            //分配年龄组
            if (AGE < 7)
            {
                MessageBox.Show("年龄过小！");
                return;
            }
            else if (AGE <= 8)
            {
                agegroup = 1;
            }
            else if (AGE <= 10)
            {
                agegroup = 2;
            }
            else if (AGE <= 12)
            {
                agegroup = 3;
            }
            else
            {
                MessageBox.Show("年龄不符合要求！");
                return;
            }
            
            if (gender == "男")
            {
                string GENDERCOUNT = string.Format("select *  from PEOPLE where GENDER = '男'"
                    +"and teamname ='{0}'and agegroup={1}", teamname,agegroup);
                OleDbDataAdapter adapter = new OleDbDataAdapter(GENDERCOUNT, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                if(table.Rows.Count>=6)//同一年龄层人满
                {
                    MessageBox.Show("同一年龄层各性别最多只能报6人");
                    return;
                }
                gender1 = "MAN";
                //findgendercount = string.Format("select MANCOUNT from TEAM where NAME = '{0}'", teamname);
            }
            else
            {
                string GENDERCOUNT = string.Format("select *  from PEOPLE where GENDER = '男'"
                    + "and teamname ='{0}'and agegroup={1}", teamname, agegroup);
                OleDbDataAdapter adapter = new OleDbDataAdapter(GENDERCOUNT, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                if (table.Rows.Count >= 6)//同一年龄层人满
                {
                    MessageBox.Show("同一年龄层各性别最多只能报6人");
                    return;
                }
                gender1 = "WOMAN";
                //findgendercount = string.Format("select WOMANCOUNT from TEAM where NAME = '{0}'", teamname);
            }

            //分配id
            string peopleid = getID(gender,true).ToString();
            
            if (!registTheGame(age, peopleid))
            {
                MessageBox.Show("报名项目失败！重新选择正确的项目！");
                //peopleid 要返回原来的值
                getID(gender, false);
                return;
            }
            string sqlstr = string.Format("insert into PEOPLE (name,age,identification,gender,status,peopleid,teamname,AGEGROUP) "
                + "values('{0}','{1}','{2}','{3}','athlete','{4}','{5}',{6})", name, age, identification, gender, peopleid, teamname,agegroup);
            OleDbCommand command = new OleDbCommand(sqlstr, conn);
            int x = command.ExecuteNonQuery();
            if (x <= 0)
            //如果影响行数大于0则说明插入成功，否则的话插入失败
            {
                MessageBox.Show("添加失败！");
            }
            updateListView();

        }
        

        int getID(string gender,bool addOrElseMinus)//设置ID并返回,第二个变量为真，返回id并且更新表中id+2，否则减二
        {
            int id;
            string findID = string.Format("select PEOPLEID as ID from NOWID where GENDER = '{0}'", gender);
            OleDbCommand command = new OleDbCommand(findID, conn);
            OleDbDataReader myreader = command.ExecuteReader();
            while (myreader.Read())
            {
                id = Convert.ToInt32(myreader["ID"]);
                //更新id表
                string setID = "";
                if(addOrElseMinus)
                    setID = string.Format("update NOWID set PEOPLEID ='{0}' where GENDER = '{1}'", (id + 2).ToString(), gender);
                else
                    setID = string.Format("update NOWID set PEOPLEID ='{0}' where GENDER = '{1}'", (id - 2).ToString(), gender);

                command = new OleDbCommand(setID, conn);
                int x = command.ExecuteNonQuery();
                if (x <= 0)
                //如果影响行数大于0则说明插入成功，否则的话插入失败
                {
                    MessageBox.Show("更新ID表失败！");
                }

                return id;
            }

            return -1;

        }

        public void updateListView()
        {
            this.listView1.Items.Clear();
            this.listView1.Columns.Clear();
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("姓名", this.listView1.Width / 5, HorizontalAlignment.Left);
            listView1.Columns.Add("年龄", this.listView1.Width / 5, HorizontalAlignment.Left);
            listView1.Columns.Add("性别", this.listView1.Width / 5, HorizontalAlignment.Left);
            listView1.Columns.Add("身份证", this.listView1.Width / 5, HorizontalAlignment.Left);
            listView1.Columns.Add("号码", this.listView1.Width / 5, HorizontalAlignment.Left);

            string emm = string.Format("select * from PEOPLE where teamname = '{0}' AND status='athlete'", teamname);
            OleDbDataAdapter adapter = new OleDbDataAdapter(emm, conn);
            DataTable table = new DataTable();
            adapter.Fill(table);
            ListViewItem temp = null;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                temp = new ListViewItem(table.Rows[i].ItemArray[0].ToString());
                temp.SubItems.Add(table.Rows[i].ItemArray[2].ToString());
                temp.SubItems.Add(table.Rows[i].ItemArray[6].ToString());
                temp.SubItems.Add(table.Rows[i].ItemArray[1].ToString());
                temp.SubItems.Add(table.Rows[i].ItemArray[5].ToString());

                listView1.Items.Add(temp);
            }
        }

        public bool registTheGame(string AGE,string peopleid)
        {
            //获取报名比赛信息
            int age = int.Parse(AGE),agegroup=-1;

            //分配年龄组
            if (age < 7)
            {
                MessageBox.Show("年龄过小！");
                return false;
            }
            else if (age <= 8)
            {
                agegroup = 1;
            }
            else if (age <= 10)
            {
                agegroup = 2;
            }
            else if (age <= 12)
            {
                agegroup = 3;
            }
            else
            {
                MessageBox.Show("年龄不符合要求！");
                return false;
            }

            string gender = "";
            if (radioButton1.Checked)
                gender = "男";
            else
                gender = "女";
            string[] games = {"", "", "", "", "", "", "" };
            int gamecount = 0;//记录报了几个项目    
            if (checkBox1.Checked)
            {
                games[gamecount] = checkBox1.Text;
                gamecount++;
            }
            if (checkBox2.Checked)
            {
                games[gamecount] = checkBox1.Text;
                gamecount++;
            }
            if (checkBox3.Checked)
            {
                games[gamecount] = checkBox3.Text;
                gamecount++;
            }
            if (checkBox4.Checked)
            {
                games[gamecount] = checkBox4.Text;
                gamecount++;
            }
            if (checkBox5.Checked)
            {
                games[gamecount] = checkBox5.Text;
                gamecount++;
            }
            if (checkBox6.Checked)
            {
                games[gamecount] = checkBox6.Text;
                gamecount++;
            }
            if (checkBox7.Checked)
            {
                games[gamecount] = checkBox7.Text;
                gamecount++;
            }

            //项目数组记录完成
            for(int i=0;i<gamecount;i++)
            {
                //查询gametype是否有同类型的比赛//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!此处的agegroup={3}未经验证
                string emm = string.Format("select count(*) C from gametype where type='{0}'and gender='{1}'and" 
                    + " agegroup={2} and preorfin = 'p'and peoplecount <8", games[i],gender,agegroup);
                OleDbCommand com = new OleDbCommand(emm, conn);
                OleDbDataReader re = com.ExecuteReader();
                int teamcount = 0;
                while(re.Read())
                {
                    teamcount = Convert.ToInt32(re["C"]);
                }
                if (teamcount == 0)
                {
                    //没有同类型的比赛
                    //人满或者没有则新建一个比赛类型插入该表中,peoplecount初始化为1，插入则是增一
                    int gameid = getGameId();
                    string add = string.Format("insert into gametype(type,gender,agegroup,preorfin,gameid,peoplecount) "
                        +" values('{0}','{1}',{2},'p',{3},1)",games[i],gender,agegroup,gameid);
                    OleDbCommand insert = new OleDbCommand(add, conn);
                    int y = insert.ExecuteNonQuery();
                    if (y <= 0)
                    {
                        MessageBox.Show("新建gametype失败");
                    }
                    
                    //result
                    add = string.Format("insert into result(name,peopleid,gameid,score1,score2,score3,score4,score5,finalscore) "
                      + " values('{0}',{1},{2},-1,-1,-1,-1,-1,-9)", textBox1.Text,peopleid,gameid);
                    insert = new OleDbCommand(add, conn);
                    y = insert.ExecuteNonQuery();
                    if (y <= 0)
                    {
                        MessageBox.Show("插入新score失败");
                    }
                }
                else
                {//有同类型的比赛
                 //插入
                    //人满或者没有则新建一个比赛类型插入该表中,peoplecount初始化为1，插入则是增一
                    //搜索人数
                    for(int j = 0;i< teamcount; i++)
                    {
                        string searchGamePeopleCount = string.Format("select peoplecount C,gameid from gametype where type = '{0}' and gender = '{1}'and agegroup = '{2}'",games[i],gender,agegroup);
                        OleDbCommand command = new OleDbCommand(searchGamePeopleCount, conn);
                        OleDbDataReader myreader = command.ExecuteReader();
                        while(myreader.Read())
                        {
                            int count = Convert.ToInt32(myreader["C"]);
                            if(count == 8)
                            {//人满
                                int gameid = getGameId();//获得id并且id加一
                                string add = string.Format("insert into gametype(type,gender,agegroup,preorfin,gameid,peoplecount) "
                        + " values('{0}','{1}',{2},'p',{3},1)", games[i], gender, agegroup, gameid);
                                OleDbCommand insert = new OleDbCommand(add, conn);
                                int y = insert.ExecuteNonQuery();
                                if (y <= 0)
                                {
                                    MessageBox.Show("新建gametype失败");
                                }
                                add = string.Format("insert into result(name,peopleid,gameid,score1,score2,score3,score4,score5,finalscore,D,P) "
                                  + " values('{0}',{1},{2},-1,-1,-1,-1,-1,-9,-1,-1)", textBox1.Text, peopleid, gameid);
                                insert = new OleDbCommand(add, conn);
                                y = insert.ExecuteNonQuery();
                                if (y <= 0)
                                {
                                    MessageBox.Show("插入新score失败");
                                }
                            }
                            else
                            {//人没满
                                string add = string.Format("update gametype set peoplecount = peoplecount+1 where type='{0}'and gender='{1}'and"
                  + " agegroup={2} and preorfin = 'p'", games[i], gender, agegroup);
                                OleDbCommand insert = new OleDbCommand(add, conn);
                                int y = insert.ExecuteNonQuery();
                                if (y <= 0)
                                {
                                    MessageBox.Show("更新gametype的peoplecount失败");
                                }
                                add = string.Format("insert into result(name,peopleid,gameid,score1,score2,score3,score4,score5,finalscore,D,P) "
                                  + " values('{0}',{1},{2},-1,-1,-1,-1,-1,-9,-1,-1)", textBox1.Text, peopleid, myreader["gameid"].ToString());
                                insert = new OleDbCommand(add, conn);
                                y = insert.ExecuteNonQuery();
                                if (y <= 0)
                                {
                                    MessageBox.Show("插入新score失败");
                                }
                            }
                        }
                    }
                   
                   
                    //peoplecount+1
                    //gamelist
                    //score
                }
                //记住分配比赛id，maxgameID表中有当前最大的id
                //插入成功则在gamelist中插入peopleid和gameID信息
                //并在score表中加入一条新行，填入gameid，score1，score2，finalscore为空，以供后期打分

            }
            return true;//流程下来返回true表示成功添加
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
        private void button1_Click(object sender, EventArgs e)
        {
            insertIntoPeople();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = null;
            this.textBox2.Text = null;
            this.textBox3.Text = null;
        }

        private void Regist_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {//返回代表队操作界面
            Regist back = new Regist(teamname);
            back.Show();
            this.Close();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            checkBox1.Text = "单杠";
            checkBox2.Text = "双杠";
            checkBox3.Text = "跳马";
            checkBox4.Text = "吊环";
            checkBox5.Text = "鞍马";
            checkBox6.Show();
            checkBox7.Show();
        }
        //男子单项分为：单杠、双杠、吊环、跳马、自由体操、鞍马和蹦床，女子分为跳马、高低杠、平衡木、自由体操和蹦床。
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            checkBox1.Text = "跳马";
            checkBox2.Text = "高低杠";
            checkBox3.Text = "平衡木";
            checkBox4.Text = "自由体操";
            checkBox5.Text = "蹦床";
            checkBox6.Hide();
            checkBox7.Hide();
            checkBox6.Checked = false;
            checkBox7.Checked = false;
        }
    }
}
