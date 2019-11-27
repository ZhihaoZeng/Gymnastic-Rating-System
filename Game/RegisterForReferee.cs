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
    public partial class RegisterForReferee : Form
    {
        OleDbConnection conn = null;//定义数据库连接
        string teamname;//正在执行报名操作的队名
        public RegisterForReferee(string teamname)
        {
            InitializeComponent();
            conn = database.dbcon.getConnection();//获取数据库连接
            this.teamname = teamname;
            updateListView();
        }
        public void insert()
        {//添加至people表
            string name = this.textBox1.Text;
            string phonenumber = this.textBox2.Text;
            string identification = this.textBox3.Text;
            string password = textBox4.Text;
            if (name == "" || phonenumber == "" || identification == ""||password=="")
            {
                MessageBox.Show("请输入完整信息！！！");
                return;
            }

            string sqlstr =  string.Format("insert into referee(name,phonenumber,identification,status,teamname)"
                + "values('{0}','{1}','{2}','referee','{3}')", name, phonenumber, identification, teamname);
          
            OleDbCommand command = new OleDbCommand(sqlstr, conn);
            try
            {
                int x = command.ExecuteNonQuery();
            }
            catch(Exception e ) {
                MessageBox.Show("添加失败！");
                return;
            }
            //更新密码
            updatePassword(name, password);
            updateListView();
        }
        public void updatePassword(string id, string password)
        {
            string sqlstr = string.Format("insert into idandpassword(id,password,status,name)"
             + "values('{0}','{1}','referee','{2}')", id, password, id);
            OleDbCommand command = new OleDbCommand(sqlstr, conn);
            int x = command.ExecuteNonQuery();
            if (x <= 0)
            //如果影响行数大于0则说明插入成功，否则的话插入失败
            {
                MessageBox.Show("添加失败！");
            }
        }
        public void updateListView()
        {
            this.listView1.Items.Clear();
            this.listView1.Columns.Clear();
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("姓名", this.listView1.Width / 5, HorizontalAlignment.Left);
            listView1.Columns.Add("身份", this.listView1.Width / 5, HorizontalAlignment.Left);
            listView1.Columns.Add("电话", this.listView1.Width / 5, HorizontalAlignment.Left);
            listView1.Columns.Add("身份证", this.listView1.Width / 5, HorizontalAlignment.Left);

            string emm = string.Format("select * from referee where teamname = '{0}'", teamname);

            OleDbDataAdapter adapter = new OleDbDataAdapter(emm, conn);
            DataTable table = new DataTable();
            adapter.Fill(table);
            ListViewItem temp = null;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                temp = new ListViewItem(table.Rows[i].ItemArray[0].ToString());
                temp.SubItems.Add(table.Rows[i].ItemArray[3].ToString());
                temp.SubItems.Add(table.Rows[i].ItemArray[1].ToString());
                temp.SubItems.Add(table.Rows[i].ItemArray[2].ToString());
                listView1.Items.Add(temp);
            }
        }
        private void RegisterForReferee_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            insert();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox2.Text = textBox3.Text = textBox4.Text = null;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //返回代表队操作界面
            Regist back = new Regist(teamname);
            back.Show();
            this.Close();
        }
    }
}
