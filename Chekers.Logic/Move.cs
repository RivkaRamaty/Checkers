using System;

namespace Checkers
{
    public class Move
    {
        private byte m_From; // source move (represented as the number of a cell)
        private byte m_To; // destination move (represented in the number of a cell)
        private bool m_MoveIsJump; // was this move a jump?
        private bool m_Forfeit = false; // press q to forfeit if you are losing

        public byte From
        {
            get
            {
                return m_From;
            }

            set
            {
                m_From = value;
            }
        }

        public byte To
        {
            get
            {
                return m_To;
            }

            set
            {
                m_To = value;
            }
        }

        public bool MoveIsJump
        {
            get
            {
                return m_MoveIsJump;
            }

            set
            {
                m_MoveIsJump = value;
            }
        }

        public bool Forfeit
        {
            get
            {
                return m_Forfeit;
            }

            set
            {
                m_Forfeit = value;
            }
        }

        // empty constructor
        public Move()
        {
        }

        // a specific constructor
        public Move(byte i_From, byte i_to, bool i_MoveIsJump = false)
        {
            this.m_From = i_From;
            this.m_To = i_to;
            this.m_MoveIsJump = i_MoveIsJump;
        }

        // converts this current move to a string
        public string ToString(eBoardSize i_BoardSize)
        {
            char UpCaseLetterFrom = (char)('A' + (this.From % (byte)i_BoardSize)),
                 LowCaseLetterFrom = (char)('a' + (this.From / (byte)i_BoardSize)),
                 UpCaseLetterTo = (char)('A' + (this.To % (byte)i_BoardSize)),
                 LowCaseLetterTo = (char)('a' + (this.To / (byte)i_BoardSize));

            return string.Format("{0}{1}>{2}{3}", UpCaseLetterFrom, LowCaseLetterFrom, UpCaseLetterTo, LowCaseLetterTo); // calculate back the string from the move
        }
    }
}
