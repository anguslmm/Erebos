using System;
using System.Collections.Generic;
using Erebos.Engine.Enums;
using Erebos.Engine.GameManagement;

namespace Erebos.Engine.Pieces
{
    public class Pawn : Piece
    {
        public bool IsEnPassantEligible { get; private set; }

        private int _deltaForward;

        private void Start()
        {
            ChessBoardCell.ChessBoard.TurnEnding += TurnEndingHandler;
        }

        private void TurnEndingHandler(object sender, TurnEndedEventArgs e)
        {
            if (ChessBoardCell.ChessBoard.CurrentTurn == Side.Opposite())
                IsEnPassantEligible = false;
        }

        public override HashSet<ChessBoardCell> FindPossibleMovementPaths()
        {
            var boardCells = new HashSet<ChessBoardCell>();

            if (ChessBoardCell.ChessBoard.TryGetCellFromPosition(ChessBoardCell.X, ChessBoardCell.Y + _deltaForward, out var boardCell))
            {
                if (boardCell.Piece == null)
                    boardCells.Add(boardCell);
            }

            if (!HasMoved && ChessBoardCell.ChessBoard.TryGetCellFromPosition(ChessBoardCell.X, ChessBoardCell.Y + _deltaForward * 2,
                out var boardCellPassant))
            {
                if (boardCell.Piece == null)
                    boardCells.Add(boardCellPassant);
            }

            foreach (var dx in new[] {-1, 1})
            {
                if (ChessBoardCell.ChessBoard.TryGetCellFromPosition(
                    ChessBoardCell.X + dx, ChessBoardCell.Y + _deltaForward, out var boardCellAttack))
                {
                    if (boardCellAttack.Piece != null)
                        boardCells.Add(boardCellAttack);
                    else if (
                        ChessBoardCell.ChessBoard.TryGetCellFromPosition(
                            ChessBoardCell.X + dx, ChessBoardCell.Y, out var boardCellEnPassantCheck) &&
                        boardCellEnPassantCheck.Piece is Pawn pawn &&
                        pawn.IsEnPassantEligible)
                        boardCells.Add(boardCellAttack);
                }
            }

            return boardCells;
        }

        public override void MoveToCell(ChessBoardCell desiredChessBoardCell)
        {
            if (Math.Abs(ChessBoardCell.Y - desiredChessBoardCell.Y) == 2)
                IsEnPassantEligible = true;

            base.MoveToCell(desiredChessBoardCell);
        }

        public override void InitializeToCell(ChessBoardCell desiredChessBoardCell)
        {
            switch (desiredChessBoardCell.Y)
            {
                case 1:
                    _deltaForward = 1;
                    break;
                case 6:
                    _deltaForward = -1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(desiredChessBoardCell.Y), desiredChessBoardCell.Y,
                        $"Attempted to initialize a pawn to a Y position that wasn't 1 or 6.  And that doesn't make sense.");
            }

            base.InitializeToCell(desiredChessBoardCell);
        }
    }
}