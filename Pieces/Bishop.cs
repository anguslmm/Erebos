using System.Collections.Generic;
using Erebos.Engine.GameManagement;

namespace Erebos.Engine.Pieces
{
    public class Bishop : Piece
    {
        public override HashSet<BoardCell> FindPossibleMovementPaths()
        {
            var possibleMovements = new HashSet<BoardCell>();

            for (var x = BoardCell.BoardPosition.X + 1; x <= 7; x++)
            for (var y = BoardCell.BoardPosition.Y + 1; y <= 7; y++)
            {
                var boardCell = BoardCell.GameBoard.GetCellFromPosition(x, BoardCell.BoardPosition.Y);
                if (boardCell.Piece != null)
                {
                    if (boardCell.Piece.Side == OpposingSide)
                        possibleMovements.Add(boardCell);
                    
                    break;
                }

                possibleMovements.Add(boardCell);
            }
            
            for (var x = BoardCell.BoardPosition.X - 1; x >= 0; x--)
            for (var y = BoardCell.BoardPosition.Y + 1; y <= 7; y++)
            {
                var boardCell = BoardCell.GameBoard.GetCellFromPosition(x, BoardCell.BoardPosition.Y);
                if (boardCell.Piece != null)
                {
                    if (boardCell.Piece.Side == OpposingSide)
                        possibleMovements.Add(boardCell);
                    
                    break;
                }
                
                possibleMovements.Add(boardCell);
            }
            
            for (var x = BoardCell.BoardPosition.X - 1; x >= 0; x--)
            for (var y = BoardCell.BoardPosition.Y - 1; y >= 0; y--)
            {
                var boardCell = BoardCell.GameBoard.GetCellFromPosition(x, BoardCell.BoardPosition.Y);
                if (boardCell.Piece != null)
                {
                    if (boardCell.Piece.Side == OpposingSide)
                        possibleMovements.Add(boardCell);
                    
                    break;
                }
                
                possibleMovements.Add(boardCell);
            }
            
            for (var x = BoardCell.BoardPosition.X + 1; x <= 7; x++)
            for (var y = BoardCell.BoardPosition.Y - 1; y >= 0; y--)
            {
                var boardCell = BoardCell.GameBoard.GetCellFromPosition(x, BoardCell.BoardPosition.Y);
                if (boardCell.Piece != null)
                {
                    if (boardCell.Piece.Side == OpposingSide)
                        possibleMovements.Add(boardCell);
                    
                    break;
                }
                
                possibleMovements.Add(boardCell);
            }

            return possibleMovements;
        }
    }
}