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
    public partial class SignIn : Form
    {
        OleDbConnection conn = null;//定义数据库连接
        OleDbCommand command = null;
        OleDbDataReader myreader = null;
        Screen screen;
        public SignIn()
        {
            InitializeComponent();
            conn = database.dbcon.getConnection();//获取数据库连接
            screen = new Screen();
            screen.Show();
            this.textBox2.KeyDown += new KeyEventHandler(textBox2_KeyDown);
        }
        private bool verify()//验证用户密码和用户名（匹配）
        {
            string userid = textBox1.Text;
            string userpassword = textBox2.Text;
            int flag = 0;
            string status = null;
            string sqlstr = string.Format("select * from idandpassword where id = '{0}'and password = '{1}'", userid, userpassword);
            OleDbDataAdapter adapter = new OleDbDataAdapter(sqlstr, conn);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            if(dt.Rows.Count==0)
            {
                MessageBox.Show("登陆失败！！！");
                return false;
            }
            else if(dt.Rows.Count>1)
            {
                MessageBox.Show("内部数据错误，用户名重复");
                return false;
            }
            else
            {
                status = dt.Rows[0].ItemArray[2].ToString();
                flag = 1;
                if (status == "team")//以代表队身份登录
                {
                    string teamname = dt.Rows[0].ItemArray[3].ToString();
                    OptionsForTeam team = new OptionsForTeam(teamname);
                    team.MdiParent = this.MdiParent;//将现在窗体的父窗体赋给新建的窗体
                    team.Show();
                }
                else if (status == "referee")//此时为普通裁判
                {
                    //获取并传入普通裁判名
                    string refereename = dt.Rows[0].ItemArray[3].ToString();
                    OptionsForReferee referee = new OptionsForReferee(refereename);
                    referee.MdiParent = this.MdiParent;
                    referee.Show();
                }
                else if (status == "manager")
                {
                    OptionsForManager o = new OptionsForManager();
                    o.Show();
                }
                else//此时为主裁判
                {
                    //获取并传入主裁判名
                    string mainrefereename = dt.Rows[0].ItemArray[3].ToString();
                    OptionsForMainReferee mainreferee = new OptionsForMainReferee(screen);
                    mainreferee.MdiParent = this.MdiParent;
                    mainreferee.Show();
                }
                return true;
            }
            return false;
        }
        private void SignIn_Load(object sender, EventArgs e)
        {

        }
        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Enter)
            {
                this.button1_Click(sender, e);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            bool a = verify();
            if (a)
            {
                textBox1.Text = "";
                textBox2.Text = "";
                /*该界面close的话后面打开的已登录的界面都会消失，其他界面close则无此情况*/
            } //this.Close();//如果登陆成功，则关闭登录界面
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();//直接退出
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            newTeam n = new newTeam();
            n.Show();
        }
    }
}
