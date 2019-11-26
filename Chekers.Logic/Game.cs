using System;
using System.Collections.Generic;

namespace Checkers
{
    public enum eTeam
    {
        Red,
        Black
    }

    public class Game
    {
        public enum eTeamStatus
        {
            Draw,
            Win,
            Lose
        }

        private GameState m_State;
        private eGameMode m_ChosenGameMode;
        private eBoardSize m_ChosenBoardSize;
        public static eTeamStatus s_BlackTeamStatus;
        public static int s_Player1Score = 0;
        public static int s_Player2Score = 0;

        public GameState State
        {
            get
            {
                return m_State;
            }
        }

        public Game(GameState i_State, eGameMode i_ChosenGameMode, eBoardSize i_ChosenBoardSize)
        {
            this.m_State = i_State;
            this.m_ChosenGameMode = i_ChosenGameMode;
            this.m_ChosenBoardSize = i_ChosenBoardSize;
        }

        public void UpdateSameTeamPlaysAgain(List<Move> i_currentTeamMoveList, byte i_JumpingPiece)
        {
            foreach (Move availableMove in i_currentTeamMoveList)
            {
                if (availableMove.From == i_JumpingPiece)
                {
                    m_State.SameTeamPlaysAgain = true; // if this move has another jump
                }
            }
        }

        public void UpdateWinningTeamStatus()
        {
            if (State.PieceCounterBlack > State.PieceCounterRed)
            {
                s_BlackTeamStatus = eTeamStatus.Win;
            }
            else if (State.PieceCounterBlack < State.PieceCounterRed)
            {
                s_BlackTeamStatus = eTeamStatus.Lose;
            }
            else
            {
                s_BlackTeamStatus = eTeamStatus.Draw;
            }
        }

        public bool CurrentTeamIsLosing()
        {
            bool currentTeamIsLosing = false;

            if ((this.State.CurrentTurn == eTeam.Black && Game.s_BlackTeamStatus == Game.eTeamStatus.Lose) // if the non-PC player is losing
            || ((this.State.CurrentTurn == eTeam.Red && Game.s_BlackTeamStatus == Game.eTeamStatus.Win)
            && !(this.State.GameMode == eGameMode.PlayerVsPC)))
            {
                currentTeamIsLosing = true;
            }

            return currentTeamIsLosing;
        }

        public Move RandomMoveForPC(List<Move> currentTeamMoveList)
        {
            Random random = new Random();
            int randomMoveIndex = random.Next(0, currentTeamMoveList.Count - 1);

            return currentTeamMoveList[randomMoveIndex];
        }

        public void UpdateScore()
        {
            int player1Score = m_State.PieceCounterBlack + (3 * m_State.KingCounterByTeam(eTeam.Black));
            int player2Score = m_State.PieceCounterRed + (3 * m_State.KingCounterByTeam(eTeam.Black));

            if (player1Score > player2Score)
            {
                s_BlackTeamStatus = eTeamStatus.Win;
                s_Player1Score += player1Score;
            }
            else if (player1Score < player2Score)
            {
                s_BlackTeamStatus = eTeamStatus.Lose;
                s_Player2Score += player2Score;
            }
        }
    }
}
