using System;
using Erebos.Engine.Pieces;

namespace Erebos.Engine.GameManagement
{
    public class BoardCell : IEquatable<BoardCell>
    {
        public GameBoard GameBoard { get; }
        public BoardPosition BoardPosition { get; }
        public Piece Piece { get; set; }

        public BoardCell(int x, int y, GameBoard gameBoard)
        {
            GameBoard = gameBoard;
            BoardPosition = new BoardPosition(x, y, this);
        }

        public bool Equals(BoardCell other)
        {
            return BoardPosition.Equals(other?.BoardPosition);
        }

        public override int GetHashCode()
        {
            return (BoardPosition != null ? BoardPosition.GetHashCode() : 0);
        }
    }
}