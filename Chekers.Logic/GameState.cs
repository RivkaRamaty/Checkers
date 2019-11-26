using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers
{
    public enum eGameMode
    {
        PlayerVsPlayer = 1,
        PlayerVsPC = 2
    }

    public class GameState
    {
        private enum eDirection
        {
            Left = 1,
            Right = -1
        }

        // $G$ DSN-999 (-3) m_Board should be readonly.
        private Board m_Board;
        private eTeam m_CurrentTeam;
        private eGameMode m_CurrentGameMode;
        private string m_PreviousMove = null;
        private bool m_SameTeamPlaysAgain = false;
        private bool m_NoPossibleMovesLeft = false;
        private bool m_FirstMove = true;
        private bool m_Forfeit = false;
        private byte m_PieceCountRed;
        private byte m_PieceCountBlack;

        public Board Board
        {
            get
            {
                return m_Board;
            }
        }

        public eTeam CurrentTurn
        {
            get
            {
                return m_CurrentTeam;
            }

            set
            {
                m_CurrentTeam = value;
            }
        }

        public eGameMode GameMode
        {
            get
            {
                return m_CurrentGameMode;
            }
        }

        public string PreviousMove
        {
            get
            {
                return m_PreviousMove;
            }

            set
            {
                m_PreviousMove = value;
            }
        }

        public bool SameTeamPlaysAgain
        {
            get
            {
                return m_SameTeamPlaysAgain;
            }

            set
            {
                m_SameTeamPlaysAgain = value;
            }
        }

        public bool NoMoreMovesAvailable
        {
            get
            {
                return m_NoPossibleMovesLeft;
            }

            set
            {
                m_NoPossibleMovesLeft = value;
            }
        }

        public bool FirstMove
        {
            get
            {
                return m_FirstMove;
            }

            set
            {
                m_FirstMove = value;
            }
        }

        public bool Forfeit
        {
            set
            {
                m_Forfeit = value;
            }
        }

        public byte PieceCounterBlack
        {
            get
            {
                return m_PieceCountBlack;
            }
        }

        public byte PieceCounterRed
        {
            get
            {
                return m_PieceCountRed;
            }
        }

        // constructor
        public GameState(Board i_Board, eGameMode i_Mode)
        {
            this.m_Board = i_Board;
            this.m_CurrentGameMode = i_Mode;
            this.m_CurrentTeam = eTeam.Black;
            this.m_PieceCountBlack = this.m_PieceCountRed = (byte)(((byte)i_Board.BoardSize / 2) * (((byte)i_Board.BoardSize / 2) - 1)); // initialize piece count for both sides
        }

        // This method saves all the possible moves for a team in a list
        public List<Move> PopulateTeamMoveList(eTeam i_CurrentTeam, out bool io_JumpsAvailable)
        {
            List<Move> currentTeamMoveList = new List<Move>();

            io_JumpsAvailable = populateJumpMovesToTeamMoveList(i_CurrentTeam, currentTeamMoveList);

            if (!io_JumpsAvailable)
            {
                populateStepMovesToTeamMoveList(i_CurrentTeam, currentTeamMoveList);
            }

            if (currentTeamMoveList.Count == 0)
            {
                this.NoMoreMovesAvailable = true;
            }

            return currentTeamMoveList;
        }

        private void populateStepMovesToTeamMoveList(eTeam i_CurrentTeam, List<Move> io_CurrentTeamMoveList)
        {
            for (byte i = 0; i < m_Board.Spaces.Length; i++)
            {
                if (m_Board.Spaces[i].SpaceState == Space.eSpaceState.Occupied
                 && m_Board.Spaces[i].Piece.Team == i_CurrentTeam)
                {
                    checkAndAddMoves(i, i_CurrentTeam, eDirection.Left, io_CurrentTeamMoveList);
                    checkAndAddMoves(i, i_CurrentTeam, eDirection.Right, io_CurrentTeamMoveList);
                }
            }
        }

        private bool populateJumpMovesToTeamMoveList(eTeam i_CurrentTeam, List<Move> io_CurrentTeamMoveList)
        {
            bool jumpsAvailableLeft = false, jumpsAvailableRight = false, jumpsAvailable = false;

            for (byte i = 0; i < m_Board.Spaces.Length; i++)
            {
                if (m_Board.Spaces[i].SpaceState == Space.eSpaceState.Occupied
                 && m_Board.Spaces[i].Piece.Team == i_CurrentTeam)
                {
                    jumpsAvailableLeft = checkAndAddJumpMoves(i, i_CurrentTeam, eDirection.Left, io_CurrentTeamMoveList);
                    jumpsAvailableRight = checkAndAddJumpMoves(i, i_CurrentTeam, eDirection.Right, io_CurrentTeamMoveList);

                    if (jumpsAvailableLeft || jumpsAvailableRight)
                    {
                        jumpsAvailable = true;
                    }
                }
            }

            return jumpsAvailable;
        }

        private void removeJumpedPiece(Move i_InputMove)
        {
            m_Board.Spaces[(i_InputMove.From + i_InputMove.To) / 2].ClearSpace(); // clear the jumped space from the piece
            // after clearing the jumped piece, Reduce the relevant team's piece counter
            if (CurrentTurn == eTeam.Black)
            {
                m_PieceCountRed--;
            }
            else
            {
                m_PieceCountBlack--;
            }
        }

        // Perform the move
        public bool MakeMove(Move i_InputMove, List<Move> i_CurentTeamMoveList, bool currentTeamIsLosing, out bool o_CurrentMoveWasJump, out byte o_JumpingPieceIndex)
        {
            bool moveMadeSuccessfully = false;
            int upperRowIndexes = (int)m_Board.BoardSize, lowerRowIndexes = (int)m_Board.BoardSize * (int)(m_Board.BoardSize - 1);

            if (i_InputMove.Forfeit && currentTeamIsLosing)
            {
                this.m_Forfeit = true;
                moveMadeSuccessfully = true;
            }
            else
            {
                foreach (Move moveIndex in i_CurentTeamMoveList)
                {
                    if ((i_InputMove.From == moveIndex.From) // if input move is available in move list
                     && (i_InputMove.To == moveIndex.To))
                    {
                        m_Board.Spaces[i_InputMove.To].PlacePieceInSpace(m_Board.Spaces[i_InputMove.From].Piece); // put the relevant piece in the destination
                        m_Board.Spaces[i_InputMove.From].ClearSpace(); // clear the piece from the source
                        if (i_InputMove.MoveIsJump)
                        {
                            removeJumpedPiece(i_InputMove);
                        }

                        if ((CurrentTurn == eTeam.Black && i_InputMove.To < upperRowIndexes)
                         || (CurrentTurn == eTeam.Red && i_InputMove.To >= lowerRowIndexes))
                        {
                            m_Board.Spaces[i_InputMove.To].Piece.IsKing = true;
                        }

                        moveMadeSuccessfully = true;
                        break;
                    }
                }
            }

            o_CurrentMoveWasJump = i_InputMove.MoveIsJump;

            if (moveMadeSuccessfully 
             && ((o_CurrentMoveWasJump 
             && !anotherJumpsAvailable(CurrentTurn)) 
             || !o_CurrentMoveWasJump))
            {
                PreviousMove = i_InputMove.ToString(Board.BoardSize);
            }

            o_JumpingPieceIndex = i_InputMove.To;

            return moveMadeSuccessfully;
        }

        private bool anotherJumpsAvailable(eTeam i_CurrentTurn)
        {
            List<Move> currentTeamMoveList = new List<Move>();

            return populateJumpMovesToTeamMoveList(i_CurrentTurn, currentTeamMoveList);
        }

        // Is the game still on?
        public bool GameOn()
        {
            bool jumpsAvailable; // dummy
            List<Move> blackTeamMoveList = new List<Move>(), redTeamMoveList = new List<Move>();

            blackTeamMoveList = PopulateTeamMoveList(eTeam.Black, out jumpsAvailable);
            redTeamMoveList = PopulateTeamMoveList(eTeam.Red, out jumpsAvailable); 
            if (blackTeamMoveList.Count == 0 || redTeamMoveList.Count == 0)
            {
                m_NoPossibleMovesLeft = true;
            }

            return !(m_PieceCountBlack == 0) && !(m_PieceCountRed == 0) && !this.m_NoPossibleMovesLeft;
        }

        private bool isDestinationWithinLeftRightBorders(byte i_SpaceIndex, eDirection i_Direction, byte i_BoardSize)
        {
            return i_Direction == eDirection.Left ? i_SpaceIndex % i_BoardSize > 1 : i_SpaceIndex % i_BoardSize < i_BoardSize - 2;
        }

        private bool isDestinationBeneathUpperBorder(int jumpTo)
        {
            return jumpTo >= 0;
        }

        private bool isDestinationAboveLowerBorder(int jumpTo)
        {
            return jumpTo < Math.Pow((byte)m_Board.BoardSize, 2);
        }

        private bool isThereAPieceToBeJumpedOver(byte i_SpaceIndex, int i_DirectionMultiplier, int i_TeamMultiplier, byte i_BoardSize)
        {
            return m_Board.Spaces[i_SpaceIndex - ((i_TeamMultiplier * i_BoardSize) + i_DirectionMultiplier)].SpaceState == Space.eSpaceState.Occupied;
        }

        private bool isThePieceIJumpOverIsOpponent(byte i_SpaceIndex, int i_DirectionMultiplier, int i_TeamMultiplier, byte i_BoardSize, eTeam i_CurrentTeam)
        {
            return m_Board.Spaces[i_SpaceIndex - ((i_TeamMultiplier * i_BoardSize) + i_DirectionMultiplier)].Piece.Team != i_CurrentTeam;
        }

        private bool isDestinationEmpty(int jumpTo)
        {
            return m_Board.Spaces[jumpTo].SpaceState == Space.eSpaceState.Empty;
        }

        private bool checkAndAddJumpMoves(byte i_SpaceIndex, eTeam i_CurrentTeam, eDirection i_Direction, List<Move> io_CurrentTeamMoveList)
        {
            int teamMultiplier = i_CurrentTeam == eTeam.Black ? 1 : -1;
            int directionMultiplier = (int)i_Direction;
            int jumpTo;
            bool checkAgainForKing = m_Board.Spaces[i_SpaceIndex].Piece.IsKing, jumpMove = false;
            byte boardSize = (byte)m_Board.BoardSize,
                 timesChecked = 0;

            do
            {
                jumpTo = i_SpaceIndex - ((2 * teamMultiplier * (byte)m_Board.BoardSize) + (2 * directionMultiplier));

                if (isDestinationBeneathUpperBorder(jumpTo)
                 && isDestinationAboveLowerBorder(jumpTo)
                 && isDestinationWithinLeftRightBorders(i_SpaceIndex, i_Direction, boardSize)
                 && isThereAPieceToBeJumpedOver(i_SpaceIndex, directionMultiplier, teamMultiplier, boardSize)
                 && isThePieceIJumpOverIsOpponent(i_SpaceIndex, directionMultiplier, teamMultiplier, boardSize, i_CurrentTeam)
                 && isDestinationEmpty(jumpTo))
                {
                    jumpMove = true;
                    Move newMove = new Move(i_SpaceIndex, (byte)jumpTo, jumpMove);
                    io_CurrentTeamMoveList.Add(newMove);
                }

                // if the current piece is a king, we check moves both upwards and downwards
                if (checkAgainForKing)
                {
                    timesChecked++;
                    teamMultiplier *= -1;
                }
            }
            while (checkAgainForKing && timesChecked < 2);

            return jumpMove;
        }

        private void checkAndAddMoves(byte i_SpaceIndex, eTeam i_CurrentTeam, eDirection i_Direction, List<Move> io_CurrentTeamMoveList)
        {
            int teamMultiplier = i_CurrentTeam == eTeam.Black ? 1 : -1;
            int directionMultiplier = (int)i_Direction;
            int stepTo;
            byte boardSize = (byte)m_Board.BoardSize;
            bool isInScopeForMove = i_Direction == eDirection.Left ? !(i_SpaceIndex % boardSize == 0) : !((i_SpaceIndex % boardSize) == (boardSize - 1));
            bool checkAgainForKing = m_Board.Spaces[i_SpaceIndex].Piece.IsKing;
            byte timesChecked = 0;

            do
            {
                stepTo = (byte)(i_SpaceIndex - ((teamMultiplier * boardSize) + directionMultiplier));

                if (isInScopeForMove                                                     // piece is not next to the left border
                && (stepTo >= 0)                                                         // The wanted move is in boundaries
                && stepTo < Math.Pow(boardSize, 2)                                       // The wanted move is in boundaries
                && m_Board.Spaces[stepTo].SpaceState == Space.eSpaceState.Empty          // wanted space to move is empty
                && ((i_SpaceIndex / boardSize) == (stepTo / boardSize) + teamMultiplier) // make sure destination is in the row it should be at BUG FIX
                )
                {
                    Move newMove = new Move(i_SpaceIndex, (byte)stepTo);
                    io_CurrentTeamMoveList.Add(newMove);
                }

                if (checkAgainForKing)
                {
                    timesChecked++;
                    teamMultiplier *= -1;
                }
            }
            while (checkAgainForKing && timesChecked < 2);
        }

        public int KingCounterByTeam(eTeam i_Team)
        {
            int kingCounter = 0;

            foreach (Space space in Board.Spaces)
            {
                if (space.SpaceState == Space.eSpaceState.Occupied
                 && space.Piece.Team == i_Team
                 && space.Piece.IsKing)
                {
                    kingCounter++;
                }
            }

            return kingCounter;
        }
    }
}
