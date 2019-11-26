using System;

namespace Checkers
{
    public enum eBoardSize
    {
        Small = 6,
        Medium = 8,
        Large = 10
    }

    public class Board
    {
        private Space[] m_Spaces;
        private eBoardSize m_BoardSize;

        public eBoardSize BoardSize
        {
            get
            {
                return m_BoardSize;
            }
        }

        public Space[] Spaces
        {
            get
            {
                return m_Spaces;
            }
        }

        public Board(eBoardSize i_BoardSize)
        {
            this.m_BoardSize = i_BoardSize;
            this.m_Spaces = new Space[(byte)Math.Pow((byte)i_BoardSize, 2)];
            initializeBoard(i_BoardSize);
        }

        private void initializeBoard(eBoardSize i_BoardSize)
        {
            byte rowSize = (byte)i_BoardSize;
            byte RedTeamInitialArea = (byte)(rowSize * ((rowSize - 2) / 2)); // Formula to calculate the range (0 - ...) for initializing Red team area
            byte BlackTeamInitialArea = (byte)(m_Spaces.Length - RedTeamInitialArea); // Formula to calculate the range (... - spaces.Length) for initializing Black team area
            byte rowIndex = 0;

            for (byte i = 0; i < m_Spaces.Length; i++)
            {
                m_Spaces[i] = new Space();
                rowIndex = (byte)(i / rowSize);

                if (((i + rowIndex) % 2 == 1) && (i < RedTeamInitialArea))
                {
                    initializePieceInSpace(eTeam.Red, i); // Place pieces of Red team in relevant spaces
                }
                else if (((i + rowIndex) % 2 == 1) && (i >= BlackTeamInitialArea))
                {
                    initializePieceInSpace(eTeam.Black, i); // Place pieces of Black team in relevant spaces
                }
                else
                {
                    m_Spaces[i].ClearSpace(); // Checkers does not use current space
                }
            }
        }

        private void initializePieceInSpace(eTeam i_TeamIndicator, byte i_BoardIndex)
        {
            Piece currentPiece = new Piece(i_TeamIndicator);
            m_Spaces[i_BoardIndex].PlacePieceInSpace(currentPiece);
        }
    }
}