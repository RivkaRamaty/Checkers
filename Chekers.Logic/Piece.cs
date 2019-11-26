using System;

namespace Checkers
{
    public class Piece
    {
        private eTeam m_Team; // which team does this piece belongs to
        private bool m_IsKing; // is this a king piece

        public eTeam Team
        {
            get
            {
                return m_Team;
            }
        }

        public bool IsKing
        {
            get
            {
                return m_IsKing;
            }

            set
            {
                m_IsKing = value;
            }
        }

        // piece constructor
        public Piece(eTeam i_Team) 
        {
            this.m_Team = i_Team;
        }
    }
}