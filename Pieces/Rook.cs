using System.Collections.Generic;
using Erebos.Engine.GameManagement;

namespace Erebos.Engine.Pieces
{
    public class Rook : Piece
    {
        public override HashSet<BoardCell> FindPossibleMovementPaths()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Called when a King is castling.
        /// </summary>
        /// <param name="kingDestinationBoardCell">The board cell to which the king is moving</param>
        public void OnCastling(BoardCell kingDestinationBoardCell)
        {
            
        }
    }
}