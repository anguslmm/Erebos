using System;
using UnityEngine;

namespace Erebos.Engine.GameManagement
{
    public class BoardPosition : IEquatable<BoardPosition>
    {
        public int X { get; }
        public BoardCell BoardCell { get; }
        public int Y { get; }

        public BoardPosition(int x, int y, BoardCell boardCell)
        {
            if (x < 0 || x > 7)
                throw new ArgumentOutOfRangeException(nameof(x), x, "X must be in the range [0, 7]");
            
            if (y < 0 || y > 7)
                throw new ArgumentOutOfRangeException(nameof(y), y, "Y must be in the range [0, 7]");
            
            Y = y;
            X = x;
            BoardCell = boardCell;
        }

        public Vector3 ToRelativeCenterPosition()
        {
            var gameBoardTileSize = BoardCell.GameBoard.tileSize;

            return new Vector3(
                gameBoardTileSize * X * -1 - gameBoardTileSize / 2, 
                BoardCell.GameBoard.firstTileStart.y, 
                gameBoardTileSize * Y * -1 - gameBoardTileSize / 2);
        }

        public bool Equals(BoardPosition other)
        {
            return other != null && X == other.X && Y == other.Y;
        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }
    }
}