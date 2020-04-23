using System;
using System.Collections.Generic;
using System.Linq;
using Erebos.Engine.GameManagement;

namespace Erebos.Engine.Pieces
{
    public class King : Piece
    {
        public bool IsInCheck => BoardCell.GameBoard.CellsUnderAttackBySide[OpposingSide].Contains(BoardCell);

        public override void MoveToCell(BoardCell desiredBoardCell)
        {
            // Are we castling?
            // If so, find the rook we're castling with and tell it we're castling.
            if (Math.Abs(BoardCell.BoardPosition.X - desiredBoardCell.BoardPosition.X) == 2)
            {
                Rook rook;
                
                if (desiredBoardCell.BoardPosition.X < BoardCell.BoardPosition.X)
                    rook = BoardCell.GameBoard.GetCellFromPosition(0, BoardCell.BoardPosition.Y).Piece as Rook;
                else
                    rook = BoardCell.GameBoard.GetCellFromPosition(7, BoardCell.BoardPosition.Y).Piece as Rook;

                if (rook == null)
                    throw new InvalidOperationException("An attempt to castle was made but we failed to find the rook with which the king was meant to castle.");
                
                rook.OnCastling(desiredBoardCell);
            }
            
            base.MoveToCell(desiredBoardCell);
        }

        public override HashSet<BoardCell> FindPossibleMovementPaths()
        {
            var boardCells = new HashSet<BoardCell>();
            for (var dx = -1; dx <= 1; dx++) {
                for (var dy = -1; dy <= 1; dy++)
                {
                    // Don't report that we can move on top of ourselves.
                    if (dx == 0 && dy == 0) 
                        continue;
                    
                    if (BoardCell.GameBoard.TryGetCellFromPosition(BoardCell.BoardPosition.X + dx, BoardCell.BoardPosition.Y + dy, out var boardCell))
                    {
                        boardCells.Add(boardCell);
                    }
                }
            }
            
            foreach (var boardCell in FindCastlingMoves())
            {
                boardCells.Add(boardCell);
            }

            return boardCells;
        }

        private IEnumerable<BoardCell> FindCastlingMoves()
        {
            if (HasMoved || IsInCheck)
                yield break;
            
            foreach (var rook in BoardCell.GameBoard.PiecesBySideInPlay[Side][typeof(Rook)].Cast<Rook>())
            {
                if (rook.HasMoved)
                    continue;

                // Are all the spaces between the rook and the king unoccupied?
                // And are the first two spaces not under attack?
                bool EvaluateCastlingMovement(int x)
                {
                    var boardCell = BoardCell.GameBoard.GetCellFromPosition(x, BoardCell.BoardPosition.Y);
                        
                    if (boardCell.Piece != null)
                    {
                        return false;
                    }
                        
                    var count = Math.Abs(x - BoardCell.BoardPosition.X);
                    if (count <= 2)
                    {
                        if (BoardCell.GameBoard.CellsUnderAttackBySide[OpposingSide].Contains(boardCell))
                        {
                            return false;
                        }
                    }

                    return true;
                }

                BoardCell moveTo = null;
                if (BoardCell.BoardPosition.X < rook.BoardCell.BoardPosition.X)
                {
                    moveTo = BoardCell.GameBoard.GetCellFromPosition(BoardCell.BoardPosition.X + 2, BoardCell.BoardPosition.Y);
                    
                    for (var x = BoardCell.BoardPosition.X; x <= rook.BoardCell.BoardPosition.X; x++)
                    {
                        if (EvaluateCastlingMovement(x)) 
                            continue;
                        
                        moveTo = null;
                        break;
                    }
                }
                else
                {
                    moveTo = BoardCell.GameBoard.GetCellFromPosition(BoardCell.BoardPosition.X - 2, BoardCell.BoardPosition.Y);
                    
                    for (var x = BoardCell.BoardPosition.X; x >= rook.BoardCell.BoardPosition.X; x--)
                    {
                        if (EvaluateCastlingMovement(x)) 
                            continue;
                        
                        moveTo = null;
                        break;
                    }
                }

                if (moveTo != null)
                    yield return moveTo;
            }
        }
    }
}