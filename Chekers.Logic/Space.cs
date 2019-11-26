using System;

namespace Checkers
{
    public class Space
    {
        public enum eSpaceState
        {
            Empty,
            Occupied
        }

        private Piece m_Piece; // a space on the board might hold a game piece
        private eSpaceState m_SpaceState; // is the current space state is occupied with a piece in it or is it empty

        public Piece Piece
        {
            get
            {
                return m_Piece;
            }
        }

        public eSpaceState SpaceState
        {
            get
            {
                return m_SpaceState;
            }
        }
        
        // Places a given piece in the current space
        public void PlacePieceInSpace(Piece i_Piece)
        {
            this.m_Piece = i_Piece;
            this.m_SpaceState = eSpaceState.Occupied;
        }

        // clears a space from pieces
        public void ClearSpace()
        {
            this.m_Piece = null;
            this.m_SpaceState = eSpaceState.Empty;
        }
    }
}