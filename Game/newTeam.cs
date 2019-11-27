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
    public partial class newTeam : Form
    {
        OleDbConnection conn = null;//定义数据库连接
        public newTeam()
        {
            InitializeComponent();
            conn = database.dbcon.getConnection();//获取数据库连接
        }
        public void regist()
        {
            string name = textBox1.Text;
            string account = textBox2.Text;
            string password1 = textBox3.Text;
            string password2 = textBox4.Text;
            if(password1 != password2)
            {
                MessageBox.Show("确认密码不一致！！请重新输入密码！！");
                textBox4.Text = "";
                return;
            }
            else
            {
                string test = string.Format("select COUNT(*) c from idandpassword where name ='{0}'or id = '{1}'", name, account);
                OleDbCommand com = new OleDbCommand(test, conn);
                OleDbDataReader reader = com.ExecuteReader();
                while(reader.Read())
                {
                    if(Convert.ToInt32(reader["c"])>0)
                    {
                        MessageBox.Show("重复用户名或账号！！");
                        return;
                    }
                }
                string str = string.Format("insert into team values('{0}','{1}')", name, account);
                com = new OleDbCommand(str,conn);
                int x = com.ExecuteNonQuery();
                if(x<=0)
                {
                    MessageBox.Show("注册失败！");
                }
                str = string.Format("insert into idandpassword values('{0}','{1}','{2}','{3}')",account,password1,"team",name);
                com = new OleDbCommand(str, conn);
                x = com.ExecuteNonQuery();
                if (x <= 0)
                {
                    MessageBox.Show("注册失败！");
                }
                MessageBox.Show("注册成功！！");
                textBox1.Text = textBox2.Text = textBox3.Text = textBox4.Text = "";
                return;
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "" )
            {
                MessageBox.Show("信息不能为空！");
                return;
            }
                regist();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void newTeam_Load(object sender, EventArgs e)
        {

        }
    }
}
