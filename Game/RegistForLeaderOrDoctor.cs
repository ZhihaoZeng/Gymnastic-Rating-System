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
    public partial class RegistForLeaderOrDoctor : Form
    {
        OleDbConnection conn = null;//定义数据库连接
        //OleDbCommand command = null;
        //OleDbDataReader myreader = null;
        string teamname;//正在执行报名操作的队名
        public RegistForLeaderOrDoctor(string teamname)
        {
            InitializeComponent();
            conn = database.dbcon.getConnection();//获取数据库连接
            this.teamname = teamname;
            updateListView();
        }

        private void RegistForLeaderOrDoctor_Load(object sender, EventArgs e)
        {

        }
        public void insert()
        {//添加至people表
            string name = this.textBox1.Text;
            string phonenumber = this.textBox2.Text;
            string identification = this.textBox3.Text;
            string status = "";
            if (radioButton2.Checked)
                status = "leader";
            else
                status = "doctor";
            
            if (name == "" || phonenumber == "" || identification == "" || status == "")
            {
                MessageBox.Show("请输入完整信息！！！");
                return;
            }

            string sqlstr = "";

            sqlstr = string.Format("insert into people(name,phonenumber,identification,status,teamname)"
                + "values('{0}','{1}','{2}','{3}','{4}')", name, phonenumber, identification, status, teamname);
            OleDbCommand command = new OleDbCommand(sqlstr, conn);
            int x = command.ExecuteNonQuery();
            if (x <= 0)
            //如果影响行数大于0则说明插入成功，否则的话插入失败
            {
                MessageBox.Show("添加失败！");
            }
            updateListView();
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
            listView1.Columns.Add("身份证", 2*this.listView1.Width / 5, HorizontalAlignment.Left);

            string emm = string.Format("select * from people where teamname = '{0}' "+
                "and status <> 'referee'and status <> 'coach' and status <> 'mainreferee'and status<>'athlete'", teamname);

            OleDbDataAdapter adapter = new OleDbDataAdapter(emm, conn);
            DataTable table = new DataTable();
            adapter.Fill(table);
            ListViewItem temp = null;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                temp = new ListViewItem(table.Rows[i].ItemArray[0].ToString());
                temp.SubItems.Add(table.Rows[i].ItemArray[4].ToString());
                temp.SubItems.Add(table.Rows[i].ItemArray[7].ToString());
                temp.SubItems.Add(table.Rows[i].ItemArray[1].ToString());
                listView1.Items.Add(temp);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            insert();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox2.Text = textBox3.Text = null;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //返回代表队操作界面
            Regist back = new Regist(teamname);
            back.Show();
            this.Close();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
