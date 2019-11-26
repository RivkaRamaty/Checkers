using System;
using System.Windows.Forms;

namespace Checkers.UI
{
    public partial class GameSettings : Form
    {
        private static bool s_PlayAgain = false;

        private eBoardSize m_BoardSize;

        private eGameMode m_GameMode;

        private string m_PlayerOneName;

        private string m_PlayerTwoName;

        public static bool PlayAgain
        {
            get
            {
                return s_PlayAgain;
            }

            set
            {
                s_PlayAgain = value;
            }
        }
        
        public string PlayerOneName
        {
            get
            {
                return m_PlayerOneName;
            }

            set
            {
                m_PlayerOneName = value;
            }
        }

        public string PlayerTwoName
        {
            get
            {
                return m_PlayerTwoName;
            }

            set
            {
                m_PlayerTwoName = value;
            }
        }

        public GameSettings()
        {
            InitializeComponent();
        }

        private void GameSettings_Load(object sender, EventArgs e)
        {
            m_BoardSize = eBoardSize.Small;
            m_GameMode = eGameMode.PlayerVsPC;
            PlayerTwoName = "[Computer]";
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.ReadOnly = !checkBox1.Checked;
            textBox2.Text = checkBox1.Checked ? string.Empty : "[Computer]";
            m_GameMode = checkBox1.Checked ? eGameMode.PlayerVsPlayer : eGameMode.PlayerVsPC;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (m_PlayerOneName == null || m_PlayerOneName == string.Empty)
            {
                MessageBox messageBox = new MessageBox("Please enter a name for Player 1.");
                messageBox.ShowDialog();
             }
            else if (checkBox1.Checked == true && m_PlayerTwoName == string.Empty)
            {
                MessageBox messageBox = new MessageBox("Please enter a name for Player 2.");
                messageBox.ShowDialog();
            }
            else
            {
                do
                {
                    Board board = new Board(m_BoardSize);
                    GameState state = new GameState(board, m_GameMode);
                    Game game = new Game(state, m_GameMode, m_BoardSize);

                    this.Opacity = 0;
                    CheckersBoard boardUI = new CheckersBoard(game, m_PlayerOneName, m_PlayerTwoName);
                    boardUI.ShowDialog();
                    MessageBox messageBox = new MessageBox(m_PlayerOneName, m_PlayerTwoName);
                    messageBox.ShowDialog();
                }
                while (s_PlayAgain);
                Close();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                m_BoardSize = eBoardSize.Small;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                m_BoardSize = eBoardSize.Medium;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked == true)
            {
                m_BoardSize = eBoardSize.Large;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            PlayerOneName = textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            PlayerTwoName = textBox2.Text;
        }
    }
}
