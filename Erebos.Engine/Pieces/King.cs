using System;
using System.Collections.Generic;
using System.Linq;
using Erebos.Engine.Enums;
using Erebos.Engine.GameManagement;

namespace Erebos.Engine.Pieces
{
    public class King : Piece
    {
        public bool IsInCheck => ChessBoardCell.ChessBoard.CellsUnderAttackBySide[Side.Opposite()].Contains(ChessBoardCell);

        public override void MoveToCell(ChessBoardCell desiredChessBoardCell)
        {
            // Are we castling?
            // If so, find the rook we're castling with and tell it we're castling.
            if (Math.Abs(ChessBoardCell.X - desiredChessBoardCell.X) == 2)
            {
                Rook rook;
                
                if (desiredChessBoardCell.X < ChessBoardCell.X)
                    rook = ChessBoardCell.ChessBoard.GetCellFromPosition(0, ChessBoardCell.Y).Piece as Rook;
                else
                    rook = ChessBoardCell.ChessBoard.GetCellFromPosition(7, ChessBoardCell.Y).Piece as Rook;

                if (rook == null)
                    throw new InvalidOperationException("An attempt to castle was made but we failed to find the rook with which the king was meant to castle.");
                
                rook.OnCastling(desiredChessBoardCell);
            }
            
            base.MoveToCell(desiredChessBoardCell);
        }

        public override HashSet<ChessBoardCell> FindPossibleMovementPaths()
        {
            var boardCells = new HashSet<ChessBoardCell>();
            for (var dx = -1; dx <= 1; dx++) {
                for (var dy = -1; dy <= 1; dy++)
                {
                    // Don't report that we can move on top of ourselves.
                    if (dx == 0 && dy == 0) 
                        continue;
                    
                    if (ChessBoardCell.ChessBoard.TryGetCellFromPosition(ChessBoardCell.X + dx, ChessBoardCell.Y + dy, out var boardCell))
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

        private IEnumerable<ChessBoardCell> FindCastlingMoves()
        {
            if (HasMoved || IsInCheck)
                yield break;
            
            foreach (var rook in ChessBoardCell.ChessBoard.PiecesBySideInPlay[Side][typeof(Rook)].Cast<Rook>())
            {
                if (rook.HasMoved)
                    continue;

                // Are all the spaces between the rook and the king unoccupied?
                // And are the first two spaces not under attack?
                bool EvaluateCastlingMovement(int x)
                {
                    var boardCell = ChessBoardCell.ChessBoard.GetCellFromPosition(x, ChessBoardCell.Y);
                        
                    if (boardCell.Piece != null)
                    {
                        return false;
                    }
                        
                    var count = Math.Abs(x - ChessBoardCell.X);
                    if (count <= 2)
                    {
                        if (ChessBoardCell.ChessBoard.CellsUnderAttackBySide[Side.Opposite()].Contains(boardCell))
                        {
                            return false;
                        }
                    }

                    return true;
                }

                ChessBoardCell moveTo = null;
                if (ChessBoardCell.X < rook.ChessBoardCell.X)
                {
                    moveTo = ChessBoardCell.ChessBoard.GetCellFromPosition(ChessBoardCell.X + 2, ChessBoardCell.Y);
                    
                    for (var x = ChessBoardCell.X; x <= rook.ChessBoardCell.X; x++)
                    {
                        if (EvaluateCastlingMovement(x)) 
                            continue;
                        
                        moveTo = null;
                        break;
                    }
                }
                else
                {
                    moveTo = ChessBoardCell.ChessBoard.GetCellFromPosition(ChessBoardCell.X - 2, ChessBoardCell.Y);
                    
                    for (var x = ChessBoardCell.X; x >= rook.ChessBoardCell.X; x--)
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