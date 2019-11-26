using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;

namespace Checkers.UI
{
    public partial class CheckersBoard : Form
    {
        public static readonly Image sr_BlackTeamPiece = Properties.Resources._1;
        public static readonly Image sr_BlackTeamKing = Properties.Resources._2;
        public static readonly Image sr_RedTeamPiece = Properties.Resources._3;
        public static readonly Image sr_RedTeamKing = Properties.Resources._4;

        private Game m_Game;

        private List<Button> buttons = new List<Button>();

        private bool m_playerToBeMovedIsChosen = false;

        private byte m_chosenPlayerIndex;

        public CheckersBoard(Game i_Game, string i_PlayerOneName, string i_PlayerTwoName)
        {
            int spaceNumber = 0, row = 0, col = 0;
            Board board = i_Game.State.Board;

            m_Game = i_Game;
            InitializeComponent();
            this.label1.Text = string.Format("{0}: {1}", i_PlayerOneName, Game.s_Player1Score);
            this.label2.Text = string.Format("{0}: {1}", i_PlayerTwoName, Game.s_Player2Score);
            foreach (Space space in board.Spaces)
            {
                Button newButton = new Button();

                newButton.Name = spaceNumber.ToString();
                if (spaceNumber > 0 && (spaceNumber % (byte)i_Game.State.Board.BoardSize == 0))
                {
                    row++;
                }

                if (col % (byte)board.BoardSize == 0)
                {
                    col = 0;
                }

                newButton.Location = new System.Drawing.Point(0 + (col * 59), 56 + (row * 59));
                newButton.Size = new System.Drawing.Size(60, 60);
                if ((spaceNumber % 2 == 0) == (row % (byte)board.BoardSize % 2 == 0))
                {
                    newButton.Enabled = false;
                    newButton.BackColor = System.Drawing.Color.Sienna;
                }
                else
                {
                    newButton.Enabled = true;
                    newButton.BackColor = System.Drawing.Color.AntiqueWhite;
                    if (board.Spaces[spaceNumber].SpaceState == Space.eSpaceState.Occupied)
                    {
                        newButton.Image = board.Spaces[spaceNumber].Piece.Team == eTeam.Red ? sr_RedTeamPiece : sr_BlackTeamPiece;
                    }
                }

                newButton.Click += new System.EventHandler(this.button_Click);
                buttons.Add(newButton);
                this.Controls.Add(newButton);
                newButton.Show();
                spaceNumber++;
                col++;
            }

            this.SetClientSizeCore(buttons[(int)Math.Pow((byte)board.BoardSize, 2) - 1].Location.Y, buttons[(int)Math.Pow((byte)board.BoardSize, 2) - 1].Location.Y + 58);
        }

        private void CheckersBoard_Load(object sender, EventArgs e)
        {
        }

        private void button_Click(object sender, EventArgs e)
        {
            if (((m_Game.State.CurrentTurn == eTeam.Black) && (((Button)sender).Image == sr_BlackTeamPiece) || (((Button)sender).Image == sr_BlackTeamKing))
                || ((m_Game.State.CurrentTurn == eTeam.Black) && (returnButtonByNumber(m_chosenPlayerIndex).Image == sr_BlackTeamPiece || returnButtonByNumber(m_chosenPlayerIndex).Image == sr_BlackTeamKing))
                || ((m_Game.State.CurrentTurn == eTeam.Red) && (((Button)sender).Image == sr_RedTeamPiece) || (((Button)sender).Image == sr_RedTeamKing))
                || ((m_Game.State.CurrentTurn == eTeam.Red) && (returnButtonByNumber(m_chosenPlayerIndex).Image == sr_RedTeamPiece || returnButtonByNumber(m_chosenPlayerIndex).Image == sr_RedTeamKing)))
            {
                bool jumpsAvailable;
                List<Move> currentTeamMoveList = m_Game.State.PopulateTeamMoveList(m_Game.State.CurrentTurn, out jumpsAvailable);

                if (((Button)sender).Image != null && m_playerToBeMovedIsChosen == false)
                {
                    /// current button is a player
                    ((Button)sender).BackColor = System.Drawing.Color.LightSkyBlue;
                    m_playerToBeMovedIsChosen = true;
                    m_chosenPlayerIndex = byte.Parse(((Button)sender).Name);
                }
                else if (((Button)sender).BackColor == System.Drawing.Color.LightSkyBlue)
                {
                    /// current button is an already chosen player
                    ((Button)sender).BackColor = System.Drawing.Color.AntiqueWhite;
                    m_playerToBeMovedIsChosen = false;
                }
                else if (((Button)sender).Image == null && m_playerToBeMovedIsChosen == true)
                {
                    /// try to make a move
                    bool validMove = false, currentMoveWasAJump = false;
                    Move wantedMove = null;

                    foreach (Move move in currentTeamMoveList)
                    {
                        /// look for wanted move
                        if ((move.From.ToString() == m_chosenPlayerIndex.ToString()) && (move.To.ToString() == ((Button)sender).Name))
                        {
                            /// if such move exist
                            validMove = true;
                            wantedMove = move;
                            break;
                        }
                    }

                    if (validMove == true)
                    {
                        callPerformMove(sender, currentTeamMoveList, wantedMove, currentMoveWasAJump, jumpsAvailable);
                    }
                    else
                    {
                        MessageBox invalidMove = new MessageBox("Invalid Move. Please Try Again");
                        invalidMove.Show();
                    }
                }
            }
        }

        private void callPerformMove(object sender, List<Move> currentTeamMoveList, Move wantedMove, bool currentMoveWasAJump, bool jumpsAvailable)
        {
            byte jumpingPiece;

            m_Game.State.MakeMove(wantedMove, currentTeamMoveList, false, out currentMoveWasAJump, out jumpingPiece);
            updateBoard(sender as Button, wantedMove, currentMoveWasAJump);
            if (currentMoveWasAJump == true)
            {
                currentTeamMoveList = m_Game.State.PopulateTeamMoveList(m_Game.State.CurrentTurn, out jumpsAvailable); // renew list to see if there is another jump
                if (jumpsAvailable)
                {
                    m_Game.UpdateSameTeamPlaysAgain(currentTeamMoveList, jumpingPiece);
                }
            }

            if (!m_Game.State.SameTeamPlaysAgain)
            {
                bool skipPCMove = false;

                m_Game.State.CurrentTurn = (eTeam)((int)(m_Game.State.CurrentTurn + 1) % 2);
                if (!m_Game.State.GameOn())
                {
                    m_Game.UpdateWinningTeamStatus();
                    m_Game.UpdateScore();
                    skipPCMove = true;
                    this.Close();
                }

                callPCMove(currentTeamMoveList, jumpingPiece, currentMoveWasAJump, jumpsAvailable, skipPCMove);
            }

            m_playerToBeMovedIsChosen = false;
            m_Game.State.SameTeamPlaysAgain = false;
        }

        private void callPCMove(List<Move> i_CurrentTeamMoveList, byte i_JumpingPiece, bool i_CurrentMoveWasAJump, bool i_JumpsAvailable, bool i_SkipPCMove)
        {
            while (m_Game.State.GameMode == eGameMode.PlayerVsPC && m_Game.State.CurrentTurn == eTeam.Red && !i_SkipPCMove)
            {
                i_CurrentTeamMoveList = m_Game.State.PopulateTeamMoveList(m_Game.State.CurrentTurn, out i_JumpsAvailable); // renew list to see if there is another jump
                Move PCMove = m_Game.RandomMoveForPC(i_CurrentTeamMoveList);
                m_Game.State.MakeMove(PCMove, i_CurrentTeamMoveList, false, out i_CurrentMoveWasAJump, out i_JumpingPiece);
                updateBoard(returnButtonByNumber(PCMove.To), PCMove, i_CurrentMoveWasAJump);
                if (i_CurrentMoveWasAJump == true)
                {
                    i_CurrentTeamMoveList = m_Game.State.PopulateTeamMoveList(m_Game.State.CurrentTurn, out i_JumpsAvailable); // renew list to see if there is another jump
                    if (i_JumpsAvailable)
                    {
                        m_Game.State.CurrentTurn = (eTeam)((int)(m_Game.State.CurrentTurn + 1) % 2);
                    }
                }

                m_Game.State.CurrentTurn = (eTeam)((int)(m_Game.State.CurrentTurn + 1) % 2);
                if (!m_Game.State.GameOn())
                {
                    m_Game.UpdateWinningTeamStatus();
                    m_Game.UpdateScore();
                    this.Close();
                }
            }
        }

        private Button returnButtonByNumber(byte i_buttonNumber)
        {
            Button button = null;

            foreach (Button currentButton in buttons)
            {
                if (currentButton.Name == i_buttonNumber.ToString())
                {
                    button = currentButton;
                    break;
                }
            }

            return button;
        }

        private void updateBoard(Button i_DestinationButton, Move i_WantedMove, bool i_CurrentMoveWasAJump)
        {
            Button sourceButton = returnButtonByNumber(i_WantedMove.From);
            i_DestinationButton.Image = sourceButton.Image;
            sourceButton.Image = null;
            sourceButton.BackColor = System.Drawing.Color.AntiqueWhite;
            if (i_CurrentMoveWasAJump == true)
            {
                returnButtonByNumber((byte)((int.Parse(sourceButton.Name) + int.Parse(i_DestinationButton.Name)) / 2)).Image = null;
            }

            if (m_Game.State.Board.Spaces[i_WantedMove.To].Piece.IsKing == true)
            {
                if (m_Game.State.Board.Spaces[i_WantedMove.To].Piece.Team == eTeam.Black)
                {
                    i_DestinationButton.Image = sr_BlackTeamKing;
                }
                else
                {
                    i_DestinationButton.Image = sr_RedTeamKing;
                }
            }
        }
    }
}
