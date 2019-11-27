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
    public partial class Regist : Form
    {
        string teamname;
        public Regist(string teamname)
        {
            InitializeComponent();
            this.teamname = teamname;
            label2.Text = teamname;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RegistForAthletes emm = new RegistForAthletes(teamname);
            emm.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RegistForLeaderOrDoctor emm = new RegistForLeaderOrDoctor(teamname);
            emm.Show();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            RegistForCoach emm = new RegistForCoach(teamname);
            emm.Show();
            this.Close();

        }

        private void Regist_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            RegisterForReferee re = new RegisterForReferee(teamname);
            re.Show();
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OptionsForTeam r = new OptionsForTeam(teamname);
            r.Show();
            this.Close();
        }
    }
}
