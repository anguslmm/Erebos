using System.Collections.Generic;
using Erebos.Engine.Enums;
using Erebos.Engine.GameManagement;

namespace Erebos.Engine.Pieces
{
    public class Bishop : Piece
    {
        public override HashSet<ChessBoardCell> FindPossibleMovementPaths()
        {
            var possibleMovements = new HashSet<ChessBoardCell>();

            for (var x = ChessBoardCell.X + 1; x <= 7; x++)
            for (var y = ChessBoardCell.Y + 1; y <= 7; y++)
            {
                var boardCell = ChessBoardCell.ChessBoard.GetCellFromPosition(x, ChessBoardCell.Y);
                if (boardCell.Piece != null)
                {
                    if (boardCell.Piece.Side == Side.Opposite())
                        possibleMovements.Add(boardCell);
                    
                    break;
                }

                possibleMovements.Add(boardCell);
            }
            
            for (var x = ChessBoardCell.X - 1; x >= 0; x--)
            for (var y = ChessBoardCell.Y + 1; y <= 7; y++)
            {
                var boardCell = ChessBoardCell.ChessBoard.GetCellFromPosition(x, ChessBoardCell.Y);
                if (boardCell.Piece != null)
                {
                    if (boardCell.Piece.Side == Side.Opposite())
                        possibleMovements.Add(boardCell);
                    
                    break;
                }
                
                possibleMovements.Add(boardCell);
            }
            
            for (var x = ChessBoardCell.X - 1; x >= 0; x--)
            for (var y = ChessBoardCell.Y - 1; y >= 0; y--)
            {
                var boardCell = ChessBoardCell.ChessBoard.GetCellFromPosition(x, ChessBoardCell.Y);
                if (boardCell.Piece != null)
                {
                    if (boardCell.Piece.Side == Side.Opposite())
                        possibleMovements.Add(boardCell);
                    
                    break;
                }
                
                possibleMovements.Add(boardCell);
            }
            
            for (var x = ChessBoardCell.X + 1; x <= 7; x++)
            for (var y = ChessBoardCell.Y - 1; y >= 0; y--)
            {
                var boardCell = ChessBoardCell.ChessBoard.GetCellFromPosition(x, ChessBoardCell.Y);
                if (boardCell.Piece != null)
                {
                    if (boardCell.Piece.Side == Side.Opposite())
                        possibleMovements.Add(boardCell);
                    
                    break;
                }
                
                possibleMovements.Add(boardCell);
            }

            return possibleMovements;
        }
    }
}