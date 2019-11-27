using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game
{
    public partial class OptionsForTeam : Form
    {
        string teamname;

        public OptionsForTeam()
        {
            InitializeComponent();
        }
        public OptionsForTeam(string teamname)
        {
            InitializeComponent();
            this.teamname = teamname;//传入队名
            label2.Text = teamname;
        }

        private void OptionsForTeam_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {//退出
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {//登记报名
            if(teamname==null)//无队名时强行退出回主界面
            {
                MessageBox.Show("未知身份操作！");
                this.Close();
                return;
            }//有队名时则允许进入
            Regist registNow = new Regist(teamname);
            registNow.Show();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            GetScores g = new GetScores(teamname);
            g.Show();
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
        {

        }
    }
}
