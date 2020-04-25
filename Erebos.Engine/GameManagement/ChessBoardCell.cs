using System;
using Erebos.Engine.Pieces;
using UnityEngine;

namespace Erebos.Engine.GameManagement
{
    public class ChessBoardCell : IEquatable<ChessBoardCell>
    {
        public ChessBoard ChessBoard { get; }
        
        public int X { get; }
        
        public int Y { get; }
        
        public Piece Piece { get; set; }

        public bool IsOccupied => Piece != null;

        public ChessBoardCell(int x, int y, ChessBoard chessBoard)
        {
            if (x < 0 || x > 7)
                throw new ArgumentOutOfRangeException(nameof(x), x, "X must be in the range [0, 7]");
            
            if (y < 0 || y > 7)
                throw new ArgumentOutOfRangeException(nameof(y), y, "Y must be in the range [0, 7]");
            
            ChessBoard = chessBoard;
            X = x;
            Y = y;
        }
        
        public Vector3 ToRelativeCenterPosition()
        {
            var gameBoardTileSize = ChessBoard.tileSize;

            return new Vector3(
                gameBoardTileSize * X * -1 - gameBoardTileSize / 2, 
                ChessBoard.firstTileStart.y, 
                gameBoardTileSize * Y * -1 - gameBoardTileSize / 2);
        }

        public bool Equals(ChessBoardCell other)
        {
            return other != null && X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }
        
        public override string ToString()
        {
            return $"{X},{Y}";
        }
    }
}