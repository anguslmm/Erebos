using System;
using System.Collections.Generic;
using Erebos.Engine.GameManagement;

namespace Erebos.Engine.Pieces
{
    public class Pawn : Piece
    {
        public bool IsEnPassantEligible { get; private set; }

        private void Start()
        {
            GameManager.Instance.OnTurnEnding += TurnEndingHandler;
        }

        private void TurnEndingHandler(object sender, TurnEndedEventArgs e)
        {
            if (GameManager.Instance.CurrentTurn == OpposingSide)
                IsEnPassantEligible = false;
        }

        public override HashSet<BoardCell> FindPossibleMovementPaths()
        {
            var boardCells = new HashSet<BoardCell>();

            var deltaForward = OriginCell.BoardPosition.Y < BoardCell.BoardPosition.Y ? 1 : -1;
            if (BoardCell.GameBoard.TryGetCellFromPosition(BoardCell.BoardPosition.X, BoardCell.BoardPosition.Y + deltaForward, out var boardCell))
            {
                if (boardCell.Piece == null)
                    boardCells.Add(boardCell);
            }

            if (!HasMoved && BoardCell.GameBoard.TryGetCellFromPosition(BoardCell.BoardPosition.X, BoardCell.BoardPosition.Y + deltaForward * 2,
                out var boardCellPassant))
            {
                if (boardCell.Piece == null)
                    boardCells.Add(boardCellPassant);
            }

            foreach (var dx in new[] {-1, 1})
            {
                if (BoardCell.GameBoard.TryGetCellFromPosition(
                    BoardCell.BoardPosition.X + dx, BoardCell.BoardPosition.Y + deltaForward, out var boardCellAttack))
                {
                    if (boardCellAttack.Piece != null)
                        boardCells.Add(boardCellAttack);
                    else if (
                        BoardCell.GameBoard.TryGetCellFromPosition(
                            BoardCell.BoardPosition.X + dx, BoardCell.BoardPosition.Y, out var boardCellEnPassantCheck) &&
                        boardCellEnPassantCheck.Piece is Pawn pawn &&
                        pawn.IsEnPassantEligible)
                        boardCells.Add(boardCellAttack);
                }
            }

            return boardCells;
        }

        public override void MoveToCell(BoardCell desiredBoardCell)
        {
            if (Math.Abs(BoardCell.BoardPosition.Y - desiredBoardCell.BoardPosition.Y) == 2)
                IsEnPassantEligible = true;

            base.MoveToCell(desiredBoardCell);
        }
    }
}