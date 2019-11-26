using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Checkers.UI
{
    public partial class MessageBox : Form
    {
        public MessageBox(string i_PlayerOneName, string i_PlayerTwoName)
        {
            InitializeComponent();
            if (Game.s_BlackTeamStatus == Game.eTeamStatus.Win)
            {
                this.label1.Text = string.Format("{0} Won!", i_PlayerOneName).ToString();
            }
            else if (Game.s_BlackTeamStatus == Game.eTeamStatus.Lose)
            {
                this.label1.Text = string.Format("{0} Won!", i_PlayerTwoName).ToString();
            }
            else 
            {
                /// Game.s_BlackTeamStatus == Game.eTeamStatus.Tie
                this.label1.Text = "Its a Tie!";
            }
        }

        public MessageBox(string i_ErrorMessage)
        {
            InitializeComponent();
            pictureBox1.Hide();
            button1.Hide();
            button2.Text = "OK";
            label1.Text = "Error:";
            label1.SetBounds(label2.Location.Y, label1.Location.Y, label1.Width, label1.Height);
            label2.Text = i_ErrorMessage;
            label2.SetBounds(label2.Location.Y, label2.Location.Y, label2.Width, label2.Height);
        }

        private void MessageBox_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GameSettings.PlayAgain = true;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "No")
            {
                GameSettings.PlayAgain = false;
            }

            Close();
        }
    }
}
