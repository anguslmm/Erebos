using System.Collections.Generic;
using Erebos.Engine.GameManagement;

namespace Erebos.Engine.Pieces
{
    public class Rook : Piece
    {
        public override HashSet<ChessBoardCell> FindPossibleMovementPaths()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Called when a King is castling.
        /// </summary>
        /// <param name="kingDestinationChessBoardCell">The board cell to which the king is moving</param>
        public void OnCastling(ChessBoardCell kingDestinationChessBoardCell)
        {
            
        }
    }
}