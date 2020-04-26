using System;
using System.Collections.Generic;
using Erebos.Engine.Enums;
using Erebos.Engine.GameManagement;
using UnityEngine;

namespace Erebos.Engine.Pieces
{
    public abstract class Piece : MonoBehaviour, IEquatable<Piece>
    {
        public Sides Side { get; set; }

        public bool HasMoved { get; set; }

        public ChessBoardCell ChessBoardCell { get; set; }

        public abstract HashSet<ChessBoardCell> FindPossibleMovementPaths();

        public virtual void MoveToCell(ChessBoardCell desiredChessBoardCell)
        {
            ChessBoardCell = desiredChessBoardCell;
            gameObject.transform.position = desiredChessBoardCell.ToRelativeCenterPosition();
        }

        public virtual void InitializeToCell(ChessBoardCell desiredChessBoardCell)
        {
            ChessBoardCell = desiredChessBoardCell;
            gameObject.transform.position = desiredChessBoardCell.ToRelativeCenterPosition();

            Side = ChessBoardCell.Y <= 1 ? Sides.White : Sides.Black;
            gameObject.name = $"{GetType().Name}-{Side}";
            var meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material =
                Side == Sides.Black ? ChessBoardCell.ChessBoard.blackPiecesMaterial : ChessBoardCell.ChessBoard.whitePiecesMaterial;
        }

        public void DestroyPiece()
        {
            Destroy(gameObject);
        }

        public void OnSelected()
        {
            Debug.Log($"{this} selected!");
        }

        public void OnDeselected()
        {
            Debug.Log($"{this} deselected!");
        }

        public bool Equals(Piece other)
        {
            return other != null && gameObject.GetInstanceID() == other.gameObject.GetInstanceID();
        }

        public override string ToString()
        {
            return gameObject.name;
        }
        
        protected IEnumerable<ChessBoardCell> GetCellPath(Vector2Int moveUnit)
        {
            var currentCell =
                ChessBoardCell.ChessBoard.GetCellFromPosition(ChessBoardCell.X + moveUnit.x,
                    ChessBoardCell.Y + moveUnit.y);
            // First return all the clear cells in the path
            while (!currentCell.IsOccupied)
            {
                yield return currentCell;
            }
            // If the cell is occupied, but an enemy piece, return it as well. (attackable)
            if (currentCell.Piece.Side != Side) yield return currentCell;
        }
    }
}
